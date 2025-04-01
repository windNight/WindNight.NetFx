namespace Schedule
{
    public class JobRemotingConfig
    {
        public string JobRemoteChannelType { get; set; } = "tcp";
        public int JobRemotePort { get; set; } = 0;
        public string JobRemoteBindName { get; set; } = "WindNight:ScheduleJob";
    }
}
