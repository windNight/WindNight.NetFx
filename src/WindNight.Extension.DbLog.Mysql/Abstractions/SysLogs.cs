using Microsoft.Extensions.Logging;
using WindNight.Core.Abstractions;
using WindNight.Core.SQL;

namespace WindNight.Extension.Logger.DbLog.Abstractions
{
    public class SysLogs : CreateBase<long>
    {
        public string SerialNumber { get; set; } = "";
        public string RequestUrl { get; set; } = "";
        public string Exceptions { get; set; } = "{}";
        public ExceptionData ExceptionObj { get; set; }

        public string? ServerIp { get; set; } = "";
        public string ClientIp { get; set; } = "";
        public long LogTs { get; set; } = 0;
        public string Level { get; set; } = "";
        /// <summary>
        /// <see cref="LogLevels"/>
        /// </summary>
        public int LevelType { get; set; }

        public string Content { get; set; } = "";
        public string LogAppCode { get; set; } = "";

        public string LogAppName { get; set; } = "";
        public string NodeCode { get; set; } = "";
        public string DbLogVersion { get; set; } = "1.0.0";

    }

    public class ExceptionData
    {
        public string Message { get; set; }
        public string StackTraceString { get; set; }
    }


}
