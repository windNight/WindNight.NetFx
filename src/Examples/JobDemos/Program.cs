using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using JobDemos.Jobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.DependencyInjection.WnExtension;
using Schedule.Model;
using Schedule;
using Schedule.Model.Enums;
using WindNight.Core.Abstractions;

namespace JobDemos
{
    class Program
    {

        static void Main(string[] args)
        {
            var jsonConfig = GetCustomConfig();
            var host =
                Host.CreateDefaultBuilder(args)
                    .ConfigureAppConfiguration((hostBuilderContent, configBuilder) =>
                    {
                        configBuilder.SetBasePath(Environment.CurrentDirectory)
                            .AddJsonObject(jsonConfig);
                    })
                    .ConfigureServices((hostContext, services) =>
                    {
                        services.AddOptions();
                        services.TryAddSingleton(hostContext.Configuration);
                        var configuration = hostContext.Configuration;
                        //  services.Configure<JobsConfig>(configuration.GetSection("ScheduleJobs"));
                        //  var ctScheduleJobConfigs = configuration.GetSection("ScheduleJobs").Get<JobsConfig>();
                        services.AddScheduleJobs(configuration);
                    })
                    .Build();
            Ioc.Instance.InitServiceProvider(host.Services);
            host.Run();

        }

        static object GetCustomConfig()
        {
            var jsonConfig = new
            {
                AppSettings = new
                {
                    JobExecuted = new
                    {
                        NoticeDingToken = "",
                    },
                },
                ScheduleJobs = new
                {
                    Items = new List<JobMeta>
                    {
                        new JobMeta
                        {
                            CronExpression= "0 0/3 * * * ?",
                            Description= "",
                            Interval= 0,
                            JobCode= "Demo1Job",
                            JobName= "Demo1任务",
                            StartTime= default,
                            State= JobStateEnum.Open,
                            IsDoNotice= true,
                        },  new JobMeta
                        {
                            CronExpression= "0 0/3 * * * ?",
                            Description= "",
                            Interval= 0,
                            JobCode= "Demo2Job",
                            JobName= "Demo2任务",
                            StartTime= default,
                            State= JobStateEnum.Open,
                            IsDoNotice= true,
                        },  new JobMeta
                        {
                            CronExpression= "0 0/1 * * * ?",
                            Description= "",
                            Interval= 0,
                            JobCode= "Demo3Job",
                            JobName= "Demo3任务",
                            StartTime= default,
                            State= JobStateEnum.Open,
                            IsDoNotice= true,
                        },
                    },
                    NoticeDingConfig = new
                    {
                        NoticeDingToken = "63c799ab6890b7c293ab32e94b862be21a961e4edb245f6187606e93cc39bcd1",
                        NoticeDingIsOpen = true,
                    },
                    MinLogLevel = LogLevels.Debug.ToString(),
                }
            };

            return jsonConfig;
        }

    }
}
