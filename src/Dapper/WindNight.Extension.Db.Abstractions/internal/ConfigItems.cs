using WindNight.Core.ConfigCenter.Extensions;

namespace WindNight.Extension.Db.Abstractions.@internal
{
    internal class ConfigItems : DefaultConfigItemBase
    {
        public static bool OpenDapperLog => DapperConfig?.OpenDapperLog ?? GetConfigValue(ConfigItemsKey.OpenDapperLogKey, false, false);


        public static bool IsLogConnectString => DapperConfig?.IsLogConnectString ?? GetConfigValue(ConfigItemsKey.IsLogConnectStringKey, false, false);


        public static long DapperWarnMs => DapperConfig?.WarnMs ?? GetConfigValue(ConfigItemsKey.DapperWarnMsKey, 500L, false);

        public static DapperConfig DapperConfig => GetSectionValue<DapperConfig>(null);

        static class ConfigItemsKey
        {
            internal static string DapperWarnMsKey = "DapperConfig:WarnMs";
            internal static string OpenDapperLogKey = "DapperConfig:OpenDapperLog";
            internal static string IsLogConnectStringKey = "DapperConfig:IsLogConnectString";

        }

    }

    internal class DapperConfig
    {
        public bool OpenDapperLog { get; set; } = false;

        public bool IsLogConnectString { get; set; } = false;

        public long WarnMs { get; set; } = 500L;
    }

}
