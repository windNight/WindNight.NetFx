using WindNight.Core.Abstractions;

namespace WindNight.Extension.Logger.DcLog.Abstractions
{
    /// <summary>
    ///     兼容统计数据的需求日志
    /// </summary>
    internal class StateDataEntry
    {
        public LogLevels Level { get; set; }

        public string ApiUrl { get; set; } = "";

        public string ClientIP { get; set; } = "";

        public string ServerIP { get; set; } = "";

        public string EventName { get; set; } = "";
        /// <summary>
        ///  耗时 毫秒数
        /// </summary>
        public long Timestamps { get; set; } = 0L;
        /// <summary>
        ///  日志产生的时间
        /// </summary>
        public long LogTs { get; set; }
        public string SerialNumber { get; set; } = "";
        public string Msg { get; set; } = "";
        public bool IsForce { get; set; } = false;

        public override string ToString()
        {
            return Msg;
        }
    }
}
