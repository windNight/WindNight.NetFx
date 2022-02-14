using System.Collections.Generic;
using Schedule.Model;
using WindNight.Core.Abstractions;

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
    }

    public class NoticeDingConfig
    {
        public string NoticeDingToken { get; set; } = "";
        public string NoticeDingPhones { get; set; } = "";
        public bool NoticeDingAtAll { get; set; } = false;
        public bool NoticeDingIsOpen { get; set; } = false;
    }
}