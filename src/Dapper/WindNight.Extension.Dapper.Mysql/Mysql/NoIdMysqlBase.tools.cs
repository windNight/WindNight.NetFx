using System.Net.Sockets;
using Newtonsoft.Json.Extension;
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
        protected abstract string InsertTableColumns { get; }
        protected abstract string InsertTableColumnValues { get; }
        protected abstract string EqualEntityCondition { get; }

        protected virtual string BaseStatusColumns => $"Status,{BaseCUColumns}";
        protected virtual string BaseStatusColumnValues => $"@Status,{BaseCUColumnValues}";

        protected virtual string BaseCColumns => "CreateUserId,CreateDate,CreateUnixTime,IsDeleted";
        protected virtual string BaseCColumnValues => "@CreateUserId,@CreateDate,@CreateUnixTime,@IsDeleted";
        protected virtual string BaseCUColumns => $"{BaseCColumns},UpdateUserId,UpdateDate,UpdateUnixTime";

        protected virtual string BaseCUColumnValues => $"{BaseCColumnValues},@UpdateUserId,@UpdateDate,@UpdateUnixTime";

        protected string DefaultUpdateInfoFiled = "UpdateUserId=@UpdateUserId,UpdateUnixTime=@UpdateUnixTime,UpdateDate=@UpdateDate";
        protected virtual string QueryByUniqueKeySql => EqualEntityCondition.IsNullOrEmpty() ? "" : $"SELECT * FROM {BaseTableName} WHERE {EqualEntityCondition} ";

        protected virtual string ToBeUpdateFiled { get; }

        protected virtual string DefaultInsertOrUpdateSql =>
            @$"INSERT INTO {BaseTableName}({InsertTableColumns}) 
