using System;
using System.Globalization;
using Microsoft.Extensions.DependencyInjection.WnExtension;
using Newtonsoft.Json.Extension;
using WindNight.Core.Abstractions;
using WindNight.ConfigCenter.Extension.Internal;
#if !NET45
using Microsoft.Extensions.Configuration;
#endif

namespace WindNight.ConfigCenter.Extension
{

    public class ConfigItemsBase
    {
         
        /// <summary>
        /// </summary>
        /// <param name="sleepTimeInMs"> default is 5000 ms </param>
        /// <param name="logService">   </param>
        /// <param name="configuration"></param>
        protected static void StartConfigCenter(int sleepTimeInMs = 5 * 1000, ILogService? logService = null
#if !NET45
            , IConfiguration? configuration = null
#endif
        )
        {
            if (logService != null) ConfigCenterLogExtension.InitLogProvider(logService);
            else
            {
                ConfigCenterLogExtension.InitLogProvider(Ioc.Instance.CurrentLogService);
            }
#if !NET45
            if (configuration != null)
            {
                ConfigProvider.Instance.SetConfiguration(configuration);
            }
            else
            {
                ConfigProvider.Instance.SetConfiguration();
            }
#endif

            ConfigProvider.Instance.Start(sleepTimeInMs);
         
        }

        public static void StopConfigCenter()
        {
            ConfigProvider.Instance.Stop();
        }

        protected static void ReInitConfigCenter()
        {
            ConfigProvider.Instance.ReInit();
        }

        protected const string TrueString = "1", FalseString = "0", ZeroString = "0";
        protected const int ZeroInt = 0;

        public static object AllConfigs => ConfigCenterContext.GetAllConfig();


        /// <summary> 服务编号： </summary>
        public static int SystemAppId =>
            GetAppSetting(ConstKeys.AppIdKey, ZeroInt);

        /// <summary> 服务代号： </summary>
        public static string SystemAppCode =>
            GetAppSetting(ConstKeys.AppCodeKey, "");

        /// <summary> 服务名称： </summary>
        public static string SystemAppName =>
            GetAppSetting(ConstKeys.AppNameKey, "");

        /// <summary> 系统平台编号： </summary>
        public static int SystemPlatformId =>
            GetAppSetting(ConstKeys.PlatformIdKey, ZeroInt);

        /// <summary> 系统项目编号: </summary>
        public static int SystemProjectId =>
            GetAppSetting(ConstKeys.ProjectIdKey, ZeroInt);


        #region ConnectionStrings

        protected static string GetConnectionString(string connKey, string defaultValue = "", bool isThrow = false)
        {
            return ReadFromConfig(ConfigCenterContext.GetConnectionString, connKey, defaultValue, isThrow);
        }

        #endregion


        private static string ReadFromConfig(Func<string, string, string> func, string configKey,
            string defaultValue = "", bool isThrow = false)
        {
            if (configKey.IsNullOrEmpty()) return defaultValue;
            var configValue = string.Empty;
            try
            {
                configValue = func.Invoke(configKey, defaultValue);
            }
            catch (Exception ex)
            {
                LogHelper.Warn($"Read config({configKey}) handler error {ex.Message}", ex);
                if (isThrow)
                    throw;
            }

            if (configValue.IsNullOrEmpty() && !defaultValue.IsNullOrEmpty()) configValue = defaultValue;

            return configValue;
        }

        private static string ReadFromConfig(Func<string, DomainSwitchNodeType, string, string> func, string nodeName,
            DomainSwitchNodeType nodeType = DomainSwitchNodeType.Unknown, string defaultValue = "",
            bool isThrow = false)
        {
            if (nodeName.IsNullOrEmpty() || nodeType == DomainSwitchNodeType.Unknown) return defaultValue;
            var configValue = string.Empty;
            try
            {
                configValue = func.Invoke(nodeName, nodeType, defaultValue);
            }
            catch (Exception ex)
            {
                LogHelper.Warn($"Read DomainConfig({nodeName}) handler error {ex.Message}", ex);
                if (isThrow)
                    throw;
            }

            if (configValue.IsNullOrEmpty() && !defaultValue.IsNullOrEmpty()) configValue = defaultValue;

            return configValue;
        }


        internal static class ConstKeys
        {
            internal const string AppIdKey = "AppId";
            internal const string AppCodeKey = "AppCode";
            internal const string AppNameKey = "AppName";
            internal const string PlatformIdKey = "PlatformId";
            internal const string ProjectIdKey = "ProjectId";
        }

        #region AppSetting

        protected static decimal GetAppSetting(string configKey, decimal defaultValue = 0M, bool isThrow = false)
        {
            var configValue = GetAppSetting(configKey, defaultValue.ToString(CultureInfo.InvariantCulture), isThrow);
            if (decimal.TryParse(configValue, out var value)) return value;

            return defaultValue;
        }

        protected static int GetAppSetting(string configKey, int defaultValue = 0, bool isThrow = false)
        {
            var configValue = GetAppSetting(configKey, defaultValue.ToString(), isThrow);
            if (int.TryParse(configValue, out var value)) return value;

            return defaultValue;
        }

