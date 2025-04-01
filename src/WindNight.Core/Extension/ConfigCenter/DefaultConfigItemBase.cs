using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.WnExtension;
using WindNight.Core.Abstractions;

namespace WindNight.Core.ConfigCenter.Extensions
{
    public partial class DefaultConfigItemBase
    {
        protected static IConfiguration Configuration => Ioc.GetService<IConfiguration>();
        protected static IConfigService ConfigService => Ioc.GetService<IConfigService>();

        protected static void CheckDIImpl(bool isThrow = false)
        {
            if (isThrow && (ConfigService == null || Configuration == null))
            {
                throw new NotImplementedException("Can't Get IConfigService Or Configuration  From DI Container.  ");
            }
        }

    }

    public partial class DefaultConfigItemBase
    {
        protected const string ZeroString = ConstantKeys.ZeroString;
        protected const int ZeroInt = ConstantKeys.ZeroInt;
        protected const long ZeroInt64 = ConstantKeys.ZeroInt64;
        protected const decimal ZeroDecimal = ConstantKeys.ZeroDecimal;

        protected static readonly string[] TrueStrings = ConstantKeys.TrueStrings,
            FalseStrings = ConstantKeys.FalseStrings;
    }

    public partial class DefaultConfigItemBase
    {
        protected static IEnumerable<string> GetAppSettingList(string configKey, IEnumerable<string> defaultValue = null,
            bool isThrow = false, bool needDistinct = true)
        {
            if (defaultValue == null)
            {
                defaultValue = Enumerable.Empty<string>();
            }

            return GetAppSettingList(configKey, m => m, defaultValue, isThrow, needDistinct);
        }

        protected static IEnumerable<int> GetAppSettingList(string configKey, IEnumerable<int> defaultValue = null,
            bool isThrow = false, bool needDistinct = true)
        {
            if (defaultValue == null)
            {
                defaultValue = Enumerable.Empty<int>();
            }

            return GetAppSettingList(configKey, m => m.ToInt(), defaultValue, isThrow, needDistinct);
        }

        protected static IEnumerable<T> GetAppSettingList<T>(string configKey, Func<string, T> convertFunc,
            IEnumerable<T> defaultValue = null, bool isThrow = false, bool needDistinct = true)
        {

            if (defaultValue == null)
            {
                defaultValue = Enumerable.Empty<T>();
            }
            if (ConfigService == null && Configuration == null)
            {
                return defaultValue;
            }
            CheckDIImpl(isThrow);
            if (ConfigService != null)
            {
                return ConfigService.GetAppSettingList(configKey, convertFunc, defaultValue, isThrow, needDistinct);
            }

            return Configuration.GetAppSettingList(configKey, convertFunc, defaultValue, isThrow, needDistinct);
        }
    }

    public partial class DefaultConfigItemBase
    {

        /// <summary>
        ///     AppSettings:xxx
        /// </summary>
        /// <param name="keyName"></param>
        /// <param name="defaultValue"></param>
        /// <param name="isThrow"></param>
        /// <returns></returns>
        protected static string GetAppSettingValue(string keyName, string defaultValue = "", bool isThrow = false)
        {
            CheckDIImpl(isThrow);
            if (ConfigService == null && Configuration == null)
            {
                return defaultValue;
            }
            if (ConfigService != null)
            {
                return ConfigService.GetAppSettingValue(keyName, defaultValue, isThrow);
            }
            return Configuration.GetAppSettingValue(keyName, defaultValue, isThrow);

        }

        protected static bool GetAppSettingValue(string keyName, bool defaultValue = false, bool isThrow = false)
        {
            CheckDIImpl(isThrow);
            if (ConfigService == null && Configuration == null)
            {
                return defaultValue;
            }
            if (ConfigService != null)
            {
                return ConfigService.GetAppSettingValue(keyName, defaultValue, isThrow);
            }
            return Configuration.GetAppSettingValue(keyName, defaultValue, isThrow);

        }


        protected static decimal GetAppSettingValue(string keyName, decimal defaultValue = 0m, bool isThrow = false)
        {
            CheckDIImpl(isThrow);
            if (ConfigService == null && Configuration == null)
            {
                return defaultValue;
            }
            if (ConfigService != null)
            {
                return ConfigService.GetAppSettingValue(keyName, defaultValue, isThrow);
            }
            return Configuration.GetAppSettingValue(keyName, defaultValue, isThrow);

        }

        protected static int GetAppSettingValue(string keyName, int defaultValue = 0, bool isThrow = false)
        {
            CheckDIImpl(isThrow);
            if (ConfigService == null && Configuration == null)
            {
                return defaultValue;
            }
            if (ConfigService != null)
            {
                return ConfigService.GetAppSettingValue(keyName, defaultValue, isThrow);
            }
            return Configuration.GetAppSettingValue(keyName, defaultValue, isThrow);

        }


        protected static long GetAppSettingValue(string keyName, long defaultValue = 0, bool isThrow = false)
        {
            CheckDIImpl(isThrow);
            if (ConfigService == null && Configuration == null)
            {
                return defaultValue;
            }
            if (ConfigService != null)
            {
                return ConfigService.GetAppSettingValue(keyName, defaultValue, isThrow);
            }
            return Configuration.GetAppSettingValue(keyName, defaultValue, isThrow);

        }


    }

