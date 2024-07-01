using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WindNight.ConfigCenter.Extension
{

    public partial class ConfigItemsBase
    {

        public static object GetAllConfigs() => ConfigCenterContext.GetAllConfig();

        public static List<AppSettingInfo> GetAppSettingList() => ConfigCenterContext.AppSettingList;

        public static List<ConnectionStringInfo> GetConnectionStringList() => ConfigCenterContext.ConnectionStringList;

        public static List<JsonFileConfigInfo> GetJsonConfigList() => ConfigCenterContext.JsonConfigList;

        public static List<XmlFileConfigInfo> GetXmlConfigList() => ConfigCenterContext.XmlConfigList;

        public static Dictionary<string, string> GetUpdateFlagDict() => ConfigProvider.Instance.UpdateFlagDict;

        public static Dictionary<string, DateTime> GetConfigUpdateTime() => ConfigProvider.Instance.ConfigUpdateTime;


        public static FileConfigInfo ReadConfigFileDirect(string fileName) => ConfigProvider.Instance.ReadConfigFileDirect(fileName);
        public static FileConfigInfo ReadSelfConfigFileDirect(string fileDir, string fileName) => ConfigProvider.Instance.ReadSelfConfigFileDirect(fileDir, fileName);

        public static Dictionary<string, string> GetCurrentConfiguration() => ConfigCenterContext.CurrentConfiguration.ToDictionary(k => k.Key, v => v.Value);

        public static IEnumerable<string> FetchSelfConfigNames(string fileDir) => ConfigProvider.Instance.FetchSelfConfigNames(fileDir);
        public static IEnumerable<string> FetchConfigNames() => ConfigProvider.Instance.FetchConfigNames();



    }
}
