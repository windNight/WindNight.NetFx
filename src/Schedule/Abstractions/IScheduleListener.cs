using Quartz;
using Schedule.Model.Enums;

namespace Schedule.Abstractions
{
    public interface IScheduleListener : ITriggerListener, IJobListener
    {
        IJobBaseInfo CurrentJob { get; }

        //IJobExecutionContext GetCurrentJobContext();

        //JobBusinessStateEnum JobStatus { get; }

        //bool JobIsRunning();

        void SetName(string name);



    }
}
