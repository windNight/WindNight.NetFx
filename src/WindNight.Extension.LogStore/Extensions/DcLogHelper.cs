using System.Diagnostics;
using System.Reflection;
using Newtonsoft.Json.Extension;
using Newtonsoft.Json.Linq;
using WindNight.Core;
using WindNight.Core.Abstractions;
using WindNight.Core.Enums.Abstractions;
using WindNight.Core.Enums.Extension;
using WindNight.Core.ExceptionExt;
using WindNight.Extension.Logger.DcLog.Abstractions;

namespace WindNight.Extension.Logger.DcLog.Extensions
{
    /// <summary> </summary>
    public static class DcLogHelper
    {
        // private static Version _version => new AssemblyName(typeof(DcLogHelper).Assembly.FullName).Version;
        // private static string _compileTime => BuildInfo.BuildTime;// File.GetLastWriteTime(typeof(DcLogHelper).Assembly.Location);

        public static string CurrentVersion => BuildInfo.BuildVersion;
        public static string CurrentCompileTime => BuildInfo.BuildTime;

        private static IDcLoggerProcessor DcLoggerProcessor => DcLoggerExtensions.LoggerProcessor;

        private static DcLogOptions DcLogOptions => DcLoggerExtensions.DcLogOptions;

        public static string LogPluginVersion => $"{nameof(DcLogHelper)}/{CurrentVersion} {CurrentCompileTime}";

        /// <summary>
        /// </summary>
        /// <param name="url"></param>
        /// <param name="msg"></param>
        /// <param name="millisecond">耗时日志</param>
        /// <param name="traceId"></param>
        /// <param name="serverIp"></param>
        /// <param name="clientIp"></param>
        public static void ApiUrlCall(string url, string msg, long millisecond, string traceId = "",
            string serverIp = "",
            string clientIp = "")
        {
            Add(msg, LogLevels.ApiUrl, traceId: traceId, millisecond: millisecond, url: url,
                serverIp: serverIp, clientIp: clientIp
            );
        }

        /// <summary>
        /// </summary>
        /// <param name="url"></param>
        /// <param name="msg"></param>
        /// <param name="exception"></param>
        /// <param name="traceId"></param>
        /// <param name="serverIp"></param>
        /// <param name="clientIp"></param>
        public static void ApiUrlException(string url, string msg, Exception exception, string traceId = "",
            string serverIp = "", long millisecond = 0,
            string clientIp = "")
        {
            Add(msg, LogLevels.ApiUrlException, exception, traceId, url: url, serverIp: serverIp,
                clientIp: clientIp, millisecond: millisecond);
        }

        /// <summary>
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="traceId"></param>
        /// <param name="url"></param>
        /// <param name="serverIp"></param>
        /// <param name="clientIp"></param>
        public static void Debug(string msg, string traceId = "", string url = "", string serverIp = "",
            long millisecond = 0,
            string clientIp = "")
        {
            Add(msg, LogLevels.Debug, traceId: traceId, url: url, serverIp: serverIp, clientIp: clientIp,
                millisecond: millisecond);
        }

        /// <summary>
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="traceId"></param>
        /// <param name="url"></param>
        /// <param name="serverIp"></param>
        /// <param name="clientIp"></param>
        public static void Info(string msg, string traceId = "", string url = "", string serverIp = "",
            long millisecond = 0,
            string clientIp = "")
        {
            Add(msg, LogLevels.Information, traceId: traceId, url: url, serverIp: serverIp,
                clientIp: clientIp, millisecond: millisecond);
        }

        /// <summary>
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="exception"></param>
        /// <param name="traceId"></param>
        /// <param name="millisecond"></param>
        /// <param name="url"></param>
        /// <param name="serverIp"></param>
        /// <param name="clientIp"></param>
        public static void Warn(string msg, Exception exception = null, string traceId = "", string url = "",
            long millisecond = 0,
            string serverIp = "", string clientIp = "")
        {
            Add(msg, LogLevels.Warning, exception, traceId, url: url, serverIp: serverIp,
                clientIp: clientIp, millisecond: millisecond);
        }