        protected static long GetAppSetting(string configKey, long defaultValue = 0, bool isThrow = false)
        {
            var configValue = GetAppSetting(configKey, defaultValue.ToString(), isThrow);
            if (long.TryParse(configValue, out var value)) return value;

            return defaultValue;
        }

        protected static bool GetAppSetting(string configKey, bool defaultValue = false, bool isThrow = false)
        {
            var configValue = GetAppSetting(configKey, defaultValue ? TrueString : FalseString, isThrow);

            return string.Equals(configValue, TrueString, StringComparison.OrdinalIgnoreCase);
        }

        protected static string GetAppSetting(string configKey, string defaultValue = "", bool isThrow = false)
        {
            return ReadFromConfig(ConfigCenterContext.GetAppSetting, configKey, defaultValue, isThrow);
        }

        /// <summary>
        /// </summary>
        /// <param name="configKey"></param>
        /// <param name="isThrow"></param>
        /// <returns></returns>
        protected static string GetConfigValue(string configKey, bool isThrow = true)
            => GetAppSetting(configKey, "", isThrow);

        /// <summary>
        /// </summary>
        /// <param name="configKey"></param>
        /// <param name="defaultValue"></param>
        /// <param name="isThrow"></param>
        /// <returns></returns>
        protected static string GetConfigValue(string configKey, string defaultValue = "", bool isThrow = true)
            => GetAppSetting(configKey, defaultValue, isThrow);

        /// <summary>
        /// </summary>
        /// <param name="configKey"></param>
        /// <param name="defaultValue"></param>
        /// <param name="isThrow"></param>
        /// <returns></returns>
        protected static int GetConfigValue(string configKey, int defaultValue = 0, bool isThrow = true)
            => GetAppSetting(configKey, defaultValue.ToString(), isThrow).ToInt(defaultValue);

        /// <summary>
        /// </summary>
        /// <param name="configKey"></param>
        /// <param name="defaultValue"></param>
        /// <param name="isThrow"></param>
        /// <returns></returns>
        protected static long GetConfigValue(string configKey, long defaultValue = 0L, bool isThrow = true)
            => GetAppSetting(configKey, defaultValue.ToString(), isThrow).ToLong(defaultValue);

        /// <summary>
        /// </summary>
        /// <param name="configKey"></param>
        /// <param name="defaultValue"></param>
        /// <param name="isThrow"></param>
        /// <returns></returns>
        protected static decimal GetConfigValue(string configKey, decimal defaultValue = 0M, bool isThrow = true) =>
            GetAppSetting(configKey, defaultValue.ToString(CultureInfo.InvariantCulture), isThrow)
                .ToDecimal(defaultValue);

        /// <summary>
        /// </summary>
        /// <param name="configKey"></param>
        /// <param name="defaultValue"></param>
        /// <param name="isThrow"></param>
        /// <returns></returns>
        protected static bool GetConfigValue(string configKey, bool defaultValue, bool isThrow)
            => GetAppSetting(configKey, defaultValue ? TrueString : FalseString, isThrow) == TrueString;


        #endregion //end AppSetting

        #region Json Config 

        protected static string GetJsonConfig(string fileKey, string defaultValue = "", bool isThrow = false)
        {
            return ReadFromConfig(ConfigCenterContext.GetJsonConfig, fileKey, defaultValue, isThrow);
        }

        protected static T GetJsonConfig<T>(string fileKey, string defaultValue = "", bool isThrow = false)
            where T : new()
        {
            var configValue = GetJsonConfig(fileKey, defaultValue, isThrow);

            // 可能的 null 引用返回。
            if (configValue.IsNullOrEmpty()) return default;
            return configValue.To<T>();
            // 可能的 null 引用返回。
        }

        #endregion //end Json Config 

        #region DomainSwitch

        protected static string GetDomainSiteHost(string nodeName, string defaultValue = "", bool isThrow = false)
        {
            return GetDomainSwitchConfig(nodeName, DomainSwitchNodeType.SiteHost, defaultValue, isThrow);
        }

        protected static string GetDomainServiceUrl(string nodeName, string defaultValue = "", bool isThrow = false)
        {
            return GetDomainSwitchConfig(nodeName, DomainSwitchNodeType.ServiceUrl, defaultValue, isThrow);
        }


        private static string GetDomainSwitchConfig(string nodeName, DomainSwitchNodeType nodeType,
            string defaultValue = "", bool isThrow = false)
        {
            return ReadFromConfig(ConfigCenterContext.GetDomainSwitchConfig, nodeName, nodeType, defaultValue, isThrow);
        }

        #endregion //end DomainSwitch

#if NET45LATER

        public static T GetSectionValue<T>(string sectionKey, T defaultValue = default, bool isThrow = false) where T : class, new()
        {
            if (defaultValue == null) defaultValue = new T();
            try
            {
                var config = Ioc.GetService<IConfiguration>();
                if (config == null)
                {
                    return defaultValue;
                }
                var configValue = config.GetSection(sectionKey).Get<T>();
                if (configValue == null)
                {
                    return defaultValue;
                }

                return configValue;

            }
            catch (Exception ex)
            {
                LogHelper.Warn($"GetSection({sectionKey}) handler error {ex.Message}", ex);
                if (isThrow)
                    throw;
            }

            return defaultValue;

        }
#endif


    }
}