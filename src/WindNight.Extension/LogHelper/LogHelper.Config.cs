using Microsoft.Extensions.DependencyInjection.WnExtension;
using WindNight.Core.Abstractions;

namespace WindNight.LogExtension
{
    public static partial class LogHelper
    {
        internal class ConfigItems
        {
            static IConfigService configService => Ioc.GetService<IConfigService>();
            protected const string TrueString = "1", FalseString = "0", ZeroString = "0";
            protected const int ZeroInt = 0;

            public static string Log4netConfigPath
                => configService?.GetAppSetting(ConstKeys.Log4netConfigPathKey, "/Config/log4net.config")
                   ?? "/Config/log4net.config";

            internal static bool Log4netOpen
                => configService?.GetAppSetting(ConstKeys.Log4netOpenKey, false, false)
                   ?? false;

            internal static bool IsAppendLogMessage
                => configService?.GetAppSetting(ConstKeys.AppendLogMessageKey, false, false)
                   ?? false;

            internal static bool LogOnConsole
                => configService?.GetAppSetting(ConstKeys.LogOnConsoleKey, false, false)
                   ?? false;


            /// <summary> 服务编号： </summary>
            internal static int SystemAppId
                => configService?.GetAppSetting(ConstKeys.AppIdKey, ZeroInt)
                   ?? ZeroInt;

            /// <summary> 服务代号： </summary>
            internal static string SystemAppCode
                => configService?.GetAppSetting(ConstKeys.AppCodeKey, "") ?? "";

            /// <summary> 服务名称： </summary>
            internal static string SystemAppName
                => configService?.GetAppSetting(ConstKeys.AppNameKey, "") ?? "";

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