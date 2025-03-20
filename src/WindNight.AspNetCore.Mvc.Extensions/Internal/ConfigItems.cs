using WindNight.Config.Extensions;
using WindNight.ConfigCenter.Extension;

namespace Microsoft.AspNetCore.Mvc.WnExtensions.@internal
{
    internal class ConfigItems : DefaultConfigItemsBase
    {
        private const int DEFAULT_API_WARNING_MIS = 200;

        public static bool ShowHiddenApi => GetAppConfigValue("ShowHiddenApi", false, false);
        public static bool HiddenSwagger => GetAppConfigValue("HiddenSwagger", false, false);

        internal static int SysAppId => GetAppConfigValue("AppId", 0, false);
        internal static string SysAppCode => GetAppConfigValue("AppCode", "", false);
        internal static string SysAppName => GetAppConfigValue("AppName", "", false);
        internal static int ApiWarningMis => GetAppConfigValue("ApiWarningMis", DEFAULT_API_WARNING_MIS, false);
        internal static bool LogProcessOpened => GetAppConfigValue("LogProcessOpened", false, false);
        internal static bool ApiUrlOpened => GetAppConfigValue("ApiUrlOpened", false, false);
        internal static bool OpenInternalApi => GetAppConfigValue("OpenInternalApi", false, false);
        internal static bool IsValidateInput => GetAppConfigValue("IsValidateInput", false, false);
    }
}
