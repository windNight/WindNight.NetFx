//// See https://aka.ms/new-console-template for more information

using System.Text;
using Microsoft.AspNetCore.Hosting.WnExtensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using WindNight.LogExtension;

Console.WriteLine("Hello, World!");
Console.WriteLine(randomString(-229985452) + " " + randomString(-147909649));
var buildType = "";
#if DEBUG
buildType = "Debug";
#else
            buildType = "Release";
#endif

ProgramBase.Init(CreateHostBuilder, buildType, args);

string randomString(int i)
{
    var ran = new Random(i);
    StringBuilder sb = new StringBuilder();
    while (true)
    {
        int k = ran.Next(27);
        if (k == 0)
            break;

        sb.Append((char)('`' + k));
    }

    return sb.ToString();
}

new Random().Next(-229985452);

static IHostBuilder CreateHostBuilder(string buildType, string[] args)
{
    return ProgramBase.CreateHostBuilderDefaults(buildType, args,
        configBuilder =>
        {
            configBuilder.SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", false, true)
                ;
        },
        configureServicesDelegate: (context, services) =>
        {
            var configuration = context.Configuration;
            // services.AddCtLogService(configuration);
            // RegisterProcessEvent(Log2ConsolePublish);
            LogHelper.Info("d");
        });
}

//Console.WriteLine("Hello, World!");
////if (args.Length > 0)
////{
////    foreach (var arg in args)
////    {
////        Console.WriteLine($"Argument={arg}");
////    }
////}
////await Task.Delay(5000);
//Console.ReadLine();
//namespace MyApp // Note: actual namespace depends on the project name.
//{
//    using System;
//    internal class Program
//    {
//        static void Main(string[] args)
//        {
//            Console.WriteLine("Hello World333!");
//            Console.ReadLine();
//        }
//    }
//}
