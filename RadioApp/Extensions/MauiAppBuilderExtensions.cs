using Core.Data;
using Core.Data.TypeHandlers;

using Dapper;

using Microsoft.Data.Sqlite;

namespace RadioApp.Extensions;

public static class MauiAppBuilderExtensions
{
    extension(MauiAppBuilder builder)
    {
        public MauiAppBuilder AddLocalDatabase()
        {
            SqlMapper.AddTypeHandler(new UriTypeHandler());

            var databasePath = Path.Combine(FileSystem.AppDataDirectory, "radioapp.db3");

            var connectionString = new SqliteConnectionStringBuilder
            {
                DataSource = databasePath,
                Mode = SqliteOpenMode.ReadWriteCreate,
                Cache = SqliteCacheMode.Shared
            }.ToString();

            builder.Services.AddSingleton(
                new SqliteConnectionFactory(connectionString));

            builder.Services.AddSingleton<DatabaseInitializer>();

            return builder;
        }
    }
}