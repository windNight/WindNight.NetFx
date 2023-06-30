using System;
using System.Collections.Generic;
using System.Linq;

namespace WindNight.ConfigCenter.Extension
{
    internal partial class ConfigCenterContext
    {
        private static string AppSettingsPathPrefix => $"{ConfigType.AppSettings}:";


        public static List<AppSettingInfo> AppSettingList
        {
            get
            {
                var keys = CurrentConfiguration.Keys.Where(m => m.Contains(AppSettingsPathPrefix));
                var list = new List<AppSettingInfo>();
                foreach (var key in keys)
                {
                    var keysp = key.Split(':');
                    list.Add(new AppSettingInfo
                    {
                        Path = key,
                        Value = CurrentConfiguration[key],
                        Key = keysp[Math.Max(0, keysp.Length - 1)]
                    });
                }

                return list;
            }
        }


        #region AppSettings

        /// <summary>
        ///     设置配置中心中配置列表的值
        /// </summary>
        /// <param name="dict"></param>
        public static void SetAppSettings(Dictionary<string, string> dict)
        {
            foreach (var item in dict) SetAppSetting(item.Key, item.Value);
        }

        /// <summary>
        ///     设置配置中心中配置列表的值
        /// </summary>
        /// <param name="configKey"></param>
        /// <param name="configValue"></param>
        public static void SetAppSetting(string configKey, string configValue)
        {
            var key = FixDictKey(ConfigType.AppSettings, configKey);
            CurrentConfiguration[key] = configValue;
        }

        /// <summary>
        ///     获取配置中心中配置列表的值
        /// </summary>
        /// <param name="configKey"></param>
        /// <param name="defaultValue"></param>
        public static string GetAppSetting(string configKey, string defaultValue = "")
        {
            if (configKey.IsNullOrEmpty()) return defaultValue;
            var key = FixDictKey(ConfigType.AppSettings, configKey);
            return GetFromConfigurationDict(key, defaultValue);
        }

        #endregion //end AppSettings
    }
}