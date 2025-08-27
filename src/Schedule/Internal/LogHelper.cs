using System;
using System.IO;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection.WnExtension;
using Newtonsoft.Json.Extension;
using Schedule.@internal;
using WindNight.Core.Abstractions;
using WindNight.Core.Enums.Abstractions;
using WindNight.Core.ExceptionExt;
using WindNight.Core.SysLogCenter.Extensions;
using WindNight.Extension;

namespace Schedule.@internal
{
    internal partial class LogHelper
    {

        public static string CurrentVersion => BuildInfo.BuildVersion;

        public static string CurrentCompileTime => BuildInfo.BuildTime;

        protected static bool OpenDebug => ConfigItems.OpenDebug;


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

            if (level < ConfigItems.JobMiniLogLevel)
            {
                return false;
            }

            return true;
        }

    }
    internal partial class LogHelper
    {

        public static void Debug(string msg, long millisecond = 0, string url = "", string serverIp = "",
            string clientIp = "", bool appendMessage = false, string traceId = "")
        {
            if (!OpenDebug)
            {
                return;
            }

            Add(msg, LogLevels.Debug, millisecond: millisecond, url: url, serverIp: serverIp, clientIp: clientIp,
                appendMessage: appendMessage, traceId: traceId);
        }

        public static void Info(string msg, long millisecond = 0, string url = "", string serverIp = "",
            string clientIp = "", bool appendMessage = false, string traceId = "")
        {
            Add(msg, LogLevels.Information, millisecond: millisecond, url: url, serverIp: serverIp, clientIp: clientIp,
                appendMessage: appendMessage, traceId: traceId);
        }

        public static void Warn(string msg, Exception exception = null, long millisecond = 0, string url = "", string serverIp = "",
            string clientIp = "", bool appendMessage = true, string traceId = "")
        {
            Add(msg, LogLevels.Warning, exception, millisecond: millisecond, url: url, serverIp: serverIp,
                clientIp: clientIp, appendMessage: appendMessage, traceId: traceId);
        }

        public static void Error(string msg, Exception exception, long millisecond = 0, string url = "", string serverIp = "",
            string clientIp = "", bool appendMessage = true, string traceId = "")
        {
            Add(msg, LogLevels.Error, exception, millisecond: millisecond, url: url, serverIp: serverIp,
                clientIp: clientIp, appendMessage: appendMessage, traceId: traceId);
        }

        public static void Fatal(string msg, Exception exception, long millisecond = 0, string url = "",
            string serverIp = "", string clientIp = "", bool appendMessage = false, string traceId = "")
        {
            Add(msg, LogLevels.Critical, exception, millisecond: millisecond, url: url, serverIp: serverIp,
                clientIp: clientIp, appendMessage: appendMessage, traceId: traceId);
        }
    }

    internal partial class LogHelper
    {

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
        private static void Add(string msg, LogLevels level, Exception errorStack = null, bool isTimeout = false,
            long millisecond = 0,
            string url = "", string serverIp = "", string clientIp = "", bool appendMessage = false, string traceId = "")
        {
            try
            {
                var canLog = CanLog(level);
                if (!canLog)
                {
                    return;
                }
                if (!JobContext.JobId.IsNullOrEmpty() && CurrentItem.GetSerialNumber != JobContext.JobId)
                {
                    CurrentItem.AddSerialNumber(JobContext.JobId, true);
                }

                if (traceId.IsNullOrEmpty())
                {
                    traceId = JobContext.JobId;
                }
                var logService = Ioc.Instance.CurrentLogService;
                if (logService != null)
                {
                    logService?.AddLog(level, msg, errorStack, millisecond, url, serverIp, clientIp, appendMessage, traceId: traceId);
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
                DoConsoleLog($"日志异常:{ex.GetMessage()}");
            }
        }

        protected static void DoConsoleLog(LogLevels logLevel, string message)
        {
            if (ConfigItems.LogOnConsole)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                if (logLevel > LogLevels.Information)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                }
                $"【{logLevel.ToString()}】:  Ioc.GetService<ILogService>() Is null.\r\n can not log info: {message}".Log2Console();
                Console.ResetColor();
            }
        }
        protected static void DoConsoleLog(string message)
        {
            if (ConfigItems.LogOnConsole)
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                message.Log2Console();
                Console.ResetColor();
            }
        }



    }
}
