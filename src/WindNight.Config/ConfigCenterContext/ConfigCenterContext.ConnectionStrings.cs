using System;
using System.Collections.Generic;
using System.Linq;

namespace WindNight.ConfigCenter.Extension
{
    internal partial class ConfigCenterContext
    {
        private static string ConnectionStringsPathPrefix => $"{ConfigType.ConnectionStrings}:";


        public static List<ConnectionStringInfo> ConnectionStringList
        {
            get
            {
                var keys = CurrentConfiguration.Keys.Where(m => m.Contains(ConnectionStringsPathPrefix));
                var list = new List<ConnectionStringInfo>();
                foreach (var key in keys)
                {
                    var keysp = key.Split(':');
                    list.Add(new ConnectionStringInfo
                    {
                        Path = key,
                        Value = CurrentConfiguration[key],
                        Key = keysp[Math.Max(0, keysp.Length - 1)]
                    });
                }

                return list;
            }
        }

        #region ConnectionStrings

        /// <summary>
        ///     设置配置中心中配置列表的值
        /// </summary>
        /// <param name="dict"></param>
        public static void SetConnectionStrings(Dictionary<string, string> dict)
        {
            foreach (var item in dict) SetConnectionString(item.Key, item.Value);
        }

        /// <summary>
        ///     设置配置中心中配置列表的值
        /// </summary>
        /// <param name="configKey"></param>
        /// <param name="configValue"></param>
        public static void SetConnectionString(string configKey, string configValue)
        {
            var key = FixDictKey(ConfigType.ConnectionStrings, configKey);
            CurrentConfiguration[key] = configValue;
        }

        /// <summary>
        ///     获取配置中心中配置列表的值
        /// </summary>
        /// <param name="configKey"></param>
        /// <param name="defaultValue"></param>
        public static string GetConnectionString(string configKey, string defaultValue = "")
        {
            if (configKey.IsNullOrEmpty()) return defaultValue;
            var key = FixDictKey(ConfigType.ConnectionStrings, configKey);
            return GetFromConfigurationDict(key, defaultValue);
        }

        #endregion //end ConnectionStrings
    }
}