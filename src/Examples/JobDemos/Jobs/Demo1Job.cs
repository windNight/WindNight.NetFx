using Quartz;
using Schedule;
using System;
using System.Threading.Tasks;

namespace JobDemos.Jobs.Demo1
{
    public class Demo1Job : BaseJob
    {

        protected override Task<bool> ExecuteWithResult(IJobExecutionContext context)
        {
            Console.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:sss} I'm {JobContext.CurrentJobBaseInfo}");
            return Task.FromResult(true);
        }


    }

    public class Demo1JobScheduleListener : BaseScheduleListener<Demo1Job>
    {
    }

    public class Demo1JobCtrl : BaseJobCtrl<Demo1Job, Demo1JobScheduleListener>
    {
    }


}
