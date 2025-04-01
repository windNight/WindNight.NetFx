#if NET45LATER
using Microsoft.Extensions.Configuration;
#endif
using System;
using System.Linq.Expressions;
using Microsoft.Extensions.DependencyInjection.WnExtension;
using WindNight.Core.Abstractions;
using WindNight.Core.ConfigCenter.Extensions;

namespace WindNight.LogExtension
{
    public static partial class LogHelper
    {
        internal class ConfigItems : DefaultConfigItemBase
        {

            public static string Log4netConfigPath
                => GetAppSettingValue(ConstKeys.Log4netConfigPathKey, "Config/log4net.config", false);


            internal static bool IsAppendLogMessage
                => GetAppSettingValue(ConstKeys.AppendLogMessageKey, false, false);





            internal static class ConstKeys
            {
                public const string Log4netConfigPathKey = "log4netConfigPath";
                public const string AppendLogMessageKey = "AppendLogMessage";
                public const string LogOnConsoleKey = "LogOnConsole";
                public const string Log4netOpenKey = "Log4netOpen";

                internal const string AppIdKey = "AppId";
                internal const string AppCodeKey = "AppCode";
                internal const string AppNameKey = "AppName";
            }
        }
    }
}
