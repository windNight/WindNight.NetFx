using System;
using System.Linq;
using System.Threading;
using Quartz;
using Schedule.Abstractions;
using Schedule.Func;
using Schedule.Model;

namespace Schedule
{
    public static class JobContext
    {

        public static JobBaseInfo Parse2JobBaseInfo(this IJobExecutionContext context)
        {
            var jobId = context.GetJobDbId();
            var jobCode = context.GetJobCode();
            var jobName = context.GetJobName();

            var jobInfo = new JobBaseInfo
            {
                JobId = jobId,
                JobCode = jobCode,
                JobName = jobName,
            };

            return jobInfo;
        }


        private static readonly AsyncLocal<JobBaseInfo> CurrentJobBaseInfoAsyncLocal = new AsyncLocal<JobBaseInfo>();

        public static void SetCurrentJobBaseInfo(JobBaseInfo jobBaseInfo)
        {
            CurrentJobBaseInfoAsyncLocal.Value = jobBaseInfo;
        }

        //public static void SetCurrentJobBaseInfo(string jobId, string jobCode, string jobName)
        //{
        //    SetCurrentJobBaseInfo(new JobBaseInfo
        //    {
        //        JobId = jobId,
        //        JobCode = jobCode,
        //        JobName = jobName,
        //    });
        //}

        public static JobBaseInfo CurrentJobBaseInfo => CurrentJobBaseInfoAsyncLocal.Value;

        public static string JobId => CurrentJobBaseInfo?.JobId ?? "";

        public static string JobCode => CurrentJobBaseInfo?.JobCode ?? "";

        public static string JobName => CurrentJobBaseInfo?.JobName ?? "";


        public static JobMeta FetchJobConfig(this IJobExecutionContext context)
        {
            var jobCode = context.GetJobCode();

            var jobConfig = ScheduleModConfig.Instance.Jobs.FirstOrDefault(m =>
                     string.Equals(m.JobCode, jobCode, StringComparison.OrdinalIgnoreCase));

            return jobConfig;

        }



    }

}
