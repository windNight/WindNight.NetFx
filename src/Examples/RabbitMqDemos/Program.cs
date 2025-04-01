using Microsoft.AspNetCore.Hosting.WnExtensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.WnExtension;
using Microsoft.Extensions.Hosting;
using WindNight.ConfigCenter.Extension;
using WindNight.Core.Abstractions;
using WindNight.Extension;
using WindNight.RabbitMq;
using WindNight.RabbitMq.Abstractions;

namespace RabbitMqDemos
{


    internal class Program
    {
        private static void Main(string[] args)
        {
            ProgramBase.Init(CreateHostBuilder, () =>
            {
                var buildType = string.Empty;
#if DEBUG
                buildType = "Debug";
#else
                buildType = "Release";
#endif
                return buildType;
            }, TestCode, args);
        }

        private static void TestCode()
        {
            Console.WriteLine("testcode hear");
#if DEBUG
            var producer = Ioc.GetService<ITestProducerService>();
            var config = producer.CurrentProducerSettings;
            var co = Ioc.GetService<IConfiguration>();
            var dd = co.GetSection("RabbitMqOptions").Get<RabbitMqOptions>();
            dd.Log2Console();
#endif
        }


        private static IHostBuilder CreateHostBuilder(string buildType, string[] args)
        {
            return ProgramBase.CreateHostBuilderDefaults(buildType,
                configBuilder =>
                {
                    configBuilder.SetBasePath(AppContext.BaseDirectory)
                        .AddJsonFile("Config/AppSettings.json", false, true)
                        //.AddJsonFile("Config/rabbitMqConfig.json", false, true)
                        .AddJsonFile("Config/rabbitMqConfigV2.json", false, true)
                        ;
                },
                configureServicesDelegate: (context, services) =>
                {
                    var configuration = context.Configuration;
                    services.AddConfigService(context.Configuration);
                    services.AddDefaultLogService(configuration);

                    services.AddRabbitMqConsumer(configuration);
                    services.AddHostedService<TestConsumerBackgroundService>();

                    services.AddRabbitMqProducer(configuration);



                    services.AddSingleton<ITestProducerService, TestProducerService>();

                    services.AddHostedService<TestProducerBackgroundService1>();
                    services.AddHostedService<TestProducerBackgroundService2>();
                    services.AddHostedService<TestProducerBackgroundService>();

                });
        }
    }

    public static class ConfigExtensionServer
    {
        public static IServiceCollection AddConfigService(this IServiceCollection services, IConfiguration configuration)
        {
            ConfigItems.Init();
            services.AddDefaultConfigService(configuration);

            return services;
        }
    }

    public class ConfigItems : ConfigItemsBase
    {
        public static void Init(int sleepTimeInMs = 5000,
            ILogService? logService = null,
            IConfiguration? configuration = null)
        {
            StartConfigCenter(sleepTimeInMs, logService, configuration);
        }
    }
}
