using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.DependencyInjection.WnExtension;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json.Extension;
using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration.Json;
using WindNight.ConfigCenter.Extension;
using WindNight.Core.Abstractions;
using WindNight.LogExtension;
using static WindNight.LogExtension.LogHelper;

namespace ConsoleExamples_NetCore3_1
{
    class Program
    {
        static void Main(string[] args)
        {
            var host = Host.CreateDefaultBuilder(args)
                           .ConfigureAppConfiguration((hostBuilderContent, configBuilder) =>
                           {
                               var config = new
                               {
                                   AppSettings = new
                                   {
                                       AppId = 301,
                                       AppCode = "ConsoleTest3_1",
                                       AppName = "ConsoleTest3_1",
                                   },
                                   ConnectionStrings = new[]
                                   {
                                      new
                                   {
                                       name = "c1",
                                       connectionString = "#HOST#:#PORT#,password=#PASSWORD#,defaultDataBase=#DATABASE#,connectRetry=3,connectTimeout=10000,Prefix=xxx1"
                                   }, new
                                   {
                                       name = "c2",
                                       connectionString = "#HOST#:#PORT#,password=#PASSWORD#,defaultDataBase=#DATABASE#,connectRetry=3,connectTimeout=10000,Prefix=xxx2"
                                   }
                                   }
                               }.ToJsonStr();

                               var configStream = new MemoryStream(Encoding.UTF8.GetBytes(config));
                               configBuilder.SetBasePath(Environment.CurrentDirectory)
                                   .AddJsonStream(configStream)
                                   .AddJsonObject(new
                                   {
                                       AppSettings = new
                                       {
                                           AppId = 302,
                                           AppCode = "ConsoleTest3_2",
                                           AppName = "ConsoleTest3_2",
                                       },
                                   });
                           })
                           .ConfigureServices((hostContext, services) =>
                           {
                               services.AddOptions();
                               services.TryAddSingleton(hostContext.Configuration);
                               ConfigItems.ConfigCenterStart(logService: new DefaultLogService(), configuration: hostContext.Configuration);
                               services.AddSingleton<IConfigService, DefaultConfigService>();
                               services.AddSingleton<ILogService, DefaultLogService>();
                               services.AddHostedService<TestLogHostService>();

                               RegisterProcessEvent(ConsolePublish);

                           })
                    .Build();

            Ioc.Instance.InitServiceProvider(host.Services);
            Console.WriteLine($"Check All Config {ConfigItems.AllConfigs.ToJsonStr()}");
            host.Run();
        }
        public class ConfigItems : ConfigItemsBase
        {
            /// <summary>
            /// </summary>
            /// <param name="sleepTimeInMs"> default is 5000 ms </param>
            /// <param name="logService">   </param>
            /// <param name="configuration">   </param>
            public static void ConfigCenterStart(int sleepTimeInMs = 5 * 1000, ILogService logService = null
                , IConfiguration configuration = null
            )
            {
                StartConfigCenter(sleepTimeInMs, configuration: configuration);
            }

        }
        public class TestLogHostService : BackgroundService
        {
            protected override Task ExecuteAsync(CancellationToken stoppingToken)
            {
                LogRegisterInfo("Debug");
                Info($"Info   Log Test");
                Debug($"Debug Log Test");
                Warn($"Warn Log Test");
                Error($"Error Log Test", new Exception("Exception Message For Error Log Test"));
                Fatal($"Fatal Log Test", new Exception("Exception Message For Fatal Log Test"));

                return Task.CompletedTask;
            }
            /// <summary>
            /// Triggered when the application host is performing a graceful shutdown.
            /// </summary>
            /// <param name="cancellationToken">Indicates that the shutdown process should no longer be graceful.</param>
            public override Task StopAsync(CancellationToken cancellationToken)
            {
                LogOfflineInfo("Debug");
                ConfigItemsBase.StopConfigCenter();
                return Task.CompletedTask;

            }
        }

        static void ConsolePublish(LogInfo logInfo)
        {
            if (logInfo == null) return;
            var logLevel = logInfo.Level;
            var message = logInfo.Content;
            var exception = logInfo.Exceptions;

            if (logInfo.Level > LogLevels.Warning)
                RecordLog.WriteLog($"日志记录异常:【{logLevel}】{message} Exception: {exception.ToJsonStr()}");

            Console.ForegroundColor = logLevel > LogLevels.Information ? ConsoleColor.Red : ConsoleColor.Green;
            Console.WriteLine(
                $"ConsolePublish:【{logLevel}】{message} {(exception == null ? "" : $"Exception:{exception.ToJsonStr()}")} ");
            Console.ResetColor();
        }





    }

}