    public partial class DefaultConfigItemBase
    {

        protected const int DEFAULT_API_WARNING_MIS = 200;

        public static int SystemAppId => ConfigService?.SystemAppId ?? Configuration?.GetAppId() ?? 0;
        public static string SystemAppCode => ConfigService?.SystemAppCode ?? Configuration?.GetAppCode() ?? "";
        public static string SystemAppName => ConfigService?.SystemAppCode ?? Configuration?.GetAppName() ?? "";

        public static bool OpenDebug =>
            GetAppSettingValue(nameof(OpenDebug), false, false);

        public static bool LogOnConsole
            => GetAppSettingValue(nameof(LogOnConsole), false, false);
        public static bool Log4netOpen
            => GetAppSettingValue(nameof(Log4netOpen), false, false);
        public static bool IsValidateInput
                => GetAppSettingValue(nameof(IsValidateInput), false, false);
        public static bool LogProcessOpened
            => GetAppSettingValue(nameof(LogProcessOpened), false, false);
        public static bool ApiUrlOpened
               => GetAppSettingValue(nameof(ApiUrlOpened), false, false);

        public static bool DefaultStaticFileEnable
               => GetAppSettingValue(nameof(DefaultStaticFileEnable), true, false);

        public static int ApiWarningMis
                => GetAppSettingValue(nameof(ApiWarningMis), 300, false);

        public static string GlobalMiniLogLevelStr => GetAppSettingValue("GlobalMiniLogLevel", "Info", false);


        protected static LogLevels Convert2LogLevel(string level)
        {
            try
            {
                if (level.IsNullOrEmpty())
                {
                    return LogLevels.Information;
                }

                if (level.StartsWith("debug", StringComparison.OrdinalIgnoreCase))
                {
                    return LogLevels.Debug;
                }

                if (level.StartsWith("info", StringComparison.OrdinalIgnoreCase))
                {
                    return LogLevels.Information;
                }

                if (level.StartsWith("warn", StringComparison.OrdinalIgnoreCase))
                {
                    return LogLevels.Warning;
                }

                var flag = Enum.TryParse<LogLevels>(level, true, out var logLevel);

                if (flag)
                {
                    return logLevel;
                }

                return LogLevels.Information;

            }
            catch (Exception ex)
            {
                return LogLevels.Information;
            }

        }

        public static LogLevels GlobalMiniLogLevel
        {
            get
            {
                try
                {
                    var configValue = GlobalMiniLogLevelStr;
                    if (configValue.StartsWith("debug", StringComparison.OrdinalIgnoreCase))
                    {
                        return LogLevels.Debug;
                    }
                    if (configValue.StartsWith("info", StringComparison.OrdinalIgnoreCase))
                    {
                        return LogLevels.Information;
                    }

                    if (configValue.StartsWith("warn", StringComparison.OrdinalIgnoreCase))
                    {
                        return LogLevels.Warning;
                    }

                    var flag = Enum.TryParse<LogLevels>(configValue, true, out var logLevel);

                    if (flag)
                    {
                        return logLevel;
                    }

                    return LogLevels.Information;

                }
                catch (Exception ex)
                {
                    return LogLevels.Information;
                }

            }
        }


    }

    public partial class DefaultConfigItemBase
    {
        /// <summary>
        ///  指定任意节点 可获取单个配置
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="keyName"></param>
        /// <param name="defaultValue"></param>
        /// <param name="isThrow"></param>
        /// <returns></returns>
        protected static T GetConfigValue<T>(string keyName, T defaultValue = default, bool isThrow = false)
        {
            CheckDIImpl(isThrow);
            if (ConfigService == null && Configuration == null)
            {
                return defaultValue;
            }
            if (ConfigService != null)
            {
                return ConfigService.GetConfigValue(keyName, defaultValue, isThrow);
            }
            return Configuration.GetConfigValue(keyName, defaultValue, isThrow);
        }

        /// <summary>
        ///  指定某个根节点
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sectionKey"></param>
        /// <param name="defaultValue"></param>
        /// <param name="isThrow"></param>
        /// <returns></returns>
        protected static T GetSectionValue<T>(string sectionKey = "", T defaultValue = default, bool isThrow = false)
            where T : class, new()
        {
            CheckDIImpl(isThrow);
            if (ConfigService == null && Configuration == null)
            {
                return defaultValue;
            }
            if (ConfigService != null)
            {
                return ConfigService.GetSectionValue(sectionKey, defaultValue, isThrow);
            }
            return Configuration.GetSectionValue(sectionKey, defaultValue, isThrow);
        }




    }

    public partial class DefaultConfigItemBase
    {
        protected static string GetConnectionString(string configKey, string defaultValue = "", bool isThrow = true)
        {
            CheckDIImpl(isThrow);
            if (ConfigService != null)
            {
                return ConfigService.GetConnString(configKey, defaultValue, isThrow);
            }
            return Configuration.GetConnString(configKey, defaultValue, isThrow);
        }

    }


}
