using System.Data;

using Dapper;

namespace Core.Data.TypeHandlers;

public sealed class UriTypeHandler : SqlMapper.TypeHandler<Uri>
{
    public override void SetValue(IDbDataParameter parameter, Uri? value)
    {
        parameter.DbType = DbType.String;
        parameter.Value = value is null ? DBNull.Value : value.AbsoluteUri;
    }

    public override Uri Parse(object value)
        => value switch
        {
            string uri => new Uri(uri, UriKind.Absolute),
            Uri uri => uri,
            _ => throw new DataException($"Cannot convert {value.GetType()?.Name} to Uri.")
        };
}