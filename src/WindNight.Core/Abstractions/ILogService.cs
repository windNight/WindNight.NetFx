using System;
using Newtonsoft.Json.Linq;
using WindNight.Core.Enums.Abstractions;

namespace WindNight.Core.Abstractions
{
    public interface ILogService
    {
        /// <summary>
        /// </summary>
        /// <param name="logLevel"></param>
        /// <param name="msg"></param>
        /// <param name="exception"></param>
        /// <param name="millisecond"></param>
        /// <param name="url"></param>
        /// <param name="serverIp"></param>
        /// <param name="clientIp"></param>
        /// <param name="appendMessage"></param>
        void AddLog(LogLevels logLevel, string msg, Exception? exception = null, long millisecond = 0, string url = "",
            string serverIp = "", string clientIp = "", bool appendMessage = true, string traceId = "");

        /// <summary>
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="exception"></param>
        /// <param name="millisecond"></param>
        /// <param name="url"></param>
        /// <param name="serverIp"></param>
        /// <param name="clientIp"></param>
        /// <param name="appendMessage"></param>
        void Error(string msg, Exception? exception, long millisecond = 0, string url = "", string serverIp = "",
            string clientIp = "", bool appendMessage = true, string traceId = "");

        /// <summary>
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="exception"></param>
        /// <param name="millisecond"></param>
        /// <param name="url"></param>
        /// <param name="serverIp"></param>
        /// <param name="clientIp"></param>
        /// <param name="appendMessage"></param>
        void Fatal(string msg, Exception? exception, long millisecond = 0, string url = "", string serverIp = "",
            string clientIp = "", bool appendMessage = false, string traceId = "");

        /// <summary>
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="millisecond"></param>
        /// <param name="url"></param>
        /// <param name="serverIp"></param>
        /// <param name="clientIp"></param>
        /// <param name="appendMessage"></param>
        void Info(string msg, long millisecond = 0, string url = "", string serverIp = "", string clientIp = "",
            bool appendMessage = false, string traceId = "");

        /// <summary>
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="exception"></param>
        /// <param name="millisecond"></param>
        /// <param name="url"></param>
        /// <param name="serverIp"></param>
        /// <param name="clientIp"></param>
        /// <param name="appendMessage"></param>
        void Warn(string msg, Exception? exception = null, long millisecond = 0, string url = "", string serverIp = "",
            string clientIp = "", bool appendMessage = true, string traceId = "");

        /// <summary>
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="millisecond"></param>
        /// <param name="url"></param>
        /// <param name="serverIp"></param>
        /// <param name="clientIp"></param>
        /// <param name="appendMessage"></param>
        void Debug(string msg, long millisecond = 0, string url = "", string serverIp = "", string clientIp = "",
            bool appendMessage = false, string traceId = "");

        /// <summary>
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="millisecond"></param>
        /// <param name="url"></param>
        /// <param name="serverIp"></param>
        /// <param name="clientIp"></param>
        /// <param name="appendMessage"></param>
        void Trace(string msg, long millisecond = 0, string url = "", string serverIp = "", string clientIp = "",
            bool appendMessage = false, string traceId = "");


        /// <summary>
        /// </summary>
        /// <param name="buildType"></param>
        /// <param name="appendMessage"></param>
        void Register(string buildType, bool appendMessage = false, string traceId = "");

        /// <summary>
        /// </summary>
        /// <param name="buildType"></param>
        /// <param name="exception"></param>
        /// <param name="appendMessage"></param>
        void Offline(string buildType, Exception? exception = null, bool appendMessage = false, string traceId = "");


        void Report(JObject obj, string traceId = "");

    }
}
