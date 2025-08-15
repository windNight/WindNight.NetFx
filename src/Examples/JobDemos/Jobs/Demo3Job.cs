using System;
using System.Threading;
using System.Threading.Tasks;
using JobDemos.Jobs.Demo1;
using Newtonsoft.Json.Extension;
using Quartz;
using Schedule;
using Schedule.Func;
using Schedule.Model.Enums;
using WindNight.Core.Attributes.Abstractions;
using WindNight.Extension;

namespace JobDemos.Jobs.Demo3
{
    [Alias(nameof(Demo3Job))]
    [DisallowConcurrentExecution]
    [ScheduleJobCanSkip(true)]
    public class Demo3Job : CommonBaseJob<Demo3Job>
    {

        protected override int CurrentUserId => 300;
        protected override Func<bool, bool> PreTodo => null;
        public override async Task<JobBusinessStateEnum> DoBizJobAsync(IJobExecutionContext context)
        {
            var jobId = JobId;
            var jobCode = JobCode;
            var d = CurrentJobCode;
            var traceId = CurrentItem.GetSerialNumber;
            var jobRunParams = context.GetJobRunParams();
            var obj = new { jobId, jobCode, CurrentJobCode, traceId, jobRunParams };
            var jobBaseInfo = context.GetJobBaseInfo();

            $"Start {HardInfo.NowString} I'm {jobBaseInfo.ToString(true)} {obj.ToJsonStr()}".Log2Console();

            Thread.Sleep(250_000);
            // Console.WriteLine($"{HardInfo.Now:yyyy-MM-dd HH:mm:sss} I'm {jobBaseInfo.ToString(true)} {obj.ToJsonStr()}{Environment.NewLine}");
            $"End {HardInfo.NowString} I'm {jobBaseInfo.ToString(true)} {obj.ToJsonStr()}".Log2Console();

            return await Task.FromResult(JobBusinessStateEnum.Fail);

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