        /// <summary>
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="exception"></param>
        /// <param name="traceId"></param>
        /// <param name="url"></param>
        /// <param name="serverIp"></param>
        /// <param name="clientIp"></param>
        public static void Error(string msg, Exception exception, string traceId = "", string url = "",
            long millisecond = 0,
            string serverIp = "",
            string clientIp = "")
        {
            Add(msg, LogLevels.Error, exception, traceId, url: url, serverIp: serverIp,
                clientIp: clientIp, millisecond: millisecond);
        }

        /// <summary>
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="exception"></param>
        /// <param name="traceId"></param>
        /// <param name="url"></param>
        /// <param name="serverIp"></param>
        /// <param name="clientIp"></param>
        public static void Fatal(string msg, Exception exception, string traceId = "", string url = "",
            long millisecond = 0,
            string serverIp = "", string clientIp = "")
        {
            Add(msg, LogLevels.Critical, exception, traceId, url: url, serverIp: serverIp,
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
            var serverIp = HardInfo.NodeIp;// IpHelper.GetLocalIPs().ToList();
            //var sysInfo = new
            //{
            //    SysAppId = appId,
            //    SysAppCode = appCode,
            //    SysAppName = appName,
            //    ServerIP = serverIp,
            //    BuildType = buildType,
            //};
            var sysInfo = GetSysInfo(buildType);
            var msg = $"register info is {sysInfo.ToJsonStr()}";
            Add(msg, LogLevels.SysRegister, serverIp: serverIp);
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
            var serverIp = HardInfo.NodeIp;// IpHelper.GetLocalIPs().ToList();
            //var sysInfo = new
            //{
            //    SysAppId = appId,
            //    SysAppCode = appCode,
            //    SysAppName = appName,
            //    ServerIP = serverIp,
            //    BuildType = buildType,
            //};
            var sysInfo = GetSysInfo(buildType);
            var msg = $"offline info is {sysInfo.ToJsonStr()}";
            Add(msg, LogLevels.SysOffline, exception, serverIp: serverIp);
        }
        private static object GetSysInfo(string buildType)
        {
            var sysInfo = HardInfo.TryGetAppRegInfo(buildType);

            return sysInfo;

        }

        public static string ReqTraceIdKey => ConstantKeys.ReqTraceIdKey;

        public static void Report(JObject jo, string traceId = "")
        {
            if (DcLoggerProcessor == null || DcLogOptions == null)
            {
                return;
            }

            var logLevel = LogLevels.Information;
            var reqTraceId = jo.SafeGetValue(ReqTraceIdKey, "");

            if (reqTraceId.IsNotNullOrEmpty())
            {
                traceId = jo[ReqTraceIdKey].ToString();
            }

            var now = HardInfo.Now;

            var logTimestamps = now.ConvertToUnixTime();

            var logDate = now.ToString("yyyyMMdd");
            var reqLogLevel = jo.SafeGetValue("level", "");
            if (reqLogLevel.IsNotNullOrEmpty())
            {
                logLevel = reqLogLevel.Convert2LogLevel();
            }


            var logMsg = new SysLogs
            {
                SerialNumber = traceId,
                LogAppCode = DcLogOptions?.LogAppCode ?? "",
                LogAppName = DcLogOptions?.LogAppName ?? "",
                Level = logLevel.ToString(),
                LevelType = (int)logLevel,
                LogTs = logTimestamps,
                NodeCode = HardInfo.NodeCode ?? "",
                LogPluginVersion = LogPluginVersion,
                //BizSvrKind = HardInfo.QueryBizSvrKind(),
                //BizSvrType = HardInfo.QueryBizSvrType(),
            };



            var logAppCode = jo.SafeGetValue("logAppCode", "");

            if (logAppCode.IsNullOrEmpty())
            {
                jo["logAppCode"] = DcLogOptions.LogAppCode;
            }

            var logAppName = jo.SafeGetValue("logAppName", "");

            if (logAppName.IsNullOrEmpty())
            {
                logAppName = DcLogOptions.LogAppName;
                jo["logAppName"] = logAppName;
            }

            var extDict = jo.SafeGetValue("ExtInfo", "{}").To<Dictionary<string, object>>() ?? new Dictionary<string, object>();

            var svrKind = jo["BizSvrKind"]?.ToString();
            if (svrKind.IsNullOrEmpty())
            {
                var proxyBizSvrKind = HardInfo.QueryBizSvrKind();
                svrKind = proxyBizSvrKind;
                jo["BizSvrKind"] = proxyBizSvrKind;
                extDict["ProxyBizSvrKind"] = proxyBizSvrKind;

            }

            logMsg.BizSvrKind = svrKind;

            var svrAppType = jo["BizSvrType"]?.ToString();
            if (svrAppType.IsNullOrEmpty())
            {
                var proxyBizSvrType = HardInfo.QueryBizSvrType();
                svrAppType = proxyBizSvrType;
                jo["BizSvrType"] = proxyBizSvrType;
                extDict["ProxyBizSvrType"] = proxyBizSvrType;
            }

            logMsg.BizSvrType = svrAppType;

            jo["ExtInfo"] = extDict.ToJObject();

            logMsg.ExtInfo = extDict;

            logMsg.LogAppCode = jo["logAppCode"].ToString();
            logMsg.LogAppName = jo["logAppName"].ToString();

            jo["logTs"] = logTimestamps;

            jo["logDate"] = logDate;
            logMsg.RunMs = jo.SafeGetValue("runMs", 0);
            logMsg.RequestUrl = jo.SafeGetValue("requestUrl", "");
            if (logLevel == LogLevels.Error)
            {
                logMsg.Exceptions = jo.SafeGetValue("exceptions", "{}");
                logMsg.ExceptionObj = logMsg.Exceptions.To<ExceptionData>();
                //if (!string.IsNullOrEmpty(jo["exceptions"]?.ToString()))
                //{
                //    logMsg.Exceptions = jo.SafeGetValue("exceptions", "{}");
                //    logMsg.ExceptionObj = logMsg.Exceptions.To<ExceptionData>();
                //}
            }

            var message = $"{logAppName} TraceId[{traceId}]:Report {jo.ToJsonStr()}";

            logMsg.Content = FixContent(message);

            DcLoggerProcessor.EnqueueMessage(logMsg);
        }




        /// <summary>
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="logLevel"></param>
        /// <param name="exception"></param>
        /// <param name="traceId"></param>
        /// <param name="millisecond"> 耗时 </param>
        /// <param name="url"></param>
        /// <param name="serverIp"></param>
        /// <param name="clientIp"></param>
        public static void Add(string msg, LogLevels logLevel, Exception exception = null, string traceId = "",
            long millisecond = 0, string url = "", string serverIp = "", string clientIp = "")
        {
            try
            {
                if (DcLoggerProcessor == null || DcLogOptions == null)
                {
                    return;
                }

                if ((int)logLevel < (int)DcLogOptions.MinLogLevel)
                {
                    return;
                }

                if (msg.Contains("syslogs"))
                {
                    // Console.WriteLine($"MinLogLevel is {DbLogOptions.MinLogLevel}({(int)DbLogOptions.MinLogLevel})");
                    return;
                }

                if (traceId.IsNullOrEmpty())
                {
                    traceId = CurrentItem.GetSerialNumber;
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
                    SerialNumber = traceId,
                    NodeCode = HardInfo.NodeCode ?? "",
                    LogPluginVersion = LogPluginVersion,
                    BizSvrKind = HardInfo.QueryBizSvrKind(),
                    BizSvrType = HardInfo.QueryBizSvrType(),

                };
                if (exception != null)
                {
                    messageEntity.ExceptionObj = new ExceptionData
                    {
                        Message = exception.Message,
                        StackTraceString = exception.StackTrace,
                    };

                    messageEntity.Exceptions = messageEntity.ExceptionObj.ToJsonStr();
                }
                else
                {
                    messageEntity.Exceptions = "{}";
                }

                //  var logMsg = FixLogMessage(msg);
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

        private static string FixContent(string logMsg)
        {
            return logMsg;
            //var configMaxLen = DcLogOptions.ContentMaxLength;
            //var msg = configMaxLen > 0 && logMsg.Length > configMaxLen
            //    ? logMsg.Substring(0, configMaxLen)
            //    : logMsg;
            //return msg;
        }

        private static string FixLogMessage(string msg) => msg;
        //  string.Concat(ConfigItems.SystemAppName, $" [请求序列号：{CurrentItem.GetSerialNumber}]-0: ", msg);
    }
}
