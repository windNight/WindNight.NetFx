using WindNight.NetCore.Extension;

namespace Microsoft.AspNetCore.Mvc.WnExtensions.Internals
{
    internal class ConfigItems : ConfigItemsBase
    {
        private const int DEFAULT_API_WARNING_MIS = 200;

        public static bool ShowHiddenApi => GetConfigValue("ShowHiddenApi", false, false);
        public static bool HiddenSwagger => GetConfigValue("HiddenSwagger", false, false);

        internal static int SysAppId => GetConfigValue("AppId", 0, false);
        internal static string SysAppCode => GetConfigValue("AppCode", false);
        internal static string SysAppName => GetConfigValue("AppName", false);
        internal static int ApiWarningMis => GetConfigValue("ApiWarningMis", DEFAULT_API_WARNING_MIS, false);
        internal static bool LogProcessOpened => GetConfigValue("LogProcessOpened", false, false);
        internal static bool IsValidateInput => GetConfigValue("IsValidateInput", false, false);
    }
}