using Quartz;
using Schedule.Model.Enums;
using WindNight.Core.Abstractions;

namespace Schedule.Abstractions
{
    public interface IScheduleListener : ITriggerListener, IJobListener
    {
        IJobBaseInfo CurrentJob { get; }
        /// <summary>
        /// Get the name of the <see cref="T:Quartz.ITriggerListener" />.
        /// </summary>
        new string Name { get; }
        //IJobExecutionContext GetCurrentJobContext();

        //JobBusinessStateEnum JobStatus { get; }

        //bool JobIsRunning();

        void SetName(string name);



    }
}
