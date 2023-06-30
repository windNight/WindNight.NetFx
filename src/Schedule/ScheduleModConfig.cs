using System.Collections.Generic;
using Quartz;
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
        internal string ConfigFilePath { set; get; }

        /// <summary>
        ///     所有job
        /// </summary>
        internal List<JobMeta> Jobs { set; get; }

        /// <summary>
        ///     default scheduler
        /// </summary>
        internal IScheduler DefaultScheduler { set; get; }
    }
}