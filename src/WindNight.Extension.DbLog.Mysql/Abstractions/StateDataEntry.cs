﻿using System;
using System.Collections.Generic;
using System.Text;
using WindNight.Core.Abstractions;

namespace WindNight.Extension.Logger.DbLog.Abstractions
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
        public long Timestamps { get; set; } = 0;

        public string SerialNumber { get; set; } = "";
        public string Msg { get; set; } = "";

        public override string ToString()
        {
            return Msg;
        }
    }
}