using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.WnExtensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System;
using WindNight.Core.Abstractions;
using static WindNight.LogExtension.LogHelper;

namespace GrpcDemo_Net5
{
    public class Program
    {
        private static void Main(string[] args)
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
                webBuilder =>
                {
                    webBuilder.ConfigureKestrel(opt =>
                    {

                    });
                    webBuilder.UseStartup<Startup>();
                },
                configureServicesDelegate: (context, services) =>
                {
                    var configuration = context.Configuration;
                    RegisterProcessEvent(Log2ConsolePublish);
                    Info("d");
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
