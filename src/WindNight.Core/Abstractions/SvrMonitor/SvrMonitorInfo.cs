using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace WindNight.Core.Abstractions
{
    public class DefaultSvrMonitorInfo : SvrRegisterInfo, ISvrMonitorInfo
    {
        public DefaultSvrMonitorInfo() : base()
        {

        }

        public virtual int MonitorInfoType { get; set; }
        public virtual string QueryDateTime { get; set; } = "";

        public virtual string ClientIp { get; set; } = "";

        public virtual string ServerIp { get; set; } = "";


        public static ISvrMonitorInfo GenSvrMonitorInfo(SvrMonitorTypeEnum monitorType)
        {
            var svrMonitorInfo = GenDefault;
            svrMonitorInfo.MonitorInfoType = (int)monitorType;

            return svrMonitorInfo;
        }


        internal static ISvrMonitorInfo Gen()
        {
            var info = new DefaultSvrMonitorInfo
            {
                RegisteredTs = HardInfo.NowUnixTime,
                ServerIp = HardInfo.NodeIp,

            };

            return info;

        }

        public static ISvrMonitorInfo GenDefault
        {
            get
            {
                try
                {
                    var model = HardInfo.SvrMonitorInfo;
                    model.QueryDateTime = HardInfo.NowFullString;
                    model.ServerIp = HardInfo.NodeIp;
                    return model;
                }
                catch
                {
                    return HardInfo.SvrMonitorInfo;
                }

            }
        }


    }




}
