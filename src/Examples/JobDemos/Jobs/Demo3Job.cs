using System;
using System.Threading.Tasks;
using JobDemos.Jobs.Demo1;
using Newtonsoft.Json.Extension;
using Quartz;
using Schedule;
using Schedule.Func;
using WindNight.Core.Attributes.Abstractions;
using WindNight.Extension;

namespace JobDemos.Jobs.Demo3
{
    [Alias(nameof(Demo3Job))]
    [DisallowConcurrentExecution]
    public class Demo3Job : CommonBaseJob<Demo3Job>
    {

        protected override int CurrentUserId => 300;
        protected override Func<bool, bool> PreTodo => null;
        public override async Task<bool> ExecuteWithResultAsync(IJobExecutionContext context)
        {
            var jobId = JobId;
            var jobCode = JobCode;
            var d = CurrentJobCode;
            var traceId = CurrentItem.GetSerialNumber;
            var jobRunParams = context.GetJobRunParams();
            var obj = new { jobId, jobCode, CurrentJobCode, traceId, jobRunParams };
            var jobBaseInfo = context.GetJobBaseInfo();
            ConsoleWriteLine($"{HardInfo.NowString} I'm {jobBaseInfo.ToString(true)} {obj.ToJsonStr()}");

            // Console.WriteLine($"{HardInfo.Now:yyyy-MM-dd HH:mm:sss} I'm {jobBaseInfo.ToString(true)} {obj.ToJsonStr()}{Environment.NewLine}");

            return await Task.FromResult(true);

        }
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
