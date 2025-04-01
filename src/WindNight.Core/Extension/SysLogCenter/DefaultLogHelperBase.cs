using System;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using System.Reflection;
using System.Text;
using Microsoft.Extensions.DependencyInjection.WnExtension;
using Newtonsoft.Json.Extension;
using Newtonsoft.Json.Linq;
using WindNight.Core.Abstractions;
using WindNight.Core.ExceptionExt;
using WindNight.Core.@internal;

namespace WindNight.Core.SysLogCenter.Extensions
{
    public partial class DefaultLogHelperBase
    {
        private static Version _version => new AssemblyName(typeof(DefaultLogHelperBase).Assembly.FullName).Version;
        private static DateTime _compileTime => File.GetLastWriteTime(typeof(DefaultLogHelperBase).Assembly.Location);

        public static string CurrentVersion => _version.ToString();

        public static DateTime CurrentCompileTime => _compileTime;
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

    public partial class DefaultLogHelperBase
    {
        /// <summary>
        /// </summary>
        /// <param name="url"></param>
        /// <param name="msg"></param>
        /// <param name="millisecond"></param>
        /// <param name="serverIp"></param>
        /// <param name="clientIp"></param>
        /// <param name="appendMessage"></param>
        public static void ApiUrlCall(string url, string msg, long millisecond, string serverIp = "",
            string clientIp = "", bool appendMessage = false, string traceId = "")
        {
            Add(msg, LogLevels.ApiUrl, millisecond: millisecond, url: url, serverIp: serverIp, clientIp: clientIp,
                appendMessage: appendMessage, traceId: traceId);
        }

        /// <summary>
        /// </summary>
        /// <param name="url"></param>
        /// <param name="msg"></param>
        /// <param name="exception"></param>
        /// <param name="serverIp"></param>
        /// <param name="clientIp"></param>
        /// <param name="appendMessage"></param>
        public static void ApiUrlException(string url, string msg, Exception exception, string serverIp = "",
            string clientIp = "", bool appendMessage = true, string traceId = "")
        {
            Add(msg, LogLevels.ApiUrlException, exception, url: url, serverIp: serverIp, clientIp: clientIp,
                appendMessage: appendMessage, traceId: traceId);
        }


        /// <summary>
        /// </summary>
        /// <param name="buildType"></param>
        /// <param name="appendMessage"></param>
        public static void LogRegisterInfo(string buildType, bool appendMessage = false, string traceId = "")
        {
            try
            {
                if (traceId.IsNullOrEmpty())
                {
                    traceId = HardInfo.NodeCode;
                }
                CurrentLogService?.Register(buildType, appendMessage, traceId);
            }
            catch (Exception ex)
            {
                DoConsoleLog($"Register 日志异常:{ex.GetMessage()}");
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="buildType"></param>
        /// <param name="exception"></param>
        /// <param name="appendMessage"></param>
        public static void LogOfflineInfo(string buildType, Exception exception = null, bool appendMessage = false,
            string traceId = "")
        {
            try
            {
                if (traceId.IsNullOrEmpty())
                {
                    traceId = HardInfo.NodeCode;
                }
                CurrentLogService?.Offline(buildType, exception, appendMessage, traceId);
            }
            catch (Exception ex)
            {
                DoConsoleLog($"Offline 日志异常:{ex.GetMessage()}");
            }
        }

        static ILogService CurrentLogService => Ioc.Instance.CurrentLogService;
        /// <summary>
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="traceId"></param>
        public static void Report(JObject obj, string traceId = "")
        {
            try
            {
                CurrentLogService?.Report(obj, traceId);
            }
            catch (Exception ex)
            {
                DoConsoleLog($"Report 日志异常:{ex.GetMessage()}");
            }
        }

    }
    public partial class DefaultLogHelperBase
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

    public partial class DefaultLogHelperBase
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
        protected static void Add(string msg, LogLevels level, Exception errorStack = null, bool isTimeout = false,
            long millisecond = 0, string url = "",
            string serverIp = "", string clientIp = "", bool appendMessage = false, string traceId = "")
        {
            try
            {
                var canLog = CanLog(level);
                if (!canLog)
                {
                    return;
                }
                var logService = Ioc.Instance.CurrentLogService;
                if (logService != null)
                {
                    logService?.AddLog(level, msg, errorStack, millisecond, url, serverIp, clientIp, appendMessage, traceId: traceId);
                }
                else
                {
                    DoConsoleLog(level, msg, errorStack);
                }
            }
            catch (Exception ex)
            {
                DoConsoleLog($"日志异常:{ex.GetMessage()}");
            }
        }

        protected static void DoConsoleLog(LogLevels logLevel, string message, Exception? exception = null)
        {

            if (ConfigItems.LogOnConsole)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                if (logLevel > LogLevels.Information) Console.ForegroundColor = ConsoleColor.Red;
                if (exception != null)
                {
                    message = $"{message} {Environment.NewLine} {exception.GetMessage()}";
                }
                Console.WriteLine($"=={HardInfo.NowString}==ConsoleLog:{Environment.NewLine}{message}");
                Console.ResetColor();
            }
        }


        protected static void DoConsoleLog(string message, Exception? exception = null)
        {
            if (ConfigItems.LogOnConsole)
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                if (exception != null)
                {
                    message = $"{message} {Environment.NewLine} {exception.GetMessage()}";
                }
                Console.WriteLine($"=={HardInfo.NowString}==ConsoleLog:{Environment.NewLine}{message}");
                Console.ResetColor();
            }
        }
    }
}
