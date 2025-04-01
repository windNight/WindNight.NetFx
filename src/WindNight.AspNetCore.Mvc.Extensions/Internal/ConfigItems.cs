using WindNight.Core.ConfigCenter.Extensions;

namespace Microsoft.AspNetCore.Mvc.WnExtensions.@internal
{
    internal class ConfigItems : DefaultConfigItemBase
    {
        private const int DEFAULT_API_WARNING_MIS = 200;



        internal static bool OpenInternalApi => GetAppSettingValue("OpenInternalApi", false, false);

        public static SwaggerConfigs? SwaggerConfigs => GetSectionValue<SwaggerConfigs>();

        /// <summary> 控制是否隐藏Swagger，默认为否 </summary>
        public static bool HiddenSwagger
        {
            get
            {
                if (SwaggerConfigs != null)
                {
                    return SwaggerConfigs.HiddenSwagger;
                }

                var configValue = GetConfigValue("AppSettings:HiddenSwagger", false, false);
                return configValue;
            }
        }

        /// <summary> 控制是否显示HiddenApi，默认为否 </summary>
        public static bool ShowHiddenApi
        {
            get
            {
                if (SwaggerConfigs != null)
                {
                    return SwaggerConfigs.ShowHiddenApi;
                }

                var configValue = GetConfigValue("AppSettings:ShowHiddenApi", false, false);

                return configValue;
            }
        }

        /// <summary> 控制是否隐藏Swagger Schemas，默认为否 </summary>

        public static bool HiddenSchemas
        {
            get
            {

                if (SwaggerConfigs != null)
                {
                    return SwaggerConfigs.HiddenSchemas;
                }

                var configValue = GetConfigValue("AppSettings:HiddenSchemas", false, false);

                return configValue;
            }
        }

        /// <summary> 控制是否显示TestApi，默认为否 </summary>
        public static bool ShowDebugApi
        {
            get
            {
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
                if (SwaggerConfigs != null)
                {
                    return SwaggerConfigs.ShowSysApi;
                }

                var configValue = GetConfigValue("AppSettings:ShowSysApi", false, false);
                return configValue;
            }
        }


        public static int ShowSysApiMiniLevel => SwaggerConfigs?.ShowSysApiMiniLevel ?? 0;
    }


    internal class SwaggerConfigs
    {
        public bool HiddenSwagger { get; set; } = true;

        public bool ShowHiddenApi { get; set; } = false;

        public bool ShowTestApi { get; set; } = false;

        public bool ShowDebugApi { get; set; } = false;

        public bool ShowSysApi { get; set; } = false;

        public int ShowSysApiMiniLevel { get; set; } = 0;
        public bool IsManageApp { get; set; } = false;

        public bool HiddenSchemas { get; set; } = false;

        public bool OpenSwaggerDebug { get; set; } = false;


    }

}
