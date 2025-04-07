using Microsoft.Extensions.DependencyInjection.WnExtension;
using Newtonsoft.Json.Extension;
using WindNight.AspNetCore.Hosting;
using WindNight.ConfigCenter.Extension;
using WindNight.Core.Abstractions;
using WindNight.Core.ConfigCenter.Extensions;

namespace Net8ApiDemo
{

    public class Program : DefaultProgramBase
    {

        public static void Main(string[] args)
        {
            var buildType = "";
#if DEBUG
            buildType = "Debug";
#else
            buildType = "Release";
#endif
            Init(CreateHostBuilder, buildType, () =>
            {

                var config1 = Ioc.GetService<IConfiguration>();
                var config2 = Ioc.GetService<IConfigService>();
                var dapperConfig1 = config1.GetSectionValue<DapperConfig>();
                var dapperConfig2 = config2?.GetSectionValue<DapperConfig>();

                dapperConfig1.ToJsonStr().Log2Console(isForce: true);
                dapperConfig2.ToJsonStr().Log2Console(isForce: true);


            }, args);
        }

        public class DapperConfig
        {
            public bool OpenDapperLog { get; set; } = false;

            public bool IsLogConnectString { get; set; } = false;

            public long WarnMs { get; set; } = 500;
        }

        private static IHostBuilder CreateHostBuilder(string buildType, string[] args)
        {
            return CreateHostBuilderDefaults(buildType, args,
                    (hostingContext, configBuilder) =>
                    {
                        configBuilder.SetBasePath(AppContext.BaseDirectory)
                            .AddJsonFile("Config/AppSettings.json", false, true)
                            .AddJsonFile("Config/ConnectionStrings.json", false, true)
                            ;
                    },
                    webHostConfigure: webBuilder =>
                    {
                        webBuilder.UseStartup<Startup>();
                    },
                    configureServicesDelegate: (context, services) =>
                    {
                        ConfigItems.Init(configuration: context.Configuration, sleepTimeInMs: 10000000);
                    })

                ;


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



        public static void Main222(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args).ConfigureAppConfiguration((hostingContext, configBuilder) =>
                {
                    configBuilder.SetBasePath(AppContext.BaseDirectory)
                        .AddJsonFile("appsettings.json", false, true)
                        ;
                })
                .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); });



        public static void Main111(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();

            var app = builder.Build();

            // Configure the HTTP request pipeline.

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }


    }
}
