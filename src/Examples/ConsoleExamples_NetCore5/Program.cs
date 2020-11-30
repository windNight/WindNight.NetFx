using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting.WnExtensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json.Extension;
using WindNight.Core.Abstractions;
using WindNight.LogExtension;
 
namespace ConsoleExamples_NetCore5
{
    class Program
    {

        static void Main(string[] args)
        {
            var buildType = "";
#if DEBUG
            buildType = "Debug";
#else
            buildType = "Release";
#endif
            Init(CreateHostBuilder, buildType, args);
        }

        private static IHostBuilder CreateHostBuilder(string buildType, string[] args)
        {
            return ProgramBase.CreateHostBuilderDefaults(buildType, args, configBuilder =>
                 {
                     configBuilder.SetBasePath(AppContext.BaseDirectory);
                     configBuilder.AddEnvironmentVariables();
                 },
                configureServicesDelegate: (context, services) =>
                {
                    var configuration = context.Configuration;
                    services.AddHostedService<TestBackgroundService>();
                    LogHelper.RegisterProcessEvent(ConsolePublish);
                });

        }
        static void Init(Func<string, string[], IHostBuilder> createHostBuilder, string buildType, string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += ProgramBase.UnhandledExceptionEventHandler;
            TaskScheduler.UnobservedTaskException += ProgramBase.UnobservedTaskHandler;
            var host = CreateHostBuilder(createHostBuilder, buildType, args);
            host.Run();
            LogHelper.LogOfflineInfo(buildType, null, false);
        }
        public static IHost CreateHostBuilder(Func<string, string[], IHostBuilder> createHostBuilder, string buildType, string[] args)
        {
            var hostBuilder = createHostBuilder.Invoke(buildType, args);
            var host = hostBuilder.Build();
            return host;
        }
        static void ConsolePublish(LogHelper.LogInfo logInfo)
        {
            if (logInfo == null) return;
            var logLevel = logInfo.Level;
            var message = logInfo.Content;
            var exception = logInfo.Exceptions;

            if (logInfo.Level > LogLevels.Warning)
                LogHelper.RecordLog.WriteLog($"日志记录异常:【{logLevel}】{message} Exception: {exception.ToJsonStr()}");

            Console.ForegroundColor = logLevel > LogLevels.Information ? ConsoleColor.Red : ConsoleColor.Green;
            Console.WriteLine(
                $"ConsolePublish:【{logLevel}】{message} {(exception == null ? "" : $"Exception:{exception.ToJsonStr()}")} ");
            Console.ResetColor();
        }



    }

    public class TestBackgroundService : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                Console.WriteLine($"Hello I'm {nameof(TestBackgroundService)} {DateTime.Now:yyyy-MM-dd HH:mm:sss}");
                Thread.Sleep(1000 * 5);
            }
        }
    }
}
