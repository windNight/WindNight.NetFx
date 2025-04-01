using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;
using WindNight.Core.ExceptionExt;
using WindNight.Core.SysLogCenter.Extensions;

namespace WindNight.Core.ConfigCenter.Extensions
{
    public static partial class ConfigurationEx
    {
        private const string AppIdKey = ConstantKeys.AppIdKey;
        private const string AppCodeKey = ConstantKeys.AppCodeKey;
        private const string AppNameKey = ConstantKeys.AppNameKey;
        private const string AppSecretKey = ConstantKeys.AppSecretKey;


        private static readonly string[] TrueStrings = ConstantKeys.TrueStrings;
        private static readonly string[] FalseStrings = ConstantKeys.FalseStrings;


        public static int GetAppId(this IConfiguration configuration, int defaultValue = 0)
        {
            return configuration.GetAppSettingValue(AppIdKey, defaultValue);
        }

        public static string GetAppCode(this IConfiguration configuration, string defaultValue = "")
        {
            return configuration.GetAppSettingValue(AppCodeKey, defaultValue);
        }

        public static string GetAppName(this IConfiguration configuration, string defaultValue = "")
        {
            return configuration.GetAppSettingValue(AppNameKey, defaultValue);
        }

        public static string GetAppSecret(this IConfiguration configuration, string defaultValue = "")
        {
            return configuration.GetAppSettingValue(AppSecretKey, defaultValue);
        }
    }


    public static partial class ConfigurationEx
    {
        public static IEnumerable<string> GetAppSettingList(this IConfiguration configuration, string configKey,
            IEnumerable<string> defaultValue = null,
            bool isThrow = false, bool needDistinct = true)
        {
            if (defaultValue == null)
            {
                defaultValue = Enumerable.Empty<string>();
            }

            return configuration.GetAppSettingList(configKey, m => m, defaultValue, isThrow, needDistinct);
        }

        public static IEnumerable<int> GetAppSettingList(this IConfiguration configuration, string configKey,
            IEnumerable<int> defaultValue = null,
            bool isThrow = false, bool needDistinct = true)
        {
            if (defaultValue == null)
            {
                defaultValue = Enumerable.Empty<int>();
            }

            return configuration.GetAppSettingList(configKey, m => m.ToInt(), defaultValue, isThrow, needDistinct);
        }

        public static IEnumerable<T> GetAppSettingList<T>(this IConfiguration configuration, string configKey,
            Func<string, T> convertFunc,
            IEnumerable<T> defaultValue = null, bool isThrow = false, bool needDistinct = true)
        {
            if (defaultValue == null)
            {
                defaultValue = Enumerable.Empty<T>();
            }

            try
            {
                var configValue = configuration.GetAppSettingValue(configKey, "", isThrow);
                if (configValue.IsNullOrEmpty())
                {
                    return defaultValue;
                }

                var value = configValue.Split(",").Select(convertFunc.Invoke);
                if (needDistinct)
                {
                    value = value.Distinct();
                }

                return value;
            }
            catch (Exception ex)
            {
                if (isThrow)
                {
                    throw;
                }

                return defaultValue;
            }
        }
    }

    public static partial class ConfigurationEx
    {
        private static string FixAppConfigKey(string keyName) => $"AppSettings:{keyName}";

        public static bool GetAppSettingValue(this IConfiguration configuration, string keyName,
            bool defaultValue = false, bool isThrow = false)
        {
            var configKey = FixAppConfigKey(keyName);
            try
            {
                if (configuration == null)
                {
                    if (isThrow)
                    {
                        throw new ArgumentOutOfRangeException("configuration", "configuration  Null ");
                    }

                    return defaultValue;
                }

                var configValue = configuration.GetAppSettingValue(keyName, defaultValue.ToString(), isThrow).ToLower();

                if (TrueStrings.Contains(configValue, StringComparer.OrdinalIgnoreCase))
                {
                    return true;
                }

                if (FalseStrings.Contains(configValue, StringComparer.OrdinalIgnoreCase))
                {
                    return false;
                }

                if (isThrow)
                {
                    throw new ArgumentOutOfRangeException("configKey",
                        $"configKey({configKey}) is not in TrueStrings({TrueStrings.Join()}) or FalseStrings({FalseStrings.Join()}) both IgnoreCase ");
                }

                return defaultValue;
            }
            catch (Exception ex)
            {
                if (isThrow)
                {
                    throw;
                }

                return defaultValue;
            }
        }

        public static string GetAppSettingValue(this IConfiguration configuration, string keyName,
            string defaultValue = "", bool isThrow = false)
            => GetAppSettingValueInternal(keyName, defaultValue, isThrow,
                configKey => configuration.GetValue(configKey, defaultValue));


