using Microsoft.Extensions.DependencyInjection.WnExtension;
using WindNight.Core.Abstractions;
using WindNight.RabbitMq.Abstractions;

namespace WindNight.RabbitMq.Internal;

internal static class ConfigItems
{
    public static bool IsCanLogDebug => GetAppSetting(ConstKey.IsCanLogDebugKey, "1", false) == "1";

    public static RabbitMqConfig RabbitMqConfig =>
        GetFileConfig<RabbitMqConfig>(ConstKey.RabbitMqConfigFileNameKey);


    private static string GetAppSetting(string configKey, string defaultValue = "", bool isThrow = true)
    {
        var configService = Ioc.GetService<IConfigService>();
        if (configService == null) return defaultValue;
        return configService.GetAppSetting(configKey, defaultValue, isThrow);
    }


    //
    // 参数:
    //   connKey:
    //
    //   defaultValue:
    //
    //   isThrow:
    private static string GetConnString(string connKey, string defaultValue = "", bool isThrow = true)
    {
        var configService = Ioc.GetService<IConfigService>();
        if (configService == null) return defaultValue;
        return configService.GetConnString(connKey, defaultValue, isThrow);
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

    //
    // 参数:
    //   fileName:
    //
    //   defaultValue:
    //
    //   isThrow:
    private static string GetFileConfigString(string fileName, string defaultValue = "", bool isThrow = true)
    {
        var configService = Ioc.GetService<IConfigService>();
        if (configService == null) return defaultValue;
        return configService.GetFileConfigString(fileName, defaultValue, isThrow);
    }

    internal static class ConstKey
    {
        public static string IsCanLogDebugKey = "IsCanLogDebugKey";
        public static string RabbitMqConfigFileNameKey = "rabbitMqConfig.json";
    }
}