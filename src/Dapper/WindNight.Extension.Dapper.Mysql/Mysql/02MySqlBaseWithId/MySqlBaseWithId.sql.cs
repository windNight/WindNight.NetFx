using Newtonsoft.Json.Extension;
using WindNight.Extension.Dapper.Mysql.@internal;
using WindNight.Linq.Extensions.Expressions;

namespace WindNight.Extension.Dapper.Mysql
{
    public abstract partial class MySqlBase<TEntity, TId>
    {


        protected virtual string QueryDataByIdSql => $"SELECT * FROM {BaseTableName} WHERE Id=@Id; ";

        protected virtual string QueryListByStatusSql => $"SELECT * FROM {BaseTableName} WHERE Status=@QueryStatus ";


        protected virtual string DeleteByIdSql => $@"UPDATE {BaseTableName} SET IsDeleted=1 WHERE Id=@Id; ";

        //protected override string QueryAllSqlCondition => $" IsDeleted=0 ";
        protected virtual string InsertWithIdSql => $"INSERT INTO {BaseTableName} (Id,{InsertTableColumns})  VALUES ( @Id,{InsertTableColumnValues}); ";


        protected virtual string InsertSql => $@"INSERT INTO {BaseTableName}({InsertTableColumns})
   VALUES ({InsertTableColumnValues});
   SELECT @@identity;";

        protected virtual string BatchInsertSql => $@"INSERT INTO {BaseTableName}({InsertTableColumns})
   VALUES ({InsertTableColumnValues})";

        /// <summary>
        ///     有些表 是没有 delete 字段的  暂不处理 delete 字段
        /// </summary>
        protected virtual string QueryAllSqlStr =>
            $"SELECT * FROM {BaseTableName} {(QueryAllSqlCondition.IsNullOrEmpty() ? "" : $" WHERE {QueryAllSqlCondition}")} ";

        protected string DefaultUpdateInfoFiled =
            "UpdateUserId=@UpdateUserId,UpdateUnixTime=@UpdateUnixTime,UpdateDate=@UpdateDate";




        protected virtual string QueryByUniqueKeySql => EqualEntityCondition.IsNullOrEmpty()
            ? ""
            : $"SELECT * FROM {BaseTableName} WHERE {EqualEntityCondition} ";

        protected virtual string UpdateByUniqueKeySql => EqualEntityCondition.IsNullOrEmpty() || ToBeUpdateFiled.IsNullOrEmpty()
            ? ""
            : $"UPDATE {BaseTableName} SET {ToBeUpdateFiled} WHERE {EqualEntityCondition} ";



        protected virtual string DefaultInsertOrUpdateSql =>
            @$"INSERT INTO {BaseTableName}({InsertTableColumns}) 
VALUES ({InsertTableColumnValues})
ON DUPLICATE KEY 
UPDATE {ToBeUpdateFiled}
;";




    }
}
