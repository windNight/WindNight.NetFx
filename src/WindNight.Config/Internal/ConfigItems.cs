
#if !NET45
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;
using Microsoft.Extensions.DependencyInjection.WnExtension;
using Microsoft.Extensions.Configuration;
using WindNight.ConfigCenter.Extension;

namespace WindNight.Config.Internal
{
    internal static class ConfigItems
    {
        public static bool OpenConfigLogs =>
            GetAppConfigValue(nameof(OpenConfigLogs), false);

        static bool GetAppConfigValue(string keyName, bool defaultValue = false, bool isThrow = false)
        {
            var config = Ioc.GetService<IConfiguration>();
            var configValue = config.GetAppConfigValue(keyName, defaultValue, isThrow);
            return configValue;

        }

        static string GetAppConfigValue(string keyName, string defaultValue = "", bool isThrow = false)
        {
            var config = Ioc.GetService<IConfiguration>();
            var configValue = config.GetAppConfigValue(keyName, defaultValue, isThrow);
            return configValue;

        }



    }
}
#endif
