using System;
using System.Threading.Tasks;
using Newtonsoft.Json.Extension;
using Quartz;
using Schedule;
using Schedule.Func;
using WindNight.Core.Attributes.Abstractions;
using WindNight.Extension;

namespace JobDemos.Jobs.Demo1
{
    [Alias(nameof(Demo1Job))]
    [DisallowConcurrentExecution]
    public class Demo1Job : CommonBaseJob<Demo1Job>
    {
        protected override int CurrentUserId => 100;
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
            var msg = $"{HardInfo.NowString} I'm {jobBaseInfo.ToString(true)} {obj.ToJsonStr()}";
            ConsoleWriteLine(msg);

            // Console.WriteLine($"{HardInfo.Now:yyyy-MM-dd HH:mm:sss} I'm {jobBaseInfo.ToString(true)} {obj.ToJsonStr()} {Environment.NewLine}");
            await DoNoticeAsync(context, $"donotice Test {msg}", "I’m ExtendInfo");

            return await Task.FromResult(true);




        }
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
