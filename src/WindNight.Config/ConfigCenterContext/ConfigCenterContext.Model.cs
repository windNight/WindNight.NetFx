using System;
using WindNight.Core;

namespace WindNight.ConfigCenter.Extension
{
    public enum ConfigType
    {
        Unknown = 0,
        AppSettings = 1,
        DomainSwitch = 2,
        ConnectionStrings = 3,
        JsonConfig = 4,
        XmlConfig = 5,

    }

    public class XmlFileConfigInfo : FileConfigInfo
    {

    }
    public class JsonFileConfigInfo : FileConfigInfo
    {

    }

    public class FileConfigInfo : ConfigFileBaseInfo
    {
        //public string Path { get; set; }
        //public string FileName { get; set; }
        public string FileContent { get; set; }
        //public DateTime LastModifyTime { get; set; }
    }


    public class ConfigFileBaseInfo
    {
        public string Path { get; set; }
        public string FileName { get; set; }
        public DateTime LastModifyTime { get; set; }

    }

    public class AppSettingInfo : ConfigBaseInfo
    {
        public bool Loop { get; set; } = false;
        //public new string Value { get; set; }
    }

    internal class DomainSwitchInfo : ConfigBaseInfo
    {
        //public new string Value { get; set; }
    }

    public class ConnectionStringInfo : ConfigBaseInfo
    {
        //public new string Value { get; set; }
    }


}