using System.Data;

using Dapper;

namespace Core.Data.TypeHandlers;

public sealed class GuidTypeHandler : SqlMapper.TypeHandler<Guid>
{
    public override void SetValue(IDbDataParameter parameter, Guid value)
    {
        parameter.DbType = DbType.Guid;
        parameter.Value = value;
    }

    public override Guid Parse(object value)
        => value switch
        {
            Guid guid => guid,
            string guid => Guid.Parse(guid),
            _ => throw new DataException($"Cannot convert {value.GetType()?.Name} to Guid.")
        };
}