using System.Collections.Generic;
using Schedule.Model;
using WindNight.Core.Abstractions;

namespace Schedule
{
    public class JobsConfig
    {
        public List<JobMeta> Items { get; set; } = new List<JobMeta>();
        public LogLevels MinLogLevel { get; set; } = LogLevels.Debug;
        public NoticeDingConfig NoticeDingConfig { get; set; }
    }

    public class NoticeDingConfig
    {
        public string NoticeDingToken { get; set; } = "";
        public string NoticeDingPhones { get; set; } = "";
        public bool NoticeDingAtAll { get; set; } = false;
        public bool NoticeDingIsOpen { get; set; } = false;
    }
}