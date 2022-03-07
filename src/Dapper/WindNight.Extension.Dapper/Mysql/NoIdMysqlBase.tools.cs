using Newtonsoft.Json.Extension;
using System.Net.Sockets;
using Microsoft.Extensions.DependencyInjection.WnExtension;
using WindNight.Core.Abstractions;
using WindNight.Extension.Dapper.Internals;


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
                LogHelper.Error($"【{(ConfigItems.IsLogConnectString ? connectString : "")} 】{actionName} Failed，{sql}.param is {param.ToJsonStr()}", ex);
            }
            finally
            {
                if (ConfigItems.OpenDapperLog)
                {
                    var milliseconds = (long)TimeSpan.FromTicks(DateTime.Now.Ticks - ticks).TotalMilliseconds;
                    LogHelper.Info(
                        $"【{(ConfigItems.IsLogConnectString ? connectString : "")} 】sql:{sql} 耗时：{milliseconds} ms. {(milliseconds >= 100 ? $"param is {param.ToJsonStr()}" : "")}");
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
                LogHelper.Error($"【{(ConfigItems.IsLogConnectString ? DbConnectString : "")} 】 {actionName} Failed，{sql}.param is {param.ToJsonStr()}", ex);
            }
            finally
            {
                if (ConfigItems.OpenDapperLog)
                {
                    var milliseconds = (long)TimeSpan.FromTicks(DateTime.Now.Ticks - ticks).TotalMilliseconds;
                    LogHelper.Info(
                        $"【{(ConfigItems.IsLogConnectString ? DbConnectString : "")} 】sql:{sql} 耗时：{milliseconds} ms. {(milliseconds >= 100 ? $"param is {param.ToJsonStr()}" : "")}");
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
                        $"【{(ConfigItems.IsLogConnectString ? connectString : "")} 】sql:{sql} 耗时：{milliseconds} ms. {(milliseconds >= 100 ? $"param is {param.ToJsonStr()}" : "")}");
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
                    var config = Ioc.Instance.CurrentConfigService;// Ioc.GetService<IConfigService>();
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