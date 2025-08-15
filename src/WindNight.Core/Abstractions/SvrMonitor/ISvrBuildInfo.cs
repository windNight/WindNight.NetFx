using System;
using System.Collections.Generic;
using System.Text;

namespace WindNight.Core.Abstractions
{
    /// <summary>
    ///  build info
    /// </summary>
    public interface ISvrBuildInfo
    {
        IReadOnlyDictionary<string, object> SvrBuildInfoDict { get; }
        string QueryBuildInfoItem(string key, string defaultValue = "");
        long QueryBuildInfoItem(string key, long defaultValue = 0L);
        string QueryBuildType();
        string QueryBuildMachineName();

    }


}
