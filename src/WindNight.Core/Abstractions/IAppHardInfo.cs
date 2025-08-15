using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using WindNight.Core.Abstractions.Ex;

namespace WindNight.Core.Abstractions
{
    //public interface IAppHardInfo
    //{
    //    string NodeCode { get; }
    //    IEnumerable<string> NodeIpList { get; }
    //    string NodeIpAddress { get; }
    //    string EnvironmentName { get; }
    //    string ApplicationName { get; }
    //    string ContentRootPath { get; }
    //    OperatorSysEnum OperatorSys { get; }
    //    string PlatName { get; }

    //}


    //public class AppHardInfo : ISvrHostInfo
    //{
    //    public AppHardInfo()
    //    {

    //    }
    //    public virtual string AppId { get; set; } = HardInfo.AppId;

    //    public virtual string AppCode { get; set; } = HardInfo.AppCode;

    //    public virtual string AppName { get; set; } = HardInfo.AppName;

    //    public virtual string NodeCode { get; protected set; } = HardInfo.NodeCode;
    //    public virtual IEnumerable<string> NodeIpList { get; protected set; } = HardInfo.NodeIpList;
    //    public virtual string NodeIpAddress { get; protected set; } = HardInfo.NodeIpAddress;
    //    public virtual string EnvironmentName { get; protected set; } = HardInfo.EnvironmentName;
    //    public virtual string ApplicationName { get; protected set; } = HardInfo.ApplicationName;
    //    public virtual string ContentRootPath { get; protected set; } = HardInfo.ContentRootPath;

    //    public virtual OperatorSysEnum OperatorSys { get; protected set; } = HardInfo.OperatorSys;

    //    public virtual string OperatorSysName { get; protected set; } = HardInfo.OperatorSysName;
    //    public virtual IAssemblyInfoDto MainAssemblyInfo { get; set; }


    //    public static ISvrHostInfo Default()
    //    {
    //        return new AppHardInfo();
    //    }

    //    static Assembly MainAssembly => Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly();

    //    public static ISvrHostInfo Gen()
    //    {
    //        return Gen(MainAssembly);

    //    }

    //    public static ISvrHostInfo Gen(Assembly assembly)
    //    {
    //        var info = new AppHardInfo
    //        {
    //            MainAssemblyInfo = assembly.Analysis(),

    //        };
    //        return info;



    //    }


    //}
}
