using System;
using Microsoft.Extensions.DependencyInjection.WnExtension;
using Newtonsoft.Json.Extension;
using WindNight.Core.Abstractions;

namespace Microsoft.AspNetCore.Mvc.WnExtensions.@internal
{
    internal static class LogHelper
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

        internal static void Warn(string msg, Exception exception = null, long millisecond = 0, string url = "", string serverIp = "",
            string clientIp = "", bool appendMessage = true, string traceId = "")
        {
            Add(msg, LogLevels.Warning, exception, millisecond: millisecond, url: url, serverIp: serverIp,
                clientIp: clientIp, appendMessage: appendMessage, traceId: traceId);
        }

        internal static void Error(string msg, Exception exception, long millisecond = 0, string url = "", string serverIp = "",
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
        internal static void Add(string msg, LogLevels level, Exception errorStack = null, bool isTimeout = false,
            long millisecond = 0, string url = "",
            string serverIp = "", string clientIp = "", bool appendMessage = false, string traceId = "")
        {
            try
            {
                var logService = Ioc.Instance.CurrentLogService;
                if (logService != null)
                    logService?.AddLog(level, msg, errorStack, millisecond, url, serverIp, clientIp, appendMessage, traceId: traceId);
                else
                    DoConsoleLog(level, msg);
            }
            catch (Exception ex)
            {
                Console.WriteLine("日志异常:{0}", ex.ToJsonStr());
            }
        }

        private static void DoConsoleLog(LogLevels logLevel, string message)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            if (logLevel > LogLevels.Information) Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(
                $"【{logLevel.ToString()}】:  Ioc.GetService<ILogService>() Is null.\r\n can not log info: {message}");
            Console.ResetColor();
        }
    }


}