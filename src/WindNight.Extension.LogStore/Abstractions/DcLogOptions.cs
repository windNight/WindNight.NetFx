using Microsoft.Extensions.Logging;

namespace WindNight.Extension.Logger.DcLog.Abstractions
{
    public class DcLogOptions
    {
        /// <summary> </summary>
        public LogLevel MinLogLevel { get; set; } = LogLevel.Debug;

        /// <summary> </summary>
        public int LogAppId { get; set; }

        /// <summary> </summary>
        public string LogAppCode { get; set; }

        /// <summary> 项目名称 </summary>
        public string LogAppName { get; set; }

        /// <summary> DcLog版本号 </summary>
        public string DcLogVersion { get; set; }

        ///// <summary> 环境 </summary>
        //public EnvEnum Env { get; set; } = EnvEnum.Online;

        /// <summary> udp地址 </summary>
        public string HostName { get; set; }

        /// <summary> udp端口  </summary>
        public int Port { get; set; } = 6090;

        /// <summary> 消息最大的缓存大小 </summary>
        public int QueuedMaxMessageCount { get; set; } = 1024;

        /// <summary> 是否输出日志 </summary>
        public bool IsConsoleLog { get; set; } = false;
        public bool IsOpenDebug { get; set; } = false;

        /// <summary> 上报时进行Gzip压缩 </summary>
        public bool OpenGZip { get; set; } = false;

        public int ContentMaxLength { get; set; } = 75 * 1000;

    }
}
