using Dapper;

namespace Core.Data;

public sealed class DatabaseInitializer(SqliteConnectionFactory connectionFactory)
{
    private const string CreateFavoriteStationsTable = """
                                                       CREATE TABLE IF NOT EXISTS FavoriteStations
                                                       (
                                                           Id INTEGER PRIMARY KEY AUTOINCREMENT,
                                                           ExternalStationId TEXT NOT NULL UNIQUE CHECK (length(ExternalStationId) = 36),
                                                           Name TEXT NOT NULL,
                                                           StreamUrl TEXT NOT NULL CHECK (length(StreamUrl) <= 4096),
                                                           Favicon TEXT NULL CHECK (Favicon IS NULL OR length(Favicon) <= 4096),
                                                           Bitrate INTEGER NULL
                                                       );
                                                       """;

    private Task? _initializationTask;

    public Task InitializeAsync()
        => _initializationTask ??= InitializeDatabaseAsync();

    private async Task InitializeDatabaseAsync()
    {
        await using var connection = connectionFactory.CreateConnection();
        await connection.OpenAsync();
        await connection.ExecuteAsync(CreateFavoriteStationsTable);
    }
}