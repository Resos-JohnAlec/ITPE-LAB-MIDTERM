using Microsoft.Data.SqlClient;

namespace CampusEvents;

public static class DatabaseBootstrap
{
    public static async Task InitializeAsync(string connectionString, bool seed)
    {
        var settings = new SqlConnectionStringBuilder(connectionString);
        var databaseName = settings.InitialCatalog;
        if (string.IsNullOrWhiteSpace(databaseName) || databaseName.Length > 128)
            throw new InvalidOperationException("Configure a dedicated database name.");
        if (new[] { "master", "model", "msdb", "tempdb" }.Contains(databaseName, StringComparer.OrdinalIgnoreCase))
            throw new InvalidOperationException("Use a dedicated application database, not a system database.");

        settings.InitialCatalog = "master";
        await using (var connection = new SqlConnection(settings.ConnectionString))
        {
            await connection.OpenAsync();
            // Identifiers cannot be SQL parameters: quote the configured identifier.
            var quotedName = "[" + databaseName.Replace("]", "]]", StringComparison.Ordinal) + "]";
            await using var command = new SqlCommand(
                $"IF DB_ID(@name) IS NULL CREATE DATABASE {quotedName};", connection);
            command.Parameters.Add("@name", System.Data.SqlDbType.NVarChar, 128).Value = databaseName;
            await command.ExecuteNonQueryAsync();
        }

        await using var database = new SqlConnection(connectionString);
        await database.OpenAsync();
        foreach (var file in seed ? new[] { "schema.sql", "seed.sql" } : new[] { "schema.sql" })
        {
            var sql = await File.ReadAllTextAsync(Path.Combine(AppContext.BaseDirectory, "database", file));
            await using var command = new SqlCommand(sql, database);
            await command.ExecuteNonQueryAsync();
        }
    }
}
