using Microsoft.AspNetCore.Mvc.WnExtensions.Internals;
using Newtonsoft.Json.Extension;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using WindNight.Core.Abstractions;
using IpHelper = WindNight.Extension.HttpContextExtension;

namespace Microsoft.AspNetCore.Mvc.WnExtensions
{
    public class DefaultLogService : ILogService
    {
        public void AddLog(LogLevels logLevel, string msg, Exception exception = null, long millisecond = 0, string url = "",
            string serverIp = "", string clientIp = "", bool appendMessage = true)
        {
            DoConsoleLog(logLevel, msg);
        }

        public void Trace(string msg, long millisecond = 0, string url = "", string serverIp = "", string clientIp = "",
            bool appendMessage = false)
        {
            AddLog(LogLevels.Trace, msg, millisecond: millisecond, url: url, serverIp: serverIp, clientIp: clientIp,
                appendMessage: appendMessage);
        }

        public void Register(string buildType, bool appendMessage = false)
        {
            var sysInfo = new
            {
                SysAppId = ConfigItems.SysAppId,
                SysAppCode = ConfigItems.SysAppCode,
                SysAppName = ConfigItems.SysAppName,
                ServerIP = IpHelper.LocalServerIps,
                BuildType = buildType,
            };
            var msg = $"register info is {sysInfo.ToJsonStr()}";
            AddLog(LogLevels.SysRegister, msg, serverIp: IpHelper.LocalServerIp, appendMessage: appendMessage);
        }

        public void Offline(string buildType, Exception exception = null, bool appendMessage = false)
        {
            var sysInfo = new
            {
                SysAppId = ConfigItems.SysAppId,
                SysAppCode = ConfigItems.SysAppCode,
                SysAppName = ConfigItems.SysAppName,
                ServerIP = IpHelper.LocalServerIps,
                BuildType = buildType,
            };
            var msg = $"offline info is {sysInfo.ToJsonStr()}";
            AddLog(LogLevels.SysOffline, msg, exception: exception, serverIp: IpHelper.LocalServerIp, appendMessage: appendMessage);

        }

        public void Report(JObject obj, string serialNumber = "")
        {
            AddLog(LogLevels.Report, obj.ToJsonStr());
        }

        public void Debug(string msg, long millisecond = 0, string url = "", string serverIp = "",
            string clientIp = "", bool appendMessage = false)
        {
            AddLog(LogLevels.Debug, msg, millisecond: millisecond, url: url, serverIp: serverIp, clientIp: clientIp,
                appendMessage: appendMessage);
        }

        public void Info(string msg, long millisecond = 0, string url = "", string serverIp = "",
            string clientIp = "", bool appendMessage = false)
        {
            AddLog(LogLevels.Information, msg, millisecond: millisecond, url: url, serverIp: serverIp, clientIp: clientIp,
                appendMessage: appendMessage);
        }

        public void Warn(string msg, Exception exception = null, long millisecond = 0, string url = "",
            string serverIp = "", string clientIp = "", bool appendMessage = true)
        {
            AddLog(LogLevels.Warning, msg, exception, millisecond: millisecond, url: url, serverIp: serverIp,
                clientIp: clientIp, appendMessage: appendMessage);
        }

        public void Error(string msg, Exception exception, long millisecond = 0, string url = "",
            string serverIp = "",
            string clientIp = "", bool appendMessage = true)
        {
            AddLog(LogLevels.Error, msg, exception, millisecond: millisecond, url: url, serverIp: serverIp,
                clientIp: clientIp, appendMessage: appendMessage);
        }

        public void Fatal(string msg, Exception exception, long millisecond = 0, string url = "",
            string serverIp = "", string clientIp = "", bool appendMessage = false)
        {
            AddLog(LogLevels.Critical, msg, exception, millisecond: millisecond, url: url, serverIp: serverIp,
                clientIp: clientIp, appendMessage: appendMessage);
        }

        void DoConsoleLog(LogLevels logLevel, string message)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            if (logLevel > LogLevels.Information) Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(
                $"【{logLevel.ToString()}】: DefaultLogService: {message}");
            Console.ResetColor();
        }
    }
}
