﻿using System;
using Microsoft.Extensions.DependencyInjection.WnExtension;
using Newtonsoft.Json.Extension;
using WindNight.Core.Abstractions;
using WindNight.Extension;

namespace Schedule
{
    internal static class LogHelper
    {
        static LogLevels minLogLevels => ConfigItems.JobsConfig?.MinLogLevel ?? LogLevels.Debug;

        internal static void Debug(string msg, long millisecond = 0, string url = "", string serverIp = "",
            string clientIp = "", bool appendMessage = false)
        {
#if DEBUG
            Add(msg, LogLevels.Information, millisecond: millisecond, url: url, serverIp: serverIp, clientIp: clientIp, appendMessage: appendMessage);
#endif
            // ConsoleLog(msg);
        }

        internal static void Info(string msg, long millisecond = 0, string url = "", string serverIp = "",
            string clientIp = "", bool appendMessage = false)
        {
            Add(msg, LogLevels.Information, millisecond: millisecond, url: url, serverIp: serverIp, clientIp: clientIp,
                appendMessage: appendMessage);
        }

        internal static void Warn(string msg, Exception exception = null, long millisecond = 0, string url = "",
            string serverIp = "", string clientIp = "", bool appendMessage = true)
        {
            Add(msg, LogLevels.Warning, exception, millisecond: millisecond, url: url, serverIp: serverIp,
                clientIp: clientIp, appendMessage: appendMessage);
        }

        internal static void Error(string msg, Exception exception, long millisecond = 0, string url = "",
            string serverIp = "",
            string clientIp = "", bool appendMessage = true)
        {
            Add(msg, LogLevels.Error, exception, millisecond: millisecond, url: url, serverIp: serverIp,
                clientIp: clientIp, appendMessage: appendMessage);
        }

        internal static void Fatal(string msg, Exception exception, long millisecond = 0, string url = "",
            string serverIp = "", string clientIp = "", bool appendMessage = false)
        {
            Add(msg, LogLevels.Critical, exception, millisecond: millisecond, url: url, serverIp: serverIp,
                clientIp: clientIp, appendMessage: appendMessage);
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
        private static void Add(string msg, LogLevels level, Exception errorStack = null, bool isTimeout = false,
            long millisecond = 0,
            string url = "", string serverIp = "", string clientIp = "", bool appendMessage = false)
        {
            try
            {
                if (level < minLogLevels) return;
                var logService = Ioc.Instance.CurrentLogService;
                if (!JobContext.JobId.IsNullOrEmpty() && CurrentItem.GetSerialNumber != JobContext.JobId)
                {
                    CurrentItem.AddItem("serialnumber", JobContext.JobId);
                }
                if (logService != null)
                    logService?.AddLog(level, msg, errorStack, millisecond, url, serverIp, clientIp, appendMessage);
                else
                    DoConsoleLog(level, msg);
            }
            catch (Exception ex)
            {
                ConsoleLog($"日志异常:{ex.ToJsonStr()}", LogLevels.Error);
            }
        }

        private static void DoConsoleLog(LogLevels logLevel, string message)
        {
            Console.ForegroundColor = logLevel > LogLevels.Information ? ConsoleColor.Red : ConsoleColor.Green;
            var newmsg = $"Ioc.GetService<ILogService>() Is null. can not log info {message}";
            ConsoleLog(newmsg);
            Console.ResetColor();
        }

        private static void ConsoleLog(string message, LogLevels logLevel = LogLevels.Debug)
        {
            Console.WriteLine($"【{logLevel.ToString()}】 {message}");
        }
    }
}