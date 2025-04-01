//using System;
//using Microsoft.Extensions.Configuration;
//using Microsoft.Extensions.DependencyInjection.WnExtension;
//using Microsoft.Extensions.Options;
//using WindNight.Config.Extensions;
//using WindNight.Core.Abstractions;

//namespace WindNight.AspNetCore.Hosting.@internal
//{
//    internal partial class DefaultConfigService : DefaultConfigItemsBase, IConfigService
//    {
//        //public IConfiguration Configuration => Ioc.GetService<IConfiguration>();

//        //public int SystemAppId => Configuration?.GetAppId() ?? 0;//?.GetAppConfigValue("AppId", 0) ?? 0;
//        //public string SystemAppCode => Configuration?.GetAppCode() ?? "";// Configuration?.GetAppConfigValue("AppCode", "") ?? "";
//        //public string SystemAppName => Configuration?.GetAppName() ?? "";// Configuration?.GetAppConfigValue("AppName", "") ?? "";

//        //public T GetSectionValue<T>(string sectionKey = "", T defaultValue = default, bool isThrow = false)
//        //    where T : class, new()
//        //{
//        //    return Configuration.GetSectionValue(sectionKey, defaultValue, isThrow);
//        //}

//        //public T GetConfigValue<T>(string keyName, T defaultValue = default, bool isThrow = false)
//        //{
//        //    var configValue = Configuration.GetConfigValue<T>(keyName, defaultValue, isThrow);
//        //    return configValue;
//        //}

//        public string GetConnString(string connKey, string defaultValue = "", bool isThrow = true)
//        {
//            return GetConnectionString(connKey, defaultValue, isThrow);
//            //if (isThrow && Configuration == null)
//            //    throw new NotImplementedException("Can't Get IConfiguration From DI Container.  ");
//            //return Configuration.GetValue<string>($"ConnectionStrings:{connKey}");
//        }

//        public string GetAppSetting(string configKey, string defaultValue = "", bool isThrow = true)
//        {
//            return GetAppConfigValue(configKey, defaultValue, isThrow);
//            //if (isThrow && Configuration == null)
//            //    throw new NotImplementedException("Can't Get IConfiguration From DI Container.  ");
//            //return Configuration.GetValue<string>($"AppSettings:{configKey}");
//        }

//        public int GetAppSetting(string configKey, int defaultValue = 0, bool isThrow = true)
//        {
//            return GetAppConfigValue(configKey, defaultValue, isThrow);
//            //var config =
//            //    GetAppSetting(configKey, string.Empty,
//            //        isThrow); //  _configuration.GetValue<string>($"AppSettings:{configKey}");
//            //if (config.IsNullOrEmpty() && isThrow)
//            //    throw new NotImplementedException("Can't Get IConfiguration From DI Container.  ");
//            //return int.TryParse(config, out var flag) ? flag : defaultValue;
//        }

//        public long GetAppSetting(string configKey, long defaultValue = 0L, bool isThrow = true)
//        {
//            return GetAppConfigValue(configKey, defaultValue, isThrow);
//            //var config =
//            //    GetAppSetting(configKey, string.Empty,
//            //        isThrow); //  _configuration.GetValue<string>($"AppSettings:{configKey}");
//            //if (config.IsNullOrEmpty() && isThrow)
//            //    throw new NotImplementedException("Can't Get IConfiguration From DI Container.  ");
//            //return long.TryParse(config, out var flag) ? flag : defaultValue;
//        }


//        public bool GetAppSetting(string configKey, bool defaultValue = false, bool isThrow = true)
//        {
//            return GetAppConfigValue(configKey, defaultValue, isThrow);
//            //var config =
//            //    GetAppSetting(configKey, string.Empty,
//            //        isThrow); // _configuration.GetValue<string>($"AppSettings:{configKey}");
//            //if (config.IsNullOrEmpty() && isThrow)
//            //    throw new Exception("Can't Get IConfiguration From DI Container.  ");
//            //if (bool.TryParse(config, out var flag))
//            //{
//            //    return flag;
//            //}

//            //return config switch
//            //{
//            //    "1" => true,
//            //    "0" => false,
//            //    _ => defaultValue,
//            //};
//        }

//        [Obsolete("方法实现有问题")]
//        public string GetFileConfigString(string fileName, string defaultValue = "", bool isThrow = true)
//        {
//            if (isThrow && Configuration == null)
//                throw new NotImplementedException("Can't Get IConfiguration From DI Container.  ");
//            return Configuration.GetSection(fileName).Get<string>();
//            //var config = Configuration.GetSection(fileName).Value;
//            //return config;
//        }

//        [Obsolete("方法实现有问题")]
//        public T GetFileConfig<T>(string fileName, bool isThrow = true) where T : new()
//        {
//            if (isThrow && Configuration == null)
//                throw new NotImplementedException("Can't Get IConfiguration From DI Container.  ");
//            var configValue = Ioc.GetService<IOptionsMonitor<T>>().CurrentValue;
//            return configValue ?? Configuration.GetSection(fileName).Get<T>();
//        }
//    }


//}
