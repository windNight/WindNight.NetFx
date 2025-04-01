using System.Collections.Generic;
using System.Collections.Specialized;
using System.Threading.Tasks;
using Quartz;
using Quartz.Impl;
using Schedule.Model;

namespace Schedule
{
    public class ScheduleModConfig
    {
        static ScheduleModConfig()
        {
            Instance = new ScheduleModConfig();
        }

        public static ScheduleModConfig Instance { get; }

        /// <summary>
        ///     配置文件所在目录
        /// </summary>
        internal string ConfigFilePath { get; private set; } = "";

        /// <summary>
        ///     所有job
        /// </summary>
        internal List<JobMeta> Jobs { get; set; } = new List<JobMeta>();

        /// <summary>
        ///     default scheduler
        /// </summary>
        internal IScheduler DefaultScheduler { get; private set; }

        internal IScheduler OnceJobScheduler { get; private set; }

        internal async Task InitDefaultSchedulerAsync(IScheduler scheduler)
        {
            DefaultScheduler = scheduler;
            await Task.CompletedTask;
        }

        internal void InitDefaultScheduler(IScheduler scheduler)
        {
            DefaultScheduler = scheduler;
        }

        internal async Task InitOnceJobSchedulerAsync()
        {
            if (OnceJobScheduler == null)
            {
                var properties = new NameValueCollection
                {
                    ["quartz.jobStore.misfireThreshold"] = (10 * 1000).ToString(), //修改misfire的时间为10秒 
                };
                OnceJobScheduler = await new StdSchedulerFactory(properties).GetScheduler();

            }
        }
        internal void InitOnceJobScheduler()
        {
            if (OnceJobScheduler == null)
            {
                var properties = new NameValueCollection
                {
                    ["quartz.jobStore.misfireThreshold"] = (10 * 1000).ToString(), //修改misfire的时间为10秒 
                };
                OnceJobScheduler = new StdSchedulerFactory(properties).GetScheduler().GetAwaiter().GetResult();

            }
        }
    }
}
