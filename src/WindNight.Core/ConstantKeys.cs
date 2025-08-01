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
        public const string AppSecretKey = "AppSecret";
        public const string TimestampKey = "Timestamp";
        public const string AppTokenKey = "AppToken";
        public const string AuthorizationKey = "Authorization";
        public const string AuthorizationTypeKey = "Bearer";
        public const string USER_AGENT_KEY = "User-Agent";

        public const string AppEnvNameKey = "EnvName";

        public const bool ObsoleteTrue = true;
        public const bool ObsoleteFalse = false;

        public static readonly string[] TrueStrings = { "是", "1", bool.TrueString.ToUpper(), "T" };

        public static readonly string[] FalseStrings = { "否", "0", bool.FalseString.ToUpper(), "F" };

        public const string ZeroString = "0";
        public const int ZeroInt = 0;
        public const long ZeroInt64 = 0L;
        public const decimal ZeroDecimal = 0m;


    }
}
