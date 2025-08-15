using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Text;
using WindNight.Core.Abstractions.Ex;
using WindNight.Core.ConfigCenter.Extensions;
using WindNight.Linq.Extensions.Expressions;

namespace WindNight.Core.Abstractions
{

    public interface IAppBaseInfo
    {
        string AppId { get; }
        string AppCode { get; }
        string AppName { get; }
    }


    public interface ISvrRegisterInfo : IAppBaseInfo
    {

        ISvrHostInfo SvrHostInfo { get; }
        ISvrBuildInfo SvrBuildInfo { get; }
        ISvrRuntimeInfo SvrRuntimeInfo { get; }

        long RegisteredTs { get; }

        string RunMachineName { get; }

        //string BuildType { get; }

        //OperatorSysEnum OperatorSys { get; }

        //string PlatName { get; }

        bool IsNullOrEmpty();

        //bool ResetBuildType(string buildType = "");

        //Dictionary<string, object> BuildExtDict { get; }

        //bool FillBuildExtDict(IReadOnlyDictionary<string, object> dict);


    }


    public interface ISvrMonitorInfo : ISvrRegisterInfo
    {
        /// <summary>
        ///  <see cref="SvrMonitorTypeEnum"/>
        /// </summary>
        int MonitorInfoType { get; set; }
        string QueryDateTime { get; set; }
        string ClientIp { get; set; }
        string ServerIp { get; set; }

    }




}
