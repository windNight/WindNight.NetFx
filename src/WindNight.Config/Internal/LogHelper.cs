using Microsoft.Extensions.DependencyInjection.WnExtension;
using WindNight.Core.Enums.Abstractions;
using WindNight.Core.ExceptionExt;
#if !NET45
using WindNight.Config.@internal;
#endif

namespace WindNight.ConfigCenter.Extension.@internal
{
    internal partial class LogHelper
    {
        public static string CurrentVersion => BuildInfo.BuildVersion;

        public static string CurrentCompileTime => BuildInfo.BuildTime;

        protected static bool OpenDebug => ConfigItems.OpenDebug;

        protected static LogLevels MiniLogLevel => ConfigItems.GlobalMiniLogLevel;

        protected static bool CanLog(LogLevels level)
        {
            if (level == LogLevels.None)
            {
                return false;
            }

            if (level == LogLevels.Debug && !OpenDebug)
            {
                return false;
            }

            if (level < MiniLogLevel)
            {
                return false;
            }

            return true;
        }
    }

    internal partial class LogHelper
    {
        internal static void Debug(string msg, long millisecond = 0, string url = "", string serverIp = "",
            string clientIp = "", bool appendMessage = false, string traceId = "")
        {
            Add(msg, LogLevels.Debug, millisecond: millisecond, url: url, serverIp: serverIp, clientIp: clientIp,
                appendMessage: appendMessage, traceId: traceId);
        }

        internal static void Info(string msg, long millisecond = 0, string url = "", string serverIp = "",
            string clientIp = "", bool appendMessage = false, string traceId = "")
        {
            Add(msg, LogLevels.Information, millisecond: millisecond, url: url, serverIp: serverIp, clientIp: clientIp,
                appendMessage: appendMessage, traceId: traceId);
        }

        internal static void Warn(string msg, Exception exception = null, long millisecond = 0, string url = "",
            string serverIp = "",
            string clientIp = "", bool appendMessage = true, string traceId = "")
        {
            Add(msg, LogLevels.Warning, exception, millisecond: millisecond, url: url, serverIp: serverIp,
                clientIp: clientIp, appendMessage: appendMessage, traceId: traceId);
        }

        internal static void Error(string msg, Exception exception, long millisecond = 0, string url = "",
            string serverIp = "",
            string clientIp = "", bool appendMessage = true, string traceId = "")
        {
            Add(msg, LogLevels.Error, exception, millisecond: millisecond, url: url, serverIp: serverIp,
                clientIp: clientIp, appendMessage: appendMessage, traceId: traceId);
        }

        internal static void Fatal(string msg, Exception exception, long millisecond = 0, string url = "",
            string serverIp = "", string clientIp = "", bool appendMessage = false, string traceId = "")
        {
            Add(msg, LogLevels.Critical, exception, millisecond: millisecond, url: url, serverIp: serverIp,
                clientIp: clientIp, appendMessage: appendMessage, traceId: traceId);
        }

        /// <summary>
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="level"></param>
        /// <param name="errorStack"></param>
        /// <param name="isTimeout"></param>
        /// <param name="millisecond"></param>
        /// <param name="url"> max lenght is 256.limit 255 in this case  </param>
        /// <param name="serverIp"></param>
        /// <param name="clientIp"></param>
        /// <param name="appendMessage"></param>
        public static void Add(string msg, LogLevels level, Exception? errorStack = null, bool isTimeout = false,
            long millisecond = 0,
            string url = "", string serverIp = "", string clientIp = "", bool appendMessage = false,
            string traceId = "")
        {
            try
            {
#if !NET45
                if (!ConfigItems.OpenConfigLogs)
                {
                    return;
                }
#endif
                var canLog = CanLog(level);
                if (!canLog)
                {
                    return;
                }

                // var logService = ConfigCenterLogExtension.ConfigCenterLogProvider ?? Ioc.Instance.CurrentLogService;
                var logService = Ioc.Instance.CurrentLogService;
                if (logService != null)
                {
                    logService.AddLog(level, msg, errorStack, millisecond, url, serverIp, clientIp, appendMessage,
                        traceId);
                }
                else
                {
                    if (errorStack != null)
                    {
                        msg = $"{msg} {Environment.NewLine} {errorStack.GetMessage()}";
                    }

                    DoConsoleLog(level, msg);
                }
            }
            catch (Exception ex)
            {
                DoConsoleLog(LogLevels.Error, "AddLog 日志异常 ", ex);
            }
        }

        private static void DoConsoleLog(LogLevels logLevel, string message, Exception? exception = null)
        {
            if (ConfigItems.LogOnConsole)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                if (logLevel > LogLevels.Information)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                }

                if (exception != null)
                {
                    message = $"{message} {Environment.NewLine} {exception.GetMessage()}";
                }

                Console.WriteLine(
                    $"=={HardInfo.NowString}==ConsoleLog:{Environment.NewLine}Ioc.GetService<ILogService>() Is null.{Environment.NewLine} can not log info{message}");
                Console.ResetColor();
            }
        }

        //public static void DoConsoleLog(LogLevels logLevel, string message)
        //{
        //    if (ConfigItems.LogOnConsole)
        //    {
        //        Console.ForegroundColor = ConsoleColor.Green;
        //        if (logLevel > LogLevels.Information) Console.ForegroundColor = ConsoleColor.Red;
        //        Console.WriteLine(
        //            $"【{logLevel.ToString()}】:  Ioc.GetService<ILogService>() Is null.\r\n can not log info: {message}");
        //        Console.ResetColor();
        //    }
        //}

        public static void DoConsoleLog(string message)
        {
            if (ConfigItems.LogOnConsole)
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine(message);
                Console.ResetColor();
            }
        }
    }
}
