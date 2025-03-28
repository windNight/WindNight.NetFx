﻿
#if !NET45
using System.Reflection.Emit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.WnExtension;
using WindNight.Core.Abstractions;
using WindNight.Extension.Logger.DcLog.Abstractions;

namespace WindNight.Extension.Logger.DcLog.Internal
{
    internal static class ConfigItems
    {

        private static IConfiguration configuration => Ioc.GetService<IConfiguration>();

        internal static int SystemAppId => configuration.GetAppConfigValue("AppId", 0, false);
        internal static string SystemAppCode => configuration.GetAppConfigValue("AppCode", "", false);
        internal static string SystemAppName => configuration.GetAppConfigValue("AppName", "", false);



        public static LogLevels GlobalMinLogLevel
        {
            get
            {
                try
                {

                    var configValue = GlobalMinLogLevelStr;

                    if (configValue.ToLower().StartsWith("info"))
                    {
                        return LogLevels.Information;
                    }

                    if (configValue.ToLower().StartsWith("warn"))
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

        public static string GlobalMinLogLevelStr => configuration.GetAppConfigValue("GlobalMinLogLevel", "Info", false);


        public static DcLogOptions DcLogOptions => configuration.GetSectionValue<DcLogOptions>();

        /// <summary> 是否输出日志 </summary>
        public static bool IsConsoleLog => DcLogOptions?.IsConsoleLog ?? false;
        public static bool IsOpenDebug => DcLogOptions?.IsOpenDebug ?? false;
        //public static string DbConnectString => DcLogOptions?.DbConnectString ?? string.Empty;


        static readonly string[] TrueStrings = { "1", "true" };
        static readonly string[] FalseStrings = { "0", "false" };

        public static string GetAppConfigValue(this IConfiguration configuration, string keyName,
            string defaultValue = "", bool isThrow = false)
            => GetAppConfigValue(keyName, defaultValue, isThrow, (configKey) => configuration.GetValue(configKey, defaultValue));


        public static decimal GetAppConfigValue(this IConfiguration configuration, string keyName,
            decimal defaultValue = 0m, bool isThrow = false)
            => GetAppConfigValue(keyName, defaultValue, isThrow, (configKey) => configuration.GetValue(configKey, defaultValue));

        public static int GetAppConfigValue(this IConfiguration configuration, string keyName,
            int defaultValue = 0, bool isThrow = false)
            => GetAppConfigValue(keyName, defaultValue, isThrow, (configKey) => configuration.GetValue(configKey, defaultValue));

        public static long GetAppConfigValue(this IConfiguration configuration, string keyName,
            long defaultValue = 0, bool isThrow = false)
            => GetAppConfigValue(keyName, defaultValue, isThrow, (configKey) => configuration.GetValue(configKey, defaultValue));


        static string FixAppConfigKey(string keyName) => $"AppSettings:{keyName}";

        static T GetAppConfigValue<T>(string keyName, T defaultValue, bool isThrow, Func<string, T> func)
        {
            var configKey = FixAppConfigKey(keyName);
            T configValue = defaultValue;
            try
            {
                configValue = func.Invoke(configKey);
                return configValue;
            }
            catch (Exception ex)
            {
                if (isThrow)
                {
                    // LogHelper.Error($"GetAppConfigValue(keyName:{keyName},defaultValue:{defaultValue},isThrow:{isThrow}) -> configValue({configValue})", ex);
                    throw;
                }
                // LogHelper.Warn($"GetAppConfigValue(keyName:{keyName},defaultValue:{defaultValue},isThrow:{isThrow}) -> configValue({configValue}) ", ex);
                return defaultValue;
            }
        }

        static T GetSectionValue<T>(this IConfiguration configuration, string sectionKey = "", T defaultValue = default, bool isThrow = false)
            where T : class, new()
        {
            if (sectionKey.IsNullOrEmpty())
            {
                sectionKey = typeof(T).Name;
            }

            return configuration.GetSectionConfigValue<T>(sectionKey, defaultValue, isThrow);
        }

        public static T GetSectionConfigValue<T>(this IConfiguration configuration, string sectionKey, T defaultValue = default, bool isThrow = false)
        {
            if (defaultValue == null) defaultValue = default;
            T configValue = defaultValue;
            try
            {
                if (configuration == null)
                {
                    return defaultValue;
                }
                configValue = configuration.GetSection(sectionKey).Get<T>();
                if (configValue == null)
                {
                    return defaultValue;
                }

                return configValue;

            }
            catch (Exception ex)
            {
                if (isThrow)
                {
                    //  LogHelper.Error($"GetSection({sectionKey}) configValue({configValue}) defaultValue({defaultValue}) isThrow({isThrow}) handler error {ex.Message}", ex);
                    throw;
                }

                //LogHelper.Warn($"GetSection({sectionKey})  configValue({configValue}) defaultValue({defaultValue}) isThrow({isThrow})  handler error {ex.Message}", ex);
            }

            return defaultValue;

        }


    }
}
#endif
