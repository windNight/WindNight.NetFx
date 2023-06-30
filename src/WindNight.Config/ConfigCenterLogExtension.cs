using WindNight.Core.Abstractions;

namespace WindNight.ConfigCenter.Extension
{
    public static class ConfigCenterLogExtension
    {
        public static ILogService ConfigCenterLogProvider ;

        public static void InitLogProvider(ILogService logService)
        {
            ConfigCenterLogProvider = logService;
        }
    }
}