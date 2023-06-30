using System.Diagnostics;
using Microsoft.Extensions.DependencyInjection.WnExtension;
using Newtonsoft.Json.Extension;
using System.Net.Sockets;
using System.Reflection;
using WindNight.Core.Abstractions;
using WindNight.Core.SQL;
using WindNight.Core.SQL.Abstractions;
using WindNight.Extension.Dapper.Internals;

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
        /// 查询自定义对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        protected virtual T DbQueryE<T>(string sql, object param = null)
        {
            return SqlTimer((_sql, _param) => Query<T>(DbConnectString, _sql, _param),
                sql, param, nameof(DbQueryE));
        }
        protected virtual T DbQueryE<T>(string conn, string sql, object param = null)
        {
            return SqlTimer(Query<T>, conn, sql, param, nameof(DbQueryE));
        }

        /// <summary>
        /// 查询自定义对象列表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        protected virtual IEnumerable<T> DbQueryEList<T>(string sql, object param = null)
        {
            return SqlTimer((_sql, _param) => QueryList<T>(DbConnectString, _sql, _param),
                sql, param, nameof(DbQueryEList));
        }
        protected virtual IEnumerable<T> DbQueryEList<T>(string conn, string sql, object param = null)
        {
            return SqlTimer(QueryList<T>, conn, sql, param, nameof(DbQueryEList));
        }

        /// <summary>
        /// 查询实体对象
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        protected virtual TEntity DbQuery(string sql, object param = null)
        {
            return SqlTimer((_sql, _param) => Query<TEntity>(DbConnectString, _sql, _param),
                sql, param, nameof(DbQuery));
        }
       
        protected virtual TEntity DbQuery(string conn, string sql, object param = null)
        {
            return SqlTimer(Query<TEntity>, conn, sql, param, nameof(DbQuery));
        }

        /// <summary>
        /// 查询实体对象列表
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        protected virtual IEnumerable<TEntity> DbQueryList(string sql, object param = null)
        {
            return SqlTimer((_sql, _param) => QueryList<TEntity>(DbConnectString, _sql, _param),
                sql, param, nameof(DbQueryList));
        }
       
        protected virtual IEnumerable<TEntity> DbQueryList(string conn, string sql, object param = null)
        {
            return SqlTimer(QueryList<TEntity>, conn, sql, param, nameof(DbQueryList));
        }
        
        /// <summary>
        /// 执行受影响行数
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        protected int DbExecute(string sql, object param = null)
        {
            return SqlTimer((_sql, _param) => Execute(DbConnectString, _sql, _param),
                sql, param, nameof(DbExecute));
        }

        protected int DbExecute(string conn, string sql, object param = null)
        {
            return SqlTimer(Execute, conn, sql, param, nameof(DbExecute));
        }

        /// <summary>
        /// 首行首列数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        protected T DbExecuteScalar<T>(string sql, object param = null)
        {
            return SqlTimer((_sql, _param) => ExecuteScalar<T>(DbConnectString, _sql, _param),
                sql, param, nameof(DbExecuteScalar));
        }
       
        protected T DbExecuteScalar<T>(string conn, string sql, object param = null)
        {
            return SqlTimer(ExecuteScalar<T>, conn, sql, param, nameof(DbExecuteScalar));
        }

        /// <summary>
        /// 分页
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="condition"></param>
        /// <param name="orderBy"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        protected virtual IPagedList<TEntity> DbPagedList(int pageIndex, int pageSize, string condition,
            string orderBy, IDictionary<string, object> parameters = null)
        {
            var pagedInfo = new QueryPageInfo
            {
                TableName = BaseTableName,
                Fields = "*",
                SqlWhere = condition,
                OrderField = orderBy,
                PageIndex = pageIndex,
                PageSize = pageSize
            };

            return PagedList<TEntity>(DbConnectString, pagedInfo, parameters);
        }


        #endregion //end Sync

        #region Async


        /// <summary>
        /// 查询自定义对象 Async
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        protected virtual async Task<T> DbQueryEAsync<T>(string sql, object param = null)
        {
            return await SqlTimerAsync(async (_1, _2) => await QueryAsync<T>(DbConnectString, _1, _2),
                sql, param, nameof(DbQueryEAsync));
        }

        protected virtual async Task<T> DbQueryEAsync<T>(string conn, string sql, object param = null)
        {
            return await SqlTimerAsync(async (_0, _1, _2) => await QueryAsync<T>(_0, _1, _2), conn, sql, param, nameof(DbQueryEAsync));
        }



        /// <summary>
        /// 查询自定义对象列表 Async
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        protected virtual async Task<IEnumerable<T>> DbQueryEListAsync<T>(string sql, object param = null)
        {
            return await SqlTimerAsync(async (_sql, _param) => await QueryListAsync<T>(DbConnectString, _sql, _param),
                sql, param, nameof(DbQueryEListAsync));
        }
        protected virtual async Task<IEnumerable<T>> DbQueryEListAsync<T>(string conn, string sql, object param = null)
        {
            return await SqlTimerAsync(async (_0, _sql, _param) => await QueryListAsync<T>(_0, _sql, _param),
                conn, sql, param, nameof(DbQueryEListAsync));
        }

        /// <summary>
        /// 查询实体对象 Async
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        protected virtual async Task<TEntity> DbQueryAsync(string sql, object param = null)
        {
            return await SqlTimerAsync(async (_sql, _param) => await QueryAsync<TEntity>(DbConnectString, _sql, _param),
                sql, param, nameof(DbQueryAsync));
        }
        protected virtual async Task<TEntity> DbQueryAsync(string conn, string sql, object param = null)
        {
            return await SqlTimerAsync(async (_0, _sql, _param) => await QueryAsync<TEntity>(_0, _sql, _param), conn, sql, param, nameof(DbQueryAsync));
        }


        /// <summary>
        /// 查询实体对象列表 Async
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        protected virtual async Task<IEnumerable<TEntity>> DbQueryListAsync(string sql, object param = null)
        {
            return await SqlTimerAsync(
                async (_sql, _param) => await QueryListAsync<TEntity>(DbConnectString, _sql, _param),
                sql, param, nameof(DbQueryList));
        }
        protected virtual async Task<IEnumerable<TEntity>> DbQueryListAsync(string conn, string sql, object param = null)
        {
            return await SqlTimerAsync(async (_0, _sql, _param) => await QueryListAsync<TEntity>(_0, _sql, _param), conn, sql, param, nameof(DbQueryList));
        }

        /// <summary>
        /// 执行受影响行数 Async
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        protected async Task<int> DbExecuteAsync(string sql, object param = null)
        {
            return await SqlTimerAsync(async (_sql, _param) => await ExecuteAsync(DbConnectString, _sql, _param),
                sql, param, nameof(DbExecuteAsync));
        }
        protected async Task<int> DbExecuteAsync(string conn, string sql, object param = null)
        {
            return await SqlTimerAsync(async (_0, _sql, _param) => await ExecuteAsync(_0, _sql, _param), conn, sql, param, nameof(DbExecuteAsync));
        }


        /// <summary>
        /// 首行首列数据 Async
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        protected async Task<T> DbExecuteScalarAsync<T>(string sql, object param = null)
        {
            return await SqlTimerAsync(
                async (_sql, _param) => await ExecuteScalarAsync<T>(DbConnectString, _sql, _param),
                sql, param, nameof(DbExecuteScalarAsync));
        }
        protected async Task<T> DbExecuteScalarAsync<T>(string conn, string sql, object param = null)
        {
            return await SqlTimerAsync(
                async (_0, _sql, _param) => await ExecuteScalarAsync<T>(_0, _sql, _param), conn, sql, param, nameof(DbExecuteScalarAsync));
        }

        /// <summary>
        ///  异步分页
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="condition"></param>
        /// <param name="orderBy"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        protected virtual async Task<IPagedList<TEntity>> DbPagedListAsync(int pageIndex,
            int pageSize, string condition, string orderBy, IDictionary<string, object> parameters = null)
        {
            var pagedInfo = new QueryPageInfo
            {
                TableName = BaseTableName,
                Fields = "*",
                SqlWhere = condition,
                OrderField = orderBy,
                PageIndex = pageIndex,
                PageSize = pageSize
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

        protected abstract string GetConnStr();

        protected virtual string BaseTableName => typeof(TEntity).Name.ToLower();

        protected virtual string DbConnectString => GetConnStr();

        protected T SqlTimer<T>(Func<string, object, T> sqlFunc, string sql, object param = null,
            string actionName = "")
        {

   
            var ticks = DateTime.Now.Ticks;
            try
            {
                return sqlFunc(sql, param);
            }
            catch (Exception ex)
            {
                LogHelper.Error($" 【{(ConfigItems.IsLogConnectString ? DbConnectString : "")} 】 {actionName} Failed，{sql}.param is {param.ToJsonStr()}", ex);
            }
            finally
            {
                if (ConfigItems.OpenDapperLog)
                {
                    var milliseconds = (long)TimeSpan.FromTicks(DateTime.Now.Ticks - ticks).TotalMilliseconds;
                    LogHelper.Info(
                        $"【{(ConfigItems.IsLogConnectString ? DbConnectString : "")} 】 sql:{sql} 耗时：{milliseconds} ms. {(milliseconds >= 100 ? $"param is {param.ToJsonStr()}" : "")}");
                }
            }

            return default;
        }


        protected async Task<T> SqlTimerAsync<T>(Func<string, object, Task<T>> sqlFunc, string sql, object param = null,
            string actionName = "")
        {
            var ticks = DateTime.Now.Ticks;
            try
            {
                return await sqlFunc(sql, param);
            }
            catch (Exception ex)
            {
                LogHelper.Error($"【{(ConfigItems.IsLogConnectString ? DbConnectString : "")} 】{actionName} Failed，{sql}.param is {param.ToJsonStr()}", ex);
            }
            finally
            {
                if (ConfigItems.OpenDapperLog)
                {
                    var milliseconds = (long)TimeSpan.FromTicks(DateTime.Now.Ticks - ticks).TotalMilliseconds;
                    LogHelper.Info(
                        $"【{(ConfigItems.IsLogConnectString ? DbConnectString : "")} 】 sql:{sql} 耗时：{milliseconds} ms. {(milliseconds >= 100 ? $"param is {param.ToJsonStr()}" : "")}");
                }
            }

            return default;
        }



        protected T SqlTimer<T>(Func<string, string, object, T> sqlFunc, string connectString, string sql, object param = null,
            string actionName = "")
        {
            var ticks = DateTime.Now.Ticks;
            try
            {
                return sqlFunc(connectString, sql, param);
            }
            catch (Exception ex)
            {
                LogHelper.Error($"【{(ConfigItems.IsLogConnectString ? connectString : "")} 】 {actionName} Failed，{sql}.param is {param.ToJsonStr()}", ex);
            }
            finally
            {
                if (ConfigItems.OpenDapperLog)
                {
                    var milliseconds = (long)TimeSpan.FromTicks(DateTime.Now.Ticks - ticks).TotalMilliseconds;
                    LogHelper.Info(
                        $"【{(ConfigItems.IsLogConnectString ? connectString : "")} 】 sql:{sql} 耗时：{milliseconds} ms. {(milliseconds >= 100 ? $"param is {param.ToJsonStr()}" : "")}");
                }
            }

            return default;
        }


        protected async Task<T> SqlTimerAsync<T>(Func<string, string, object, Task<T>> sqlFunc, string connectString, string sql, object param = null,
            string actionName = "")
        {
            var ticks = DateTime.Now.Ticks;
            try
            {
                return await sqlFunc(connectString, sql, param);
            }
            catch (Exception ex)
            {
                LogHelper.Error($"【{(ConfigItems.IsLogConnectString ? connectString : "")} 】{actionName} Failed，{sql}.param is {param.ToJsonStr()}", ex);
            }
            finally
            {
                if (ConfigItems.OpenDapperLog)
                {
                    var milliseconds = (long)TimeSpan.FromTicks(DateTime.Now.Ticks - ticks).TotalMilliseconds;
                    LogHelper.Info(
                        $"【{(ConfigItems.IsLogConnectString ? connectString : "")} 】 sql:{sql} 耗时：{milliseconds} ms. {(milliseconds >= 100 ? $"param is {param.ToJsonStr()}" : "")}");
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

            static class ConfigItemsKey
            {
                internal static string OpenDapperLogKey = "DapperConfig:OpenDapperLog";
                internal static string IsLogConnectStringKey = "DapperConfig:IsLogConnectString";

            }
        }
    }



}
