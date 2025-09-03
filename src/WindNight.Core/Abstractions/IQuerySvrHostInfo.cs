using System;
using System.Collections.Generic;
using System.Text;

namespace WindNight.Core.Abstractions
{
    public interface IQuerySvrHostInfo
    {
        //Assembly MainAssembly { get; }

        bool SysApiCheckIp(string ip);

        IEnumerable<string> QueryWhiteIpList();

        //ISvrHostInfo QuerySvrHostInfo();

        ISvrBuildInfo QuerySvrBuildInfo();

        //string QueryBuildInfoItem(string key, string defaultValue = "");

        //long QueryBuildInfoItem(string key, long defaultValue = 0L);

        string QueryBuildType();

        string QueryBuildMachineName();
        bool IsTestEnv(bool defaultValue = true);
    }

}