VALUES ({InsertTableColumnValues})
ON DUPLICATE KEY 
UPDATE {ToBeUpdateFiled}
;";




        protected abstract string Db { get; }

        protected abstract string GetConnStr();

        protected virtual string BaseTableName => typeof(TEntity).Name.ToLower();

        protected virtual string DbConnectString => GetConnStr();

        /// <summary>
        /// 使用自定义连接
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sqlFunc"></param>
        /// <param name="connectString"></param>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <param name="actionName"></param>
        /// <returns></returns>
        protected virtual T SqlTimer<T>(Func<string, string, object, T> sqlFunc, string connectString, string sql, object param = null,
            string actionName = "")
        {
            var ticks = DateTime.Now.Ticks;
            try
            {
                return sqlFunc(connectString, sql, param);
            }
            catch (Exception ex)
            {
                if (param is IEntity entity)
                {
                    LogHelper.Error($" sql执行报错 {(ConfigItems.IsLogConnectString ? $"【{connectString}】" : "")}  {actionName} Failed，{sql}.param is {entity.ToParamString()}", ex);
                }
                else
                {
                    LogHelper.Error($" sql执行报错 {(ConfigItems.IsLogConnectString ? $"【{connectString}】" : "")}  {actionName} Failed，{sql}.param is {param.ToJsonStr()}", ex);
                }

            }
            finally
            {
                try
                {
                    var milliseconds = (long)TimeSpan.FromTicks(DateTime.Now.Ticks - ticks).TotalMilliseconds;
                    if (milliseconds > ConfigItems.DapperWarnMs)
                    {
                        LogHelper.Warn($"sql执行耗时：{milliseconds} ms.{(ConfigItems.IsLogConnectString ? $"【{connectString}】" : "")}  sql:{sql}  {(milliseconds >= 100 ? $"param is {param.ToJsonStr()}" : "")}", millisecond: milliseconds);

                    }
                    else if (ConfigItems.OpenDapperLog)
                    {
                        LogHelper.Info($"sql执行耗时：{milliseconds} ms.{(ConfigItems.IsLogConnectString ? $"【{connectString}】" : "")} sql:{sql}  {(milliseconds >= 100 ? $"param is {param.ToJsonStr()}" : "")}", millisecond: milliseconds);
                    }
                }
                catch
                {

                }
            }

            return default;
        }

        /// <summary>
        ///  使用默认连接
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sqlFunc"></param>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <param name="actionName"></param>
        /// <returns></returns>
        protected virtual T SqlTimer<T>(Func<string, object, T> sqlFunc, string sql, object param = null,
            string actionName = "")
        {
            var ticks = DateTime.Now.Ticks;
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
                    var milliseconds = (long)TimeSpan.FromTicks(DateTime.Now.Ticks - ticks).TotalMilliseconds;
                    if (milliseconds > ConfigItems.DapperWarnMs)
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

        protected virtual async Task<T> SqlTimerAsync<T>(Func<string, string, object, Task<T>> sqlFunc, string connectString, string sql, object param = null,
            string actionName = "")
        {
            var ticks = DateTime.Now.Ticks;
            try
            {
                return await sqlFunc(connectString, sql, param);
            }
            catch (Exception ex)
            {
                if (param is IEntity entity)
                {
                    LogHelper.Error($" sql执行报错 {(ConfigItems.IsLogConnectString ? $"【{connectString}】" : "")}  {actionName} Failed，{sql}.param is {entity.ToParamString()}", ex);
                }
                else
                {
                    LogHelper.Error($" sql执行报错 {(ConfigItems.IsLogConnectString ? $"【{connectString}】" : "")}  {actionName} Failed，{sql}.param is {param.ToJsonStr()}", ex);
                }

            }
            finally
            {
                try
                {
                    var milliseconds = (long)TimeSpan.FromTicks(DateTime.Now.Ticks - ticks).TotalMilliseconds;
                    if (milliseconds > ConfigItems.DapperWarnMs)
                    {
                        LogHelper.Warn($"sql执行耗时：{milliseconds} ms.{(ConfigItems.IsLogConnectString ? $"【{connectString}】" : "")}  sql:{sql}  {(milliseconds >= 100 ? $"param is {param.ToJsonStr()}" : "")}", millisecond: milliseconds);

                    }
                    else if (ConfigItems.OpenDapperLog)
                    {
                        LogHelper.Info(
                            $"sql执行耗时：{milliseconds} ms.{(ConfigItems.IsLogConnectString ? $"【{connectString}】" : "")} sql:{sql}  {(milliseconds >= 100 ? $"param is {param.ToJsonStr()}" : "")}", millisecond: milliseconds);
                    }
                }
                catch
                {

                }
            }

            return default;
        }

        protected virtual async Task<T> SqlTimerAsync<T>(Func<string, object, Task<T>> sqlFunc, string sql, object param = null,
            string actionName = "")
        {
            var ticks = DateTime.Now.Ticks;
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
                    var milliseconds = (long)TimeSpan.FromTicks(DateTime.Now.Ticks - ticks).TotalMilliseconds;
                    if (milliseconds > ConfigItems.DapperWarnMs)
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

        protected virtual void DoRetryWhenHandlerSocketException(Action action, string actionName, int retryCount = 3)
        {
            var runCount = 0;
            var isRun = true;
            while (isRun)
                try
                {
                    runCount++;
                    action.Invoke();
                    if (runCount > 1) LogHelper.Warn($"[{actionName}] 经过[{runCount}]次重试后成功！");
                    break;
                }
                catch (Exception ex)
                {
                    if (ex is SocketException)
                    {
                        LogHelper.Warn($"[{actionName}] 第{runCount}次重试后失败并捕获异常SocketException！{ex.Message}", ex);
                        if (runCount >= retryCount)
                        {
                            LogHelper.Error(
                                $"[{actionName}] 第[{runCount}]次重试后达到重试次数上限[{retryCount}]次，将不再重试！{ex.Message}", ex);
                            isRun = false;
                        }

                        Thread.Sleep(1000 * 1);
                        continue;
                    }

                    isRun = false;
                    LogHelper.Error($"[{actionName}] 捕获未知异常，将不再重试！{ex.Message}", ex);
                }
        }


    }




}