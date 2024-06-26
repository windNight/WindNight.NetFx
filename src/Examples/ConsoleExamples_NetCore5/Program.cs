﻿using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting.WnExtensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.WnExtension;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json.Extension;
using WindNight.Core.Abstractions;
using WindNight.LogExtension;

namespace ConsoleExamples_NetCore5
{



    class Program
    {


        static async Task Main(string[] args)
        {

            var buildType = "";
#if DEBUG
            buildType = "Debug";
#else
            buildType = "Release";
#endif
            await ProgramBase.InitAsync(CreateHostBuilder, buildType, args);
        }



        private static IHostBuilder CreateHostBuilder(string buildType, string[] args)
        {
            return ProgramBase.CreateHostBuilderDefaults(buildType, args,
                (hostContext, configBuilder) =>
                 {
                     configBuilder.SetBasePath(AppContext.BaseDirectory);
                     // 不支持动态更换 如果和json内key的重复了也不会动态更新
                     configBuilder.AddJsonObject(new
                     {
                         AppSettings = new
                         {
                             //AppId = 0,
                             //AppCode = "",
                             //AppName = "",
                             LogOnConsole = 1,
                             Log4netOpen = 1,
                         }
                     });

                     configBuilder.AddEnvironmentVariables();
                 },
                configureServicesDelegate: (context, services) =>
                {
                    var configuration = context.Configuration;
                    services.AddHostedService<TestBackgroundService>();
                    services.Configure<AppSettings>(configuration.GetSection($"{nameof(AppSettings)}"));
                    //   LogHelper.RegisterProcessEvent(ConsolePublish);
                });

        }


        //static void Init(Func<string, string[], IHostBuilder> createHostBuilder, string buildType, string[] args)
        //{
        //    AppDomain.CurrentDomain.UnhandledException += ProgramBase.UnhandledExceptionEventHandler;
        //    TaskScheduler.UnobservedTaskException += ProgramBase.UnobservedTaskHandler;
        //    var host = CreateHostBuilder(createHostBuilder, buildType, args);
        //    host.Run();
        //    LogHelper.LogOfflineInfo(buildType, null, false);
        //}
        //public static IHost CreateHostBuilder(Func<string, string[], IHostBuilder> createHostBuilder, string buildType, string[] args)
        //{
        //    var hostBuilder = createHostBuilder.Invoke(buildType, args);
        //    var host = hostBuilder.Build();
        //    return host;
        //}

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
                Console.WriteLine($"Hello I'm {nameof(TestBackgroundService)} {HardInfo.Now:yyyy-MM-dd HH:mm:sss}");
                Console.WriteLine($"AppSettings is {Ioc.GetService<IConfigService>()?.GetFileConfig<AppSettings>("AppSettings", false)}  {HardInfo.Now:yyyy-MM-dd HH:mm:sss}");
                Thread.Sleep(1000 * 5);



                await Task.CompletedTask;
            }
        }
    }

    public class AppSettings
    {
        public int AppId { get; set; }
        public string AppCode { get; set; }
        public string AppName { get; set; }
        public int LogOnConsole { get; set; }
        public int Log4netOpen { get; set; }
        public override string ToString()
        {
            return this.ToJsonStr();
        }
    }
}
