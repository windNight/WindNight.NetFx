using System.Reflection;
using Newtonsoft.Json.Extension;
using WindNight.Core.Abstractions;
using WindNight.Extension.Logger.DcLog.Abstractions;
using IpHelper = WindNight.Extension.Logger.DcLog.@internal.HttpContextExtension;

namespace WindNight.Extension.Logger.DcLog.Extensions
{
    /// <summary> </summary>
    public static class DcLogHelper
    {
        private static Version _version => new AssemblyName(typeof(DcLogHelper).Assembly.FullName).Version;
        private static DateTime _compileTime => File.GetLastWriteTime(typeof(DcLogHelper).Assembly.Location);

        public static string CurrentVersion => _version.ToString();

        public static DateTime CurrentCompileTime => _compileTime;

        private static IDcLoggerProcessor DcLoggerProcessor => DcLoggerExtensions.LoggerProcessor;

        private static DcLogOptions DcLogOptions => DcLoggerExtensions.DcLogOptions;


        /// <summary>
        /// </summary>
        /// <param name="url"></param>
        /// <param name="msg"></param>
        /// <param name="millisecond">耗时日志</param>
        /// <param name="serialNumber"></param>
        /// <param name="serverIp"></param>
        /// <param name="clientIp"></param>
        public static void ApiUrlCall(string url, string msg, long millisecond, string serialNumber = "",
            string serverIp = "",
            string clientIp = "")
        {
            Add(msg, LogLevels.ApiUrl, serialNumber: serialNumber, millisecond: millisecond, url: url,
                serverIp: serverIp, clientIp: clientIp
            );
        }

        /// <summary>
        /// </summary>
        /// <param name="url"></param>
        /// <param name="msg"></param>
        /// <param name="exception"></param>
        /// <param name="serialNumber"></param>
        /// <param name="serverIp"></param>
        /// <param name="clientIp"></param>
        public static void ApiUrlException(string url, string msg, Exception exception, string serialNumber = "",
            string serverIp = "", long millisecond = 0,
            string clientIp = "")
        {
            Add(msg, LogLevels.ApiUrlException, exception, serialNumber, url: url, serverIp: serverIp,
                clientIp: clientIp, millisecond: millisecond);
        }

        /// <summary>
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="serialNumber"></param>
        /// <param name="url"></param>
        /// <param name="serverIp"></param>
        /// <param name="clientIp"></param>
        public static void Debug(string msg, string serialNumber = "", string url = "", string serverIp = "", long millisecond = 0,
            string clientIp = "")
        {
            Add(msg, LogLevels.Debug, serialNumber: serialNumber, url: url, serverIp: serverIp, clientIp: clientIp, millisecond: millisecond);
        }

        /// <summary>
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="serialNumber"></param>
        /// <param name="url"></param>
        /// <param name="serverIp"></param>
        /// <param name="clientIp"></param>
        public static void Info(string msg, string serialNumber = "", string url = "", string serverIp = "", long millisecond = 0,
            string clientIp = "")
        {
            Add(msg, LogLevels.Information, serialNumber: serialNumber, url: url, serverIp: serverIp,
                clientIp: clientIp, millisecond: millisecond);
        }

        /// <summary>
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="exception"></param>
        /// <param name="serialNumber"></param>
        /// <param name="millisecond"></param>
        /// <param name="url"></param>
        /// <param name="serverIp"></param>
        /// <param name="clientIp"></param>
        public static void Warn(string msg, Exception exception = null, string serialNumber = "", string url = "", long millisecond = 0,
            string serverIp = "", string clientIp = "")
        {
            Add(msg, LogLevels.Warning, exception, serialNumber, url: url, serverIp: serverIp,
                clientIp: clientIp, millisecond: millisecond);
        }

        /// <summary>
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="exception"></param>
        /// <param name="serialNumber"></param>
        /// <param name="url"></param>
        /// <param name="serverIp"></param>
        /// <param name="clientIp"></param>
        public static void Error(string msg, Exception exception, string serialNumber = "", string url = "", long millisecond = 0,
            string serverIp = "",
            string clientIp = "")
        {
            Add(msg, LogLevels.Error, exception, serialNumber, url: url, serverIp: serverIp,
                clientIp: clientIp, millisecond: millisecond);
        }

        /// <summary>
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="exception"></param>
        /// <param name="serialNumber"></param>
        /// <param name="url"></param>
        /// <param name="serverIp"></param>
        /// <param name="clientIp"></param>
        public static void Fatal(string msg, Exception exception, string serialNumber = "", string url = "", long millisecond = 0,
            string serverIp = "", string clientIp = "")
        {
            Add(msg, LogLevels.Critical, exception, serialNumber, url: url, serverIp: serverIp,
                clientIp: clientIp, millisecond: millisecond);
        }

