using JobDemos.Jobs.Demo1;
using JobDemos.Jobs.Demo2;
using JobDemos.Jobs.Demo3;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Schedule.Abstractions;
using Schedule.NetCore;

namespace JobDemos.Jobs
{
    public static class JobExtensions
    {
        public static IServiceCollection AddScheduleJobs(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IJobCtrl, Demo1JobCtrl>();
            services.AddSingleton<IScheduleListener, Demo1JobScheduleListener>();

            services.AddSingleton<IJobCtrl, Demo2JobCtrl>();
            services.AddSingleton<IScheduleListener, Demo2JobScheduleListener>();

            services.AddSingleton<IJobCtrl, Demo3JobCtrl>();
            services.AddSingleton<IScheduleListener, Demo3JobScheduleListener>();
            services.UseQuartz(configuration);

            return services;
        }
    }
}
