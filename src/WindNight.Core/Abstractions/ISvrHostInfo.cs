using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection.WnExtension;
using Microsoft.Extensions.Hosting;
using WindNight.Core.Abstractions.Ex;
using WindNight.Core.ConfigCenter.Extensions;
using WindNight.Core.@internal;
using WindNight.Linq.Extensions.Expressions;

namespace WindNight.Core.Abstractions
{
    public interface IQuerySvrHostInfo
    {
        ISvrHostInfo GetSvrHostInfo();
        string QueryBuildType();
        string QueryBuildMachineName();
    }

    public interface ISvrHostInfo
    {

        string BuildType { get; set; }
        string BuildMachineName { get; set; }

        string CompileTime { get; set; }
        string MainAssemblyName { get; set; }

        string MainAssemblyVersion { get; set; }
        //string NowTime { get; set; }

    }


    public interface IAppHardInfo
    {
        string NodeCode { get; }
        IEnumerable<string> NodeIpList { get; }
        string NodeIpAddress { get; }
        string EnvironmentName { get; }
        string ApplicationName { get; }
        string ContentRootPath { get; }
        OperatorSysEnum OperatorSys { get; }
    }

    public interface IAppBaseInfo
    {
        string AppId { get; }
        string AppCode { get; }
        string AppName { get; }
    }

    public class DefaultAppBaseInfo : IAppBaseInfo
    {
        public DefaultAppBaseInfo()
        {
            AppId = DefaultConfigItemBase.SystemAppId.ToString();
            AppCode = DefaultConfigItemBase.SystemAppCode;
            AppName = DefaultConfigItemBase.SystemAppName;
        }

        public string AppId { get; set; }
        public string AppCode { get; set; }
        public string AppName { get; set; }

    }

    public interface IAppRegisterInfo : IAppBaseInfo
    {
        IAppHardInfo HardInfo { get; }

        long RegisteredTs { get; }
        ISvrHostInfo SvrHostInfo { get; }
        string BuildType { get; }
        OperatorSysEnum OperatorSys { get; }
        string PlatName { get; }
        bool IsNullOrEmpty();
        bool ResetBuildType(string buildType = "");


        Dictionary<string, object> BuildExtDict { get; }

        bool FillBuildExtDict(IReadOnlyDictionary<string, object> dict);

    }

    public class AppRegisterInfo : IAppRegisterInfo
    {
        public IAppHardInfo HardInfo { get; private set; }

        public string AppId { get; private set; }

        public string AppCode { get; private set; }

        public string AppName { get; private set; }

        public long RegisteredTs { get; private set; }

        public ISvrHostInfo SvrHostInfo => System.HardInfo.SvrHostInfo;
        public Dictionary<string, object> BuildExtDict { get; private set; } = new Dictionary<string, object>();

        private string buildType = "";

        public string BuildType
        {
            get
            {
                if (!buildType.IsNullOrEmpty())
                {
                    return buildType;
                }
                return SvrHostInfo?.BuildType ?? "";
            }
            private set { buildType = value; }
        }

        public bool ResetBuildType(string buildType = "")
        {
            if (!buildType.IsNullOrEmpty())
            {
                BuildType = buildType;
            }

            return true;
        }

        public OperatorSysEnum OperatorSys => HardInfo?.OperatorSys ?? OperatorSysEnum.Unknown;

        public string PlatName => OperatorSys.ToName();

        public static IAppRegisterInfo Gen()
        {
            var info = new AppRegisterInfo
            {

                AppId = DefaultConfigItemBase.SystemAppId.ToString(),
                AppCode = DefaultConfigItemBase.SystemAppCode,
                AppName = DefaultConfigItemBase.SystemAppName,
                RegisteredTs = System.HardInfo.NowUnixTime,
                HardInfo = AppHardInfo.Gen(),
                //SvrHostInfo = System.HardInfo.SvrHostInfo,
            };

            return info;
        }

        public bool FillBuildExtDict(IReadOnlyDictionary<string, object> dict)
        {
            if (!dict.IsNullOrEmpty())
            {
                foreach (var item in dict)
                {
                    if (BuildExtDict.ContainsKey(item.Key))
                    {
                        continue;
                    }

                    BuildExtDict[item.Key] = item.Value;
                }
            }
            return true;
        }
        public bool IsNullOrEmpty()
        {
            return AppCode.IsNullOrEmpty();
        }


    }

    public class SvrHostBaseInfo : ISvrHostInfo
    {

        public virtual string BuildType { get; set; } = "";
        public virtual string BuildMachineName { get; set; } = "";

        public virtual string CompileTime { get; set; } = "";
        public virtual string MainAssemblyName { get; set; } = "";
        public virtual string MainAssemblyVersion { get; set; } = "";
        //public virtual string NowTime { get; set; } = "";
    }

    public class DefaultSvrHostInfo : SvrHostBaseInfo
    {
        public virtual int AppId { get; set; }

        public virtual string AppCode { get; set; } = "";

        public virtual string AppName { get; set; } = "";
        public virtual string QueryDateTime { get; set; }

        public virtual string EnvironmentName { get; set; } = "";

        public virtual string ContentRootPath { get; set; } = "";
        public virtual string ClientIp { get; set; } = "";
        public virtual string ServerIp { get; set; } = "";
        public virtual string NodeCode { get; set; } = "";
        public virtual string RunMachineName { get; set; } = "";

        protected static string GetRunMachineName()
        {
            try
            {
                return Environment.MachineName;
            }
            catch (Exception ex)
            {
                return "";
            }
        }

        public static DefaultSvrHostInfo GenDefault
        {
            get
            {
                var env = Ioc.GetService<IHostEnvironment>();
                var environmentName = env?.EnvironmentName ?? "";
                var applicationName = env?.ApplicationName ?? "";
                var rootPath = env?.ContentRootPath ?? "";
                if (applicationName.IsNullOrEmpty())
                {
                    applicationName = ConfigItems.SystemAppName;
                }


                var model = new DefaultSvrHostInfo
                {
                    AppId = ConfigItems.SystemAppId.ToInt(),
                    AppCode = ConfigItems.SystemAppCode,
                    AppName = applicationName,
                    ContentRootPath = rootPath,
                    EnvironmentName = environmentName,
                    QueryDateTime = HardInfo.NowFullString,
                    ServerIp = HardInfo.GetLocalIp(),
                    NodeCode = HardInfo.NodeCode,
                    RunMachineName = GetRunMachineName(),
                };
                var impl = Ioc.GetService<IQuerySvrHostInfo>();
                if (impl != null)
                {
                    var svrBaseInfo = impl.GetSvrHostInfo();
                    if (svrBaseInfo != null)
                    {
                        model.BuildType = svrBaseInfo.BuildType;
                        model.MainAssemblyVersion = svrBaseInfo.MainAssemblyVersion;
                        model.MainAssemblyName = svrBaseInfo.MainAssemblyName;
                        model.CompileTime = svrBaseInfo.CompileTime;
                        model.BuildMachineName = svrBaseInfo.BuildMachineName;
                    }
                }
                return model;
            }
        }
    }



}
