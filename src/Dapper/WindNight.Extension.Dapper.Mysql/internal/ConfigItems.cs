using WindNight.Core.ConfigCenter.Extensions;

namespace WindNight.Extension.Dapper.Mysql.@internal
{
    internal class ConfigItems : DefaultConfigItemBase
    {
        public static bool OpenDapperLog => GetAppSettingValue(ConfigItemsKey.OpenDapperLogKey, false, false);


        public static bool IsLogConnectString => GetAppSettingValue(ConfigItemsKey.IsLogConnectStringKey, false, false);

        public static long DapperWarnMs => GetAppSettingValue(ConfigItemsKey.DapperWarnMsKey, 100L, false);



        static class ConfigItemsKey
        {
            internal static string DapperWarnMsKey = "DapperConfig:WarnMs";
            internal static string OpenDapperLogKey = "DapperConfig:OpenDapperLog";
            internal static string IsLogConnectStringKey = "DapperConfig:IsLogConnectString";

        }

    }
}
