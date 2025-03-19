using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.WnExtension;
using WindNight.Config.Extensions;
using WindNight.ConfigCenter.Extension;
using WindNight.Core.Abstractions;

namespace Microsoft.AspNetCore.Mvc.WnExtensions
{
    public class DefaultConfigService : IConfigService
    {
        //   private readonly IConfiguration _configuration;
        public IConfiguration Configuration => Ioc.GetService<IConfiguration>();

        public int SystemAppId => Configuration?.GetAppConfigValue("AppId", 0, false) ?? 0;
        public string SystemAppCode => Configuration?.GetAppConfigValue("AppCode", "", false) ?? "";
        public string SystemAppName => Configuration?.GetAppConfigValue("AppName", "", false) ?? "";

        public DefaultConfigService()
        {
            // _configuration = configuration;
        }

        public T GetSectionValue<T>(string sectionKey = "", T defaultValue = default, bool isThrow = false)
            where T : class, new()
        {
            return Configuration.GetSectionValue(sectionKey, defaultValue, isThrow);
        }

        public T GetConfigValue<T>(string keyName, T defaultValue = default, bool isThrow = false)
        {
            var configValue = Configuration.GetConfigValue<T>(keyName, defaultValue, isThrow);
            return configValue;
        }

        static string FixAppConfigKey(string keyName) => $"{nameof(ConfigType.AppSettings)}:{keyName}";

        T GetAppConfigValue<T>(string keyName, T defaultValue = default(T), bool isThrow = true)
        {
            if (Configuration == null)
            {
                if (isThrow)
                {
                    throw new ArgumentNullException(nameof(IConfiguration));
                }

                return defaultValue;
            }
            try
            {
                var configKey = FixAppConfigKey(keyName);
                var configValue = Configuration.GetConfigValue<T>(configKey, defaultValue, isThrow);
                return configValue;
            }
            catch (Exception e)
            {
                //LogHelper.
                if (isThrow)
                {
                    throw;
                }

                return defaultValue;
            }
        }

        string GetConnectionString(string configKey, string defaultValue = "", bool isThrow = true)
        {
            if (Configuration == null)
            {
                if (isThrow)
                {
                    throw new ArgumentNullException(nameof(IConfiguration));
                }

                return defaultValue;
            }
            try
            {
                var configValue = Configuration.GetConnectionString(configKey);
                return configValue;
            }
            catch (Exception e)
            {
                //LogHelper.
                if (isThrow)
                {
                    throw;
                }

                return defaultValue;
            }
        }

        public string GetAppSetting(string configKey, string defaultValue = "", bool isThrow = true)
            => GetAppConfigValue<string>(configKey, defaultValue, isThrow);

        public int GetAppSetting(string configKey, int defaultValue = 0, bool isThrow = true)
            => GetAppConfigValue<int>(configKey, defaultValue, isThrow);

        public long GetAppSetting(string configKey, long defaultValue = 0L, bool isThrow = true)
            => GetAppConfigValue<long>(configKey, defaultValue, isThrow);

        public bool GetAppSetting(string configKey, bool defaultValue = false, bool isThrow = true)
            => GetAppConfigValue<bool>(configKey, defaultValue, isThrow);

        public string GetConnString(string connKey, string defaultValue = "", bool isThrow = true)
            => GetConnectionString(connKey, defaultValue, isThrow);

        // TODO 暂未实现
        public T GetFileConfig<T>(string fileName, bool isThrow = true) where T : new()
        {
            return default(T);
        }

        public string GetFileConfigString(string fileName, string defaultValue = "", bool isThrow = true)
        {
            return "";
        }
    }
}
