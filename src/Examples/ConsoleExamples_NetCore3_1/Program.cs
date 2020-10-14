using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.DependencyInjection.WnExtension;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json.Extension;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration.Json;
using WindNight.ConfigCenter.Extension;
using WindNight.Core.Abstractions;
using WindNight.LogExtension;
using static WindNight.LogExtension.LogHelper;
using WindNight.ParserIp.Core;


namespace ConsoleExamples_NetCore3_1
{
    class Program
    {

        static string GetRandomIP()
        {
            var intList = GeneratorIntList(4);
            return string.Join(".", intList);

            //    new Random(Guid.NewGuid().GetHashCode()).Next(0, 255).ToString() + "."
            //  + new Random(Guid.NewGuid().GetHashCode()).Next(0, 255).ToString() + "."
            //  + new Random(Guid.NewGuid().GetHashCode()).Next(0, 255).ToString() + "."
            //  + new Random(Guid.NewGuid().GetHashCode()).Next(0, 255).ToString();

        }

        static List<int> GeneratorIntList(int length = 0)
        {
            var intList = new List<int>();
            if (length == 0) return intList;
            for (int i = 0; i < length; i++)
            {
                intList.Add(new Random(Guid.NewGuid().GetHashCode()).Next(0, 255));
            }
            return intList;
        }

        static void Main(string[] args)
        {

            var ips = new[] { "223.104.160.19", "81.68.163.50", "223.104.148.214", "151.20.45.131", "175.223.34.161", 
                "223.104.159.182", "223.104.158.86", "39.144.4.106", "223.104.160.34", "112.51.176.227", "223.104.148.12", 
                "223.104.47.38", "112.51.135.152", "220.195.70.63", "223.104.148.28", "117.136.120.176", "103.1.30.156", 
                "116.138.201.49", "116.162.3.219", "223.104.92.103", "223.104.160.102", "223.24.62.186", "220.195.69.36", 
                "223.104.159.202", "39.144.27.124", "223.104.148.41", "223.104.160.86", "223.104.48.240", "116.136.147.69",
                "116.179.211.104", "223.104.48.158", "223.104.159.186", "68.61.154.191", "223.104.159.166", "223.104.92.243",
                "36.148.71.159", "220.195.71.5", "223.104.159.250", "223.104.47.68", "223.104.160.22", "111.18.180.110", 
                "103.116.226.183", "223.104.148.26", "112.51.128.84", "220.195.71.34", "223.104.160.88", "223.104.92.71",
                "39.144.34.238", "223.104.160.64", "39.144.34.220", "39.144.34.176", "36.154.87.153", "36.148.77.90", 
                "112.51.177.66", "116.179.236.16", "223.104.148.8", "120.230.209.174", "103.57.12.39", "116.179.192.11",
                "84.54.86.126", "220.195.71.3", "220.195.69.107", "223.104.159.254", "223.104.159.196", "220.195.70.211",
                "223.104.148.17", "223.104.159.152", "39.144.34.180", "223.104.47.122", "103.98.240.209", "223.104.148.224",
                "36.128.50.232", "36.170.9.94", "220.195.69.102", "223.104.160.120", "223.104.159.201", "223.104.148.64", 
                "39.144.49.184", "223.104.159.252", "223.104.93.28", "223.104.159.147", "223.104.47.238", "60.232.199.220", 
                "223.104.47.5", "39.144.34.13", "223.104.160.87", "58.96.35.142", "223.104.48.122", "36.148.64.90", "103.98.240.76",
                "120.231.37.154", "79.42.108.228", "103.98.240.22", "103.78.124.0", "223.104.160.18", "223.104.148.21",
                "39.144.49.153", "223.104.47.162", "103.98.240.122", "223.104.158.38", "39.144.22.41", "223.104.92.87", 
                "103.57.12.123", "223.104.160.100", "39.144.24.211", "223.104.47.216", "39.144.35.120", "223.104.159.206", 
                "116.136.147.42", "223.104.158.13", "116.136.147.18", "220.195.69.158", "223.104.159.145", "39.144.34.132",
                "220.195.71.192", "106.101.65.16", "36.154.87.174", "223.104.160.115", "223.104.160.23", "60.140.140.142", 
                "103.193.194.144", "223.104.159.234", "223.104.100.1", "36.154.100.27", "223.104.148.25", "223.104.159.211", 
                "223.104.92.63", "39.144.34.29", "223.104.160.122", "223.104.160.2", "112.51.135.168", "77.162.177.34", 
                "39.144.17.228", "36.148.103.137", "223.104.104.104", "117.20.113.66", "223.104.158.98", "39.144.13.109", 
                "223.104.159.137", "103.98.241.193", "223.104.48.163", "39.144.34.213"

            };

            // var _search = new DbSearcher(Path.Combine(Environment.CurrentDirectory, "db", "ip2region.db"));
            //var ip = GetRandomIP();
            //// var btreeIpInfo = _search.BtreeSearch(ip);
            //// Console.WriteLine($"ip is {ip} ipInfo use BtreeSearch is {btreeIpInfo.ToJsonStr()}");
            ////  ConfigTest(args);

            // IpToolSettings.ChinaDbPath = Path.Combine(Environment.CurrentDirectory, "db", "ip2region.db");
            //IpToolSettings.LoadInternationalDbToMemory = true;
            //Console.WriteLine($"Default Searcher：IpTool.Search({ip}).City=" + IpTool.Search(ip).City);

            //Console.WriteLine($"IPTools.International  IpTool.IpAllSearcher.Search({ip}).City=" + IpTool.IpAllSearcher?.Search(ip)?.City);
            //Console.WriteLine($"IPTools.International Searcher with i18n： IpTool.IpAllSearcher.SearchWithI18N({ip}).City=" + IpTool.IpAllSearcher?.SearchWithI18N(ip)?.City);

            //Console.WriteLine($"IPTools.China Searcher： IpTool.IpChinaSearcher.Search({ip}).City=" + IpTool.IpChinaSearcher.Search(ip).City);

            foreach (var ip in ips)
            {
                var ipInfo = IpHelper.Parser(ip).ToJsonStr();
                if (ipInfo.IndexOf("中国", StringComparison.OrdinalIgnoreCase) == -1)
                    Console.WriteLine($"{ip} \r\n {ipInfo}");
            }
            //for (int i = 0; i < 255; i++)
            //{

            //    for (int j = 0; j < 255; j++)
            //    {
            //        var info = IpTool.Search($"171.{i}.{j}.163");
            //    }
            //}

            //for (int i = 0; i < 255; i++)
            //{

            //    for (int j = 0; j < 255; j++)
            //    {
            //        var info = IpTool.IpAllSearcher.Search($"171.{i}.{j}.163");
            //    }
            //}

            Console.ReadLine();

        }

        static void ConfigTest(string[] args)
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
