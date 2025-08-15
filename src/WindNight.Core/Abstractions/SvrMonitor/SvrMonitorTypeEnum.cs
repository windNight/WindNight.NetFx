using System;
using System.Collections.Generic;
using System.Text;

namespace WindNight.Core.Abstractions
{
    public enum SvrMonitorTypeEnum
    {
        Unknown = 0,
        Register = 10,
        Offline = 11,
        Heartbeat = 20,
        Error = 30,
        Query = 40,
    }
}
