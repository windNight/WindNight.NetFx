using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using MJsonIgnore = System.Text.Json.Serialization.JsonIgnoreAttribute;
using NJsonIgnore = Newtonsoft.Json.JsonIgnoreAttribute;

namespace WindNight.Core.Abstractions
{
    public interface IQuerySvrHostInfo
    {
        Assembly MainAssembly { get; }

        bool SysApiCheckIp(string ip);

        IEnumerable<string> QueryWhiteIpList();

        ISvrHostInfo QuerySvrHostInfo();
        ISvrBuildInfo QuerySvrBuildInfo();
        string QueryBuildInfoItem(string key, string defaultValue = "");
        long QueryBuildInfoItem(string key, long defaultValue = 0L);
        string QueryBuildType();
        string QueryBuildMachineName();

    }





    /// <summary>
    ///   dll runtime Info
    /// </summary>
    public interface ISvrHostInfo : IAppBaseInfo
    {
        #region Obsolete

        //[Obsolete("data from ISvrBuildInfo ", true)]
        //string BuildType { get; set; }
        //[Obsolete("data from ISvrBuildInfo ", true)]
        //string BuildMachineName { get; set; }

        //[Obsolete("data from ISvrBuildInfo ", true)]
        //string CompileTime { get; set; }

        //[Obsolete("data from MainAssemblyInfo ", true)]
        //string MainAssemblyName { get; set; }

        //[Obsolete("data from MainAssemblyInfo ", true)]
        //string MainAssemblyVersion { get; set; }
        //[Obsolete("data from MainAssemblyInfo ", true)]
        //string MainAssemblyLastModifyTime { get; set; }

        #endregion

        string NodeCode { get; }
        IEnumerable<string> NodeIpList { get; }
        [MJsonIgnore, NJsonIgnore]
        string NodeIpAddress { get; }
        string EnvironmentName { get; }
        string ApplicationName { get; }
        string ContentRootPath { get; }

        OperatorSysEnum OperatorSys { get; }

        string OperatorSysName { get; }

        IAssemblyInfoDto MainAssemblyInfo { get; }



    }







}
