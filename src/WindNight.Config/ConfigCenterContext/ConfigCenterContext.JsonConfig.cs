using WindNight.Config.@internal;

namespace WindNight.ConfigCenter.Extension
{
    internal partial class ConfigCenterContext
    {
        private static string JsonConfigPathPrefix => $"{ConfigType.JsonConfig}";


        public static List<JsonFileConfigInfo> JsonConfigList
        {
            get
            {
                var keys = CurrentConfiguration.Keys.Where(m => m.Contains(JsonConfigPathPrefix));
                var list = new List<JsonFileConfigInfo>();
                foreach (var key in keys)
                {
                    var keysp = key.Split(':');
                    var fileName = keysp[Math.Max(0, keysp.Length - 1)];
                    var fileContent = GetJsonConfig(fileName);
                    list.Add(new JsonFileConfigInfo
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
        //public static void SetJsonConfigList(Dictionary<string, string> dict)
        //{
        //    foreach (var item in dict) SetJsonConfig(item.Key, item.Value);
        //}

        /// <summary>
        ///     设置配置中心中配置列表的值
        /// </summary>
        /// <param name="dict"></param>
        public static void SetConfigList(Dictionary<string, string> dict)
        {
            foreach (var item in dict)
            {
                SetConfig(item.Key, item.Value);
            }
        }

        /// <summary>
        ///     设置配置中心中配置列表的值
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="fileContent"></param>
        public static void SetConfig(string fileName, string fileContent, bool fixKey = true)
        {
            var key = fileName;
            if (fixKey)
            {
                var configType = fileName.ParserConfigType();
                key = FixDictKey(configType, fileName);
            }

            CurrentConfiguration[key] = fileContent;
        }

        ///// <summary>
        /////     设置配置中心中配置列表的值
        ///// </summary>
        ///// <param name="fileName"></param>
        ///// <param name="fileContent"></param>
        //public static void SetJsonConfig(string fileName, string fileContent, bool fixKey = true)
        //{
        //    var key = fileName;
        //    if (fixKey)
        //    {
        //        var configType = fileName.ParserConfigType();
        //        key = FixDictKey(configType, fileName);
        //    }
        //    CurrentConfiguration[key] = fileContent;
        //}

        /// <summary>
        ///     获取配置中心中配置列表的值
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="defaultValue"></param>
        public static string GetJsonConfig(string fileName, string defaultValue = "")
        {
            if (fileName.IsNullOrEmpty())
            {
                return defaultValue;
            }

            var key = FixDictKey(ConfigType.JsonConfig, fileName);
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