        public static decimal GetAppSettingValue(this IConfiguration configuration, string keyName,
            decimal defaultValue = 0m, bool isThrow = false)
            => GetAppSettingValueInternal(keyName, defaultValue, isThrow,
                configKey => configuration.GetValue(configKey, defaultValue));

        public static int GetAppSettingValue(this IConfiguration configuration, string keyName,
            int defaultValue = 0, bool isThrow = false)
            => GetAppSettingValueInternal(keyName, defaultValue, isThrow,
                configKey => configuration.GetValue(configKey, defaultValue));

        public static long GetAppSettingValue(this IConfiguration configuration, string keyName,
            long defaultValue = 0, bool isThrow = false)
            => GetAppSettingValueInternal(keyName, defaultValue, isThrow,
                configKey => configuration.GetValue(configKey, defaultValue));



    }

    public static partial class ConfigurationEx
    {
        public static string GetConnString(this IConfiguration configuration, string keyName,
            string defaultValue = "", bool isThrow = false)
        {
            return GetConfigValueInternal(keyName, defaultValue, isThrow,
                configuration.GetConnectionString);
        }
    }

    public static partial class ConfigurationEx
    {
        /// <summary>
        ///     指定某个根节点
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sectionKey"></param>
        /// <param name="defaultValue"></param>
        /// <param name="isThrow"></param>
        /// <returns></returns>
        public static T GetSectionValue<T>(this IConfiguration configuration, string sectionKey = "",
            T defaultValue = default, bool isThrow = false)
            where T : class, new()
        {
            if (sectionKey.IsNullOrEmpty())
            {
                sectionKey = typeof(T).Name;
            }

            return configuration.GetSectionConfigValue(sectionKey, defaultValue, isThrow);
        }

        public static T GetSectionConfigValue<T>(this IConfiguration configuration, string sectionKey,
                T defaultValue = default, bool isThrow = false)
        //   where T : class, new()
        {
            if (defaultValue == null) defaultValue = default;
            var configValue = defaultValue;
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
                try
                {
                    if (isThrow)
                    {
                        DefaultLogHelperBase.Error(
                            $"GetSection({sectionKey}) configValue({configValue}) defaultValue({defaultValue}) isThrow({isThrow}) handler error {ex.Message}",
                            ex);
                        throw;
                    }

                    DefaultLogHelperBase.Warn(
                        $"GetSection({sectionKey})  configValue({configValue}) defaultValue({defaultValue}) isThrow({isThrow})  handler error {ex.Message}",
                        ex);
                }
                catch
                {
                }
            }

            return defaultValue;
        }

        /// <summary>
        ///     指定任意节点 可获取单个配置
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="keyName"></param>
        /// <param name="defaultValue"></param>
        /// <param name="isThrow"></param>
        /// <returns></returns>
        public static T GetConfigValue<T>(this IConfiguration configuration, string keyName,
            T defaultValue = default, bool isThrow = false)
        {
            return GetConfigValueInternal(keyName, defaultValue, isThrow,
                configKey => configuration.GetValue(configKey, defaultValue));
        }
    }

    public static partial class ConfigurationEx
    {
        private static T GetConfigValueInternal<T>(string keyName, T defaultValue, bool isThrow, Func<string, T> func)
        {
            var configKey = keyName; // FixAppConfigKey(keyName);
            var configValue = defaultValue;
            try
            {
                configValue = func.Invoke(configKey);
                return configValue;
            }
            catch (Exception ex)
            {
                if (isThrow)
                {
                    throw;
                }

                return defaultValue;
            }
        }

        private static T GetAppSettingValueInternal<T>(string keyName, T defaultValue, bool isThrow, Func<string, T> func)
        {
            var configKey = FixAppConfigKey(keyName);

            return GetConfigValueInternal<T>(configKey, defaultValue, isThrow, func);

            //var configValue = defaultValue;
            //try
            //{
            //    configValue = func.Invoke(configKey);
            //    return configValue;
            //}
            //catch (Exception ex)
            //{
            //    try
            //    {
            //        if (isThrow)
            //        {
            //            DefaultLogHelperBase.Error(
            //                $"GetAppSettingValue(keyName:{keyName},defaultValue:{defaultValue},isThrow:{isThrow}) -> configValue({configValue})",
            //                ex);
            //            throw;
            //        }

            //        DefaultLogHelperBase.Warn(
            //            $"GetAppSettingValue(keyName:{keyName},defaultValue:{defaultValue},isThrow:{isThrow}) -> configValue({configValue}) {ex.GetMessage()} ",
            //            ex);

            //    }
            //    catch
            //    {
            //    }
            //    return defaultValue;

            //}


        }
    }
}
