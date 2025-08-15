
using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection.WnExtension;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json.Extension;
using WindNight.Core.Extension;
using WindNight.Linq.Extensions.Expressions;

namespace WindNight.Core.Abstractions
{
    /// <summary>
    ///   dll runtime Info
    /// </summary>
    public class SvrHostBaseInfo : DefaultAppBaseInfo, ISvrHostInfo
    {
        protected SvrHostBaseInfo() : base()
        {

        }



        public virtual string NodeCode { get; protected set; } = HardInfo.NodeCode;

        public virtual IEnumerable<string> NodeIpList { get; protected set; } = HardInfo.NodeIpList;

        public virtual string NodeIpAddress { get; protected set; } = HardInfo.NodeIpAddress;

        public virtual string EnvironmentName { get; protected set; } = HardInfo.EnvironmentName;

        public virtual string ApplicationName { get; protected set; } = HardInfo.ApplicationName;

        public virtual string ContentRootPath { get; protected set; } = HardInfo.ContentRootPath;

        public virtual OperatorSysEnum OperatorSys { get; protected set; } = HardInfo.OperatorSys;

        public virtual string OperatorSysName { get; protected set; } = HardInfo.OperatorSysName;

        public virtual IAssemblyInfoDto MainAssemblyInfo { get; set; }



        static Assembly MainAssembly => Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly();

        public new static SvrHostBaseInfo Gen()
        {

            return Gen(MainAssembly);

        }

        public static SvrHostBaseInfo Gen(Assembly assembly)
        {
            if (assembly == null)
            {
                assembly = MainAssembly;
            }
            var info = new SvrHostBaseInfo
            {
                MainAssemblyInfo = assembly.Analysis(),
            };
            return info;

        }



    }



    //public class DefaultSvrHostInfo : AppRegisterInfo, ISvrReportInfo // SvrHostBaseInfo
    //{
    //    protected DefaultSvrHostInfo()
    //    {


    //    }


    //    public virtual string QueryDateTime { get; set; }

    //    public virtual string ClientIp { get; set; } = "";

    //    public virtual string ServerIp { get; set; } = "";

    //    public virtual string RunMachineName { get; set; } = "";


    //    public new static ISvrReportInfo Gen()
    //    {
    //        var info = new DefaultSvrHostInfo
    //        {
    //            RegisteredTs = HardInfo.NowUnixTime,
    //            RunMachineName = HardInfo.QueryRunMachineName(),
    //            ServerIp = HardInfo.NodeIpAddress,
    //        };

    //        return info;


    //    }

    //    public static ISvrReportInfo GenDefault
    //    {
    //        get
    //        {
    //            var model = HardInfo.AppRegisteredInfo;
    //            model.QueryDateTime = HardInfo.Now.FormatDateTimeFullString();
    //            model.RunMachineName = HardInfo.QueryRunMachineName();
    //            model.ServerIp = HardInfo.NodeIpAddress;

    //            return model;

    //        }
    //    }


    //}


    public class DefaultAppBaseInfo : IAppBaseInfo
    {

        protected DefaultAppBaseInfo()
        {

        }

        public static IAppBaseInfo Gen()
        {

            return new DefaultAppBaseInfo();

        }
        public virtual string AppId { get; set; } = HardInfo.AppId;

        public virtual string AppCode { get; set; } = HardInfo.AppCode;

        public virtual string AppName { get; set; } = HardInfo.AppName;

    }





}
