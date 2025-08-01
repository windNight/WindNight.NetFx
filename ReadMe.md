# Index

[TOC]
## Overview
WindNight.NetFx is an open-source .NET library designed to provide a robust foundation for building applications with Microsoft .NET technologies. It leverages modern .NET practices, including ASP.NET Core and dependency injection, to create scalable and maintainable applications. The library is hosted on GitHub and is licensed under the permissive MIT License, allowing flexible use in various projects. It has a low active ecosystem with minimal reported bugs and vulnerabilities, making it a reliable choice for developers.

## Features
- **ASP.NET Core Integration**: Utilizes Microsoft.AspNetCore.Hosting.WnExtensions for streamlined web hosting.
- **Configuration Management**: Supports flexible configuration through Microsoft.Extensions.Configuration, including JSON and environment variables.
- **Dependency Injection**: Leverages Microsoft.Extensions.DependencyInjection for service registration and management.
- **Logging Capabilities**: Includes a custom logging helper (`LogHelper`) for console-based log output with support for different log levels.
- **Background Services**: Supports hosted services, such as `TestBackgroundService`, for running background tasks.
- **Build Type Detection**: Automatically detects Debug or Release build types for conditional compilation.

## Getting Started

### Prerequisites

- .NET SDK (version compatible with ASP.NET Core)
- A development environment like Visual Studio or VS Code
- Basic understanding of .NET Core and dependency injection


### Installation

``` ps
dotnet add package WindNight.AspNetCore.Hosting --version xxxx
```

## xx的用法

```C#
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting.WnExtensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WindNight.Core.Abstractions;

namespace xxxx
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

        static IHostBuilder CreateHostBuilder(string buildType, string[] args)
        {
            return ProgramBase.CreateHostBuilderDefaults(buildType, args, configBuilder =>
            {
                configBuilder.SetBasePath(AppContext.BaseDirectory);
                configBuilder.AddEnvironmentVariables();
            }, configureServicesDelegate: (context, services) =>
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
            {
                LogHelper.RecordLog.WriteLog($"日志记录异常:【{logLevel}】{message} Exception: {exception.ToJsonStr()}");
            }
            Console.ForegroundColor = logLevel > LogLevels.Information ? ConsoleColor.Red : ConsoleColor.Green;
            Console.WriteLine(message);
        }
    }
}
```

## Configuration

