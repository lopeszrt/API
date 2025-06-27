using MySqlConnector;
using System.Data;

namespace API.Services
{
    public class Database
    {
        private readonly string _connectionString;

        public Database()
        {
            var host = Environment.GetEnvironmentVariable("MARIADB_HOST");
            var port = Environment.GetEnvironmentVariable("MARIADB_PORT");
            var database = Environment.GetEnvironmentVariable("MARIADB_DATABASE");
            var user = Environment.GetEnvironmentVariable("MARIADB_API_USER");
            var password = Environment.GetEnvironmentVariable("MARIADB_API_PASSWORD");

            _connectionString = $"Server={host};Port={port};Database={database};User ID={user};Password={password};SslMode=Preferred;";
        }

        public async Task<bool> ExecuteNonQueryAsync(string query, Dictionary<string, object> parameters)
        {
            await using var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();

            await using var command = new MySqlCommand(query, connection);
            foreach (var param in parameters)
            {
                command.Parameters.AddWithValue(param.Key, param.Value);
            }

            return await command.ExecuteNonQueryAsync() > 0;
        }

        public async Task<long> ExecuteInsertAsync(string query, Dictionary<string, object> parameters)
        {
            await using var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();

            await using var command = new MySqlCommand(query, connection);

            foreach (var param in parameters)
            {
                command.Parameters.AddWithValue(param.Key, param.Value);
            }

            await command.ExecuteNonQueryAsync();
            return command.LastInsertedId;
        }

        public async Task<DataTable> ExecuteQueryAsync(string query, Dictionary<string, object> parameters)
        {
            await using var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();

            await using var command = new MySqlCommand(query, connection);
            foreach (var param in parameters)
            {
                command.Parameters.AddWithValue(param.Key, param.Value);
            }

            using var reader = await command.ExecuteReaderAsync();
            var dataTable = new DataTable();
            dataTable.Load(reader);
            return dataTable;
        }

        public async Task<object?> ExecuteScalarAsync(string query, Dictionary<string, object> parameters)
        {
            await using var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();

            await using var command = new MySqlCommand(query, connection);
            foreach (var param in parameters)
            {
                command.Parameters.AddWithValue(param.Key, param.Value);
            }

            return await command.ExecuteScalarAsync();
        }
    }
}