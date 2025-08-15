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
            services.AddTransient<IJobCtrl, Demo1JobCtrl>();
            services.AddTransient<IScheduleListener, Demo1JobScheduleListener>();

            services.AddTransient<IJobCtrl, Demo2JobCtrl>();
            services.AddTransient<IScheduleListener, Demo2JobScheduleListener>();

            services.AddTransient<IJobCtrl, Demo3JobCtrl>();
            services.AddTransient<IScheduleListener, Demo3JobScheduleListener>();
            services.UseQuartz(configuration);

            return services;
        }
    }
}
