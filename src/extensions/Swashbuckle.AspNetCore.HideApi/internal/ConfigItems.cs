using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Swashbuckle.AspNetCore.Extensions.Abstractions;
using WindNight.Core.ConfigCenter.Extensions;
using MJsonIgnore = System.Text.Json.Serialization.JsonIgnoreAttribute;
using NJsonIgnore = Newtonsoft.Json.JsonIgnoreAttribute;

namespace Swashbuckle.AspNetCore.Extensions.@internal
{
    internal class SwaggerConfigs
    {
        public bool ShowHiddenApi { get; set; } = false;
        public bool HiddenSwagger { get; set; } = true;

        [NJsonIgnore, MJsonIgnore]
        public bool HiddenSchemas { get; set; } = false;
        public bool ShowTestApi { get; set; } = false;
        public bool ShowSysApi { get; set; } = false;

        [NJsonIgnore, MJsonIgnore]
        public bool IsManageApp { get; set; } = false;

        public int ShowSysApiMiniLevel { get; set; } = 0;

        public bool OpenSwaggerDebug { get; set; } = false;

        [NJsonIgnore, MJsonIgnore]
        public List<SwaggerSignConfig> SignConfigs { get; set; } = new List<SwaggerSignConfig>();

        [NJsonIgnore, MJsonIgnore]
        public Dictionary<string, string> ResConfigs { get; set; } = new Dictionary<string, string>();

        public Dictionary<string, string> GetSignDict()
        {
            var dict = new Dictionary<string, string>();
            foreach (var signConfig in SignConfigs)
            {
                var key = signConfig.Key;
                if (!dict.ContainsKey(key))
                {
                    dict[key] = signConfig.Name;
                }
            }
            return dict;
        }

        public bool CheckClientIp { get; set; } = true;

        public List<string> LimitIps { get; set; } = new List<string>();

        //public Dictionary<string, string> GetResDict()
        //{
        //    var dict = new Dictionary<string, string>();
        //    foreach (var item in ResConfigs)
        //    {
        //        var key = item.Key;
        //        if (!dict.ContainsKey(key))
        //        {
        //            dict[key] = item.Name;
        //        }
        //    }
        //    return dict;
        //}


    }

    internal class SwaggerResConfig
    {
        public string Key { get; set; } = "";
        public string Name { get; set; } = "";
    }

    internal class SwaggerSignConfig
    {
        public string Key { get; set; } = "";
        public string Name { get; set; } = "";
        public bool AutoFill { get; set; } = false;

        public override string ToString()
        {
            return $"{Key}:{Name}";
        }
    }


    internal class ConfigItems : DefaultConfigItemBase
    {
        /// <summary> 控制是否显示HiddenApi，默认为否 </summary>
        public static bool ShowHiddenApi
        {
            get
            {
                if (SwaggerConfigImpl != null)
                {
                    return SwaggerConfigImpl.GetShowHiddenApiConfig();
                }

                if (SwaggerConfigs != null)
                {
                    return SwaggerConfigs.ShowHiddenApi;
                }

                var configValue = GetConfigValue("AppSettings:ShowHiddenApi", false, false);

                return configValue;
            }
        }

        /// <summary> 控制是否隐藏Swagger，默认为否 </summary>
        public static bool HiddenSwagger
        {
            get
            {
                if (SwaggerConfigImpl != null)
                {
                    return SwaggerConfigImpl.GetHiddenSwaggerConfig();
                }

                if (SwaggerConfigs != null)
                {
                    return SwaggerConfigs.HiddenSwagger;
                }

                var configValue = GetConfigValue("AppSettings:HiddenSwagger", false, false);
                return configValue;
            }
        }

        /// <summary> 控制是否隐藏Swagger Schemas，默认为否 </summary>

        public static bool HiddenSchemas
        {
            get
            {
                if (SwaggerConfigImpl != null)
                {
                    return SwaggerConfigImpl.GetHiddenSchemasConfig();
                }

                if (SwaggerConfigs != null)
                {
                    return SwaggerConfigs.HiddenSchemas;
                }

                var configValue = GetConfigValue("AppSettings:HiddenSchemas", false, false);

                return configValue;
            }
        }

        /// <summary> 控制是否显示DebugApi，默认为否 </summary>
        public static bool ShowDebugApi
        {
            get
            {
                if (SwaggerConfigImpl != null)
                {
                    return SwaggerConfigImpl.GetShowTestApiConfig();
                }

                if (SwaggerConfigs != null)
                {
                    return SwaggerConfigs.ShowTestApi;
                }

                var configValue = GetConfigValue("AppSettings:ShowTestApi", false, false);
                return configValue;
            }
        }

        /// <summary> 控制是否显示SysApi，默认为否 </summary>
        public static bool ShowSysApi
        {
            get
            {
                if (SwaggerConfigImpl != null)
                {
                    return SwaggerConfigImpl.GetShowTestApiConfig();
                }

                if (SwaggerConfigs != null)
                {
                    return SwaggerConfigs.ShowSysApi;
                }

                var configValue = GetConfigValue("AppSettings:ShowSysApi", false, false);
                return configValue;
            }
        }

        public static int ShowSysApiMiniLevel => SwaggerConfigs?.ShowSysApiMiniLevel ?? 0;
        public static bool OpenSwaggerDebug => SwaggerConfigs?.OpenSwaggerDebug ?? false;

        public static string EnvName =>
            GetAppSettingValue("EnvName", "online", false);

        public static bool SwaggerOnlineDebug =>
            GetAppSettingValue("SwaggerOnlineDebug", false, false);

        public static bool SwaggerCanDebug => (IsOnline && SwaggerOnlineDebug) || !IsOnline;

        public static bool IsOnline =>
            EnvName.IsNullOrEmpty() || EnvName.Equals("pre", StringComparison.OrdinalIgnoreCase) || EnvName.Equals("online", StringComparison.OrdinalIgnoreCase) || EnvName.Equals("production", StringComparison.OrdinalIgnoreCase);


        public static List<string> LimitIps => SwaggerConfigs?.LimitIps ?? new List<string>();

        public static bool CheckClientIp => SwaggerConfigs?.CheckClientIp ?? true;

        public static SwaggerConfigs SwaggerConfigs => GetSectionValue<SwaggerConfigs>();

        public static List<SwaggerSignConfig> SwaggerSignConfigs => SwaggerConfigs?.SignConfigs ?? new List<SwaggerSignConfig>();


        /// <summary>
        ///     初始化 ISwaggerConfig
        /// </summary>
        /// <param name="swaggerConfig"></param>
        public static void InitSwaggerConfig(ISwaggerConfig swaggerConfig)
        {
            SwaggerConfigImpl = swaggerConfig;
        }



        /// <summary> 优先级高于配置 </summary>
        protected static ISwaggerConfig SwaggerConfigImpl { get; set; }

    }


}
