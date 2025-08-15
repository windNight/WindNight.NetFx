using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection.WnExtension;
using Quartz;
using Schedule.Abstractions;
using Schedule.Extension;
using Schedule.Func;
using Schedule.@internal;
using Schedule.Model;
using Schedule.Model.Enums;

namespace Schedule
{

    public abstract partial class BaseJob : IJob
    {

        protected static string CurrentPluginVersion => BuildInfo.BuildVersion;

        protected static string CurrentPluginCompileTime => BuildInfo.BuildTime;

        /// <summary>
        ///     重写后必须返回正确的值 业务代码执行结果 true|false
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public abstract Task<JobBusinessStateEnum> ExecuteWithResultAsync(IJobExecutionContext context);

        /// <summary> </summary>
        protected virtual string JobId { get; private set; } = string.Empty;

        protected virtual string JobCode { get; private set; } = string.Empty;

        protected virtual string JobName { get; private set; } = string.Empty;

        public abstract string CurrentJobCode { get; }

        protected JobMeta CurrentJobMeta => ConfigItems.JobsConfig?.FetchJobConfig(CurrentJobCode) ?? null;

        protected virtual IJobExecutionContext CurrentJobContext { get; private set; }

        protected virtual void SetCurrentJobContext(IJobExecutionContext context)
        {
            CurrentJobContext = context;
        }



    }

    public abstract partial class BaseJob
    {

        public virtual async Task Execute(IJobExecutionContext context)
        {
            JobBaseInfo jobInfo = null;
            try
            {

                CurrentJobContext = context;
                JobId = context.GetJobDbId();
                JobCode = context.GetJobCode();
                JobName = context.GetJobName();
                jobInfo = context.GetJobBaseInfo();
                var limitTs = HardInfo.Now.AddMinutes(-2).ConvertToUnixTime();
                if (jobInfo == null || jobInfo.JobExecTs < limitTs)
                {
                    jobInfo = context.Parse2JobBaseInfo();
                    //   new JobBaseInfo
                    // {
                    //  JobId = JobId,
                    //  JobCode = JobCode,
                    // JobName = JobName,
                    jobInfo.JobExecTs = HardInfo.NowUnixTime;
                    jobInfo.ExecTag = "BaseJob.Execute";
                    //  };

                    context.JobDetail.SetJobBaseInfo(jobInfo);

                    JobContext.SetCurrentJobBaseInfo(jobInfo);

                    JobLogHelper.Warn($"未知异常 在Execute执行 Parse2JobBaseInfo {jobInfo.ToString(true)}", actionName: $"BaseJob.{nameof(Execute)}");

                }

                var job = context.JobDetail;
                var state = JobBusinessStateEnum.Processing;
                job.SetJobBusinessState(state);

                state = await ExecuteWithResultAsync(context);

                // state = rlt ? JobBusinessStateEnum.Success : JobBusinessStateEnum.Fail;
                var bizContent = context.GetBizContent();

                if (state != JobBusinessStateEnum.Success && bizContent.IsNullOrEmpty())
                {
                    var msg = $"[BaseJob] 未知原因 ExecuteWithResult 返回({state.ToString()}) ，且无业务异常信息!";

                    JobLogHelper.Warn($"{jobInfo}->{msg}", actionName: $"BaseJob.{nameof(Execute)}");

                    job.SetBizContent(msg);

                }

                job.SetJobBusinessState(state);

            }
            catch (Exception ex)
            {
                JobLogHelper.Error($"{jobInfo?.ToString(true)} BaseJob.Execute Handler Error {ex.Message}", ex, "BaseExecute");
            }

        }


    }

    public abstract partial class BaseJob
    {

        public virtual async Task<bool> RunTestAtStartAsync(int delayS = 2)
        {
            return await Task.FromResult(true);
        }

        public virtual bool RunTestAtStart(int delayS = 2)
        {
            return true;
        }

    }

    public abstract partial class BaseJob
    {
        protected virtual void ConsoleWriteLine(string msg, bool isForce = true)
        {
            //var stringBuilder = new StringBuilder(format.Length + args.Length * 8);
            //stringBuilder.AppendFormat(null, format, args);
            //var msg = stringBuilder.ToString();
            msg.Log2Console(isForce: true);

        }
        protected static IScheduleNotice _scheduleNotice => Ioc.GetService<IScheduleNotice>();

        protected virtual async Task DoNoticeAsync(IJobExecutionContext context, string message, string extendInfo = "")
        {
            await context.DoNoticeAsync(message, extendInfo);
        }



    }


}
