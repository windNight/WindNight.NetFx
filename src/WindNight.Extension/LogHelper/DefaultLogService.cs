using System;
using Newtonsoft.Json.Linq;
using WindNight.Core.Abstractions;

namespace WindNight.LogExtension
{
    public class DefaultLogService : ILogService
    {
        public void AddLog(LogLevels logLevel, string msg, Exception exception = null, long millisecond = 0,
            string url = "",
            string serverIp = "", string clientIp = "", bool appendMessage = true)
        {
            switch (logLevel)
            {
                case LogLevels.Debug:
                    LogHelper.Debug(msg, millisecond, url, serverIp, clientIp, appendMessage);
                    break;
                case LogLevels.Information:
                    LogHelper.Info(msg, millisecond, url, serverIp, clientIp, appendMessage);
                    break;
                case LogLevels.Warning:
                    LogHelper.Warn(msg, exception, millisecond, url, serverIp, clientIp, appendMessage);
                    break;
                case LogLevels.Error:
                    LogHelper.Error(msg, exception, millisecond, url, serverIp, clientIp, appendMessage);
                    break;
                case LogLevels.Critical:
                    LogHelper.Fatal(msg, exception, millisecond, url, serverIp, clientIp, appendMessage);
                    break;
                case LogLevels.ApiUrl:
                    LogHelper.ApiUrlCall(msg: msg, millisecond: millisecond, url: url, serverIp: serverIp,
                        clientIp: clientIp, appendMessage: appendMessage);
                    break;
                case LogLevels.ApiUrlException:
                    LogHelper.ApiUrlException(msg: msg, exception: exception, url: url, serverIp: serverIp,
                        clientIp: clientIp, appendMessage: appendMessage);
                    break;
                case LogLevels.SysRegister:
                case LogLevels.SysOffline:
                case LogLevels.Report:
                case LogLevels.None:
                case LogLevels.Trace:
                default:
                    break;
            }
        }

        public void Error(string msg, Exception exception, long millisecond = 0, string url = "", string serverIp = "",
            string clientIp = "", bool appendMessage = true)
        {
            AddLog(LogLevels.Error, msg, exception, millisecond, url,
                serverIp, clientIp, appendMessage);
        }

        public void Fatal(string msg, Exception exception, long millisecond = 0, string url = "", string serverIp = "",
            string clientIp = "", bool appendMessage = false)
        {
            AddLog(LogLevels.Critical, msg, exception, millisecond, url,
                serverIp, clientIp, appendMessage);
        }

        public void Info(string msg, long millisecond = 0, string url = "", string serverIp = "", string clientIp = "",
            bool appendMessage = false)
        {
            AddLog(LogLevels.Information, msg, null, millisecond, url,
                serverIp, clientIp, appendMessage);
        }

        public void Warn(string msg, Exception exception = null, long millisecond = 0, string url = "",
            string serverIp = "",
            string clientIp = "", bool appendMessage = true)
        {
            AddLog(LogLevels.Warning, msg, exception, millisecond, url,
                serverIp, clientIp, appendMessage);
        }

        public void Debug(string msg, long millisecond = 0, string url = "", string serverIp = "", string clientIp = "",
            bool appendMessage = false)
        {
            AddLog(LogLevels.Debug, msg, null, millisecond, url,
                serverIp, clientIp, appendMessage);
        }

        public void Trace(string msg, long millisecond = 0, string url = "", string serverIp = "", string clientIp = "",
            bool appendMessage = false)
        {
            AddLog(LogLevels.Debug, msg, null, millisecond, url,
                serverIp, clientIp, appendMessage);
        }

        public void Register(string buildType, bool appendMessage = false)
        {
            LogHelper.LogRegisterInfo(buildType);
        }


        public void Offline(string buildType, Exception exception = null, bool appendMessage = false)
        {
            LogHelper.LogOfflineInfo(buildType, exception);
        }

        public void Report(JObject obj, string serialNumber = "")
        {
        }
    }
}