using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection.WnExtension;
using Microsoft.Extensions.Hosting;
using WindNight.Core.@internal;

namespace WindNight.Core.Abstractions
{
    public interface IQuerySvrHostInfo
    {
        ISvrHostInfo GetSvrHostInfo();
    }

    public interface ISvrHostInfo
    {

        string BuildType { get; set; }

        string CompileTime { get; set; }
        string AssemblyVersion { get; set; }

    }



    public class SvrHostBaseInfo : ISvrHostInfo
    {

        public virtual string BuildType { get; set; }

        public virtual string CompileTime { get; set; }
        public virtual string AssemblyVersion { get; set; }
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
                };
                var impl = Ioc.GetService<IQuerySvrHostInfo>();
                if (impl != null)
                {
                    var svrBaseInfo = impl.GetSvrHostInfo();
                    if (svrBaseInfo != null)
                    {
                        model.BuildType = svrBaseInfo.BuildType;
                        model.AssemblyVersion = svrBaseInfo.AssemblyVersion;
                        model.CompileTime = svrBaseInfo.CompileTime;
                    }
                }
                return model;
            }
        }
    }



}
