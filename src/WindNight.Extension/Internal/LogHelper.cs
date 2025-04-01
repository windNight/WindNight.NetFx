//using System;
//using System.IO;
//using System.Reflection;
//using Microsoft.Extensions.DependencyInjection.WnExtension;
//using Newtonsoft.Json.Extension;
//using WindNight.Core.Abstractions;
//using WindNight.Core.ExceptionExt;

//namespace WindNight.Extension.@internal
//{
//    internal partial class LogHelper
//    {
//        private static Version _version => new AssemblyName(typeof(LogHelper).Assembly.FullName).Version;
//        private static DateTime _compileTime => File.GetLastWriteTime(typeof(LogHelper).Assembly.Location);

//        public static string CurrentVersion => _version.ToString();

//        public static DateTime CurrentCompileTime => _compileTime;
//        protected static bool OpenDebug => ConfigItems.OpenDebug;

//        protected static LogLevels MinLogLevel => ConfigItems.GlobalMinLogLevel;

//        protected static bool CanLog(LogLevels level)
//        {
//            if (level == LogLevels.None)
//            {
//                return false;
//            }

//            if (level == LogLevels.Debug && !OpenDebug)
//            {
//                return false;
//            }

//            if (level < MinLogLevel)
//            {
//                return false;
//            }

//            return true;
//        }
//    }

//    internal partial class LogHelper
//    {
//        internal static void Debug(string msg, long millisecond = 0, string url = "", string serverIp = "",
//            string clientIp = "", bool appendMessage = false, string traceId = "")
//        {
//            Add(msg, LogLevels.Debug, millisecond: millisecond, url: url, serverIp: serverIp, clientIp: clientIp,
//                appendMessage: appendMessage, traceId: traceId);
//        }

//        internal static void Info(string msg, long millisecond = 0, string url = "", string serverIp = "",
//            string clientIp = "", bool appendMessage = false, string traceId = "")
//        {
//            Add(msg, LogLevels.Information, millisecond: millisecond, url: url, serverIp: serverIp, clientIp: clientIp,
//                appendMessage: appendMessage, traceId: traceId);
//        }

//        internal static void Warn(string msg, Exception exception = null, long millisecond = 0, string url = "",
//            string serverIp = "",
//            string clientIp = "", bool appendMessage = true, string traceId = "")
//        {
//            Add(msg, LogLevels.Warning, exception, millisecond: millisecond, url: url, serverIp: serverIp,
//                clientIp: clientIp, appendMessage: appendMessage, traceId: traceId);
//        }

//        internal static void Error(string msg, Exception exception, long millisecond = 0, string url = "",
//            string serverIp = "",
//            string clientIp = "", bool appendMessage = true, string traceId = "")
//        {
//            Add(msg, LogLevels.Error, exception, millisecond: millisecond, url: url, serverIp: serverIp,
//                clientIp: clientIp, appendMessage: appendMessage, traceId: traceId);
//        }

//        internal static void Fatal(string msg, Exception exception, long millisecond = 0, string url = "",
//            string serverIp = "", string clientIp = "", bool appendMessage = false, string traceId = "")
//        {
//            Add(msg, LogLevels.Critical, exception, millisecond: millisecond, url: url, serverIp: serverIp,
//                clientIp: clientIp, appendMessage: appendMessage, traceId: traceId);
//        }

//        /// <summary>
//        /// </summary>
//        /// <param name="msg"></param>
//        /// <param name="level"></param>
//        /// <param name="errorStack"></param>
//        /// <param name="isTimeout"></param>
//        /// <param name="millisecond"></param>
//        /// <param name="url"> max lenght is 256.limit 255 in this case  </param>
//        /// <param name="serverIp"></param>
//        /// <param name="clientIp"></param>
//        /// <param name="appendMessage"></param>
//        internal static void Add(string msg, LogLevels level, Exception errorStack = null, bool isTimeout = false,
//            long millisecond = 0, string url = "",
//            string serverIp = "", string clientIp = "", bool appendMessage = false, string traceId = "")
//        {
//            try
//            {
//                var canLog = CanLog(level);
//                if (!canLog)
//                {
//                    return;
//                }

//                var logService = Ioc.Instance.CurrentLogService;
//                if (logService != null)
//                {
//                    logService?.AddLog(level, msg, errorStack, millisecond, url, serverIp, clientIp, appendMessage,
//                        traceId);
//                }
//                else
//                {
//                    if (errorStack != null)
//                    {
//                        msg = $"{msg} {Environment.NewLine} {errorStack.GetMessage()}";
//                    }

//                    DoConsoleLog(level, msg);
//                }
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine("日志异常:{0}", ex.ToJsonStr());
//            }
//        }

//        private static void DoConsoleLog(LogLevels logLevel, string message)
//        {
//            if (ConfigItems.LogOnConsole)
//            {
//                Console.ForegroundColor = ConsoleColor.Green;
//                if (logLevel > LogLevels.Information) Console.ForegroundColor = ConsoleColor.Red;
//                Console.WriteLine(
//                    $"【{logLevel.ToString()}】:  Ioc.GetService<ILogService>() Is null.\r\n can not log info: {message}");
//                Console.ResetColor();
//            }
//        }

//        private static void DoConsoleLog(string message)
//        {
//            if (ConfigItems.LogOnConsole)
//            {
//                Console.ForegroundColor = ConsoleColor.Cyan;
//                Console.WriteLine(message);
//                Console.ResetColor();
//            }
//        }
//    }
//}
