using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection.WnExtension;
using WindNight.Core.Extension;
using WindNight.Linq.Extensions.Expressions;

namespace WindNight.Core.Abstractions
{
    public class SvrBuildInfoDto : ISvrBuildInfo
    {
        protected virtual IQuerySvrHostInfo QuerySvrHostInfoImpl => HardInfo.QuerySvrHostInfoImpl;

        public virtual IReadOnlyDictionary<string, object> SvrBuildInfoDict { get; set; } =
            new Dictionary<string, object>();

        public virtual void SetSvrBuildInfoDict(IReadOnlyDictionary<string, object> dict)
        {
            if (!dict.IsNullOrEmpty())
            {
                SvrBuildInfoDict = dict;
            }
        }

        public virtual string QueryBuildInfoItem(string key, string defaultValue = "")
        {
            try
            {
                if (!SvrBuildInfoDict.ContainsKey(key))
                {
                    return defaultValue;
                }
                var info = SvrBuildInfoDict.SafeGetValue(key, "").ToString();
                if (info.IsNullOrEmpty(true))
                {
                    return defaultValue;

                }
                return info;
            }
            catch
            {
                return defaultValue;

            }
        }
        public virtual long QueryBuildInfoItem(string key, long defaultValue = 0L)
        {
            try
            {
                if (!SvrBuildInfoDict.ContainsKey(key))
                {
                    return defaultValue;
                }
                var info = SvrBuildInfoDict.SafeGetValue(key, "").ToString();
                if (info.IsNullOrEmpty(true))
                {
                    return defaultValue;

                }
                return info.ToLong(defaultValue);
            }
            catch
            {
                return defaultValue;

            }
        }

        public virtual string QueryBuildType()
        {
            try
            {
                var buildType = SvrBuildInfoDict.SafeGetValue("BuildConfiguration", "").ToString();
                if (buildType.IsNullOrEmpty())
                {
                    buildType = QuerySvrHostInfoImpl?.QueryBuildType() ?? "";
                }

                return buildType;
            }
            catch (Exception ex)
            {
                return "";
            }
        }

        public virtual string QueryBuildMachineName()
        {
            try
            {
                var name = SvrBuildInfoDict.SafeGetValue("MachineName", "").ToString();
                if (name.IsNullOrEmpty())
                {
                    name = QuerySvrHostInfoImpl?.QueryBuildMachineName() ?? "";
                }
                return name;
            }
            catch (Exception ex)
            {
                return "";
            }


        }


    }


}
