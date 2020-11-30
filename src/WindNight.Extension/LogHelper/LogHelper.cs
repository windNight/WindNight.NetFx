using Newtonsoft.Json.Extension;
using System;
using WindNight.Core.Abstractions;
using IpHelper = WindNight.Extension.HttpContextExtension;

namespace WindNight.LogExtension
{
    public static partial class LogHelper
    {

        public delegate void PublishLogInfoEvent(LogInfo logInfo);

        public static event PublishLogInfoEvent PublishLogInfoHandleEvent;

        public static void RegisterProcessEvent(PublishLogInfoEvent publishLogInfoEvent)
        {
            PublishLogInfoHandleEvent -= publishLogInfoEvent;
            PublishLogInfoHandleEvent += publishLogInfoEvent;
        }

        private static void OnPublishLogInfoHandleEvent(LogInfo logInfo)
        {
            PublishLogInfoHandleEvent?.Invoke(logInfo);
        }


        /// <summary>
        /// </summary>
        /// <param name="url"></param>
        /// <param name="msg"></param>
        /// <param name="millisecond"></param>
        /// <param name="serverIp"></param>
        /// <param name="clientIp"></param>
        /// <param name="appendMessage"></param>
        public static void ApiUrlCall(string url, string msg, long millisecond, string serverIp = "",
            string clientIp = "", bool appendMessage = false)
        {
            Add(msg, LogLevels.ApiUrl, millisecond: millisecond, url: url, serverIp: serverIp, clientIp: clientIp,
                appendMessage: appendMessage);
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
            string clientIp = "", bool appendMessage = true)
        {
            Add(msg, LogLevels.ApiUrlException, exception, url: url, serverIp: serverIp, clientIp: clientIp,
                appendMessage: appendMessage);
        }

        /// <summary>
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="millisecond"></param>
        /// <param name="url"></param>
        /// <param name="serverIp"></param>
        /// <param name="clientIp"></param>
        /// <param name="appendMessage"></param>
        public static void Debug(string msg, long millisecond = 0, string url = "", string serverIp = "",
            string clientIp = "", bool appendMessage = false)
        {
            Add(msg, LogLevels.Debug, millisecond: millisecond, url: url, serverIp: serverIp, clientIp: clientIp,
                appendMessage: appendMessage);
        }

        /// <summary>
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="millisecond"></param>
        /// <param name="url"></param>
        /// <param name="serverIp"></param>
        /// <param name="clientIp"></param>
        /// <param name="appendMessage"></param>
        public static void Info(string msg, long millisecond = 0, string url = "", string serverIp = "",
            string clientIp = "", bool appendMessage = false)
        {
            Add(msg, LogLevels.Information, millisecond: millisecond, url: url, serverIp: serverIp, clientIp: clientIp,
                appendMessage: appendMessage);
        }

        /// <summary>
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="exception"></param>
        /// <param name="millisecond"></param>
        /// <param name="url"></param>
        /// <param name="serverIp"></param>
        /// <param name="clientIp"></param>
        /// <param name="appendMessage"></param>
        public static void Warn(string msg, Exception exception = null, long millisecond = 0, string url = "",
            string serverIp = "", string clientIp = "", bool appendMessage = true)
        {
            Add(msg, LogLevels.Warning, exception, millisecond: millisecond, url: url, serverIp: serverIp,
                clientIp: clientIp, appendMessage: appendMessage);
        }

        /// <summary>
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="exception"></param>
        /// <param name="millisecond"></param>
        /// <param name="url"></param>
        /// <param name="serverIp"></param>
        /// <param name="clientIp"></param>
        /// <param name="appendMessage"></param>
        public static void Error(string msg, Exception exception, long millisecond = 0, string url = "",
            string serverIp = "",
            string clientIp = "", bool appendMessage = true)
        {
            Add(msg, LogLevels.Error, exception, millisecond: millisecond, url: url, serverIp: serverIp,
                clientIp: clientIp, appendMessage: appendMessage);
        }

        /// <summary>
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="exception"></param>
        /// <param name="millisecond"></param>
        /// <param name="url"></param>
        /// <param name="serverIp"></param>
        /// <param name="clientIp"></param>
        /// <param name="appendMessage"></param>
        public static void Fatal(string msg, Exception exception, long millisecond = 0, string url = "",
            string serverIp = "", string clientIp = "", bool appendMessage = false)
        {
            Add(msg, LogLevels.Critical, exception, millisecond: millisecond, url: url, serverIp: serverIp,
                clientIp: clientIp, appendMessage: appendMessage);
        }

        /// <summary>
        /// </summary>
        /// <param name="buildType"></param>
        /// <param name="appendMessage"></param>
        public static void LogRegisterInfo(string buildType, bool appendMessage = false)
        {
            var sysInfo = GetSysInfo(buildType);
            var msg = $"register info is {sysInfo.ToJsonStr()}";
            Add(msg, LogLevels.SysRegister, serverIp: IpHelper.LocalServerIp, appendMessage: appendMessage);
        }

        /// <summary>
        /// </summary>
        /// <param name="buildType"></param>
        /// <param name="exception"></param>
        /// <param name="appendMessage"></param>
        public static void LogOfflineInfo(string buildType, Exception exception = null, bool appendMessage = false)
        {
            var sysInfo = GetSysInfo(buildType);
            var msg = $"offline info is {sysInfo.ToJsonStr()}";
            Add(msg, LogLevels.SysOffline, exception, serverIp: IpHelper.LocalServerIp, appendMessage: appendMessage);
        }

        static object GetSysInfo(string buildType)
        {
            var sysInfo = new
            {
                SysAppId = ConfigItems.SystemAppId,
                SysAppCode = ConfigItems.SystemAppCode,
                SysAppName = ConfigItems.SystemAppName,
                HardInfo = HardInfo.ToString(),
                BuildType = buildType
            };
            return sysInfo;

        }

        /// <summary>
        /// </summary>
        /// <param name="buildType"></param>
        /// <param name="appId"></param>
        /// <param name="appCode"></param>
        /// <param name="appName"></param>
        /// <param name="appendMessage"></param>
        public static void LogRegisterInfo(string buildType, int appId, string appCode, string appName,
            bool appendMessage = false)
        {
            var sysInfo = GetSysInfo(buildType);
            var msg = $"register info is {sysInfo.ToJsonStr()}";
            Add(msg, LogLevels.SysRegister, serverIp: IpHelper.LocalServerIp, appendMessage: appendMessage);
        }

        /// <summary>
        /// </summary>
        /// <param name="buildType"></param>
        /// <param name="appId"></param>
        /// <param name="appCode"></param>
        /// <param name="appName"></param>
        /// <param name="exception"></param>
        /// <param name="appendMessage"></param>
        public static void LogOfflineInfo(string buildType, int appId, string appCode, string appName,
            Exception exception = null, bool appendMessage = false)
        {
            var sysInfo = GetSysInfo(buildType);
            var msg = $"offline info is {sysInfo.ToJsonStr()}";
            Add(msg, LogLevels.SysOffline, exception, serverIp: IpHelper.LocalServerIp, appendMessage: appendMessage);
        }


    }
}