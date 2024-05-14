using Quartz;
using Schedule;
using System;
using System.Threading.Tasks;
using WindNight.Core.Abstractions;

namespace JobDemos.Jobs.Demo3
{
    [Alias(nameof(Demo3Job))]
    public class Demo3Job : BaseJob
    {

        protected override Task<bool> ExecuteWithResult(IJobExecutionContext context)
        {
            Console.WriteLine($"{HardInfo.Now:yyyy-MM-dd HH:mm:sss} I'm {JobContext.CurrentJobBaseInfo}");
            return Task.FromResult(true);
        }

        public override string CurrentJobCode => nameof(Demo3Job);
    }

    [Alias(nameof(Demo3Job))]
    public class Demo3JobScheduleListener : BaseScheduleListener<Demo3Job>
    {
    }

    [Alias(nameof(Demo3Job))]
    public class Demo3JobCtrl : BaseJobCtrl<Demo3Job, Demo3JobScheduleListener>
    {
    }

}
