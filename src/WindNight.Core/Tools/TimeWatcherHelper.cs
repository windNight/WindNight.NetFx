using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection.WnExtension;
using WindNight.Core.@internal;

namespace WindNight.Core.Tools
{
    /// <summary>
    /// </summary>
    public static class TimeWatcherHelper
    {
        private const int DefaultWarnMiSeconds = 200;

        private const string TimeWatcherIsOpenKey = "TimeWatcherIsOpen";

        private static bool TimeWatcherIsOpen
        {
            get
            {
                var configService = Ioc.Instance.CurrentConfigService;
                if (configService == null)
                {
                    return false;
                }
                var configValue = configService.GetAppSettingValue(TimeWatcherIsOpenKey, false, false);
                return configValue;
            }
        }

        private static int TimeWatcherWarnMiSeconds
        {
            get
            {
                try
                {
                    var configService = Ioc.Instance.CurrentConfigService;
                    if (configService == null)
                    {
                        return DefaultWarnMiSeconds;
                    }
                    var configValue =
                        configService.GetAppSettingValue("TimeWatcherWarnMiSeconds", DefaultWarnMiSeconds, false);
                    return configValue;
                }
                catch
                {
                    return DefaultWarnMiSeconds;
                }
            }
        }

        private static bool DebugIsOpen
        {
            get
            {
                var configService = Ioc.Instance.CurrentConfigService;
                if (configService == null)
                {
                    return false;
                }
                var configValue = configService.GetAppSettingValue("DebugIsOpen", false, false);
                return configValue;
            }
        }

        private static int FixWarnMiSeconds(int warnMiSeconds)
        {
            return warnMiSeconds > 0 ? warnMiSeconds : TimeWatcherWarnMiSeconds;
        }

        /// <summary>
        ///     Safe ,not throw Exception
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="func"></param>
        /// <param name="realTs"></param>
        /// <param name="watcherName"></param>
        /// <param name="appendMessage"></param>
        /// <param name="warnMiSeconds"></param>
        /// <returns></returns>
        public static T TimeWatcher<T>(Func<T> func, out long realTs, string watcherName = "", bool appendMessage = false, int warnMiSeconds = 200)
        {
            return DoWatcherFunc(func, watcherName, appendMessage, warnMiSeconds, false, out realTs);
        }

        /// <summary>
        ///     Safe ,not throw Exception
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="func"></param>
        /// <param name="watcherName"></param>
        /// <param name="appendMessage"></param>
        /// <param name="warnMiSeconds"></param>
        /// <returns></returns>
        public static T TimeWatcher<T>(Func<T> func, string watcherName = "", bool appendMessage = false, int warnMiSeconds = 200)
        {
            return TimeWatcher(func, out var realTs, watcherName, appendMessage, warnMiSeconds);
        }

        /// <summary>
        ///     Unsafe ,will throw Exception
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="func"></param>
        /// <param name="realTs"></param>
        /// <param name="watcherName"></param>
        /// <param name="appendMessage"></param>
        /// <param name="warnMiSeconds"></param>
        /// <returns></returns>
        public static T TimeWatcherUnsafe<T>(Func<T> func, out long realTs, string watcherName = "", bool appendMessage = false, int warnMiSeconds = 200)
        {
            return DoWatcherFunc(func, watcherName, appendMessage, warnMiSeconds, true, out realTs);
        }

        /// <summary>
        ///     Unsafe ,will throw Exception
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="func"></param>
        /// <param name="watcherName"></param>
        /// <param name="appendMessage"></param>
        /// <param name="warnMiSeconds"></param>
        /// <returns></returns>
        public static T TimeWatcherUnsafe<T>(Func<T> func, string watcherName = "", bool appendMessage = false, int warnMiSeconds = 200)
        {
            return TimeWatcherUnsafe(func, out var realTs, watcherName, appendMessage, warnMiSeconds);
        }


        /// <summary>
        ///     Safe ,not throw Exception
        /// </summary>
        /// <param name="action"></param>
        /// <param name="watcherName"></param>
        /// <param name="appendMessage"></param>
        /// <param name="warnMiSeconds"></param>
        public static void TimeWatcher(Action action, string watcherName = "", bool appendMessage = false, int warnMiSeconds = 200)
        {
            TimeWatcher(action, out var realTs, watcherName, appendMessage, warnMiSeconds);
        }

        /// <summary>
        ///     Safe ,not throw Exception
        /// </summary>
        /// <param name="action"></param>
        /// <param name="realTs"></param>
        /// <param name="watcherName"></param>
        /// <param name="appendMessage"></param>
        /// <param name="warnMiSeconds"></param>
        public static void TimeWatcher(Action action, out long realTs, string watcherName = "", bool appendMessage = false, int warnMiSeconds = 200)
        {
            DoWatcherAction(action, watcherName, appendMessage, warnMiSeconds, false, out realTs);
        }

        /// <summary>
        ///     will throw Exception
        /// </summary>
        /// <param name="action"></param>
        /// <param name="realTs"></param>
        /// <param name="watcherName"></param>
        /// <param name="appendMessage"></param>
        /// <param name="warnMiSeconds"></param>
        public static void TimeWatcherUnsafe(Action action, out long realTs, string watcherName = "", bool appendMessage = false, int warnMiSeconds = 200)
        {
            DoWatcherAction(action, watcherName, appendMessage, warnMiSeconds, true, out realTs);
        }

