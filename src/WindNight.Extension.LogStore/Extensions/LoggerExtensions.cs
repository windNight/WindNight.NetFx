using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Extension;
using WindNight.Core.Abstractions;
using WindNight.Core.ExceptionExt;
using WindNight.Extension.Logger.DcLog.Abstractions;
using IpHelper = WindNight.Extension.Logger.DcLog.@internal.HttpContextExtension;

namespace WindNight.Extension.Logger.DcLog.Extensions
{
    public static class LoggerExtensions
    {
        /// <summary>
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="url"></param>
        /// <param name="msg"></param>
        /// <param name="millisecond"></param>
        /// <param name="serverIp"></param>
        /// <param name="clientIp"></param>
        /// <param name="serialNumber"></param>
        public static void ApiUrlCall(this ILogger logger, string url, string msg, long millisecond,
            string serverIp = "",
            string clientIp = "", string serialNumber = "")
        {
            Add(logger, msg, LogLevels.ApiUrl, millisecond: millisecond, url: url, serverIp: serverIp,
                clientIp: clientIp, serialNumber: serialNumber);
        }

        /// <summary>
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="url"></param>
        /// <param name="msg"></param>
        /// <param name="exception"></param>
        /// <param name="serverIp"></param>
        /// <param name="clientIp"></param>
        /// <param name="serialNumber"></param>
        public static void ApiUrlException(this ILogger logger, string url, string msg, Exception exception,
            string serverIp = "",
            string clientIp = "", string serialNumber = "")
        {
            Add(logger, msg, LogLevels.ApiUrlException, exception, url: url, serverIp: serverIp, clientIp: clientIp,
                serialNumber: serialNumber);
        }

        /// <summary>
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="msg"></param>
        /// <param name="millisecond"></param>
        /// <param name="url"></param>
        /// <param name="serverIp"></param>
        /// <param name="clientIp"></param>
        /// <param name="serialNumber"></param>
        public static void Debug(this ILogger logger, string msg, long millisecond = 0, string url = "",
            string serverIp = "",
            string clientIp = "", string serialNumber = "")
        {
            Add(logger, msg, LogLevels.Debug, millisecond: millisecond, url: url, serverIp: serverIp,
                clientIp: clientIp, serialNumber: serialNumber);
        }

        /// <summary>
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="msg"></param>
        /// <param name="millisecond"></param>
        /// <param name="url"></param>
        /// <param name="serverIp"></param>
        /// <param name="clientIp"></param>
        /// <param name="serialNumber"></param>
        public static void Info(this ILogger logger, string msg, long millisecond = 0, string url = "",
            string serverIp = "",
            string clientIp = "", string serialNumber = "")
        {
            Add(logger, msg, LogLevels.Information, millisecond: millisecond, url: url, serverIp: serverIp,
                clientIp: clientIp, serialNumber: serialNumber);
        }

        /// <summary>
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="msg"></param>
        /// <param name="exception"></param>
        /// <param name="millisecond"></param>
        /// <param name="url"></param>
        /// <param name="serverIp"></param>
        /// <param name="clientIp"></param>
        /// <param name="serialNumber"></param>
        public static void Warn(this ILogger logger, string msg, Exception exception = null, long millisecond = 0,
            string url = "",
            string serverIp = "", string clientIp = "", string serialNumber = "")
        {
            Add(logger, msg, LogLevels.Warning, exception, millisecond, url, serverIp,
                clientIp, serialNumber);
        }

        /// <summary>
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="msg"></param>
        /// <param name="exception"></param>
        /// <param name="millisecond"></param>
        /// <param name="url"></param>
        /// <param name="serverIp"></param>
        /// <param name="clientIp"></param>
        /// <param name="serialNumber"></param>
        public static void Error(this ILogger logger, string msg, Exception exception, long millisecond = 0,
            string url = "",
            string serverIp = "", string clientIp = "", string serialNumber = "")
        {
            Add(logger, msg, LogLevels.Error, exception, millisecond, url, serverIp,
                clientIp, serialNumber);
        }

        /// <summary>
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="msg"></param>
        /// <param name="exception"></param>
        /// <param name="millisecond"></param>
        /// <param name="url"></param>
        /// <param name="serverIp"></param>
        /// <param name="clientIp"></param>
        /// <param name="serialNumber"></param>
        public static void Fatal(this ILogger logger, string msg, Exception exception, long millisecond = 0,
            string url = "",
            string serverIp = "", string clientIp = "", string serialNumber = "")
        {
            Add(logger, msg, LogLevels.Critical, exception, millisecond, url, serverIp,
                clientIp, serialNumber);
        }

