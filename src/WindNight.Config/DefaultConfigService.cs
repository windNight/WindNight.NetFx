﻿using WindNight.Core.Abstractions;
#if !NET45
using Microsoft.Extensions.DependencyInjection.WnExtension;
using Microsoft.Extensions.Configuration;
#endif

namespace WindNight.ConfigCenter.Extension
{
    public class DefaultConfigService : ConfigItemsBase, IConfigService
    {
#if !NET45
        public IConfiguration Configuration => Ioc.GetService<IConfiguration>();

        public new int SystemAppId => Configuration?.GetAppConfigValue("AppId", 0, false) ?? 0;
        public new string SystemAppCode => Configuration?.GetAppConfigValue("AppCode", "", false) ?? "";
        public new string SystemAppName => Configuration?.GetAppConfigValue("AppName", "", false) ?? "";


#endif

        public virtual string GetConnString(string connKey, string defaultValue = "", bool isThrow = false)
        {
            return GetConnectionString(connKey, defaultValue, isThrow);
        }

        public new string GetAppSetting(string configKey, string defaultValue = "", bool isThrow = false)
        {
            return ConfigItemsBase.GetAppSetting(configKey, defaultValue, isThrow);
        }

        public new int GetAppSetting(string configKey, int defaultValue = ZeroInt, bool isThrow = false)
        {
            return ConfigItemsBase.GetAppSetting(configKey, defaultValue, isThrow);
        }


        public new long GetAppSetting(string configKey, long defaultValue = ZeroInt64, bool isThrow = false)
        {
            return ConfigItemsBase.GetAppSetting(configKey, defaultValue, isThrow);
        }

        public new bool GetAppSetting(string configKey, bool defaultValue = false, bool isThrow = false)
        {
            return ConfigItemsBase.GetAppSetting(configKey, defaultValue, isThrow);
        }

        public virtual string GetFileConfigString(string fileName, string defaultValue = "", bool isThrow = true)
        {
            return GetJsonConfig(fileName, defaultValue, isThrow);
        }

        public virtual T GetFileConfig<T>(string fileName, bool isThrow = true) where T : new()
        {
            return GetJsonConfig<T>(fileName, "{}", isThrow);
        }
    }
}