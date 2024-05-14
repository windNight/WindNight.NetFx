using Quartz;
using Schedule;
using System;
using System.Threading.Tasks;
using WindNight.Core.Abstractions;

namespace JobDemos.Jobs.Demo1
{
    [Alias(nameof(Demo1Job))]
    public class Demo1Job : BaseJob
    {

        protected override Task<bool> ExecuteWithResult(IJobExecutionContext context)
        {
            Console.WriteLine($"{HardInfo.Now:yyyy-MM-dd HH:mm:sss} I'm {JobContext.CurrentJobBaseInfo}");
            return Task.FromResult(true);
        }

        public override string CurrentJobCode => nameof(Demo1Job);
    }

    [Alias(nameof(Demo1Job))]
    public class Demo1JobScheduleListener : BaseScheduleListener<Demo1Job>
    {
    }

    [Alias(nameof(Demo1Job))]
    public class Demo1JobCtrl : BaseJobCtrl<Demo1Job, Demo1JobScheduleListener>
    {
    }


}
