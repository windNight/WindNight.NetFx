using System.Net.Sockets;
using Newtonsoft.Json.Extension;
using WindNight.Core;
using WindNight.Core.SQL;
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
        protected abstract string BusinessColumns { get; }

        protected abstract string BusinessColumnValues { get; }


        protected abstract string InsertTableColumns { get; }
        protected abstract string InsertTableColumnValues { get; }

        protected abstract string EqualEntityCondition { get; }

        protected virtual string QueryAllSqlCondition => $" ";

        /// <summary>
        ///  有些表 是没有 delete 字段的  暂不处理 delete 字段
        /// </summary>
        protected virtual string QueryAllSqlStr => $"SELECT * FROM {BaseTableName} {(QueryAllSqlCondition.IsNullOrEmpty() ? "" : $" WHERE {QueryAllSqlCondition}")} ";


        protected virtual string BaseTreeColumns => $"ParentId,{BaseStatusColumns}";
        protected virtual string BaseTreeColumnValues => $"@ParentId,{BaseStatusColumnValues}";

        protected virtual string BaseStatusColumns => $"Status,{BaseCUColumns}";
        protected virtual string BaseStatusColumnValues => $"@Status,{BaseCUColumnValues}";

        protected virtual string BaseCColumns => "CreateUserId,CreateDate,CreateUnixTime,IsDeleted";
        protected virtual string BaseCColumnValues => "@CreateUserId,@CreateDate,@CreateUnixTime,@IsDeleted";
        protected virtual string BaseCUColumns => $"{BaseCColumns},UpdateUserId,UpdateDate,UpdateUnixTime";

        protected virtual string BaseCUColumnValues => $"{BaseCColumnValues},@UpdateUserId,@UpdateDate,@UpdateUnixTime";

        protected string DefaultUpdateInfoFiled = "UpdateUserId=@UpdateUserId,UpdateUnixTime=@UpdateUnixTime,UpdateDate=@UpdateDate";

        protected virtual string QueryByUniqueKeySql => EqualEntityCondition.IsNullOrEmpty() ? "" : $"SELECT * FROM {BaseTableName} WHERE {EqualEntityCondition} ";

        protected virtual string UpdateByUniqueKeySql => EqualEntityCondition.IsNullOrEmpty()
            ? ""
            : $"UPDATE {BaseTableName} SET {ToBeUpdateFiled} WHERE {EqualEntityCondition} ";


        protected virtual string ToBeUpdateFiled { get; }

        protected virtual string DefaultInsertOrUpdateSql =>
            @$"INSERT INTO {BaseTableName}({InsertTableColumns}) 
VALUES ({InsertTableColumnValues})
ON DUPLICATE KEY 
UPDATE {ToBeUpdateFiled}
;";
         


        protected abstract string Db { get; }

        protected abstract string GetConnStr();

        protected virtual string BaseTableName => this.GenDefaultTableName<TEntity>(toLower: true, appendPlural: false); // typeof(TEntity).Name.ToLower();

        protected virtual string DbConnectString => GetConnStr();

        /// <summary>
        ///  使用默认连接
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sqlFunc"></param>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <param name="actionName"></param>
        /// <param name="warnMs"></param>
        /// <returns></returns>
        protected virtual T SqlTimer<T>(Func<string, object, T> sqlFunc, string sql, object param = null,
            string actionName = "", long warnMs = -1)
        {

            var ticks = HardInfo.Now.Ticks;
            try
            {
                return sqlFunc(sql, param);
            }
            catch (Exception ex)
            {
                if (param is IEntity entity)
                {
                    LogHelper.Error($" sql执行报错 {(ConfigItems.IsLogConnectString ? $"【{DbConnectString}】" : "")}  {actionName} Failed，{sql}.param is {entity.ToParamString()}", ex);
                }
                else
                {
                    LogHelper.Error($" sql执行报错 {(ConfigItems.IsLogConnectString ? $"【{DbConnectString}】" : "")}  {actionName} Failed，{sql}.param is {param.ToJsonStr()}", ex);
                }

            }
            finally
            {
                try
                {
                    var milliseconds = (long)TimeSpan.FromTicks(HardInfo.Now.Ticks - ticks).TotalMilliseconds;
                    warnMs = FixWarnMs(warnMs);
                    if (milliseconds > warnMs)
                    {
                        LogHelper.Warn($"sql执行耗时：{milliseconds} ms.{(ConfigItems.IsLogConnectString ? $"【{DbConnectString}】" : "")}  sql:{sql}  {(milliseconds >= 100 ? $"param is {param.ToJsonStr()}" : "")}", millisecond: milliseconds);

                    }
                    else if (ConfigItems.OpenDapperLog)
                    {
                        LogHelper.Info(
                            $"sql执行耗时：{milliseconds} ms.{(ConfigItems.IsLogConnectString ? $"【{DbConnectString}】" : "")} sql:{sql}  {(milliseconds >= 100 ? $"param is {param.ToJsonStr()}" : "")}", millisecond: milliseconds);
                    }
                }
                catch
                {

                }
            }

            return default;


        }

         
        protected virtual async Task<T> SqlTimerAsync<T>(Func<string, object, Task<T>> sqlFunc, string sql, object param = null,
            string actionName = "", long warnMs = -1)
        {
            var ticks = HardInfo.Now.Ticks;
            try
            {
                return await sqlFunc(sql, param);
            }
            catch (Exception ex)
            {
                if (param is IEntity entity)
                {
                    LogHelper.Error($" sql执行报错 {(ConfigItems.IsLogConnectString ? $"【{DbConnectString}】" : "")}  {actionName} Failed，{sql}.param is {entity.ToParamString()}", ex);
                }
                else
                {
                    LogHelper.Error($" sql执行报错 {(ConfigItems.IsLogConnectString ? $"【{DbConnectString}】" : "")}  {actionName} Failed，{sql}.param is {param.ToJsonStr()}", ex);
                }
            }
            finally
            {
                try
                {
                    var milliseconds = (long)TimeSpan.FromTicks(HardInfo.Now.Ticks - ticks).TotalMilliseconds;
                    warnMs = FixWarnMs(warnMs);
                    if (milliseconds > warnMs)
                    {
                        LogHelper.Warn($"sql执行耗时：{milliseconds} ms.{(ConfigItems.IsLogConnectString ? $"【{DbConnectString}】" : "")}  sql:{sql}  {(milliseconds >= 100 ? $"param is {param.ToJsonStr()}" : "")}", millisecond: milliseconds);

                    }
                    else if (ConfigItems.OpenDapperLog)
                    {
                        LogHelper.Info(
                            $"sql执行耗时：{milliseconds} ms.{(ConfigItems.IsLogConnectString ? $"【{DbConnectString}】" : "")} sql:{sql}  {(milliseconds >= 100 ? $"param is {param.ToJsonStr()}" : "")}", millisecond: milliseconds);
                    }
                }
                catch
                {

                }
            }

            return default;

        }



    }




}
