using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

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



        public MySqlConnection GetConnection()
        {
            return new MySqlConnection(_sb.ConnectionString);
        }



        public async Task<bool> TestConnectionAsync()
        {
            try
            {
                using var connection = GetConnection();

                await connection.OpenAsync();

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
            using var connection = GetConnection();
            await connection.OpenAsync();
            using var command = new MySqlCommand(query, connection);
            if (parameters != null)
            {
                foreach (var param in parameters)
                {
                    command.Parameters.AddWithValue(param.Key, param.Value);
                }
            }
            return await command.ExecuteNonQueryAsync();
        }

        public async Task<object?> ExecuteScalarAsync(string query, Dictionary<string, object>? parameters = null)
        {
            using var connection = GetConnection();
            await connection.OpenAsync();
            using var command = new MySqlCommand(query, connection);
            if (parameters != null)
            {
                foreach (var param in parameters)
                {
                    command.Parameters.AddWithValue(param.Key, param.Value);
                }
            }
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
            using var connection = GetConnection();
            await connection.OpenAsync().ConfigureAwait(false);
            using var command = new MySqlCommand(query, connection);
            if (parameters != null)
            {
                foreach (var param in parameters)
                {
                    command.Parameters.AddWithValue(param.Key, param.Value);
                }
            }
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
            throw new InvalidOperationException("Nie można przetworzyć następnego ID studenta.");

        }

        private static MySqlCommand GenerateSelect(string query, Dictionary<string, object>? parameters = null)
        {
            MySqlCommand command = new(query);
            if (parameters != null)
            {
                foreach (var param in parameters)
                {
                    command.Parameters.AddWithValue(param.Key, param.Value);
                }
            }
            return command;
        }
        public async Task<DataTable> GenerateDatatableAsync(string query, Dictionary<string, object>? parameters = null)
        {
            MySqlDataAdapter adapter = new(GenerateSelect(query, parameters));
            DataTable dataTable = new();
            using var connection = GetConnection();
            await connection.OpenAsync();
            adapter.SelectCommand.Connection = connection;
            adapter.Fill(dataTable);
            return dataTable;

        }


        private static string ExtractEmbeddedPem(string resourceName)
        {
            var assembly = Assembly.GetExecutingAssembly();
            using var stream = assembly.GetManifestResourceStream(resourceName);
            if (stream == null)
                throw new FileNotFoundException("Resource not found: " + resourceName);

            string tempFile = Path.GetTempFileName();
            using var fileStream = File.Create(tempFile);
            stream.CopyTo(fileStream);
            return tempFile;
        }

        public async Task<int> AuthenticateAsync(LoginWrapper wrapper)
        {
            string query = "SELECT uprawnienia FROM sesje WHERE login = @username AND token = @token AND data_waznosci > NOW()";
            var parameters = new Dictionary<string, object>
            {
                { "@username", wrapper.Username },
                { "@token", wrapper.Token }
            };
            var result = await ExecuteQueryAsync(query, parameters).ConfigureAwait(false);
            if (result.Count == 1) // Jeśli są mnogie tokeny, to zablokuj wszystkie
            {
                return (int) result[0]["uprawnienia"];
            }
            return 0;
        }

        public async void DestroySession(string username)
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
    }
    }
