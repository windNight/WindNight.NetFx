using WindNight.Core.SQL.Abstractions;

namespace WindNight.Extension.Dapper.Mysql
{
    ///<inheritdoc />
    public abstract class MySqlBase<TEntity> : MySqlBase<TEntity, int>
        where TEntity : class, IEntity<int>, new()
    {

    }



}