        /// <summary>
        /// </summary>
        /// <param name="buildType"></param>
        /// <param name="appId"></param>
        /// <param name="appCode"></param>
        /// <param name="appName"></param>
        public static void LogRegisterInfo(string buildType, int appId, string appCode, string appName)
        {
            var serverIp = IpHelper.GetLocalIPs().ToList();
            var sysInfo = new
            {
                SysAppId = appId,
                SysAppCode = appCode,
                SysAppName = appName,
                ServerIP = serverIp,
                BuildType = buildType
            };
            var msg = $"register info is {sysInfo.ToJsonStr()}";
            Add(msg, LogLevels.SysRegister, serverIp: serverIp.FirstOrDefault());
        }

        /// <summary>
        /// </summary>
        /// <param name="buildType"></param>
        /// <param name="appId"></param>
        /// <param name="appCode"></param>
        /// <param name="appName"></param>
        /// <param name="exception"></param>
        public static void LogOfflineInfo(string buildType, int appId, string appCode, string appName,
            Exception exception = null)
        {
            var serverIp = IpHelper.GetLocalIPs().ToList();
            var sysInfo = new
            {
                SysAppId = appId,
                SysAppCode = appCode,
                SysAppName = appName,
                ServerIP = serverIp,
                BuildType = buildType
            };
            var msg = $"offline info is {sysInfo.ToJsonStr()}";
            Add(msg, LogLevels.SysOffline, exception, serverIp: serverIp.FirstOrDefault());
        }



        /// <summary>
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="logLevel"></param>
        /// <param name="exception"></param>
        /// <param name="serialNumber"></param>
        /// <param name="millisecond"> 耗时 </param>
        /// <param name="url"></param>
        /// <param name="serverIp"></param>
        /// <param name="clientIp"></param>
        public static void Add(string msg, LogLevels logLevel, Exception exception = null, string serialNumber = "",
            long millisecond = 0, string url = "", string serverIp = "", string clientIp = "")
        {

            try
            {
                if (DcLoggerProcessor == null || DcLogOptions == null) return;

                if ((int)logLevel < (int)DcLogOptions.MinLogLevel)
                {
                    return;
                }

                if (msg.Contains("syslogs"))
                {
                    // Console.WriteLine($"MinLogLevel is {DbLogOptions.MinLogLevel}({(int)DbLogOptions.MinLogLevel})");
                    return;
                }

                if (serialNumber.IsNullOrEmpty())
                {
                    serialNumber = CurrentItem.GetSerialNumber;
                }
                var messageEntity = new SysLogs
                {
                    //Content = msg,
                    Level = logLevel.ToString(),
                    LevelType = (int)logLevel,
                    LogAppCode = DcLogOptions?.LogAppCode ?? "",
                    LogAppName = DcLogOptions?.LogAppName ?? "",
                    ClientIp = clientIp,
                    ServerIp = serverIp,
                    // EventName = logLevel >= LogLevels.ApiUrl ? logLevel.ToString() : "LogEvent",
                    RunMs = millisecond,
                    LogTs = HardInfo.NowUnixTime,
                    RequestUrl = url,
                    SerialNumber = serialNumber,
                    NodeCode = HardInfo.NodeCode ?? "",
                    LogPluginVersion = $"{nameof(DcLogHelper)}/{CurrentVersion} {CurrentCompileTime:yyyy-MM-dd HH:mm:ss}",

                };
                if (exception != null)
                {
                    messageEntity.ExceptionObj = new ExceptionData
                    { Message = exception.Message, StackTraceString = exception.StackTrace };
                    messageEntity.Exceptions = messageEntity.ExceptionObj.ToJsonStr();
                }
                else
                {
                    messageEntity.Exceptions = "{}";
                }

                var logMsg = FixLogMessage(msg);
                messageEntity.Content = FixContent(msg);


                //DcLogOptions.ContentMaxLength > 0 && logMsg.Length > DcLogOptions.ContentMaxLength ?
                //logMsg.Substring(0, DcLogOptions.ContentMaxLength)
                //:
                //logMsg;

                DcLoggerProcessor.EnqueueMessage(messageEntity);
            }
            catch (Exception ex)
            {
                Console.WriteLine("日志异常:{0}", ex.ToJsonStr());

            }
        }

        static string FixContent(string logMsg)
        {
            var configMaxLen = DcLogOptions.ContentMaxLength;
            var msg = configMaxLen > 0 && logMsg.Length > configMaxLen ?
                   logMsg.Substring(0, configMaxLen)
                   :
                   logMsg;
            return msg;
        }

        static string FixLogMessage(string msg) => msg;
        //  string.Concat(ConfigItems.SystemAppName, $" [请求序列号：{CurrentItem.GetSerialNumber}]-0: ", msg);

    }
}
