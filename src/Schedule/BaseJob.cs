using Quartz;
using Schedule.Func;
using Schedule.Model.Enums;
using System;
using System.Text;
using System.Threading.Tasks;

namespace Schedule
{
    public abstract class BaseJob : IJob
    {
        /// <summary> </summary>
        protected string JobId { get; private set; } = string.Empty;

        protected string JobCode { get; private set; } = string.Empty;
        protected string JobName { get; private set; } = string.Empty;

        public virtual async Task Execute(IJobExecutionContext context)
        {
            JobId = context.GetJobDbId();
            JobCode = context.GetJobCode();
            JobName = context.GetJobName();
            JobContext.SetCurrentJobBaseInfo(JobId, JobCode, JobName);

            var job = context.JobDetail;
            var state = JobBusinessStateEnum.Processing;
            job.SetJobBusinessState(state); 
            var rlt = ExecuteWithResult(context);

            state = await rlt ? JobBusinessStateEnum.Success : JobBusinessStateEnum.Fail;
            job.SetJobBusinessState(state);
        }

        /// <summary>
        ///     重写后必须返回正确的值 业务代码执行结果 true|false
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        protected abstract Task<bool> ExecuteWithResult(IJobExecutionContext context);

        protected void ConsoleWriteLine(string format, params object[] args)
        {
#if DEBUG
            var stringBuilder = new StringBuilder(format.Length + args.Length * 8);
            stringBuilder.AppendFormat(null, format, args);
            var msg = stringBuilder.ToString();
            Console.WriteLine(msg);
#endif
        }
    }
}