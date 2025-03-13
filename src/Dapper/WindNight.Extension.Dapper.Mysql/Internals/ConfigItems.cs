using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.WnExtension;

namespace WindNight.Extension.Dapper.Mysql.@internal
{
    internal class ConfigItems //: ConfigItemsBase
    {
        public static bool OpenDapperLog => GetConfigBoolValue(ConfigItemsKey.OpenDapperLogKey);


        public static bool IsLogConnectString => GetConfigBoolValue(ConfigItemsKey.IsLogConnectStringKey);

        public static long DapperWarnMs => GetConfigIntValue(ConfigItemsKey.DapperWarnMsKey, 100L);


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
        static string GetConfigIntValue(string key, string defaultValue = "")
        {
            try
            {
                var config = Ioc.Instance.CurrentConfigService;
                if (config == null)
                {
                    return defaultValue;
                }
                var value1 = config.Configuration.GetSection(key).Get<string>();
                if (!value1.IsNullOrEmpty())
                {
                    return value1;
                }
                var value2 = config.GetAppSetting(key, defaultValue, false);
                if (!value2.IsNullOrEmpty())
                {
                    return value2;
                }
                return defaultValue;
            }
            catch (Exception ex)
            {
                return defaultValue;
            }

        }


        static long GetConfigIntValue(string key, long defaultValue = 0L)
        {
            try
            {
                var config = Ioc.Instance.CurrentConfigService;
                if (config == null) return defaultValue;
                var value1 = config.Configuration.GetSection(key).Get<long>();
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
