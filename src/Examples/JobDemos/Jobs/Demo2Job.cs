using Quartz;
using Schedule;
using System;
using System.Threading.Tasks;
using WindNight.Core.Abstractions;

namespace JobDemos.Jobs.Demo2
{
    [Alias(nameof(Demo2Job))]
    public class Demo2Job : BaseJob
    {

        protected override Task<bool> ExecuteWithResult(IJobExecutionContext context)
        {
            Console.WriteLine($"{HardInfo.Now:yyyy-MM-dd HH:mm:sss} I'm {JobContext.CurrentJobBaseInfo}");
            return Task.FromResult(true);
        }

        public override string CurrentJobCode => nameof(Demo2Job);
    }

    [Alias(nameof(Demo2Job))]
    public class Demo2JobScheduleListener : BaseScheduleListener<Demo2Job>
    {
    }

    [Alias(nameof(Demo2Job))]
    public class Demo2JobCtrl : BaseJobCtrl<Demo2Job, Demo2JobScheduleListener>
    {
    }

}
