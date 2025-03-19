using WindNight.Core.Abstractions;
#if !NET45
using Microsoft.Extensions.DependencyInjection.WnExtension;
using Microsoft.Extensions.Configuration;
#endif

namespace WindNight.ConfigCenter.Extension
{
    internal class DefaultConfigService : IConfigService
    {
#if !NET45
        public IConfiguration Configuration => Ioc.GetService<IConfiguration>();

        public int SystemAppId => Configuration?.GetAppConfigValue("AppId", 0, false) ?? 0;
        public string SystemAppCode => Configuration?.GetAppConfigValue("AppCode", "", false) ?? "";
        public string SystemAppName => Configuration?.GetAppConfigValue("AppName", "", false) ?? "";
        public virtual T GetSectionValue<T>(string sectionKey = "", T defaultValue = default, bool isThrow = false)
            where T : class, new()
        {
            return Configuration.GetSectionValue<T>(sectionKey, defaultValue, isThrow);

        }
        public T GetConfigValue<T>(string keyName, T defaultValue = default, bool isThrow = false)
        {
            var configValue = Configuration.GetConfigValue<T>(keyName, defaultValue, isThrow);
            return configValue;
        }
#endif
        protected const string ZeroString = "0";
        protected const int ZeroInt = 0;
        protected const long ZeroInt64 = 0L;
        protected const decimal ZeroDecimal = 0m;

        public virtual string GetConnString(string connKey, string defaultValue = "", bool isThrow = false)
        {
            return Configuration.GetConnStr(connKey, defaultValue, isThrow);
        }

        public new string GetAppSetting(string configKey, string defaultValue = "", bool isThrow = false)
        {
            return Configuration.GetAppConfigValue(configKey, defaultValue, isThrow);
        }

        public new int GetAppSetting(string configKey, int defaultValue = ZeroInt, bool isThrow = false)
        {
            return Configuration.GetAppConfigValue(configKey, defaultValue, isThrow);
        }


        public new long GetAppSetting(string configKey, long defaultValue = ZeroInt64, bool isThrow = false)
        {
            return Configuration.GetAppConfigValue(configKey, defaultValue, isThrow);
        }

        public new bool GetAppSetting(string configKey, bool defaultValue = false, bool isThrow = false)
        {
            return Configuration.GetAppConfigValue(configKey, defaultValue, isThrow);
        }

        public virtual string GetFileConfigString(string fileName, string defaultValue = "", bool isThrow = true)
        {
            return "";
            //return GetJsonConfig(fileName, defaultValue, isThrow);
        }

        public virtual T GetFileConfig<T>(string fileName, bool isThrow = true) where T : new()
        {

            return default;
            // return GetJsonConfig<T>(fileName, "{}", isThrow);
        }
    }
}
