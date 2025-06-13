using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
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
        private MySqlConnection _conn;

        public DatabaseHandler()
        {
            _sb = new()
            {
                Server = "mysql-3740706b-poig-projekt.j.aivencloud.com",
                Database = "projekt",
                UserID = "avnadmin",
                Password = "AVNS_VF46Jvt9IUHAENA4NeD",
                Port = 19306,
                SslCa = ExtractEmbeddedPem("Projekt.Resources.ca.pem"),
                SslMode = MySqlSslMode.VerifyCA
            };

        }



        public MySqlConnection GetConnection()
            => _conn ??= new(_sb.ConnectionString);
        

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
            return await command.ExecuteScalarAsync();
        }

        public async Task<List<Dictionary<string, object>>> ExecuteQueryAsync(string query, Dictionary<string, object>? parameters = null)
        {
            var results = new List<Dictionary<string, object>>();
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
    }
    }
