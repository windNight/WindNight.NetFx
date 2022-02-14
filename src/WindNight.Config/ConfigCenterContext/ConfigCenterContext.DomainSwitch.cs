using System;
using System.Collections.Generic;
using System.Linq;

namespace WindNight.ConfigCenter.Extension
{
    internal partial class ConfigCenterContext
    {
        private static string DomainSwitchPathPrefix => $"{ConfigType.DomainSwitch}:";

        public static List<DomainSwitchInfo> DomainSwitchList
        {
            get
            {
                var keys = CurrentConfiguration.Keys.Where(m => m.Contains(DomainSwitchPathPrefix));
                var list = new List<DomainSwitchInfo>();
                foreach (var key in keys)
                {
                    var keysp = key.Split(':');
                    list.Add(new DomainSwitchInfo
                    {
                        Path = key,
                        Value = CurrentConfiguration[key],
                        Key = keysp[Math.Max(0, keysp.Length - 1)]
                    });
                }

                return list;
            }
        }


        #region DomainSwitch Config 

        /// <summary>
        ///     设置配置中心中配置列表的值
        /// </summary>
        /// <param name="dict"></param>
        public static void SetDomainSwitch(Dictionary<string, string> dict)
        {
            foreach (var item in dict) SetDomainSwitch(item.Key, item.Value);
        }

        /// <summary>
        ///     设置配置中心中配置列表的值
        /// </summary>
        /// <param name="configKey"></param>
        /// <param name="configValue"></param>
        public static void SetDomainSwitch(string configKey, string configValue)
        {
            var key = FixDictKey(ConfigType.DomainSwitch, configKey);
            CurrentConfiguration[key] = configValue;
        }

        /// <summary>
        ///     读取配置中心中配置列表的值
        /// </summary>
        /// <param name="nodeName"></param>
        /// <param name="nodeType"></param>
        /// <param name="defaultValue"></param>
        public static string GetDomainSwitchConfig(string nodeName,
            DomainSwitchNodeType nodeType = DomainSwitchNodeType.Unknown, string defaultValue = "")
        {
            if (nodeName.IsNullOrEmpty() || nodeType == DomainSwitchNodeType.Unknown) return defaultValue;
            var key = FixDictKey(ConfigType.DomainSwitch, $"{nodeType}:{nodeName}");
            var configValue = GetFromConfigurationDict(key, defaultValue);
            if (!configValue.IsNullOrEmpty()) return configValue;
            var loadRlt = ConfigProvider.Instance.LoadDomainSwitch(configValue, nodeType);

            configValue = loadRlt.Item1 == 0 ? loadRlt.Item3 : defaultValue;
            return configValue;
        }

        #endregion //end DomainSwitch Config 
    }
}