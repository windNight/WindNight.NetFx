using Newtonsoft.Json.Extension;
using System;
using System.IO;
using System.Linq;
using WindNight.Core.Abstractions;
using WindNight.Extension;
using IpHelper = WindNight.Extension.HttpContextExtension;

namespace WindNight.LogExtension
{
    public static partial class LogHelper
    {
        public static void Add(string msg, LogLevels logLevel, Exception errorStack = null, bool isTimeout = false,
            long millisecond = 0,
            // [Maxlength(255)]
            string url = "", string serverIp = "", string clientIp = "", bool appendMessage = false)
        {
            try
            {
                var logInfo = GeneratorLogInfo(msg, logLevel, errorStack, millisecond, url, serverIp, clientIp,
                    appendMessage);

                OnPublishLogInfoHandleEvent(logInfo);
            }
            catch (Exception ex)
            {
                Log(LogLevels.Warning, $"AddLogs error {ex.Message}", ex);
                DoConsoleLog(LogLevels.Warning, $"AddLogs error {ex.Message}", ex);
            }
        }

        private static LogInfo GeneratorLogInfo(string msg, LogLevels level, Exception exceptions = null,
            long millisecond = 0, string url = "", string serverIp = "", string clientIp = "",
            bool appendMessage = false)
        {
            var logMsg = FixLogMessage(msg);
            var logInfo = new LogInfo
            {
                Content = logMsg,
                Level = level,
                Exceptions = exceptions,
                ClientIp = clientIp,
                ServerIp = serverIp,
                RequestUrl = url,
                Timestamps = millisecond,
                SerialNumber = CurrentItem.GetSerialNumber,
                NodeCode = HardInfo.NodeCode,
            };

            FixLogInfo(logInfo, appendMessage);
            return logInfo;
        }
        static string FixLogMessage(string msg) => string.Concat(ConfigItems.SystemAppName, $" [请求序列号：{CurrentItem.GetSerialNumber}]-0: ", msg);

        private static void DoConsoleLog(LogLevels logLevel, string message, Exception exception = null)
        {
            if (logLevel > LogLevels.Warning)
                RecordLog.WriteLog($"日志记录异常:【{logLevel}】{message} Exception: {exception.ToJsonStr()}");

            Console.ForegroundColor = logLevel > LogLevels.Information ? ConsoleColor.Red : ConsoleColor.Green;
            Console.WriteLine(
                $"ConsoleLog:【{logLevel}】{message} Exception:{exception.ToJsonStr()}");
            Console.ResetColor();
        }

        static void FixLogInfo(LogInfo logInfo, bool appendMessage)
        {
            if (CurrentItem.Items != null)
            {
                if (string.IsNullOrEmpty(logInfo.ServerIp))
                {
                    var ip = CurrentItem.GetItem<string>(ThreadContext.SERVERIP);
                    if (!string.IsNullOrEmpty(ip)) //截取中间部分 用于显示端口
                    {
                        // if (ip.Length > 32 && !is2Es) ip = ip.Replace(ip.Substring(5, 13), "***");
                    }
                    else
                    {
                        ip = IpHelper.GetServerIp();
                    }

                    logInfo.ServerIp = ip;
                }

                if (string.IsNullOrEmpty(logInfo.ClientIp)) logInfo.ClientIp = CurrentItem.GetItem<string>(ThreadContext.CLIENTIP);
                if (string.IsNullOrEmpty(logInfo.RequestUrl)) logInfo.RequestUrl = CurrentItem.GetItem<string>(ThreadContext.REQUESTPATH);
                if (appendMessage && ConfigItems.IsAppendLogMessage)
                {
                    var msg = logInfo.Content;
                    msg = msg.AppendLogMessage();
                    logInfo.Content = msg;
                }

            }

            if (string.IsNullOrEmpty(logInfo.ServerIp))
                logInfo.ServerIp = string.Join(",", IpHelper.LocalServerIps);
            if (string.IsNullOrEmpty(logInfo.ClientIp)) logInfo.ClientIp = IpHelper.GetClientIp();
            if (string.IsNullOrEmpty(logInfo.RequestUrl)) logInfo.RequestUrl = IpHelper.GetCurrentUrl();
        }

        static string AppendLogMessage(this string msg)
        {
            return string.Concat(msg ?? "", "【请求中间信息】：", CurrentItem.ToString());
        }

        public static class RecordLog
        {
            public static void WriteLog(string msg)
            {
                try
                {
                    var now = DateTime.Now;
                    var dir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs", "LogException");
                    var filePath = Path.Combine(dir, now.FormatDateTime("yyyyMMdd"), "err.log");
                    if (!Directory.Exists(dir))
                        Directory.CreateDirectory(dir);
                    if (!File.Exists(filePath))
                        File.Create(filePath).Close();
                    using (var streamWriter = new StreamWriter(filePath, true))
                    {
                        try
                        {
                            streamWriter.WriteLine($"{now:yyyy-MM-dd HH:mm:ss:fff}:{msg}");
                        }
                        catch
                        {

                        }
                        finally
                        {
                            streamWriter.Close();
                        }
                    }
                }
                catch
                {
                }
            }
        }
    }
}