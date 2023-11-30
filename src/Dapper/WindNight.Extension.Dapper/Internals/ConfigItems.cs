using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.WnExtension;

namespace WindNight.Extension.Dapper.Internals
{
    internal class ConfigItems //: ConfigItemsBase
    {
        public static bool OpenDapperLog => GetConfigBoolValue(ConfigItemsKey.OpenDapperLogKey);
        //{
        //    get
        //    {
        //        var config = Ioc.Instance.CurrentConfigService;// Ioc.GetService<IConfigService>();
        //        if (config == null)
        //        {

        //            return false;
        //        }

        //        return config.Configuration.GetSection(ConfigItemsKey.OpenDapperLogKey).Get<bool>() ||
        //               config.GetAppSetting(ConfigItemsKey.OpenDapperLogKey, false, false);

        //    }
        //}

        public static bool IsLogConnectString => GetConfigBoolValue(ConfigItemsKey.IsLogConnectStringKey);
        public static int DapperWarnMs => GetConfigIntValue(ConfigItemsKey.DapperWarnMsKey, 100);
        //{
        //    get
        //    {
        //        return GetConfigBoolValue(ConfigItemsKey.IsLogConnectStringKey);
        //        // var config = Ioc.Instance.CurrentConfigService;
        //        // if (config == null) return false;
        //        // return config.GetAppSetting(ConfigItemsKey.IsLogConnectStringKey, false, false);
        //    }
        //}

        static bool GetConfigBoolValue(string key)
        {
            var config = Ioc.Instance.CurrentConfigService;
            if (config == null) return false;

            return config.Configuration.GetSection(key).Get<bool>() ||
                   config.GetAppSetting(key, false, false);

        }

        static int GetConfigIntValue(string key, int defaultValue = 0)
        {
            try
            {
                var config = Ioc.Instance.CurrentConfigService;
                if (config == null) return defaultValue;
                var value1 = config.Configuration.GetSection(key).Get<int>();
                var value2 = config.GetAppSetting(key, defaultValue, false);
                return Math.Max(value2, value1);
            }
            catch (Exception ex)
            {
                return defaultValue;
            }

        }

        static class ConfigItemsKey
        {
            internal static string DapperWarnMsKey = "DapperConfig:WarnMs";
            internal static string OpenDapperLogKey = "DapperConfig:OpenDapperLog";
            internal static string IsLogConnectStringKey = "DapperConfig:IsLogConnectString";

        }

    }
}
