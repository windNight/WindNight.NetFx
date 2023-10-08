using System;
using RepoDemos.Internal;
using WindNight.Core.SQL.Abstractions;
using WindNight.Extension.Dapper.Mysql;

namespace RepoDemos
{
    public abstract class DbBase<TEntity> : DbBase<TEntity, int>
        where TEntity : class, IEntity<int>, new()
    {
        //        protected string DefaultUpdateInfoFiled =
        //            "UpdateUserId=@UpdateUserId,UpdateUnixTime=@UpdateUnixTime,UpdateDate=@UpdateDate";

        //        protected virtual string ToBeUpdateFiled { get; }

        //        protected string InsertOrUpdateSql => @$"INSERT INTO
        //{BaseTableName}({InsertTableColumns})
        //VALUES ({InsertTableColumnValues})
        //ON DUPLICATE KEY
        //UPDATE {ToBeUpdateFiled}
        //;";

    }

    public abstract partial class DbBase<TEntity, TId>
    {
    }

    public abstract partial class DbBase<TEntity, TId> : MySqlBase<TEntity, TId>
        where TEntity : class, IEntity<TId>, new()
        where TId : IEquatable<TId>, IComparable<TId>
    {
        protected override string Db => "engridplcdb";

        protected override string QueryAllSqlStr => $"SELECT * FROM {BaseTableName} WHERE IsDeleted=0;";

        protected override string GetConnStr()
        {
            var d = ConfigItems.DefaultDBConnectString;
            return d;
        }
    }

    public abstract class DbTreeBase<TEntity> : DbTreeBase<TEntity, int>
        where TEntity : class, ITreeEntity<int>, IEntity<int>, new()
    {

        protected override string Db => "testdb1";
    }

    public abstract partial class DbTreeBase<TEntity, TId>
    {
    }

    public abstract partial class DbTreeBase<TEntity, TId> : MySqlTreeBase<TEntity, TId>
        where TEntity : class, ITreeEntity<TId>, IEntity<TId>, new()
        where TId : IEquatable<TId>, IComparable<TId>
    {

        protected override string QueryAllSqlStr => $"SELECT * FROM {BaseTableName} WHERE IsDeleted=0;";

        protected override string GetConnStr()
        {
            var d = ConfigItems.DefaultDBConnectString;
            return d;
        }

    }
}
