using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.WnExtension;
using WindNight.Core.Abstractions;
using WindNight.Core.ConfigCenter.Extensions;
using WindNight.RabbitMq.Abstractions;

namespace WindNight.RabbitMq.@internal
{
    internal class ConfigItems : DefaultConfigItemBase
    {
        public static bool IsCanLogDebug
        {
            get
            {
                if (RabbitMqOptions != null)
                {
                    return RabbitMqOptions.CanLogDebug;
                }

                return GetAppSettingValue(ConstKey.IsCanLogDebugKey, false, false);
            }
        }



        public static bool IsStopConsumer =>
            GetAppSettingValue("IsStopConsumer", false, false);



        public static RabbitMqConfig RabbitMqConfig
        {
            get
            {
                if (RabbitMqOptions != null)
                {
                    return new RabbitMqConfig { Items = RabbitMqOptions.Items };
                }

                return GetFileConfig<RabbitMqConfig>(ConstKey.RabbitMqConfigFileNameKey);
            }
        }

        public static RabbitMqOptions RabbitMqOptions
        {
            get
            {
                // if (Configuration == null) return null;
                var config = GetSectionValue<RabbitMqOptions>(nameof(RabbitMqOptions));
                return config;
            }
        }






        //
        // 参数:
        //   fileName:
        //
        //   isThrow:
        //
        // 类型参数:
        //   T:
        private static T GetFileConfig<T>(string fileName, bool isThrow = true) where T : new()
        {
            var configService = Ioc.GetService<IConfigService>();
            if (configService == null) return default;
            return configService.GetFileConfig<T>(fileName, isThrow);
        }


        internal static class ConstKey
        {
            public static string IsCanLogDebugKey = "IsCanLogDebugKey";
            public static string RabbitMqConfigFileNameKey = "rabbitMqConfig.json";
        }
    }
}
