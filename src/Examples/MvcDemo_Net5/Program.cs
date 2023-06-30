using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.WnExtensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WindNight.Core.Abstractions;
using WindNight.LogExtension;
using static WindNight.LogExtension.LogHelper;

namespace MvcDemo_Net5
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var buildType = "";
#if DEBUG
            buildType = "Debug";
#else
            buildType = "Release";
#endif
            ProgramBase.Init(CreateHostBuilder, buildType, args);
        }

        private static IHostBuilder CreateHostBuilder(string buildType, string[] args)
        {
            return ProgramBase.CreateHostBuilderDefaults(buildType, args,
                configBuilder =>
                {
                    configBuilder.SetBasePath(AppContext.BaseDirectory)
                        .AddJsonFile("appsettings.json", false, true)
                        ;
                },
                webHostConfigure: webBuilder => { webBuilder.UseStartup<Startup>(); },
                configureServicesDelegate: (context, services) =>
                {
                    var configuration = context.Configuration;
                    // services.AddCtLogService(configuration);
                    RegisterProcessEvent(Log2ConsolePublish);
                    LogHelper.Info("d");
                });
        }

        private static void Log2ConsolePublish(LogInfo logInfo)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            if (logInfo.Level > LogLevels.Information) Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(
                $"¡¾{logInfo.Level}¡¿:Console.WriteLine-> {logInfo}");
            Console.ResetColor();
        }
    }
}
