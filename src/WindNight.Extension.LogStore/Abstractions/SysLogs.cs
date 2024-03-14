using WindNight.Core.Abstractions;

namespace WindNight.Extension.Logger.DcLog.Abstractions
{

    public class SysLogs //: CreateBase<long>
    {
        public bool IsForce { get; set; } = false;

        public string SerialNumber { get; set; } = "";
        public string RequestUrl { get; set; } = "";
        public string Exceptions { get; set; } = "{}";
        public ExceptionData ExceptionObj { get; set; }

        public string? ServerIp { get; set; } = "";
        public string ClientIp { get; set; } = "";

        /// <summary>
        ///     运行耗时 毫秒
        /// </summary>
        public long RunMs { get; set; } = 0;

        /// <summary>
        ///     日志触发时间戳
        /// </summary>
        public long LogTs { get; set; } = 0;

        public string Level { get; set; } = "";

        /// <summary>
        ///     <see cref="LogLevels" />
        /// </summary>
        public int LevelType { get; set; }

        public string Content { get; set; } = "";
        public string LogAppCode { get; set; } = "";

        public string LogAppName { get; set; } = "";
        public string NodeCode { get; set; } = "";
        public string LogPluginVersion { get; set; } = "1.0.0";

        public override string ToString()
        {
            return Content;
        }
    }

    public class ExceptionData
    {
        public string Message { get; set; }
        public string StackTraceString { get; set; }
    }

}