        /// <summary>
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="buildType"></param>
        /// <param name="appId"></param>
        /// <param name="appCode"></param>
        /// <param name="appName"></param>
        public static void LogRegisterInfo(this ILogger logger, string buildType, int appId, string appCode,
            string appName)
        {
            var serverIp = IpHelper.GetLocalIPs();
            var sysInfo = new
            {
                SysAppId = appId,
                SysAppCode = appCode,
                SysAppName = appName,
                ServerIP = serverIp,
                BuildType = buildType
            };
            var msg = $"register info is {sysInfo.ToJsonStr()}";
            Add(logger, msg, LogLevels.SysRegister, serverIp: serverIp.FirstOrDefault());
        }

        /// <summary>
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="buildType"></param>
        /// <param name="appId"></param>
        /// <param name="appCode"></param>
        /// <param name="appName"></param>
        /// <param name="exception"></param>
        public static void LogOfflineInfo(this ILogger logger, string buildType, int appId, string appCode,
            string appName, Exception exception = null)
        {
            var serverIp = IpHelper.GetLocalIPs();
            var sysInfo = new
            {
                SysAppId = appId,
                SysAppCode = appCode,
                SysAppName = appName,
                ServerIP = serverIp,
                BuildType = buildType
            };
            var msg = $"offline info is {sysInfo.ToJsonStr()}";
            Add(logger, msg, LogLevels.SysOffline, exception, serverIp: serverIp.FirstOrDefault());
        }


        private static void Add(ILogger logger, string msg, LogLevels logLevel, Exception exception = null,
            long millisecond = 0, string url = "", string serverIp = "", string clientIp = "", string serialNumber = "")
        {
            if (logger == null) throw new ArgumentNullException(nameof(logger));
            try
            {
                var state = new StateDataEntry
                {
                    ApiUrl = url,
                    ClientIP = clientIp,
                    ServerIP = serverIp,
                    Level = logLevel,
                    EventName = logLevel.ToString(), //logLevel >= LogLevels.ApiUrl ? logLevel.ToString() : "LogEvent",
                    Msg = msg,
                    Timestamps = millisecond,
                    SerialNumber = serialNumber,
                    LogTs = HardInfo.NowUnixTime,

                };

                logger.Log(logLevel.SwitchLogLevel(), new EventId(), state, exception, MessageFormatter);
            }
            catch (Exception ex)
            {
                Console.WriteLine("日志异常:{0}", ex.GetMessage());
            }
        }

        private static LogLevel SwitchLogLevel(this LogLevels logLevel)
        {
            var level = LogLevel.Debug;
            switch (logLevel)
            {
                case LogLevels.Trace:
                    level = LogLevel.Trace;
                    break;
                case LogLevels.None:
                case LogLevels.Debug:
                    break;
                case LogLevels.Warning:
                    level = LogLevel.Warning;
                    break;
                case LogLevels.Error:
                    level = LogLevel.Error;
                    break;
                case LogLevels.Critical:
                    level = LogLevel.Critical;
                    break;
                case LogLevels.ApiUrlException:
                    level = LogLevel.Error;
                    break;
                case LogLevels.ApiUrl:
                case LogLevels.Information:
                case LogLevels.Report:
                case LogLevels.SysRegister:
                case LogLevels.SysOffline:
                default:
                    level = LogLevel.Information;
                    break;
            }

            return level;
        }

        /// <summary>
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="eventName"></param>
        /// <param name="msg"></param>
        /// <param name="logLevel"></param>
        /// <param name="exception"></param>
        /// <param name="millisecond"></param>
        /// <param name="url"></param>
        /// <param name="serverIp"></param>
        /// <param name="clientIp"></param>
        /// <param name="serialNumber"></param>
        public static void LogEvent(this ILogger logger, string eventName, string msg,
            LogLevels logLevel, Exception exception = null,
            long millisecond = 0, string url = "", string serverIp = "", string clientIp = "", string serialNumber = ""
        )
        {
            if (logger == null)
            {
                throw new ArgumentNullException(nameof(logger));
            }
            if (eventName.IsNullOrEmpty())
            {
                throw new ArgumentNullException(nameof(eventName));
            }
            try
            {
                var state = new StateDataEntry
                {
                    ApiUrl = url,
                    ClientIP = clientIp,
                    ServerIP = serverIp,
                    Level = logLevel,
                    EventName = eventName,
                    Msg = msg,
                    Timestamps = millisecond,
                    SerialNumber = serialNumber,
                    LogTs = HardInfo.NowUnixTime,
                };

                logger.Log(logLevel.SwitchLogLevel(), new EventId(), state, exception, MessageFormatter);
            }
            catch (Exception ex)
            {
                Console.WriteLine("日志异常:{0}", ex.GetMessage());
            }
        }

        private static string MessageFormatter(StateDataEntry state, Exception exception)
        {
            return state.ToString();
        }


    }
}
