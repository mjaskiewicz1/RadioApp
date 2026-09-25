using Microsoft.Data.Sqlite;

namespace Core.Data;

public sealed class SqliteConnectionFactory(string connectionString)
{
    public SqliteConnection CreateConnection()
        => new(connectionString);
}