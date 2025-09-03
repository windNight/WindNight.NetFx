namespace WindNight.ConfigCenter.Extension
{
    internal partial class ConfigCenterContext
    {
        private static string XmlConfigPathPrefix => $"{ConfigType.XmlConfig}";


        public static List<XmlFileConfigInfo> XmlConfigList
        {
            get
            {
                var keys = CurrentConfiguration.Keys.Where(m => m.Contains(XmlConfigPathPrefix));
                var list = new List<XmlFileConfigInfo>();
                foreach (var key in keys)
                {
                    var keysp = key.Split(':');
                    var fileName = keysp[Math.Max(0, keysp.Length - 1)];
                    var fileContent = GetXmlConfig(fileName);
                    list.Add(new XmlFileConfigInfo
                    {
                        FileName = fileName,
                        LastModifyTime = ConfigProvider.Instance.ConfigUpdateTime[fileName],
                        FileContent = fileContent,
                        Path = key,
                    });
                }

                return list;
            }
        }


        #region Json Config

        ///// <summary>
        /////     设置配置中心中配置列表的值
        ///// </summary>
        ///// <param name="dict"></param>
        //public static void SetXmlConfigList(Dictionary<string, string> dict)
        //{
        //    foreach (var item in dict) SetXmlConfig(item.Key, item.Value);
        //}

        ///// <summary>
        /////     设置配置中心中配置列表的值
        ///// </summary>
        ///// <param name="fileName"></param>
        ///// <param name="fileContent"></param>
        //public static void SetXmlConfig(string fileName, string fileContent)
        //{
        //    var key = FixDictKey(ConfigType.XmlConfig, fileName);
        //    CurrentConfiguration[key] = fileContent;
        //}

        /// <summary>
        ///     获取配置中心中配置列表的值
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="defaultValue"></param>
        public static string GetXmlConfig(string fileName, string defaultValue = "")
        {
            if (fileName.IsNullOrEmpty())
            {
                return defaultValue;
            }

            var key = FixDictKey(ConfigType.XmlConfig, fileName);
            var configValue = GetFromConfigurationDict(key, defaultValue);
            if (configValue.IsNotNullOrEmpty())
            {
                return configValue;
            }

            var loadRlt = ConfigProvider.Instance.LoadConfigFile(configValue);

            if (loadRlt.Item1 == 0)
            {
                configValue = loadRlt.Item3;
            }
            else
            {
                configValue = defaultValue;
            }

            return configValue;
        }

        #endregion //end  Json Config
    }
}
