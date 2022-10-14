using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json.Extension;
using WindNight.Core.Abstractions;
using WindNight.Core.SQL;

namespace WindNight.Extension.Logger.DbLog.Abstractions
{
    public class SysLogs : CreateBase<long>
    {
        public string SerialNumber { get; set; }
        public string RequestUrl { get; set; }
        public string Exceptions { get; set; }
        public Exception ExceptionObj  => Exceptions.To<Exception>();
        public string? ServerIp { get; set; }
        public string ClientIp { get; set; }
        public long LogTs { get; set; }
        public LogLevels Level { get; set; }
        public string Content { get; set; }
        public string AppCode { get; set; }
        public string NodeCode { get; set; }

    }
}
