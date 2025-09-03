using System.Collections.Concurrent;

namespace WindNight.ConfigCenter.Extension
{
    internal partial class ConfigCenterContext
    {
        /// <summary>
        ///     Configuration
        /// </summary>
        public static ConcurrentDictionary<string, string> CurrentConfiguration { get; } =
            new();


        public static object GetAllConfig()
        {
            var obj = new
            {
                AppSettingList,
                //DomainSwitchList,
                JsonConfigList,
                XmlConfigList,
                ConnectionStringList,
                ConfigProvider.Instance.UpdateFlagDict,
                ConfigProvider.Instance.ConfigUpdateTime,
            };
            return obj;
        }


        private static string FixConfigPathPrefix(ConfigType configType)
        {
            var prefix = "";
            switch (configType)
            {
                case ConfigType.AppSettings:
                    prefix = AppSettingsPathPrefix;
                    break;
                //case ConfigType.DomainSwitch:
                //    prefix = DomainSwitchPathPrefix;
                //    break;
                case ConfigType.ConnectionStrings:
                    prefix = ConnectionStringsPathPrefix;
                    break;
                case ConfigType.JsonConfig:
                    prefix = JsonConfigPathPrefix;
                    break;
                case ConfigType.XmlConfig:
                    prefix = XmlConfigPathPrefix;
                    break;

                case ConfigType.Unknown:
                default:
                    break;
            }

            return prefix;
        }

        private static string FixDictKey(ConfigType configType, string configKey)
        {
            return $"{FixConfigPathPrefix(configType)}:{configKey}";
        }


        private static string GetFromConfigurationDict(string configKey, string defaultValue = "")
        {
            if (configKey.IsNullOrEmpty())
            {
                return defaultValue;
            }

            if (CurrentConfiguration.TryGetValue(configKey, out var configValue) && configValue.IsNotNullOrEmpty())
            {
                return configValue;
            }

            return defaultValue;
        }
    }
}
