using System;
using System.Collections.Generic;
using System.Text;

namespace WindNight.Core.Abstractions
{
    public class SvrRegisterInfo : ISvrRegisterInfo
    {
        protected SvrRegisterInfo()
        {

        }

        public virtual string AppId => HardInfo.AppId;

        public virtual string AppCode => HardInfo.AppCode;

        public virtual string AppName => HardInfo.AppName;

        public virtual long RegisteredTs { get; protected set; }

        public virtual string RunMachineName => HardInfo.QueryRunMachineName();

        public virtual ISvrHostInfo SvrHostInfo => HardInfo.QuerySvrHostInfo();

        public virtual ISvrBuildInfo SvrBuildInfo => HardInfo.QuerySvrBuildInfo();

        public virtual ISvrRuntimeInfo SvrRuntimeInfo => HardInfo.QuerySvrRuntimeInfo();



        public virtual bool IsNullOrEmpty()
        {
            return AppCode.IsNullOrEmpty();
        }



    }

}
