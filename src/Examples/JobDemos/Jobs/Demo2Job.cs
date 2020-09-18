using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Quartz;
using Schedule;

namespace JobDemos.Jobs.Demo2
{
    public class Demo2Job : BaseJob
    {

        protected override Task<bool> ExecuteWithResult(IJobExecutionContext context)
        {
            Console.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:sss} I'm {JobContext.CurrentJobBaseInfo}");
            return Task.FromResult(true);
        }


    }

    public class Demo2JobScheduleListener : BaseScheduleListener<Demo2Job>
    {
    }

    public class Demo2JobCtrl : BaseJobCtrl<Demo2Job, Demo2JobScheduleListener>
    {
    }

}
