using System;
using System.Text;
using System.Threading.Tasks;
using Quartz;
using Schedule.Abstractions;
using Schedule.Func;
using Schedule.@internal;
using Schedule.Model;
using Schedule.Model.Enums;

namespace Schedule
{
    public abstract partial class BaseJob : IJob
    {

        /// <summary>
        ///     重写后必须返回正确的值 业务代码执行结果 true|false
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public abstract Task<bool> ExecuteWithResultAsync(IJobExecutionContext context);

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
            CurrentJobContext = context;
            JobId = context.GetJobDbId();
            JobCode = context.GetJobCode();
            JobName = context.GetJobName();
            var jobInfo = context.GetJobBaseInfo();
            if (jobInfo == null || jobInfo.JobExecTs < HardInfo.Now.AddMinutes(-2).ConvertToUnixTime())
            {
                jobInfo = new JobBaseInfo
                {
                    JobId = JobId,
                    JobCode = JobCode,
                    JobName = JobName,
                    JobExecTs = HardInfo.NowUnixTime,
                    ExecTag = "BaseJob.Execute",
                };
                context.JobDetail.SetJobBaseInfo(jobInfo);
                JobContext.SetCurrentJobBaseInfo(jobInfo);

            }


            var job = context.JobDetail;
            var state = JobBusinessStateEnum.Processing;
            job.SetJobBusinessState(state);

            var rlt = await ExecuteWithResultAsync(context);

            state = rlt ? JobBusinessStateEnum.Success : JobBusinessStateEnum.Fail;
            var bizContent = context.GetBizContent();
            if (!rlt && bizContent.IsNullOrEmpty())
            {
                job.SetBizContent($"[BaseJob] 未知原因 ExecuteWithResult 返回({rlt}) ，无业务异常信息!");
            }

            job.SetJobBusinessState(state);
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
    }


}
