using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Quartz;
using Schedule;

namespace JobDemos.Jobs.Demo3
{
    public class Demo3Job : BaseJob
    {

        protected override Task<bool> ExecuteWithResult(IJobExecutionContext context)
        {
            Console.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:sss} I'm {JobContext.CurrentJobBaseInfo}");
            return Task.FromResult(true);
        }


    }

    public class Demo3JobScheduleListener : BaseScheduleListener<Demo3Job>
    {
    }

    public class Demo3JobCtrl : BaseJobCtrl<Demo3Job, Demo3JobScheduleListener>
    {
    }

}
