using System.Data;

namespace API.Services
{
    public class DatabaseCalls
    {
        private readonly List<string> _allowedTables;
        private readonly Dictionary<string, List<string>> _ForeignKeys;
        private readonly Database _db;

        public DatabaseCalls(Database database, IConfiguration config)
        {
            _db = database;

            var allowedTablesRaw = config["ALLOWED_TABLES"];
            if (string.IsNullOrWhiteSpace(allowedTablesRaw))
                throw new InvalidOperationException("Environment variable 'AllowedTables' is not set or is empty.");

            _allowedTables = [.. allowedTablesRaw.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)];

            var foreignKeysSection = config.GetSection("ForeignKeysPerTable");
            var foreignKeys = new Dictionary<string, List<string>>();

            foreach (var child in foreignKeysSection.GetChildren())
            {
                var tableName = child.Key;
                if (!_allowedTables.Contains(tableName))
                    throw new InvalidOperationException($"Foreign key definition for '{tableName}' is not allowed.");

                var columns = (child.Value ?? "")
                    .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                    .ToList();

                foreignKeys[tableName] = columns;
            }

            _ForeignKeys = foreignKeys;
        }

        public async Task<DataTable> GetFromTableAsync(string tableName, string? id = null)
        {
            if (string.IsNullOrEmpty(tableName) || !_allowedTables.Contains(tableName))
            {
                throw new ArgumentException("Table name cannot be null or empty.", nameof(tableName));
            }
            var parameters = new Dictionary<string, object>();

            var query = $"SELECT * FROM `{tableName}`";
            if (!string.IsNullOrEmpty(id))
            {
                query += " WHERE id = @id";
                parameters.Add("@id", id);
            }
            return await _db.ExecuteQueryAsync(query, parameters);
        }

        public async Task<DataTable> GetFromTableFilteredAsync(string tableName,Dictionary<string, object> filters)
        {
            if (string.IsNullOrEmpty(tableName) || !_allowedTables.Contains(tableName))
                throw new ArgumentException("Invalid table name.");

            if (!_ForeignKeys.TryGetValue(tableName, out var allowedColumns))
                throw new ArgumentException("No column definitions for table.");

            ArgumentNullException.ThrowIfNull(filters);

            var parameters = new Dictionary<string, object>();
            var whereClauses = new List<string>();

            foreach (var hash in filters)
            {
                if (!allowedColumns.Contains(hash.Key))
                    throw new ArgumentException($"Invalid filter column: {hash.Key}");

                string paramName = $"@{hash.Key}";
                whereClauses.Add($"{hash.Key} = {paramName}");
                parameters[paramName] = hash.Value;
            }

            var whereSql = whereClauses.Count > 0 ? " WHERE " + string.Join(" AND ", whereClauses) : "";
            var query = $"SELECT * FROM `{tableName}`{whereSql}";

            return await _db.ExecuteQueryAsync(query, parameters);
        }


        public async Task<long> InsertAsync(string tableName, Dictionary<string, object> data)
        {
            if (string.IsNullOrEmpty(tableName) || !_allowedTables.Contains(tableName))
                throw new ArgumentException("Invalid table name.");

            if (data == null || data.Count == 0)
                throw new ArgumentException("No data to insert.");

            var normalizedData = data.ToDictionary(
                hash => hash.Key.StartsWith("@") ? hash.Key : "@" + hash.Key,
                hash => hash.Value
            );


            var columns = string.Join(", ", normalizedData.Keys.Select(k => k.TrimStart('@')));
            var placeholders = string.Join(", ", normalizedData.Keys);


            var query = $"INSERT INTO `{tableName}` ({columns}) VALUES ({placeholders})";

            return await _db.ExecuteInsertAsync(query, data);
        }

        public async Task<bool> UpdateAsync(string tableName, string id, Dictionary<string, object> data)
        {
            if (string.IsNullOrEmpty(tableName) || !_allowedTables.Contains(tableName))
                throw new ArgumentException("Invalid table name.");
            if (string.IsNullOrEmpty(id))
                throw new ArgumentException("ID cannot be null or empty.", nameof(id));
            if (data == null || data.Count == 0)
                throw new ArgumentException("No data to update.");
            var setClause = string.Join(", ", data.Keys.Select(k => $"{k.TrimStart('@')} = {k}"));
            var query = $"UPDATE `{tableName}` SET {setClause} WHERE id = @id";
            data.Add("@id", id);
            return await _db.ExecuteNonQueryAsync(query, data);
        }

        public async Task<bool> DeleteAsync(string tableName, string id)
        {
            if (string.IsNullOrEmpty(tableName) || !_allowedTables.Contains(tableName))
                throw new ArgumentException("Invalid table name.");
            if (string.IsNullOrEmpty(id))
                throw new ArgumentException("ID cannot be null or empty.", nameof(id));
            var query = $"DELETE FROM `{tableName}` WHERE id = @id";
            var parameters = new Dictionary<string, object> { { "@id", id } };
            return await _db.ExecuteNonQueryAsync(query, parameters);
        }
    }
}
