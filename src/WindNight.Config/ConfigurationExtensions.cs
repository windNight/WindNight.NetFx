#if !NET45

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using Microsoft.Extensions.Configuration;
using WindNight.ConfigCenter.Extension.Internal;

namespace WindNight.ConfigCenter.Extension
{
    public static class ConfigurationEx
    {
        static readonly string[] TrueStrings = { "1", "true" };
        static readonly string[] FalseStrings = { "0", "false" };

        static string FixAppConfigKey(string keyName) => $"{nameof(ConfigType.AppSettings)}:{keyName}";

        public static bool GetAppConfigValue(this IConfiguration configuration, string keyName,
            bool defaultValue = false, bool isThrow = false)
        {

            var configKey = FixAppConfigKey(keyName);
            try
            {
                var configValue = configuration.GetAppConfigValue(keyName, "false", isThrow).ToLower();

                if (TrueStrings.Contains(configValue))
                {
                    return true;
                }

                if (FalseStrings.Contains(configValue))
                {
                    return false;
                }

                if (isThrow)
                {
                    throw new ArgumentOutOfRangeException($"configKey", $"configKey({configKey}) configValue({configValue}) is not in TrueStrings([{(string.Join(",", TrueStrings))}]) or FalseStrings([{string.Join(",", FalseStrings)}]) ");
                }
                return defaultValue;
            }
            catch (Exception ex)
            {
                if (isThrow) throw;
                return defaultValue;
            }
        }

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
                    LogHelper.Error($"GetAppConfigValue(keyName:{keyName},defaultValue:{defaultValue},isThrow:{isThrow}) -> configValue({configValue})", ex);
                    throw;
                }
                LogHelper.Warn($"GetAppConfigValue(keyName:{keyName},defaultValue:{defaultValue},isThrow:{isThrow}) -> configValue({configValue}) ", ex);
                return defaultValue;
            }
        }


        public static T GetSectionValue<T>(this IConfiguration configuration, string sectionKey = "", T defaultValue = default, bool isThrow = false)
            where T : class, new()
        {
            if (sectionKey.IsNullOrEmpty())
            {
                sectionKey = nameof(T);
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
                    LogHelper.Error($"GetSection({sectionKey}) configValue({configValue}) defaultValue({defaultValue}) isThrow({isThrow}) handler error {ex.Message}", ex);
                    throw;
                }

                LogHelper.Warn($"GetSection({sectionKey})  configValue({configValue}) defaultValue({defaultValue}) isThrow({isThrow})  handler error {ex.Message}", ex);
            }

            return defaultValue;

        }


    }
}

#endif
