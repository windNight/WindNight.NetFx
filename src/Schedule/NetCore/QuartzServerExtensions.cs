using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.WnExtension;
using Schedule.Abstractions;
using Schedule.Ctrl;
using Schedule.@internal;
using WindNight.Linq.Extensions.Expressions;
using static Schedule.@internal.ConfigItems;

namespace Schedule.NetCore
{
    public static class QuartzServerExtensions
    {
        public static void UseQuartz(this IServiceCollection services, IConfiguration configuration, params Type[] jobs)
        {
            Ioc.Instance.InitServiceProvider(services);
            var jobConfigs = ConfigItems.JobsConfig;
            if (jobConfigs == null || jobConfigs.Items.IsNullOrEmpty())
            {
                //  configuration.GetSectionValue<JobsConfig>(ConfigItemsKey.ScheduleJobNodeName));
                services.Configure<JobsConfig>(configuration.GetSection(ConfigItemsKey.ScheduleJobNodeName));
                jobConfigs = configuration.GetSection(ConfigItemsKey.ScheduleJobNodeName).Get<JobsConfig>();
                if (jobConfigs == null || jobConfigs.Items.IsNullOrEmpty())
                {
                    throw new ArgumentNullException("配置 ScheduleJobs 不能为空！请注册节点【 ScheduleJobs】");
                }

                SetJobsConfig(jobConfigs);
            }

            services.AutoAddJobs(configuration);

            //  jobConfigs = ConfigItems.JobsConfig;
            //services.Add(jobs.Select(jobType => new ServiceDescriptor(typeof(IJob), jobType, ServiceLifetime.Singleton)));
            services.AddSingleton<IScheduleNotice, DefaultScheduleNotice>();
            services.AddSingleton<ICommandCtrl, ScheduleCtrl>();
            services.AddHostedService<ScheduleModBackgroundService>();
            Ioc.Instance.InitServiceProvider(services);
        }
    }
}
