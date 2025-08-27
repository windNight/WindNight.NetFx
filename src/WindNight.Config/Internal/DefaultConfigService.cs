using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.WnExtension;
using Newtonsoft.Json.Extension;
using WindNight.ConfigCenter.Extension;
using WindNight.ConfigCenter.Extension.@internal;
using WindNight.Core.Abstractions;
using WindNight.Core.ConfigCenter.Extensions;

namespace WindNight.Config.@internal
{
    internal partial class DefaultConfigService : IConfigService
    {
        public virtual string GetConnString(string connKey, string defaultValue = "", bool isThrow = true)
        {
            CheckConfiguration(isThrow);
            return Configuration?.GetConnString(connKey, defaultValue, isThrow) ?? "";
        }


    }

    internal partial class DefaultConfigService
    {

        public virtual IEnumerable<string> GetAppSettingList(string configKey, IEnumerable<string> defaultValue = null, bool isThrow = false, bool needDistinct = true)
        {
            CheckConfiguration(isThrow);
            if (defaultValue == null)
            {
                defaultValue = HardInfo.EmptyList<string>();
            }
            return Configuration?.GetAppSettingList(configKey, defaultValue, isThrow, needDistinct) ?? defaultValue;
        }

        public virtual IEnumerable<int> GetAppSettingList(string configKey, IEnumerable<int> defaultValue = null, bool isThrow = false, bool needDistinct = true)
        {
            CheckConfiguration(isThrow);
            if (defaultValue == null)
            {
                defaultValue = HardInfo.EmptyList<int>();
            }
            return Configuration?.GetAppSettingList(configKey, defaultValue, isThrow, needDistinct) ?? defaultValue;
        }

        public virtual IEnumerable<T> GetAppSettingList<T>(string configKey, Func<string, T> convert, IEnumerable<T> defaultValue = null, bool isThrow = true, bool needDistinct = true)
        {
            CheckConfiguration(isThrow);
            if (defaultValue == null)
            {
                defaultValue = HardInfo.EmptyList<T>();
            }
            return Configuration?.GetAppSettingList<T>(configKey, convert, defaultValue, isThrow, needDistinct) ?? defaultValue;
        }
    }

    internal partial class DefaultConfigService
    {
        public virtual string GetAppSettingValue(string configKey, string defaultValue = "", bool isThrow = false)
        {
            CheckConfiguration(isThrow);
            return Configuration?.GetAppSettingValue(configKey, defaultValue, isThrow) ?? defaultValue;
        }

        public virtual int GetAppSettingValue(string configKey, int defaultValue = 0, bool isThrow = false)
        {
            CheckConfiguration(isThrow);
            return Configuration?.GetAppSettingValue(configKey, defaultValue, isThrow) ?? defaultValue;
        }

        public virtual long GetAppSettingValue(string configKey, long defaultValue = 0, bool isThrow = false)
        {
            CheckConfiguration(isThrow);
            return Configuration?.GetAppSettingValue(configKey, defaultValue, isThrow) ?? defaultValue;
        }

        public virtual bool GetAppSettingValue(string configKey, bool defaultValue = false, bool isThrow = false)
        {
            CheckConfiguration(isThrow);
            return Configuration?.GetAppSettingValue(configKey, defaultValue, isThrow) ?? defaultValue;
        }

        public virtual decimal GetAppSettingValue(string configKey, decimal defaultValue = 0m, bool isThrow = false)
        {
            CheckConfiguration(isThrow);
            return Configuration?.GetAppSettingValue(configKey, defaultValue, isThrow) ?? defaultValue;
        }

    }
    internal partial class DefaultConfigService
    {

        public virtual T GetSectionValue<T>(string sectionKey = "", T defaultValue = default, bool isThrow = false) where T : class, new()
        {
            CheckConfiguration(isThrow);
            return Configuration?.GetSectionValue<T>(sectionKey, defaultValue, isThrow) ?? defaultValue;
        }

        public virtual T GetConfigValue<T>(string keyName, T defaultValue = default, bool isThrow = false)
        {
            CheckConfiguration(isThrow);
            return Configuration.GetConfigValue(keyName, defaultValue, isThrow) ?? defaultValue;
        }
    }

    internal partial class DefaultConfigService
    {
        public virtual IConfiguration Configuration => Ioc.GetService<IConfiguration>();

        public virtual string SystemAppId => Configuration?.GetAppId() ?? "";
        public virtual string SystemAppCode => Configuration?.GetAppCode() ?? "";
        public virtual string SystemAppName => Configuration?.GetAppName() ?? "";

        void CheckConfiguration(bool isThrow = true)
        {
            if (isThrow && Configuration == null)
            {
                throw new NotImplementedException("Can't Get IConfiguration From DI Container.  ");
            }
        }

    }

    internal partial class DefaultConfigService
    {
        /// <summary>
        ///  content from configName  
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="defaultValue"></param>
        /// <param name="isThrow"></param>
        /// <returns></returns>
        public virtual string GetFileConfigString(string fileName, string defaultValue = "", bool isThrow = true)
        {
            return ConfigCenterContext.GetJsonConfig(fileName);
        }

        /// <summary>
        ///  content from configName  To T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fileName">configName</param>
        /// <param name="isThrow"></param>
        /// <returns></returns> 
        public virtual T GetFileConfig<T>(string fileName, bool isThrow = true) where T : new()
        {
            var configValue = ConfigCenterContext.GetJsonConfig(fileName);
            if (configValue.IsNullOrEmpty()) return default;

            return configValue.To<T>();

        }



    }
}
