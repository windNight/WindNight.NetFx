using System;
using System.Threading.Tasks;
using JobDemos.Jobs.Demo1;
using Newtonsoft.Json.Extension;
using Quartz;
using Schedule;
using Schedule.Func;
using WindNight.Core.Abstractions;
using WindNight.Core.Attributes.Abstractions;
using WindNight.Extension;

namespace JobDemos.Jobs.Demo2
{
    [Alias(nameof(Demo2Job))]
    [DisallowConcurrentExecution]
    public class Demo2Job : CommonBaseJob<Demo2Job>
    {

        protected override int CurrentUserId => 200;
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

            //  Console.WriteLine($"{HardInfo.NowString} I'm {jobBaseInfo.ToString(true)} {obj.ToJsonStr()}{Environment.NewLine}");
            return await Task.FromResult(true);

        }



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
