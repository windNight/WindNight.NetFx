using System;
using System.Collections.Generic;
using System.Text;

namespace WindNight.Core.Abstractions.SvrMonitor
{
    public interface ISvrCenterMonitorApp
    {
        ISvrCenterReportRes PushSvrHeartInfo(ISvrMonitorInfo svrInfo);
        ISvrCenterReportRes PushSvrOfflineInfo(ISvrMonitorInfo svrInfo);
        ISvrCenterReportRes PushSvrRegisterInfo(ISvrMonitorInfo svrInfo);
    }
}
