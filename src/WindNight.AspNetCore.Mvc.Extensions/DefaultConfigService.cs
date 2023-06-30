using Microsoft.Extensions.Configuration;
using System;
using Microsoft.Extensions.DependencyInjection.WnExtension;
using WindNight.Core.Abstractions;

namespace Microsoft.AspNetCore.Mvc.WnExtensions
{
    public class DefaultConfigService : IConfigService
    {
     //   private readonly IConfiguration _configuration;
        public IConfiguration Configuration => Ioc.GetService<IConfiguration>();

        public DefaultConfigService( )
        {
           // _configuration = configuration;
        }

        T GetConfig<T>(string configKey, T defaultValue = default(T), bool isThrow = true)
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
                var configValue = Configuration.GetSection(configKey).Get<T>();
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
            => GetConfig<string>(configKey, defaultValue, isThrow);

        public int GetAppSetting(string configKey, int defaultValue = 0, bool isThrow = true)
            => GetConfig<int>(configKey, defaultValue, isThrow);

        public bool GetAppSetting(string configKey, bool defaultValue = false, bool isThrow = true)
            => GetConfig<bool>(configKey, defaultValue, isThrow);

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
