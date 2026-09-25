using System.Collections.Immutable;

using Core.Data.Entities;
using Core.Interfaces;

using Dapper;

namespace Core.Data.Repositories;

public sealed class FavoriteStationRepository(
    SqliteConnectionFactory connectionFactory,
    DatabaseInitializer databaseInitializer) : IFavoriteStationRepository
{
    private const string GetAllSql = """
                                     SELECT
                                         Id,
                                         ExternalStationId,
                                         Name,
                                         StreamUrl,
                                         Favicon,
                                         Bitrate
                                     FROM FavoriteStations
                                     ORDER BY Id DESC;
                                     """;

    private const string AddSql = """
                                  INSERT INTO FavoriteStations
                                  (
                                      ExternalStationId,
                                      Name,
                                      StreamUrl,
                                      Favicon,
                                      Bitrate
                                  )
                                  VALUES
                                  (
                                      @ExternalStationId,
                                      @Name,
                                      @StreamUrl,
                                      @Favicon,
                                      @Bitrate
                                  )
                                  RETURNING Id;
                                  """;

    private const string RemoveSql = """
                                     DELETE FROM FavoriteStations
                                     WHERE Id = @Id;
                                     """;

    private const string ExistsSql = """
                                     SELECT EXISTS
                                     (
                                         SELECT 1
                                         FROM FavoriteStations
                                         WHERE ExternalStationId = @ExternalStationId
                                     );
                                     """;

    public async Task<ImmutableList<FavoriteStation>> GetAllAsync()
    {
        await databaseInitializer.InitializeAsync();

        await using var connection = connectionFactory.CreateConnection();
        await connection.OpenAsync();

        var stations = await connection.QueryAsync<FavoriteStation>(GetAllSql);

        return [.. stations];
    }

    public async Task<FavoriteStation> AddAsync(FavoriteStation station)
    {
        await databaseInitializer.InitializeAsync();

        await using var connection = connectionFactory.CreateConnection();
        await connection.OpenAsync();

        station.Id = await connection.ExecuteScalarAsync<int>(AddSql, station);

        return station;
    }

    public async Task<bool> RemoveAsync(int id)
    {
        await databaseInitializer.InitializeAsync();

        await using var connection = connectionFactory.CreateConnection();
        await connection.OpenAsync();

        var affectedRows = await connection.ExecuteAsync(RemoveSql, new { Id = id });

        return affectedRows > 0;
    }

    public async Task<bool> ExistsAsync(Guid externalStationId)
    {
        await databaseInitializer.InitializeAsync();

        await using var connection = connectionFactory.CreateConnection();
        await connection.OpenAsync();

        var exists =
            await connection.ExecuteScalarAsync<long>(ExistsSql, new { ExternalStationId = externalStationId });

        return exists == 1;
    }
}