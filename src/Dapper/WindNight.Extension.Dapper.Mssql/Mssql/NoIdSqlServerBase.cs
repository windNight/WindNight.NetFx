using System.Net.Sockets;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.WnExtension;
using Newtonsoft.Json.Extension;
using WindNight.Core.Abstractions;
using WindNight.Core.SQL;
using WindNight.Core.SQL.Abstractions;
using WindNight.Extension.Dapper.Mssql.@internal;

namespace WindNight.Extension.Dapper.Mssql
{
    /// <summary>
    ///     自定义基于Dapper的Mssql基类
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <inheritdoc />
    public abstract partial class NoIdSqlServerBase<TEntity> : SqlServerBase
        where TEntity : class, IEntity, new()
    {
        #region Sync

        /// <summary>
        ///     查询自定义对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        protected virtual T DbQueryE<T>(string sql, object param = null,
            Action<Exception, string> execErrorHandler = null)
        {
            return SqlTimer((_sql, _param, _3)
                    => Query<T>(DbConnectString, _sql, _param, _3),
                sql, param, nameof(DbQueryE), execErrorHandler);
        }

        protected virtual T DbQueryE<T>(string conn, string sql, object param = null,
            Action<Exception, string> execErrorHandler = null)
        {
            return SqlTimer(Query<T>,
                conn, sql, param, nameof(DbQueryE), execErrorHandler);
        }

