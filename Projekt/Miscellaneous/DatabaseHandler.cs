using MySql.Data.MySqlClient;
using System.Data;
using System.IO;
using System.Reflection;

namespace Projekt.Miscellaneous
{
    public class DatabaseHandler
    {
        private readonly MySqlConnectionStringBuilder _sb;
        private MySqlConnection? _conn;

        public DatabaseHandler()
        {
            _sb = new()
            {
                Server = "mysql-3740706b-poig-projekt.j.aivencloud.com",
                Database = "szkola",
                UserID = "avnadmin",
                Password = "AVNS_VF46Jvt9IUHAENA4NeD",
                Port = 19306,
                SslCa = ExtractEmbeddedPem("Projekt.Resources.ca.pem"),
                SslMode = MySqlSslMode.VerifyCA

            };

        }


        public async Task<MySqlConnection> GetConnectionAsync()
        {
            _conn = new MySqlConnection(_sb.ConnectionString);
            await _conn.OpenAsync();
            return _conn;
        }

        public async Task<bool> TestConnectionAsync()
        {
            try
            {
                using var connection = await GetConnectionAsync();

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error connecting to database: {ex.Message}");
                return false;
            }
        }
        public async Task<int> ExecuteNonQueryAsync(string query, Dictionary<string, object>? parameters = null)
        {

            using var command = CreateCommand(query, parameters);
            using var connection = await GetConnectionAsync();

            command.Connection = connection;
            return await command.ExecuteNonQueryAsync();
        }

        public async Task<object?> ExecuteScalarAsync(string query, Dictionary<string, object>? parameters = null)
        {

            using var command = CreateCommand(query, parameters);
            using var connection = await GetConnectionAsync();
            command.Connection = connection;
            object? result = await command.ExecuteScalarAsync();
            return result;
        }

        /// <summary>
        /// Executes a query and returns the results as a list of dictionaries, where each dictionary represents a row.
        /// </summary>
        /// <param name="query">A MySQL query. Replace parameters with @parameter_name.</param>
        /// <param name="parameters">A list of parameters, of format {"@parameter_name" : object}</param>
        /// <returns></returns>
        public async Task<List<Dictionary<string, object>>> ExecuteQueryAsync(string query, Dictionary<string, object>? parameters = null)
        {
            var results = new List<Dictionary<string, object>>();
            using var connection = await GetConnectionAsync();
            using var command = CreateCommand(query, parameters);
            command.Connection = connection;
            using var reader = await command.ExecuteReaderAsync().ConfigureAwait(false);
            while (await reader.ReadAsync().ConfigureAwait(false))
            {
                var row = new Dictionary<string, object>();
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    row[reader.GetName(i)] = reader.IsDBNull(i) ? null! : reader.GetValue(i);
                }
                results.Add(row);
            }
            return results;
        }
        public async Task<int> SuggestStudentID()
        {
            // Łatwa funkcja, MAX() + 1
            string query = "SELECT MAX(indeks) + 1 FROM student";
            var result = await ExecuteScalarAsync(query);
            if (result == null || result == DBNull.Value)
            {
                return 1; // Jeśli nie ma żadnych studentów, zaczynamy od 1
            }
            if (int.TryParse(result.ToString(), out int nextId))
            {
                return nextId;
            }
            return 0; // Nie rzucaj użytkownikom wyjątków

        }


        public async Task<DataTable> GenerateDatatableAsync(string query, Dictionary<string, object>? parameters = null)
        {

            MySqlDataAdapter adapter = new(CreateCommand(query, parameters));
            DataTable dataTable = new();
            using var connection = await GetConnectionAsync(); 
            adapter.SelectCommand.Connection = connection;
            adapter.Fill(dataTable);
            return dataTable;
        }


        private static string ExtractEmbeddedPem(string resourceName)
        {
            var assembly = Assembly.GetExecutingAssembly();
            using var stream = assembly.GetManifestResourceStream(resourceName) ?? throw new FileNotFoundException("Resource not found: " + resourceName);
            string tempFile = Path.GetTempFileName();
            using var fileStream = File.Create(tempFile);
            stream.CopyTo(fileStream);
            return tempFile;
        }

        public async Task<int> AuthenticateAsync(LoginWrapper wrapper)
        {
            // TODO: nie zezwalaj na przedawnione tokeny
            string query = "SELECT uprawnienia FROM sesje WHERE login = @username AND token = @token AND data_waznosci > NOW();";
            var parameters = new Dictionary<string, object>
            {
                { "@username", wrapper.Username ?? string.Empty},
                { "@token", wrapper.Token ?? string.Empty}
            };
            var result = await ExecuteQueryAsync(query, parameters).ConfigureAwait(false);
            if (result.Count == 1) // Jeśli są mnogie tokeny, to zablokuj wszystkie
            {
                return (int)result[0]["uprawnienia"];
            }
            return 0;
        }

        public async Task DestroySession(string username)
        {
            await ExecuteNonQueryAsync(
                "DELETE FROM sesje WHERE login = @username",
                new Dictionary<string, object> { { "@username", username } }

            );
            if (_conn != null)
            {
                _conn.Close();
                _conn.Dispose();
                _conn = null;
            }
        }
        /// <summary>
        /// Executes a set of database operations within a transaction. Rolls back if an exception occurs.
        /// </summary>
        /// <param name="operation">A function that receives an open MySqlConnection and MySqlTransaction to perform operations.</param>
        /// <returns>True if committed successfully, false if rolled back due to an error.</returns>
        public async Task<bool> ExecuteInTransactionAsync(params MySqlCommand[] commands)
        {
            using var connection = await GetConnectionAsync();

            using var transaction = await connection.BeginTransactionAsync();
            try
            {

                foreach (var command in commands)
                {
                    command.Connection = connection;
                    await command.ExecuteNonQueryAsync();
                }
                await transaction.CommitAsync();
                await connection.CloseAsync();
                return true;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                await connection.CloseAsync();
                Console.WriteLine($"Transaction rolled back due to error: {ex.Message}");
                return false;
            }
        }

        public static MySqlCommand CreateCommand(string query, Dictionary<string, object>? parameters)
        {
            MySqlCommand command = new(query);

            if (parameters != null)
                foreach (var param in parameters)
                {
                    command.Parameters.AddWithValue(param.Key, param.Value);
                }
            return command;
        }
    }
}
