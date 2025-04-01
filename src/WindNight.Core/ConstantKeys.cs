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

        public const bool ObsoleteTrue = true;
        public const bool ObsoleteFalse = false;

        public static readonly string[] TrueStrings = { "1", bool.TrueString.ToUpper(), "T" };

        public static readonly string[] FalseStrings = { "0", bool.FalseString.ToUpper(), "F" };
        public const string ZeroString = "0";
        public const int ZeroInt = 0;
        public const long ZeroInt64 = 0L;
        public const decimal ZeroDecimal = 0m;


    }
}