        /// <summary>
        ///     查询自定义对象列表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        protected virtual IEnumerable<T> DbQueryEList<T>(string sql, object param = null,
            Action<Exception, string> execErrorHandler = null)
        {
            return SqlTimer((_sql, _param, _3)
                    => QueryList<T>(DbConnectString, _sql, _param, _3),
                sql, param, nameof(DbQueryEList), execErrorHandler);
        }

        protected virtual IEnumerable<T> DbQueryEList<T>(string conn, string sql, object param = null,
            Action<Exception, string> execErrorHandler = null)
        {
            return SqlTimer(QueryList<T>,
                conn, sql, param, nameof(DbQueryEList), execErrorHandler);
        }

        /// <summary>
        ///     查询实体对象
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        protected virtual TEntity DbQuery(string sql, object param = null,
            Action<Exception, string> execErrorHandler = null)
        {
            return SqlTimer((_sql, _param, _3)
                    => Query<TEntity>(DbConnectString, _sql, _param, _3),
                sql, param, nameof(DbQuery), execErrorHandler);
        }

        protected virtual TEntity DbQuery(string conn, string sql, object param = null,
            Action<Exception, string> execErrorHandler = null)
        {
            return SqlTimer(Query<TEntity>,
                conn, sql, param, nameof(DbQuery), execErrorHandler);
        }

        /// <summary>
        ///     查询实体对象列表
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        protected virtual IEnumerable<TEntity> DbQueryList(string sql, object param = null,
            Action<Exception, string> execErrorHandler = null)
        {
            return SqlTimer((_sql, _param, _3)
                    => QueryList<TEntity>(DbConnectString, _sql, _param, _3),
                sql, param, nameof(DbQueryList), execErrorHandler);
        }

        protected virtual IEnumerable<TEntity> DbQueryList(string conn, string sql, object param = null,
            Action<Exception, string> execErrorHandler = null)
        {
            return SqlTimer(QueryList<TEntity>,
                conn, sql, param, nameof(DbQueryList), execErrorHandler);
        }

        /// <summary>
        ///     执行受影响行数
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        protected int DbExecute(string sql, object param = null, Action<Exception, string> execErrorHandler = null)
        {
            return SqlTimer((_sql, _param, _3)
                    => Execute(DbConnectString, _sql, _param, _3),
                sql, param, nameof(DbExecute), execErrorHandler);
        }

        protected int DbExecute(string conn, string sql, object param = null,
            Action<Exception, string> execErrorHandler = null)
        {
            return SqlTimer(Execute,
                conn, sql, param, nameof(DbExecute), execErrorHandler);
        }

        /// <summary>
        ///     首行首列数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        protected T DbExecuteScalar<T>(string sql, object param = null,
            Action<Exception, string> execErrorHandler = null)
        {
            return SqlTimer((_sql, _param, _3)
                    => ExecuteScalar<T>(DbConnectString, _sql, _param, _3),
                sql, param, nameof(DbExecuteScalar), execErrorHandler);
        }

        protected T DbExecuteScalar<T>(string conn, string sql, object param = null,
            Action<Exception, string> execErrorHandler = null)
        {
            return SqlTimer(ExecuteScalar<T>,
                conn, sql, param, nameof(DbExecuteScalar), execErrorHandler);
        }

        /// <summary>
        ///     分页
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="condition"></param>
        /// <param name="orderBy"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        protected virtual IPagedList<TEntity> DbPagedList(int pageIndex, int pageSize, string condition,
            string orderBy, IDictionary<string, object> parameters = null,
            Action<Exception, string> execErrorHandler = null)
        {
            var pagedInfo = new QueryPageInfo
            {
                TableName = BaseTableName,
                Fields = "*",
                SqlWhere = condition,
                OrderField = orderBy,
                PageIndex = pageIndex,
                PageSize = pageSize,
            };

            return PagedList<TEntity>(DbConnectString, pagedInfo, parameters, execErrorHandler: execErrorHandler);
        }

        #endregion //end Sync

        #region Async

        /// <summary>
        ///     查询自定义对象 Async
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        protected virtual async Task<T> DbQueryEAsync<T>(string sql, object param = null,
            Action<Exception, string> execErrorHandler = null)
        {
            return await SqlTimerAsync(async (_1, _2, _3)
                    => await QueryAsync<T>(DbConnectString, _1, _2, _3),
                sql, param, nameof(DbQueryEAsync), execErrorHandler);
        }

        protected virtual async Task<T> DbQueryEAsync<T>(string conn, string sql, object param = null,
            Action<Exception, string> execErrorHandler = null)
        {
            return await SqlTimerAsync(async (_0, _1, _2, _3)
                    => await QueryAsync<T>(_0, _1, _2, _3), conn, sql, param,
                nameof(DbQueryEAsync), execErrorHandler);
        }


        /// <summary>
        ///     查询自定义对象列表 Async
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        protected virtual async Task<IEnumerable<T>> DbQueryEListAsync<T>(string sql, object param = null,
            Action<Exception, string> execErrorHandler = null)
        {
            return await SqlTimerAsync(
                async (_sql, _param, _3)
                    => await QueryListAsync<T>(DbConnectString, _sql, _param, _3),
                sql, param, nameof(DbQueryEListAsync), execErrorHandler);
        }

        protected virtual async Task<IEnumerable<T>> DbQueryEListAsync<T>(string conn, string sql, object param = null,
            Action<Exception, string> execErrorHandler = null)
        {
            return await SqlTimerAsync(async (_0, _sql, _param, _3)
                    => await QueryListAsync<T>(_0, _sql, _param, _3),
                conn, sql, param, nameof(DbQueryEListAsync), execErrorHandler);
        }

        /// <summary>
        ///     查询实体对象 Async
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        protected virtual async Task<TEntity> DbQueryAsync(string sql, object param = null,
            Action<Exception, string> execErrorHandler = null)
        {
            return await SqlTimerAsync(
                async (_sql, _param, _3)
                    => await QueryAsync<TEntity>(DbConnectString, _sql, _param, _3),
                sql, param, nameof(DbQueryAsync), execErrorHandler);
        }

        protected virtual async Task<TEntity> DbQueryAsync(string conn, string sql, object param = null,
            Action<Exception, string> execErrorHandler = null)
        {
            return await SqlTimerAsync(async (_0, _sql, _param, _3)
                    => await QueryAsync<TEntity>(_0, _sql, _param, _3),
                conn, sql, param, nameof(DbQueryAsync), execErrorHandler);
        }


        /// <summary>
        ///     查询实体对象列表 Async
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        protected virtual async Task<IEnumerable<TEntity>> DbQueryListAsync(string sql, object param = null,
            Action<Exception, string> execErrorHandler = null)
        {
            return await SqlTimerAsync(
                async (_sql, _param, _3)
                    => await QueryListAsync<TEntity>(DbConnectString, _sql, _param, _3),
                sql, param, nameof(DbQueryList), execErrorHandler);
        }

        protected virtual async Task<IEnumerable<TEntity>> DbQueryListAsync(string conn, string sql,
            object param = null, Action<Exception, string> execErrorHandler = null)
        {
            return await SqlTimerAsync(
                async (_0, _sql, _param, _3)
                    => await QueryListAsync<TEntity>(_0, _sql, _param, _3), conn, sql, param,
                nameof(DbQueryList), execErrorHandler);
        }

        /// <summary>
        ///     执行受影响行数 Async
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        protected async Task<int> DbExecuteAsync(string sql, object param = null,
            Action<Exception, string> execErrorHandler = null)
        {
            return await SqlTimerAsync(
                async (_sql, _param, _3)
                    => await ExecuteAsync(DbConnectString, _sql, _param, _3),
                sql, param, nameof(DbExecuteAsync), execErrorHandler);
        }

        protected async Task<int> DbExecuteAsync(string conn, string sql, object param = null,
            Action<Exception, string> execErrorHandler = null)
        {
            return await SqlTimerAsync(async (_0, _sql, _param, _3)
                    => await ExecuteAsync(_0, _sql, _param, _3), conn,
                sql, param, nameof(DbExecuteAsync), execErrorHandler);
        }


        /// <summary>
        ///     首行首列数据 Async
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        protected async Task<T> DbExecuteScalarAsync<T>(string sql, object param = null,
            Action<Exception, string> execErrorHandler = null)
        {
            return await SqlTimerAsync(
                async (_sql, _param, _3)
                    => await ExecuteScalarAsync<T>(DbConnectString, _sql, _param, _3),
                sql, param, nameof(DbExecuteScalarAsync), execErrorHandler);
        }

        protected async Task<T> DbExecuteScalarAsync<T>(string conn, string sql, object param = null,
            Action<Exception, string> execErrorHandler = null)
        {
            return await SqlTimerAsync(
                async (_0, _sql, _param, _3)
                    => await ExecuteScalarAsync<T>(_0, _sql, _param, _3), conn, sql, param,
                nameof(DbExecuteScalarAsync), execErrorHandler);
        }

        /// <summary>
        ///     异步分页
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="condition"></param>
        /// <param name="orderBy"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        protected virtual async Task<IPagedList<TEntity>> DbPagedListAsync(int pageIndex,
            int pageSize, string condition, string orderBy, IDictionary<string, object> parameters = null,
            Action<Exception, string> execErrorHandler = null)
        {
            var pagedInfo = new QueryPageInfo
            {
                TableName = BaseTableName,
                Fields = "*",
                SqlWhere = condition,
                OrderField = orderBy,
                PageIndex = pageIndex,
                PageSize = pageSize,
            };

            return await PagedListAsync<TEntity>(DbConnectString, pagedInfo, parameters);
        }

        #endregion
    }


    /// <summary>
    ///     自定义基于Dapper的Mssql基类 通用方法
    /// </summary>
    public abstract partial class NoIdSqlServerBase<TEntity>
    {
        protected abstract string InsertTableColumns { get; }
        protected abstract string InsertTableColumnValues { get; }
        protected abstract string EqualEntityCondition { get; }

        protected abstract string Db { get; }

        protected virtual string BaseTableName => typeof(TEntity).Name.ToLower();

        protected virtual string DbConnectString => GetConnStr();

        protected abstract string GetConnStr();

        protected T SqlTimer<T>(Func<string, object, Action<Exception, string>, T> sqlFunc, string sql,
            object param = null,
            string actionName = "", Action<Exception, string> execErrorHandler = null)
        {
            var ticks = HardInfo.Now.Ticks;
            try
            {
                return sqlFunc(sql, param, execErrorHandler);
            }
            catch (Exception ex)
            {
                LogHelper.Error(
                    $" sql执行报错 {(ConfigItems.IsLogConnectString ? $"【{DbConnectString}】" : "")}  {actionName} Failed，{sql} .param is {param.ToJsonStr()}",
                    ex);
            }
            finally
            {
                try
                {
                    var milliseconds = (long)TimeSpan.FromTicks(HardInfo.Now.Ticks - ticks).TotalMilliseconds;
                    if (milliseconds > ConfigItems.DapperWarnMs)
                    {
                        LogHelper.Warn(
                            $"sql执行耗时：{milliseconds} ms.{(ConfigItems.IsLogConnectString ? $"【{DbConnectString}】" : "")}  sql:{sql}  {(milliseconds >= 100 ? $"param is {param.ToJsonStr()}" : "")}",
                            millisecond: milliseconds);
                    }
                    else if (ConfigItems.OpenDapperLog)
                    {
                        LogHelper.Info(
                            $"sql执行耗时：{milliseconds} ms.{(ConfigItems.IsLogConnectString ? $"【{DbConnectString}】" : "")} sql:{sql}  {(milliseconds >= 100 ? $"param is {param.ToJsonStr()}" : "")}",
                            milliseconds);
                    }
                }
                catch
                {
                }
            }

            return default;
        }


        protected async Task<T> SqlTimerAsync<T>(Func<string, object, Action<Exception, string>, Task<T>> sqlFunc,
            string sql, object param = null,
            string actionName = "", Action<Exception, string> execErrorHandler = null)
        {
            var ticks = HardInfo.Now.Ticks;
            try
            {
                return await sqlFunc(sql, param, execErrorHandler);
            }
            catch (Exception ex)
            {
                LogHelper.Error(
                    $" sql执行报错 {(ConfigItems.IsLogConnectString ? $"【{DbConnectString}】" : "")}  {actionName} Failed，{sql}.param is {param.ToJsonStr()}",
                    ex);
            }
            finally
            {
                try
                {
                    var milliseconds = (long)TimeSpan.FromTicks(HardInfo.Now.Ticks - ticks).TotalMilliseconds;
                    if (milliseconds > ConfigItems.DapperWarnMs)
                    {
                        LogHelper.Warn(
                            $"sql执行耗时：{milliseconds} ms.{(ConfigItems.IsLogConnectString ? $"【{DbConnectString}】" : "")}  sql:{sql}  {(milliseconds >= 100 ? $"param is {param.ToJsonStr()}" : "")}",
                            millisecond: milliseconds);
                    }
                    else if (ConfigItems.OpenDapperLog)
                    {
                        LogHelper.Info(
                            $"sql执行耗时：{milliseconds} ms.{(ConfigItems.IsLogConnectString ? $"【{DbConnectString}】" : "")} sql:{sql}  {(milliseconds >= 100 ? $"param is {param.ToJsonStr()}" : "")}",
                            milliseconds);
                    }
                }
                catch
                {
                }
            }

            return default;
        }


        protected T SqlTimer<T>(Func<string, string, object, Action<Exception, string>, T> sqlFunc,
            string connectString, string sql, object param = null,
            string actionName = "", Action<Exception, string> execErrorHandler = null)
        {
            var ticks = HardInfo.Now.Ticks;
            try
            {
                return sqlFunc(connectString, sql, param, execErrorHandler);
            }
            catch (Exception ex)
            {
                LogHelper.Error(
                    $" sql执行报错 {(ConfigItems.IsLogConnectString ? $"【{DbConnectString}】" : "")}  {actionName} Failed，{sql}.param is {param.ToJsonStr()}",
                    ex);
            }
            finally
            {
                try
                {
                    var milliseconds = (long)TimeSpan.FromTicks(HardInfo.Now.Ticks - ticks).TotalMilliseconds;
                    if (milliseconds > ConfigItems.DapperWarnMs)
                    {
                        LogHelper.Warn(
                            $"sql执行耗时：{milliseconds} ms.{(ConfigItems.IsLogConnectString ? $"【{DbConnectString}】" : "")}  sql:{sql}  {(milliseconds >= 100 ? $"param is {param.ToJsonStr()}" : "")}",
                            millisecond: milliseconds);
                    }
                    else if (ConfigItems.OpenDapperLog)
                    {
                        LogHelper.Info(
                            $"sql执行耗时：{milliseconds} ms.{(ConfigItems.IsLogConnectString ? $"【{DbConnectString}】" : "")} sql:{sql}  {(milliseconds >= 100 ? $"param is {param.ToJsonStr()}" : "")}",
                            milliseconds);
                    }
                }
                catch
                {
                }
            }

            return default;
        }


        protected async Task<T> SqlTimerAsync<T>(
            Func<string, string, object, Action<Exception, string>, Task<T>> sqlFunc, string connectString, string sql,
            object param = null,
            string actionName = "", Action<Exception, string> execErrorHandler = null)
        {
            var ticks = HardInfo.Now.Ticks;
            try
            {
                return await sqlFunc(connectString, sql, param, execErrorHandler);
            }
            catch (Exception ex)
            {
                LogHelper.Error(
                    $" sql执行报错 {(ConfigItems.IsLogConnectString ? $"【{DbConnectString}】" : "")} {actionName} Failed，{sql}.param is {param.ToJsonStr()}",
                    ex);
            }
            finally
            {
                try
                {
                    var milliseconds = (long)TimeSpan.FromTicks(HardInfo.Now.Ticks - ticks).TotalMilliseconds;
                    if (milliseconds > ConfigItems.DapperWarnMs)
                    {
                        LogHelper.Warn(
                            $"sql执行耗时：{milliseconds} ms.{(ConfigItems.IsLogConnectString ? $"【{DbConnectString}】" : "")}  sql:{sql}  {(milliseconds >= 100 ? $"param is {param.ToJsonStr()}" : "")}",
                            millisecond: milliseconds);
                    }
                    else if (ConfigItems.OpenDapperLog)
                    {
                        LogHelper.Info(
                            $"sql执行耗时：{milliseconds} ms.{(ConfigItems.IsLogConnectString ? $"【{DbConnectString}】" : "")} sql:{sql}  {(milliseconds >= 100 ? $"param is {param.ToJsonStr()}" : "")}",
                            milliseconds);
                    }
                }
                catch
                {
                }
            }

            return default;
        }

        protected void DoRetryWhenHandlerSocketException(Action action, string actionName, int retryCount = 3)
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

        internal class ConfigItems //: ConfigItemsBase
        {
            public static bool OpenDapperLog
            {
                get
                {
                    var config = Ioc.GetService<IConfigService>();
                    if (config == null) return false;
                    return config.GetAppSetting(ConfigItemsKey.OpenDapperLogKey, false, false);
                }
            }

            public static bool IsLogConnectString
            {
                get
                {
                    var config = Ioc.Instance.CurrentConfigService;
                    if (config == null) return false;
                    return config.GetAppSetting(ConfigItemsKey.IsLogConnectStringKey, false, false);
                }
            }

            public static int DapperWarnMs => GetConfigIntValue(ConfigItemsKey.DapperWarnMsKey, 100);

            private static int GetConfigIntValue(string key, int defaultValue = 0)
            {
                try
                {
                    var config = Ioc.Instance.CurrentConfigService;
                    if (config == null) return defaultValue;
                    var value1 = config.Configuration.GetSection(key).Get<int>();
                    var value2 = config.GetAppSetting(key, defaultValue, false);
                    return Math.Max(value2, value1);
                }
                catch (Exception ex)
                {
                    return defaultValue;
                }
            }

            private static class ConfigItemsKey
            {
                internal static readonly string OpenDapperLogKey = "DapperConfig:OpenDapperLog";
                internal static readonly string IsLogConnectStringKey = "DapperConfig:IsLogConnectString";
                internal static readonly string DapperWarnMsKey = "DapperConfig:WarnMs";
            }
        }
    }
}
