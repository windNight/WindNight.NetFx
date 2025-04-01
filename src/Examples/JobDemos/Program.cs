using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using JobDemos.Jobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.DependencyInjection.WnExtension;
using Microsoft.Extensions.Hosting;
using Schedule;
using Schedule.Model;
using Schedule.Model.Enums;
using WindNight.ConfigCenter.Extension;
using WindNight.Core.Abstractions;

namespace JobDemos
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var jsonConfig = GetCustomConfig();
            var host =
                Host.CreateDefaultBuilder(args)
                    .ConfigureAppConfiguration((hostBuilderContent, configBuilder) =>
                    {
                        configBuilder.SetBasePath(Environment.CurrentDirectory)
                            .AddJsonObject(jsonConfig)
                            ;
                        configBuilder.AddInMemoryCollection(new Dictionary<string, string>());
                    })
                    .ConfigureServices((hostContext, services) =>
                    {
                        var configuration = hostContext.Configuration;
                        services.AddDefaultConfigService(configuration);

                        services.AddOptions();
                        services.TryAddSingleton(hostContext.Configuration);

                        services.Configure<JobsConfig>(configuration.GetSection("ScheduleJobs"));
                        //  services.AddScheduleJobs(configuration);
                        var scheduleJobConfigs = configuration.GetSection("ScheduleJobs").Get<JobsConfig>();
                        services.AddScheduleJobs(configuration);
                    })
                    .Build();
            Ioc.Instance.InitServiceProvider(host.Services);

            Task.Run(() =>
            {
                var ts = HardInfo.Now.Ticks;
                while (true)
                {
                    if (ScheduleModIniter.Instance.IsInit)
                    {
                        break;
                    }
                    Thread.Sleep(10);
                }
                var ttl = (long)TimeSpan.FromTicks(HardInfo.Now.Ticks - ts).TotalMicroseconds;
                $"ScheduleModIniter 耗时{ttl} ms".Log2Console(isForce: true);
                var allJobs = Ioc.GetServices<IJobBase>();
                var toTestJobs = allJobs.Where(m => m.CanRunTest).ToList();

                foreach (var job in toTestJobs)
                {
                    job.RunTestAtStart(5);
                    Thread.Sleep(1000);
                }
            });


            host.Run();
        }

        private static object GetCustomConfig()
        {
            var jsonConfig = new
            {
                AppSettings = new { JobExecuted = new { NoticeDingToken = "" } },
                ScheduleJobs = new
                {
                    Items = new List<JobMeta>
                    {
                        new()
                        {
                            CronExpression = "11 0/3 * * * ?",
                            Description = "",
                            Interval = 0,
                            JobCode = "Demo1Job",
                            JobName = "Demo1任务",
                            StartTime = default,
                            State = JobStateEnum.Open,
                            IsDoNotice = true,
                            CanRunTest = true,
                            SupportOnceJob = true,
                        },
                        new()
                        {
                            CronExpression = "1 0/5 * * * ?",
                            Description = "",
                            Interval = 0,
                            JobCode = "Demo2Job",
                            JobName = "Demo2任务",
                            StartTime = default,
                            State = JobStateEnum.Open,
                            IsDoNotice = true,
                            CanRunTest = true,
                            SupportOnceJob = true,
                        },
                        new()
                        {
                            CronExpression = "19 0/4 * * * ?",
                            Description = "",
                            Interval = 0,
                            JobCode = "Demo3Job",
                            JobName = "Demo3任务",
                            StartTime = default,
                            State = JobStateEnum.Open,
                            IsDoNotice = true,
                            CanRunTest = true,
                            SupportOnceJob = true,
                        },
                    },
                    NoticeDingConfig = new
                    {
                        NoticeDingToken = "",//""63c799ab6890b7c293ab32e94b862be21a961e4edb245f6187606e93cc39bcd1",
                        NoticeDingIsOpen = false,
                    },
                    MiniLogLevel = LogLevels.Debug.ToString(),
                },
            };

            return jsonConfig;
        }
    }

    public static class TT
    {
        public static IServiceCollection AddConfig<T>(this IServiceCollection services, IConfiguration configuration)
            where T : class, new()

        {
            services.Configure<T>(configuration.GetSection(nameof(T)));

            return services;
        }
    }
}
