using System;
using System.Collections.Generic;
using System.Text;

namespace WindNight.Core.Abstractions
{

    public class ServerCpuUsageInfo : IServerCpuUsageInfo
    {
        public ServerCpuUsageInfo()
        {

        }

        public ServerCpuUsageInfo(TimeSpan userTime, TimeSpan privilegedTime, TimeSpan totalTime)
        {

        }

        public virtual TimeSpan UserTime { get; protected set; }

        public virtual TimeSpan PrivilegedTime { get; protected set; }

        public virtual TimeSpan TotalTime { get; protected set; }

    }



    /// <inheritdoc /> 
    public class AssemblyInfoDto : IAssemblyInfoDto
    {
        public virtual bool IsSuccess { get; set; }
        public virtual string AssemblyCodeBase { get; set; } = "";
        public virtual string AssemblyFullName { get; set; } = "";
        public virtual string AssemblyName { get; set; } = "";
        public virtual string AssemblyLocation { get; set; } = "";
        public virtual string AssemblyVersion { get; set; } = "";
        public virtual string AssemblyLastCreateTime { get; set; }
        public virtual string AssemblyLastModifyTime { get; set; } = "";




    }



}
