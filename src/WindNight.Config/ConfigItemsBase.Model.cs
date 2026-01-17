using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WindNight.ConfigCenter.Extension
{

    public partial class ConfigItemsBase
    {

        public static object GetAllConfigs() => ConfigCenterContext.GetAllConfig();

        public static IEnumerable<AppSettingInfo> GetAppSettingList() => ConfigCenterContext.AppSettingList;

        public static IEnumerable<ConnectionStringInfo> GetConnectionStringList() => ConfigCenterContext.ConnectionStringList;

        public static IEnumerable<JsonFileConfigInfo> GetJsonConfigList() => ConfigCenterContext.JsonConfigList;

        public static IEnumerable<XmlFileConfigInfo> GetXmlConfigList() => ConfigCenterContext.XmlConfigList;

        public static IDictionary<string, string> GetUpdateFlagDict() => ConfigProvider.Instance.UpdateFlagDict;

        public static IDictionary<string, DateTime> GetConfigUpdateTime() => ConfigProvider.Instance.ConfigUpdateTime;


        public static FileConfigInfo ReadConfigFileDirect(string fileName) => ConfigProvider.Instance.ReadConfigFileDirect(fileName);
      
        public static FileConfigInfo ReadSelfConfigFileDirect(string fileDir, string fileName) => ConfigProvider.Instance.ReadSelfConfigFileDirect(fileDir, fileName);

        public static IDictionary<string, string> GetCurrentConfiguration() => ConfigCenterContext.CurrentConfiguration.ToDictionary(k => k.Key, v => v.Value);

        public static IEnumerable<string> FetchSelfConfigNames(string fileDir) => ConfigProvider.Instance.FetchSelfConfigNames(fileDir);
      
        public static IEnumerable<string> FetchConfigNames() => ConfigProvider.Instance.FetchConfigNames();


        public static IEnumerable<ConfigFileBaseInfo> FetchSelfConfigFileInfos(string fileDir) => ConfigProvider.Instance.FetchSelfConfigFileInfos(fileDir);

        public static IEnumerable<ConfigFileBaseInfo> FetchConfigFileInfos() => ConfigProvider.Instance.FetchConfigFileInfos();


    }
}
