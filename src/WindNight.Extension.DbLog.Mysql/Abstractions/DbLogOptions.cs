using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using WindNight.Core.Abstractions;

namespace WindNight.Extension.Logger.DbLog.Abstractions
{
    public class DbLogOptions
    {
        /// <summary> </summary>
        public LogLevel MinLogLevel { get; set; } = LogLevel.Debug;

        /// <summary> </summary>
        public int LogAppId { get; set; }

        /// <summary> </summary>
        public string LogAppCode { get; set; }

        /// <summary> 项目名称 </summary>
        public string LogAppName { get; set; }

        /// <summary> DbLog版本号 </summary>
        public string DbLogVersion { get; set; }

        public string DbConnectString { get; set; }

        /// <summary> 消息最大的缓存大小 </summary>
        public int QueuedMaxMessageCount { get; set; } = 1024;

        /// <summary> 是否输出日志 </summary>
        public bool IsConsoleLog { get; set; } = false;

        /// <summary> 上报时进行Gzip压缩 </summary>
        public bool OpenGZip { get; set; } = false;

        public int ContentMaxLength { get; set; } = 2000;

    }
}
