using RepoDemos.Internal;
using WindNight.Core.SQL.Abstractions;
using WindNight.Extension.Dapper.Mysql;

namespace RepoDemos
{
    /// <inheritdoc cref="NoIdMysqlBase{TEntity}" />
    public abstract class DbNoIdBase<TEntity> : NoIdMysqlBase<TEntity>
        where TEntity : class, IEntity, new()
    {

        protected override string Db => "db1";

        protected override string GetConnStr()
        {
            var d = ConfigItems.DefaultDBConnectString;
            return d;
        }
    }
}
