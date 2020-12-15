using System;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.WnExtension;
using Schedule.Abstractions;
using static Schedule.ConfigItems;

namespace Schedule.NetCore
{
    public static class QuartzServerExtensions
    {
        public static void UseQuartz(this IServiceCollection services, IConfiguration configuration, params Type[] jobs)
        {
            Ioc.Instance.InitServiceProvider(services.BuildServiceProvider());
            var jobConfigs = ConfigItems.JobsConfig;
            if (jobConfigs == null || !jobConfigs.Items.Any())
            {
                services.Configure<JobsConfig>(configuration.GetSection(ConfigItemsKey.ScheduleJobNodeName));
                jobConfigs = configuration.GetSection(ConfigItemsKey.ScheduleJobNodeName).Get<JobsConfig>();
                if (jobConfigs == null || !jobConfigs.Items.Any())
                    throw new ArgumentNullException($"配置 ScheduleJobs 不能为空！请注册节点【 ScheduleJobs】");
                SetJobsConfig(jobConfigs);
            }

            jobConfigs = ConfigItems.JobsConfig;
            //services.Add(jobs.Select(jobType => new ServiceDescriptor(typeof(IJob), jobType, ServiceLifetime.Singleton)));
            services.AddSingleton<IScheduleNotice, DefaultScheduleNotice>();
            services.AddHostedService<ScheduleModBackgroundService>();
            Ioc.Instance.InitServiceProvider(services.BuildServiceProvider());
        }
    }
}