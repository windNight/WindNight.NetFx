#if NET45LATER
using Microsoft.Extensions.Configuration;
#endif
using System;
using System.Linq.Expressions;
using Microsoft.Extensions.DependencyInjection.WnExtension;
using WindNight.Core.Abstractions;

namespace WindNight.LogExtension
{
    public static partial class LogHelper
    {
        internal class ConfigItems
        {
#if NET45LATER

            private static IConfiguration configuration => Ioc.GetService<IConfiguration>();
#endif

            static IConfigService configService => Ioc.Instance.CurrentConfigService;
            protected const string TrueString = "1", FalseString = "0", ZeroString = "0";
            protected const int ZeroInt = 0;

            public static string Log4netConfigPath
                => GetAppSetting(ConstKeys.Log4netConfigPathKey, "/Config/log4net.config", false);

            internal static bool Log4netOpen
                => GetAppSetting(ConstKeys.Log4netOpenKey, false, false);

            internal static bool IsAppendLogMessage
                => GetAppSetting(ConstKeys.AppendLogMessageKey, false, false);

            internal static bool LogOnConsole
                => GetAppSetting(ConstKeys.LogOnConsoleKey, false, false);


            /// <summary> 服务编号： </summary>
            internal static int SystemAppId
                => GetAppSetting(ConstKeys.AppIdKey, ZeroInt, false);

            /// <summary> 服务代号： </summary>
            internal static string SystemAppCode
                => GetAppSetting(ConstKeys.AppCodeKey, "", false);

            /// <summary> 服务名称： </summary>
            internal static string SystemAppName
                => GetAppSetting(ConstKeys.AppNameKey, "", false);

            static int GetAppSetting(string configKey, int defaultValue = 0, bool isThrow = true)
            {
                var configValue = GetAppSetting(configKey, "", isThrow: isThrow);
                if (configValue.IsNullOrEmpty())
                {
                    return defaultValue;
                }

                return configValue.ToInt(0);
            }


            static bool GetAppSetting(string configKey, bool? defaultValue = null, bool isThrow = true)
            {
                var configValue = GetAppSetting(configKey, "", isThrow: isThrow);
                if (configValue.IsNullOrEmpty())
                {
                    return defaultValue ?? false;
                }

                return configValue.ToInt() != ZeroInt;
            }

            static string GetAppSetting(string configKey, string defaultValue = "", bool isThrow = true)
            {
#if NET45LATER
                if (configuration != null)
                {
                    var configValue = configuration.GetSection($"AppSettings:{configKey}").Value;
                    if (!configValue.IsNullOrEmpty())
                    {
                        return configValue;
                    }
                }
#endif
                if (configService != null)
                {
                    return configService.GetAppSetting(configKey, defaultValue, isThrow);
                }

                return defaultValue;
            }


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