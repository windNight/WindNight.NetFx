using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Reflection;
using System.Text.Extension;
using Microsoft.Extensions.DependencyInjection.WnExtension;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Newtonsoft.Json.Extension;
using WindNight.Core.Abstractions;
using WindNight.Extensions.Hosting;

namespace System
{

    /// <summary>
    ///  monitor info
    /// </summary>
    public partial class HardInfo
    {
        public static IHostEnvironment HostEnv => Ioc.GetService<IHostEnvironment>();

        public static string EnvironmentName => HostEnv?.EnvironmentName ?? "";

        public static string ApplicationName => HostEnv?.ApplicationName ?? "";

        public static string ContentRootPath => HostEnv?.ContentRootPath ?? "";

        public static bool IsEnvName(string envName, bool defaultValue = false) => HostEnv.IsEnvName(envName, defaultValue: defaultValue);

        public static bool IsTestEnv(bool defaultValue = true) => QuerySvrHostInfoImpl?.IsTestEnv(defaultValue) ?? defaultValue;

        public static bool HasWebApi(bool defaultValue = true) => QuerySvrHostInfoImpl?.HasWebApi(defaultValue) ?? defaultValue;

        public static bool SysApiCheckIp(string ip,bool defaultValue = true) => QuerySvrHostInfoImpl?.SysApiCheckIp(ip) ?? defaultValue;


        public static IQuerySvrHostInfo QuerySvrHostInfoImpl => Ioc.GetService<IQuerySvrHostInfo>();

        public static string QueryBuildType() => QuerySvrHostInfoImpl?.QueryBuildType() ?? "";

        public static long QuerySvrRegisteredTs() => SvrMonitorInfo?.RegisteredTs ?? 0L;

        public static long QueryBuildTs() => QueryBuildInfoItem("BuildTs", 0L);

        public static string QueryBuildDateTime() => QueryBuildInfoItem("BuildTime", "");

        public static string QueryBuildVersion() => QueryBuildInfoItem("BuildVersion", "");

        public static string QueryBuildUserName() => QueryBuildInfoItem("UserName", "");

        public static string QueryBuildMachineName() => QueryBuildInfoItem("MachineName", "");

        public static string QueryBuildProjectName() => QueryBuildInfoItem("BuildProjectName", "");

        public static string QueryBuildGitBranch() => QueryBuildInfoItem("GitBranch", "");

        public static string QueryBuildHashCode() => QueryBuildInfoItem("HashCode", "");

        public static string QueryBizSvrType() => QueryBuildInfoItem("BizSvrType", "");

        public static string QueryBizSvrKind() => QueryBuildInfoItem("BizSvrKind", "");
     
        public static string QueryBuildInfoItem(string itemKey, string defaultValue = "") => QuerySvrBuildInfo().QueryBuildInfoItem(itemKey, defaultValue);

        public static long QueryBuildInfoItem(string itemKey, long defaultValue = 0L) => QuerySvrBuildInfo().QueryBuildInfoItem(itemKey, defaultValue);

        public static int QueryRuntimeProcessId() => QuerySvrRuntimeInfo().ProcessId;

        public static string QueryRunMachineUserName() => QuerySvrRuntimeInfo()?.RunMachineUserName ?? "";

        public static string QueryRunMachineName()
        {
            try
            {
                var implName = QuerySvrRuntimeInfo()?.RunMachineName ?? "";
                if (implName.IsNotNullOrEmpty())
                {
                    return implName;
                }
                return Environment.MachineName;
            }
            catch (Exception ex)
            {
                return "";
            }

        }


        public static ISvrMonitorInfo SvrMonitorInfo { get; private set; }

        /// <summary>
        ///  服务基础信息相关  AppId, AppCode, AppName 等
        /// </summary>
        public static IAppBaseInfo QueryAppBaseInfo() => DefaultAppBaseInfo.Gen();


        public static ISvrRuntimeInfo QuerySvrRuntimeInfo() => SvrRuntimeInfo.Gen();

        public static IEnumerable<string> QueryWhiteIpList() => QuerySvrHostInfoImpl?.QueryWhiteIpList() ?? EmptyList<string>();

        /// <summary>
        ///  包含服务基础信息外 包含服务的主程序的相关信息
        /// </summary>
        public static ISvrHostInfo QuerySvrHostInfo() => SvrHostBaseInfo.Gen();

        public static ISvrBuildInfo QuerySvrBuildInfo() => QuerySvrHostInfoImpl?.QuerySvrBuildInfo() ?? new SvrBuildInfoDto();

        public static ISvrMonitorInfo GenSvrMonitorInfo(SvrMonitorTypeEnum monitorType) => DefaultSvrMonitorInfo.GenSvrMonitorInfo(monitorType);

        public static object Obj()
        {
            var obj = new
            {
                AppId,
                AppCode,
                AppName,
                RegisteredTs = QuerySvrRegisteredTs(),
                BuildType = QueryBuildType() ?? "",
                BuildMachine = QueryBuildMachineName() ?? "",
                ApiVersion = SvrMonitorInfo?.SvrHostInfo?.MainAssemblyInfo?.AssemblyVersion ?? "",
                BuildVersion = QueryBuildVersion() ?? "",
                BuildTime = QueryBuildDateTime() ?? "",
                PId = SvrMonitorInfo?.SvrRuntimeInfo?.ProcessId ?? -1,
                SvrHostInfo = SvrMonitorInfo?.SvrHostInfo,
                SvrBuildInfo = SvrMonitorInfo?.SvrBuildInfo,
                CurrentVersion,
                CurrentCompileTime,
            };

            return obj;
        }


 
        public new static string ToString() => Obj().ToJsonStr();

        public static string ToString(Formatting formatting) => Obj().ToJsonStr(formatting);


        public static ISvrRegisterInfo TryGetAppRegInfo(string buildType)
        {
            var sysInfo = SvrMonitorInfo;

            return sysInfo;

        }

    }



}
