using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Threading;
using System.Threading.Tasks;
using JobDemos.Jobs;
using JobDemos.Jobs.Demo1;
using JobDemos.Jobs.Demo2;
using JobDemos.Jobs.Demo3;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.DependencyInjection.WnExtension;
using Microsoft.Extensions.Hosting;
using Schedule;
using Schedule.Abstractions;
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

                        services.AddSingleton<IScheduleOrderCtrl, EnScheduleOrderCtrl>();
                        services.Configure<JobsConfig>(configuration.GetSection("ScheduleJobs"));
                        //  services.AddScheduleJobs(configuration);
                        var scheduleJobConfigs = configuration.GetSection("ScheduleJobs").Get<JobsConfig>();
                        services.AddScheduleJobs(configuration);
                    })
                    .Build();

            Ioc.Instance.InitServiceProvider(host.Services);

            //Task.Run(() =>
            //{
            //    var ts = HardInfo.Now.Ticks;
            //    while (true)
            //    {
            //        if (ScheduleModIniter.Instance.IsInit)
            //        {
            //            break;
            //        }
            //        Thread.Sleep(10);
            //    }
            //    var ttl = (long)TimeSpan.FromTicks(HardInfo.Now.Ticks - ts).TotalMicroseconds;
            //    $"ScheduleModIniter 耗时{ttl} ms".Log2Console(isForce: true);
            //    var allJobs = Ioc.GetServices<IJobBase>();
            //    var toTestJobs = allJobs.Where(m => m.CanRunTest).ToList();

            //    foreach (var job in toTestJobs)
            //    {
            //        job.RunTestAtStart(5);
            //        Thread.Sleep(1000);
            //    }
            //});


            host.Run();
        }

        private static object GetCustomConfig()
        {
            var jsonConfig = new
            {
                AppSettings = new
                {
                    LogOnConsole = true,
                    JobExecuted = new
                    {
                        NoticeDingToken = "",
                    },
                },
                ScheduleJobs = new
                {
                    Items = new List<JobMeta>
                    {
                        new()
                        {
                            CronExpression = "01 0/1 * * * ?",
                            Description = "",
                            Interval = 0,
                            JobCode = nameof(Demo1Job),
                            JobName = "Demo1任务",
                            StartTime = default,
                            State = JobStateEnum.Open,
                            IsDoNotice = true,
                            CanRunTest = true,
                            SupportOnceJob = true,
                        },
                        new()
                        {
                            CronExpression = "11 0/1 * * * ?",
                            Description = "",
                            Interval = 0,
                            JobCode = nameof(Demo2Job),
                            JobName = "Demo2任务",
                            StartTime = default,
                            State = JobStateEnum.Open,
                            IsDoNotice = true,
                            CanRunTest = true,
                            SupportOnceJob = true,
                        },
                        //new()
                        //{
                        //    CronExpression = "21 0/1 * * * ?",
                        //    Description = "",
                        //    Interval = 0,
                        //    JobCode = nameof(Demo3Job),
                        //    JobName = "Demo3任务",
                        //    StartTime = default,
                        //    State = JobStateEnum.Open,
                        //    IsDoNotice = true,
                        //    CanRunTest = true,
                        //    SupportOnceJob = true,
                        //},
                    },
                    NoticeDingConfig = new
                    {
                        NoticeDingToken = "",
                        NoticeDingSignKey = "",
                        Keywords = "",
                        NoticeDingIsOpen = false,
                    },
                    MiniLogLevel = nameof(LogLevels.Debug),
                },
            };

            return jsonConfig;
        }
    }

    internal class EnScheduleOrderCtrl : IScheduleOrderCtrl
    {

        public async Task<JobRunStateEnum> WaitJobCompleted(string sourceJob, List<string> jobNames, DateTime jobStartTime)
        {
            $"sourceJob({sourceJob}):[{jobNames.Join(",")}]".Log2Console();
            return await Task.FromResult(JobRunStateEnum.Ok);
        }

        public async Task StartJobSafety(string jobId, string jobName, string jobCode, string runParams, bool onceJob)
        {
            $"jobId({jobId}):jobName({jobName}):jobCode({jobCode}):runParams({runParams}):onceJob({onceJob})".Log2Console();

            await Task.CompletedTask;
        }


        public async Task<bool> CompleteJobSafety(string jobId, JobRunStateEnum jobRunState, string bizContent = "")
        {
            $"jobId({jobId}):jobRunState({jobRunState}):bizContent({bizContent})".Log2Console();

            return await Task.FromResult(true);
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
