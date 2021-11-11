# Index

[TOC]

``` ps
dotnet add package WindNight.AspNetCore.Hosting --version 1.0.3.2
```

## xx的用法

```C#
using System; 
using System.Threading; 
using System.Threading.Tasks; 
using Microsoft.AspNetCore.Hosting.WnExtensions; 
using Microsoft.Extensions.Configuration; 
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.DependencyInjection; 
using Microsoft.Extensions.Hosting; 
using Newtonsoft.Json.Extension; 
using WindNight.Core.Abstractions; 
namespace xx{

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

        static IHostBuilder CreateHostBuilder(string buildType, string[] args)
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
```

## Thanks for the Rider IDE provided by JetBrains
[![](https://resources.jetbrains.com/storage/products/company/brand/logos/jb_beam.svg)](https://www.jetbrains.com/?from=WindNight.NetFx)
