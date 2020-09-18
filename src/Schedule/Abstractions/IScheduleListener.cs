using Quartz;

namespace Schedule.Abstractions
{
    public interface IScheduleListener : ITriggerListener, IJobListener
    {
        void SetName(string name);
    }
}