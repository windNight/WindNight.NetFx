using System;
using Microsoft.Extensions.DependencyInjection.WnExtension;
using WindNight.Core;
using WindNight.Core.Abstractions;
using WindNight.Core.Internals;

namespace WindNight.Core.Tools
{
    /// <summary>
    /// </summary>
    public static class TimeWatcherHelper
    {
        private const int DefaultWarnMiSeconds = 100;
        private static readonly string TimeWatcherIsOpenKey = "TimeWatcherIsOpen";

        private static bool TimeWatcherIsOpen
        {
            get
            {
                var configService = Ioc.GetService<IConfigService>();
                if (configService == null) return false;
                var configValue = configService.GetAppSetting(TimeWatcherIsOpenKey, "1", false);
                return configValue == "1";
            }
        }

        private static int FixWarnMiSeconds(int warnMiSeconds)
        {
            return warnMiSeconds > 0 ? warnMiSeconds : DefaultWarnMiSeconds;
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
        public static T TimeWatcher<T>(Func<T> func, out long realTs, string watcherName = "", bool appendMessage = false,
            int warnMiSeconds = 200)
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
        public static T TimeWatcher<T>(Func<T> func, string watcherName = "", bool appendMessage = false,
            int warnMiSeconds = 200)
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
        public static T TimeWatcherUnsafe<T>(Func<T> func, out long realTs, string watcherName = "", bool appendMessage = false,
            int warnMiSeconds = 200)
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
        public static T TimeWatcherUnsafe<T>(Func<T> func, string watcherName = "", bool appendMessage = false,
            int warnMiSeconds = 200)
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
        public static void TimeWatcher(Action action, string watcherName = "", bool appendMessage = false,
            int warnMiSeconds = 200)
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
        public static void TimeWatcher(Action action, out long realTs, string watcherName = "", bool appendMessage = false,
            int warnMiSeconds = 200)
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
        public static void TimeWatcherUnsafe(Action action, out long realTs, string watcherName = "", bool appendMessage = false,
            int warnMiSeconds = 200)
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
        public static void TimeWatcherUnsafe(Action action, string watcherName = "", bool appendMessage = false,
            int warnMiSeconds = 200)
        {
            TimeWatcherUnsafe(action, out var realTs, watcherName, appendMessage, warnMiSeconds);
        }

        private static void DoWatcherAction(Action action, string watcherName, bool appendMessage, int warnMiSeconds,
            bool isThrow, out long realTs)
        {
            var ticks = DateTime.Now.Ticks;
            try
            {
                action.Invoke();
            }
            catch (BusinessException bex)
            {
                LogHelper.Warn($"TimeWatcher({watcherName}) 捕获业务异常：{bex.BusinessCode}", bex,
                    appendMessage: appendMessage);
                if (isThrow) throw;
            }
            catch (Exception ex)
            {
                LogHelper.Error($"TimeWatcher({watcherName}) 捕获未知异常:{ex.GetType()}", ex, appendMessage: appendMessage);
                if (isThrow) throw;
            }
            finally
            {
                if (string.IsNullOrEmpty(watcherName)) watcherName = nameof(action);
                realTs = (long)TimeSpan.FromTicks(DateTime.Now.Ticks - ticks).TotalMilliseconds;
                var fwarnMiS = FixWarnMiSeconds(warnMiSeconds);
                if (realTs > fwarnMiS)
                    LogHelper.Warn($"{watcherName} 耗时{realTs} ms ", appendMessage: appendMessage);
                else if (realTs > 1 && TimeWatcherIsOpen)
                    LogHelper.Info($"{watcherName} 耗时{realTs} ms ");
            }
        }

        private static T DoWatcherFunc<T>(Func<T> func, string watcherName, bool appendMessage, int warnMiSeconds,
            bool isThrow, out long realTs)
        {
            T rlt;
            var ticks = DateTime.Now.Ticks;

            if (string.IsNullOrEmpty(watcherName)) watcherName = nameof(func);
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
                if (isThrow) throw;
            }
            finally
            {
                realTs = (long)TimeSpan.FromTicks(DateTime.Now.Ticks - ticks).TotalMilliseconds;
                var fwarnMiS = FixWarnMiSeconds(warnMiSeconds);
                if (realTs > fwarnMiS)
                    LogHelper.Warn($"{watcherName} 耗时{realTs} ms ", appendMessage: appendMessage);
                else if (realTs > 1 && TimeWatcherIsOpen)
                    LogHelper.Info($"{watcherName} 耗时{realTs} ms ");
            }

            return rlt;
        }

    }
}