        /// <summary>
        ///     will throw Exception
        /// </summary>
        /// <param name="action"></param>
        /// <param name="watcherName"></param>
        /// <param name="appendMessage"></param>
        /// <param name="warnMiSeconds"></param>
        public static void TimeWatcherUnsafe(Action action, string watcherName = "", bool appendMessage = false, int warnMiSeconds = 200)
        {
            TimeWatcherUnsafe(action, out var realTs, watcherName, appendMessage, warnMiSeconds);
        }

        private static void DoWatcherAction(Action action, string watcherName, bool appendMessage, int warnMiSeconds, bool isThrow, out long realTs)
        {
            var ticks = HardInfo.Now.Ticks;
            try
            {
                action.Invoke();
            }
            catch (BusinessException bex)
            {
                LogHelper.Warn($"TimeWatcher({watcherName}) 捕获业务异常：{bex.BusinessCode}", bex,
                    appendMessage: appendMessage);
                if (isThrow)
                {
                    throw;
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error($"TimeWatcher({watcherName}) 捕获未知异常:{ex.GetType()}", ex, appendMessage: appendMessage);
                if (isThrow)
                {
                    throw;
                }
            }
            finally
            {
                if (watcherName.IsNullOrEmpty()) watcherName = nameof(action);
                realTs = (long)TimeSpan.FromTicks(HardInfo.Now.Ticks - ticks).TotalMilliseconds;
                var fwarnMiS = FixWarnMiSeconds(warnMiSeconds);
                if (realTs > fwarnMiS)
                {
                    LogHelper.Warn($"{watcherName} 耗时{realTs} ms ", appendMessage: appendMessage, millisecond: realTs);
                }
                else if (realTs > 1 && TimeWatcherIsOpen)
                {
                    LogHelper.Info($"{watcherName} 耗时{realTs} ms ", realTs);
                }
            }
        }

        private static T DoWatcherFunc<T>(Func<T> func, string watcherName, bool appendMessage, int warnMiSeconds,
            bool isThrow, out long realTs)
        {
            T rlt;
            var ticks = HardInfo.Now.Ticks;
            // TODO Try to get real name of this func
            if (watcherName.IsNullOrEmpty()) watcherName = nameof(func);
            try
            {
                rlt = func.Invoke();
            }
            catch (BusinessException bex)
            {
                LogHelper.Warn($"TimeWatcher({watcherName}) 捕获业务异常：{bex.BusinessCode}", bex,
                    appendMessage: appendMessage);
                throw;
            }
            catch (Exception ex)
            {
                rlt = default;
                LogHelper.Error($"TimeWatcher({watcherName}) 捕获未知异常:{ex.GetType()}", ex, appendMessage: appendMessage);
                if (isThrow)
                {
                    throw;
                }
            }
            finally
            {
                realTs = (long)TimeSpan.FromTicks(HardInfo.Now.Ticks - ticks).TotalMilliseconds;
                var fwarnMiS = FixWarnMiSeconds(warnMiSeconds);
                if (realTs > fwarnMiS)
                {
                    LogHelper.Warn($"{watcherName} 耗时{realTs} ms ", appendMessage: appendMessage, millisecond: realTs);
                }
                else if (realTs > 1 && TimeWatcherIsOpen)
                {
                    LogHelper.Info($"{watcherName} 耗时{realTs} ms ", realTs);
                }
            }

            return rlt;
        }


        private static async Task<T> DoWatcherFuncAsync<T>(Func<Task<T>> func, string watcherName, bool appendMessage,
            int warnMiSeconds,
            bool isThrow)
        {
            T rlt;
            var ticks = HardInfo.Now.Ticks;
            // TODO Try to get real name of this func
            if (watcherName.IsNullOrEmpty()) watcherName = nameof(func);
            try
            {
                rlt = await func.Invoke();
            }
            catch (BusinessException bex)
            {
                LogHelper.Warn($"TimeWatcher({watcherName}) 捕获业务异常：{bex.BusinessCode}", bex,
                    appendMessage: appendMessage);
                throw;
            }
            catch (Exception ex)
            {
                rlt = default;
                LogHelper.Error($"TimeWatcher({watcherName}) 捕获未知异常:{ex.GetType()}", ex, appendMessage: appendMessage);
                if (isThrow)
                {
                    throw;
                }
            }
            finally
            {
                var realTs = (long)TimeSpan.FromTicks(HardInfo.Now.Ticks - ticks).TotalMilliseconds;
                var fwarnMiS = FixWarnMiSeconds(warnMiSeconds);
                if (realTs > fwarnMiS)
                {
                    LogHelper.Warn($"{watcherName} 耗时{realTs} ms ", appendMessage: appendMessage, millisecond: realTs);
                }
                else if (realTs > 1 && TimeWatcherIsOpen)
                {
                    LogHelper.Info($"{watcherName} 耗时{realTs} ms ", realTs);
                }
            }

            return rlt;
        }
    }
}
