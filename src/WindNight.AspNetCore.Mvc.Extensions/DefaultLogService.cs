//using System;
//using System.Linq;
//using Microsoft.AspNetCore.Mvc.WnExtensions.@internal;
//using Newtonsoft.Json.Extension;
//using Newtonsoft.Json.Linq;
//using WindNight.Core.Abstractions;
//using WindNight.Core.ExceptionExt;
//using IpHelper = WindNight.Extension.HttpContextExtension;

//namespace Microsoft.AspNetCore.Mvc.WnExtensions
//{
//    public class DefaultLogService : ILogService
//    {
//        public void Trace(string msg, long millisecond = 0, string url = "", string serverIp = "", string clientIp = "",
//            bool appendMessage = false, string traceId = "")
//        {
//            AddLog(LogLevels.Trace, msg, millisecond: millisecond, url: url, serverIp: serverIp, clientIp: clientIp,
//                appendMessage: appendMessage, traceId: traceId);
//        }

//        public void Register(string buildType, bool appendMessage = false, string traceId = "")
//        {
//            var sysInfo = new
//            {
//                SysAppId = ConfigItems.SysAppId,
//                SysAppCode = ConfigItems.SysAppCode,
//                SysAppName = ConfigItems.SysAppName,
//                ServerIP = IpHelper.LocalServerIps,
//                BuildType = buildType,
//            };
//            var msg = $"register info is {sysInfo.ToJsonStr()}";
//            AddLog(LogLevels.SysRegister, msg, serverIp: IpHelper.LocalServerIp, appendMessage: appendMessage, traceId: traceId);
//        }

//        public void Offline(string buildType, Exception? exception = null, bool appendMessage = false, string traceId = "")
//        {
//            var sysInfo = new
//            {
//                SysAppId = ConfigItems.SysAppId,
//                SysAppCode = ConfigItems.SysAppCode,
//                SysAppName = ConfigItems.SysAppName,
//                ServerIP = IpHelper.LocalServerIps,
//                BuildType = buildType,
//            };
//            var msg = $"offline info is {sysInfo.ToJsonStr()}";
//            AddLog(LogLevels.SysOffline, msg, exception: exception, serverIp: IpHelper.LocalServerIp, appendMessage: appendMessage, traceId: traceId);

//        }

//        public void Report(JObject obj, string traceId = "")
//        {
//            AddLog(LogLevels.Report, obj.ToJsonStr());
//        }

//        public void Debug(string msg, long millisecond = 0, string url = "", string serverIp = "",
//            string clientIp = "", bool appendMessage = false, string traceId = "")
//        {
//            AddLog(LogLevels.Debug, msg, millisecond: millisecond, url: url, serverIp: serverIp, clientIp: clientIp,
//                appendMessage: appendMessage, traceId: traceId);
//        }

//        public void Info(string msg, long millisecond = 0, string url = "", string serverIp = "",
//            string clientIp = "", bool appendMessage = false, string traceId = "")
//        {
//            AddLog(LogLevels.Information, msg, millisecond: millisecond, url: url, serverIp: serverIp, clientIp: clientIp,
//                appendMessage: appendMessage, traceId: traceId);
//        }

//        public void Warn(string msg, Exception? exception = null, long millisecond = 0, string url = "",
//            string serverIp = "", string clientIp = "", bool appendMessage = true, string traceId = "")
//        {
//            AddLog(LogLevels.Warning, msg, exception, millisecond: millisecond, url: url, serverIp: serverIp,
//                clientIp: clientIp, appendMessage: appendMessage, traceId: traceId);
//        }

//        public void Error(string msg, Exception? exception, long millisecond = 0, string url = "",
//            string serverIp = "", string clientIp = "", bool appendMessage = true, string traceId = "")
//        {
//            AddLog(LogLevels.Error, msg, exception, millisecond: millisecond, url: url, serverIp: serverIp,
//                clientIp: clientIp, appendMessage: appendMessage, traceId: traceId);
//        }

//        public void Fatal(string msg, Exception? exception, long millisecond = 0, string url = "",
//            string serverIp = "", string clientIp = "", bool appendMessage = false, string traceId = "")
//        {
//            AddLog(LogLevels.Critical, msg, exception, millisecond: millisecond, url: url, serverIp: serverIp,
//                clientIp: clientIp, appendMessage: appendMessage, traceId: traceId);
//        }



//        public void AddLog(LogLevels logLevel, string msg, Exception? exception = null, long millisecond = 0, string url = "",
//            string serverIp = "", string clientIp = "", bool appendMessage = true, string traceId = "")
//        {
//            DoConsoleLog(logLevel, msg);
//        }



//        protected static void DoConsoleLog(LogLevels logLevel, string message, Exception? exception = null)
//        {
//            if (ConfigItems.LogOnConsole)
//            {
//                Console.ForegroundColor = ConsoleColor.Green;
//                if (logLevel > LogLevels.Information) Console.ForegroundColor = ConsoleColor.Red;
//                if (exception != null)
//                {
//                    message = $"{message} {Environment.NewLine} {exception.GetMessage()}";
//                }
//                Console.WriteLine($"=={HardInfo.NowString}==ConsoleLog:{Environment.NewLine}{message}");
//                Console.ResetColor();
//            }
//        }


//        protected static void DoConsoleLog(string message)
//        {
//            if (ConfigItems.LogOnConsole)
//            {
//                Console.ForegroundColor = ConsoleColor.Cyan;
//                Console.WriteLine($"=={HardInfo.NowString}==ConsoleLog:{Environment.NewLine}{message}");
//                Console.ResetColor();
//            }
//        }
//    }

//}
