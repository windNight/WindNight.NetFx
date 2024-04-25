using System;
using System.Collections.Generic;
using System.Linq;
using Schedule.Model;
using WindNight.Core.Abstractions;
using WindNight.Linq.Extensions.Expressions;

namespace Schedule
{
    public class JobsConfig
    {
        /// <summary> </summary>
        public List<JobMeta> Items { get; set; } = new List<JobMeta>();


        public LogLevels MinLogLevel { get; set; } = LogLevels.Debug;


        /// <summary> </summary>
        public NoticeDingConfig NoticeDingConfig { get; set; }

        /// <summary> </summary>
        public string JobInstanceName { get; set; } = "WindNight:ScheduleJob";

        /// <summary> </summary>
        public bool JobRemoteIsOpen { get; set; } = false;

        /// <summary> </summary>
        public JobRemotingConfig JobRemotingConfig { get; set; } = new JobRemotingConfig();



        public JobMeta FetchJobConfig(string jobCode)
        {
            if (Items.IsNullOrEmpty())
            {
                return null;
            }

            var config = Items.FirstOrDefault(m =>
                string.Equals(m.JobCode, jobCode, StringComparison.InvariantCultureIgnoreCase));

            return config;

        }
    }

    public class NoticeDingConfig
    {
        public string NoticeDingToken { get; set; } = "";
        public string NoticeDingPhones { get; set; } = "";
        public bool NoticeDingAtAll { get; set; } = false;
        public bool NoticeDingIsOpen { get; set; } = false;
    }
}