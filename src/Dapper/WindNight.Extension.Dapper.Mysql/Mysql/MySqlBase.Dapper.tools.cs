using System.Net.Sockets;
using Newtonsoft.Json.Extension;
using WindNight.Core.SQL.Abstractions;
using WindNight.Extension.Dapper.Mysql.@internal;
using WindNight.Extension.Db.Extensions;

namespace WindNight.Extension.Dapper.Mysql
{
    /// <inheritdoc />
    public partial class MySqlBase
    {
        /// <summary>
        ///     使用自定义连接
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sqlFunc"></param>
        /// <param name="connectString"></param>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <param name="actionName"></param>
        /// <param name="warnMs"></param>
        /// <param name="execErrorHandler"></param>
        /// <returns></returns>
        protected virtual T SqlTimer<T>(
            Func<string, string, object, Action<Exception, string>, T> sqlFunc,
            string connectString, string sql, object param = null, string actionName = "",
            long warnMs = -1, Action<Exception, string> execErrorHandler = null)
        {
            var ticks = HardInfo.Now.Ticks;
            try
            {
                return sqlFunc(connectString, sql, param, execErrorHandler);
            }
            catch (Exception ex)
            {
                if (param is IEntity entity)
                {
                    LogHelper.Error(
                        $" sql执行报错 {(ConfigItems.IsLogConnectString ? $"【{connectString}】" : "")}  {actionName} Failed，{sql}.param is {entity.ToParamString()}",
                        ex);
                }
                else
                {
                    LogHelper.Error(
                        $" sql执行报错 {(ConfigItems.IsLogConnectString ? $"【{connectString}】" : "")}  {actionName} Failed，{sql}.param is {param.ToJsonStr()}",
                        ex);
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
                        LogHelper.Warn(
                            $"sql执行耗时：{milliseconds} ms.{(ConfigItems.IsLogConnectString ? $"【{connectString}】" : "")}  sql:{sql}  {(milliseconds >= 100 ? $"param is {param.ToJsonStr()}" : "")}",
                            millisecond: milliseconds);
                    }
                    else if (ConfigItems.OpenDapperLog)
                    {
                        LogHelper.Info(
                            $"sql执行耗时：{milliseconds} ms.{(ConfigItems.IsLogConnectString ? $"【{connectString}】" : "")} sql:{sql}  {(milliseconds >= 100 ? $"param is {param.ToJsonStr()}" : "")}",
                            milliseconds);
                    }
                }
                catch
                {
                }
            }

            return default;
        }


        /// <summary>
        ///     使用自定义连接
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sqlFunc"></param>
        /// <param name="connectString"></param>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <param name="actionName"></param>
        /// <param name="warnMs"></param>
        /// <param name="execErrorHandler"></param>
        /// <returns></returns>
        protected virtual async Task<T> SqlTimerAsync<T>(
            Func<string, string, object, Action<Exception, string>, Task<T>> sqlFunc,
            string connectString, string sql, object param = null, string actionName = "",
            long warnMs = -1, Action<Exception, string> execErrorHandler = null)
        {
            var ticks = HardInfo.Now.Ticks;
            try
            {
                return await sqlFunc(connectString, sql, param, execErrorHandler);
            }
            catch (Exception ex)
            {
                if (param is IEntity entity)
                {
                    LogHelper.Error(
                        $"sql执行报错 {(ConfigItems.IsLogConnectString ? $"【{connectString}】" : "")}  {actionName} Failed，{sql}.param is {entity.ToParamString()}",
                        ex);
                }
                else
                {
                    LogHelper.Error(
                        $"sql执行报错 {(ConfigItems.IsLogConnectString ? $"【{connectString}】" : "")}  {actionName} Failed，{sql}.param is {param.ToJsonStr()}",
                        ex);
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
                        LogHelper.Warn(
                            $"sql执行耗时：{milliseconds} ms.{(ConfigItems.IsLogConnectString ? $"【{connectString}】" : "")}  sql:{sql}  {(milliseconds >= 100 ? $"param is {param.ToJsonStr()}" : "")}",
                            millisecond: milliseconds);
                    }
                    else if (ConfigItems.OpenDapperLog)
                    {
                        LogHelper.Info(
                            $"sql执行耗时：{milliseconds} ms.{(ConfigItems.IsLogConnectString ? $"【{connectString}】" : "")} sql:{sql}  {(milliseconds >= 100 ? $"param is {param.ToJsonStr()}" : "")}",
                            milliseconds);
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


        public virtual long FixWarnMs(long warnMs = -1)
        {
            var configW = ConfigItems.DapperWarnMs;

            configW = Math.Max(configW, 50);
            if (warnMs > 0)
            {
                configW = Math.Max(configW, warnMs);
            }

            return configW;
        }
    }
}
