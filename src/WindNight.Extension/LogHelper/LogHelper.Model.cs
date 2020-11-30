using System;
using Newtonsoft.Json.Extension;
using WindNight.Core.Abstractions;

namespace WindNight.LogExtension
{
    public static partial class LogHelper
    {
        public class LogInfo
        {
            public string SerialNumber { get; set; }
            public string RequestUrl { get; set; }
            public Exception Exceptions { get; set; }
            public string ServerIp { get; set; }
            public string ClientIp { get; set; }
            public long Timestamps { get; set; }
            public LogLevels Level { get; set; }
            public string Content { get; set; }
            public string NodeCode { get; set; }

            public override string ToString()
            {
                return this.ToJsonStr();
            }
        }
        internal class ThreadContext
        {
            /// <summary> 业务序列号  </summary>
            internal const string SERIZLNUMBER = "serialnumber";

            /// <summary>  IP  </summary>
            internal const string CLIENTIP = "___clientip_____";

            internal const string SERVERIP = "___serverip_____";
            internal const string REQUESTPATH = "___request_path_";
        }

    }
}