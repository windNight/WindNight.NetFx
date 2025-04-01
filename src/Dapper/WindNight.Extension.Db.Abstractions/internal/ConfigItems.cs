using WindNight.Core.ConfigCenter.Extensions;

namespace WindNight.Extension.Db.Abstractions.@internal
{
    internal class ConfigItems : DefaultConfigItemBase
    {
        public static bool OpenDapperLog => GetAppSettingValue(ConfigItemsKey.OpenDapperLogKey, false, false);
        //{
        //    get
        //    {
        //        var config = Ioc.Instance.CurrentConfigService;// Ioc.GetService<IConfigService>();
        //        if (config == null)
        //        {

        //            return false;
        //        }

        //        return config.Configuration.GetSection(ConfigItemsKey.OpenDapperLogKey).Get<bool>() ||
        //               config.GetAppSetting(ConfigItemsKey.OpenDapperLogKey, false, false);

        //    }
        //}

        public static bool IsLogConnectString => GetAppSettingValue(ConfigItemsKey.IsLogConnectStringKey, false, false);
        public static int DapperWarnMs => GetAppSettingValue(ConfigItemsKey.DapperWarnMsKey, 100, false);
        //{
        //    get
        //    {
        //        return GetConfigBoolValue(ConfigItemsKey.IsLogConnectStringKey);
        //        // var config = Ioc.Instance.CurrentConfigService;
        //        // if (config == null) return false;
        //        // return config.GetAppSetting(ConfigItemsKey.IsLogConnectStringKey, false, false);
        //    }
        //}

        static class ConfigItemsKey
        {
            internal static string DapperWarnMsKey = "DapperConfig:WarnMs";
            internal static string OpenDapperLogKey = "DapperConfig:OpenDapperLog";
            internal static string IsLogConnectStringKey = "DapperConfig:IsLogConnectString";

        }

    }
}
