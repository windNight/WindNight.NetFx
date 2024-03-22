using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.WnExtension;
using WindNight.Core.Abstractions;
using WindNight.RabbitMq.Abstractions;

namespace WindNight.RabbitMq.Internal;

internal static class ConfigItems
{
    public static bool IsCanLogDebug
    {
        get
        {
            if (RabbitMqOptions != null)
            {
                return RabbitMqOptions.CanLogDebug;
            }
            return GetAppSetting(ConstKey.IsCanLogDebugKey, "1", false) == "1";

        }
    }

    public static string SysAppId =>
        CurrentConfig.GetAppSetting("AppId", "");

    public static string SysAppCode =>
        CurrentConfig.GetAppSetting("AppCode", "");

    public static string SysAppName =>
        CurrentConfig.GetAppSetting("AppName", "");

    public static bool IsStopConsumer =>
        CurrentConfig.GetAppSetting($"IsStopConsumer", false, false);

    private static IConfiguration Configuration => Ioc.GetService<IConfiguration>();
    private static IConfigService CurrentConfig => Ioc.GetService<IConfigService>();

    public static RabbitMqConfig RabbitMqConfig
    {
        get
        {
            if (RabbitMqOptions != null)
            {
                return new RabbitMqConfig
                {
                    Items = RabbitMqOptions.Items,
                };
            }
            return GetFileConfig<RabbitMqConfig>(ConstKey.RabbitMqConfigFileNameKey);
        }
    }

    public static RabbitMqOptions RabbitMqOptions
    {
        get
        {
            if (Configuration == null) return null;
            var config = Configuration.GetSectionConfigValue<RabbitMqOptions>(nameof(RabbitMqOptions));
            return config;
        }
    }

    static T GetSectionConfigValue<T>(
        this IConfiguration configuration,
        string sectionKey,
        T defaultValue = default(T),
        bool isThrow = false)
    {
        if ((object)defaultValue == null)
            defaultValue = default(T);
        T obj = defaultValue;
        try
        {
            if (configuration == null)
                return defaultValue;
            obj = configuration.GetSection(sectionKey).Get<T>();
            return obj ?? defaultValue;
        }
        catch (Exception ex)
        {
            if (isThrow)
            {
                LogHelper.Error(
                    $"GetSection({(object)sectionKey}) configValue({(object)obj}) defaultValue({(object)defaultValue}) isThrow({(object)isThrow}) handler error {(object)ex.Message}", ex);
                throw;
            }
            else
                LogHelper.Warn(
                    $"GetSection({(object)sectionKey})  configValue({(object)obj}) defaultValue({(object)defaultValue}) isThrow({(object)isThrow})  handler error {(object)ex.Message}", ex);
        }
        return defaultValue;
    }

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