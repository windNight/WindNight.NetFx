using Microsoft.Extensions.Configuration;
using System;
using Microsoft.Extensions.DependencyInjection.WnExtension;
using WindNight.Core.Abstractions;
using Newtonsoft.Json.Extension;
using Microsoft.Extensions.Options;

namespace WindNight.AspNetCore.Hosting.Internals
{
    internal class DefaultConfigService : IConfigService
    {
        public IConfiguration Configuration => Ioc.GetService<IConfiguration>();

        public DefaultConfigService()
        {
        }

        public string GetConnString(string connKey, string defaultValue = "", bool isThrow = true)
        {
            if (isThrow && Configuration == null)
                throw new NotImplementedException($"Can't Get IConfiguration From DI Container.  ");
            return Configuration.GetValue<string>($"ConnectionStrings:{connKey}");
        }

        public string GetAppSetting(string configKey, string defaultValue = "", bool isThrow = true)
        {
            if (isThrow && Configuration == null)
                throw new NotImplementedException($"Can't Get IConfiguration From DI Container.  ");
            return Configuration.GetValue<string>($"AppSettings:{configKey}");

        }

        public int GetAppSetting(string configKey, int defaultValue = 0, bool isThrow = true)
        {
            var config = GetAppSetting(configKey, string.Empty, isThrow);//  _configuration.GetValue<string>($"AppSettings:{configKey}");
            if (string.IsNullOrEmpty(config) && isThrow)
                throw new NotImplementedException($"Can't Get IConfiguration From DI Container.  ");
            return int.TryParse(config, out var flag) ? flag : defaultValue;
        }

        public bool GetAppSetting(string configKey, bool defaultValue = false, bool isThrow = true)
        { 
            var config = GetAppSetting(configKey, string.Empty, isThrow);// _configuration.GetValue<string>($"AppSettings:{configKey}");
            if (string.IsNullOrEmpty(config) && isThrow)
                throw new Exception($"Can't Get IConfiguration From DI Container.  ");
            if (bool.TryParse(config, out var flag))
            {
                return flag;
            }

            return config switch
            {
                "1" => true,
                "0" => false,
                _ => defaultValue
            };
        }

        public string GetFileConfigString(string fileName, string defaultValue = "", bool isThrow = true)
        {
            if (isThrow && Configuration == null)
                throw new NotImplementedException($"Can't Get IConfiguration From DI Container.  ");
            return Configuration.GetSection(fileName).Get<string>();
            //var config = Configuration.GetSection(fileName).Value;
            //return config;
        }

        public T GetFileConfig<T>(string fileName, bool isThrow = true) where T : new()
        {
            if (isThrow && Configuration == null)
                throw new NotImplementedException($"Can't Get IConfiguration From DI Container.  ");
            var configValue = Ioc.GetService<IOptionsMonitor<T>>().CurrentValue;
            return configValue ?? Configuration.GetSection(fileName).Get<T>();
        }

    }
}
