using System;
using System.Collections.Generic;
using System.Text;

namespace WindNight.Core
{
    public static class ConstantKeys
    {
        public const string AppIdKey = "AppId";
        public const string AppCodeKey = "AppCode";
        public const string AppNameKey = "AppName";
        public const string EnvNameKey = "EnvName";
        public const string AppSecretKey = "AppSecret";
        public const string TimestampKey = "Timestamp";
        public const string AppTokenKey = "AppToken";

        public const string AuthorizationKey = "Authorization";

        public const string AuthorizationTypeKey = "Bearer";

        public const string UserAgentKey = "User-Agent";

        public const string AppEnvNameKey = "EnvName";

        public const string ReqTraceIdKey = "ReqTraceId";


        public const string HttpPluginKey = "Plugin-HttpHelper";

        public const string SvrCenterDomainConfigKey = "SvrCenter";

        public const string ConfigCenterDomainConfigKey = "ConfigCenter";

        public const string LogCenterDomainConfigKey = "LogCenter";


        public const bool ObsoleteTrue = true;

        public const bool ObsoleteFalse = false;


        public const string ZeroString = "0";

        public const string EmptyString = "";

        public const int ZeroInt = 0;

        public const long ZeroInt64 = 0L;

        public const decimal ZeroDecimal = 0m;

        public static readonly string[] TrueStrings = { "是", "1", bool.TrueString.ToUpper(), "T", "YES", "Y" };

        public static readonly string[] FalseStrings = { "否", "0", bool.FalseString.ToUpper(), "F", "NO", "N" };


    }
}
