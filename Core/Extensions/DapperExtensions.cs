using Core.Data.TypeHandlers;

using Dapper;

using Microsoft.Extensions.DependencyInjection;

namespace Core.Extensions;

public static class DapperExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddDapperTypeHandlers()
        {
            SqlMapper.AddTypeHandler(new UriTypeHandler());

            return services;
        }
    }
}