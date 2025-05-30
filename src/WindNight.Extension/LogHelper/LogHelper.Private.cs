using System;
using System.IO;
using System.Text.Extension;
using Newtonsoft.Json.Extension;
using Newtonsoft.Json.Linq;
using WindNight.Core.Abstractions;
using WindNight.Core.ExceptionExt;
using WindNight.Extension;
using IpHelper = WindNight.Extension.HttpContextExtension;

namespace WindNight.LogExtension
{
    public static partial class LogHelper
    {
        public static void Add(string msg, LogLevels logLevel, Exception? errorStack = null, bool isTimeout = false,
            long millisecond = 0, // [Maxlength(255)]
            string url = "", string serverIp = "", string clientIp = "", bool appendMessage = false, string traceId = "")
        {
            try
            {
                var canLog = CanLog(logLevel);
                if (!canLog)
                {
                    return;
                }
                var logInfo = GeneratorLogInfo(msg, logLevel, errorStack, millisecond, url, serverIp, clientIp,
                    appendMessage, traceId);

                OnPublishLogInfoHandleEvent(logInfo);
            }
            catch (Exception ex)
            {
                Log4(LogLevels.Warning, $"AddLogs error {ex.Message}", ex);
                DoConsoleLog(LogLevels.Warning, $"AddLogs error {ex.Message}", ex);
            }
        }

        private static LogInfo GeneratorLogInfo(string msg, LogLevels level, Exception? exceptions = null,
            long millisecond = 0, string url = "", string serverIp = "", string clientIp = "",
            bool appendMessage = false, string traceId = "")
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
                LogTs = HardInfo.NowUnixTime,
                SerialNumber = traceId.IsNullOrEmpty() ? CurrentItem.GetSerialNumber : traceId,
                NodeCode = HardInfo.NodeCode,
            };
            FixLogInfo(logInfo, appendMessage);
            return logInfo;
        }


        private static LogInfo GeneratorLogInfo(JObject jo)
        {
            var logLevel = LogLevels.Information;
            var traceId = GuidHelper.GenerateOrderNumber();
            if (!string.IsNullOrEmpty(jo["serialNumber"]?.ToString()))
            {
                traceId = jo["serialNumber"].ToString();
            }
            if (!string.IsNullOrEmpty(jo["level"]?.ToString()))
            {
                logLevel = Convert2LogLevel(jo["level"].ToString());
            }
            var now = HardInfo.Now;
            var logTimestamps = now.ConvertToUnixTime();

            var logMsg = new LogInfo
            {
                SerialNumber = traceId,
                Level = logLevel,
                LogTs = logTimestamps,
                NodeCode = HardInfo.NodeCode ?? "",
            };
            logMsg.Content = jo.ToJsonStr();
            return logMsg;


        }
        static LogLevels Convert2LogLevel(string level)
        {
            try
            {
                if (level.IsNullOrEmpty())
                {
                    return LogLevels.Information;
                }

                if (level.StartsWith("debug", StringComparison.OrdinalIgnoreCase))
                {
                    return LogLevels.Debug;
                }

                if (level.StartsWith("info", StringComparison.OrdinalIgnoreCase))
                {
                    return LogLevels.Information;
                }

                if (level.StartsWith("warn", StringComparison.OrdinalIgnoreCase))
                {
                    return LogLevels.Warning;
                }

                var flag = Enum.TryParse<LogLevels>(level, true, out var logLevel);

                if (flag)
                {
                    return logLevel;
                }

                return LogLevels.Information;

            }
            catch (Exception ex)
            {
                return LogLevels.Information;
            }

        }


        static string FixLogMessage(string msg) => ConfigItems.SystemAppName.Concat($" TraceId:[{CurrentItem.GetSerialNumber}]:", msg);


        static void DoConsoleLog(LogLevels logLevel, string message, Exception? exception = null)
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


        static void DoConsoleLog(string message)
        {
            if (ConfigItems.LogOnConsole)
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine($"=={HardInfo.NowString}==ConsoleLog:{Environment.NewLine}{message}");
                Console.ResetColor();
            }
        }

        static void FixLogInfo(LogInfo logInfo, bool appendMessage)
        {
            if (CurrentItem.Items != null)
            {
                if (logInfo.ServerIp.IsNullOrEmpty())
                {
                    var ip = CurrentItem.GetItem<string>(ThreadContext.SERVERIP);
                    if (!ip.IsNullOrEmpty()) //截取中间部分 用于显示端口
                    {
                        // if (ip.Length > 32 && !is2Es) ip = ip.Replace(ip.Substring(5, 13), "***");
                    }
                    else
                    {
                        ip = IpHelper.GetLocalServerIp();
                    }

                    logInfo.ServerIp = ip;
                }

                if (logInfo.ClientIp.IsNullOrEmpty())
                {
                    var clientIp = CurrentItem.GetItem<string>(ThreadContext.CLIENTIP);
                    if (clientIp.IsNullOrEmpty())
                    {
                        clientIp = IpHelper.GetClientIp();
                    }

                    logInfo.ClientIp = clientIp;
                }

                if (logInfo.RequestUrl.IsNullOrEmpty())
                {
                    logInfo.RequestUrl = CurrentItem.GetItem<string>(ThreadContext.REQUESTPATH);
                }
                if (appendMessage || ConfigItems.IsAppendLogMessage)
                {
                    var msg = logInfo.Content;
                    msg = msg?.AppendLogMessage();
                    logInfo.Content = msg;
                }

            }

            if (logInfo.ServerIp.IsNullOrEmpty())
            {
                logInfo.ServerIp = IpHelper.GetLocalServerIp();
            }

            if (logInfo.ClientIp.IsNullOrEmpty())
            {
                logInfo.ClientIp = IpHelper.GetClientIp();
            }

            if (logInfo.RequestUrl.IsNullOrEmpty())
            {
                logInfo.RequestUrl = IpHelper.GetCurrentUrl();
            }

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
                    var now = HardInfo.Now;
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
