using Newtonsoft.Json.Extension;
using WindNight.Core;
using WindNight.Core.SQL.Abstractions;
using WindNight.Extension.Dapper.Mysql.@internal;
using WindNight.Extension.Db.Extensions;

namespace WindNight.Extension.Dapper.Mysql
{
    /// <summary>
    ///     自定义基于Dapper的Mysql基类 通用方法
    /// </summary>
    public abstract partial class NoIdMysqlBase<TEntity>
    {

        protected abstract string Db { get; }


        protected abstract string BusinessColumns { get; }

        protected abstract string BusinessColumnValues { get; }


        protected abstract string InsertTableColumns { get; }
        protected abstract string InsertTableColumnValues { get; }

        protected abstract string EqualEntityCondition { get; }

        protected virtual string BaseTableName =>
            this.GenDefaultTableName<TEntity>(); // typeof(TEntity).Name.ToLower();

        protected virtual string DbConnectString => GetConnStr();




    }
}
