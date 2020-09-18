using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Schedule.Abstractions;

namespace Schedule
{ 
    public class JobContext
    {
        private static readonly AsyncLocal<JobBaseInfo> CurrentJobBaseInfoAsyncLocal = new AsyncLocal<JobBaseInfo>();

        public static void SetCurrentJobBaseInfo(JobBaseInfo jobBaseInfo)
        {
            CurrentJobBaseInfoAsyncLocal.Value = jobBaseInfo;
        }

        public static void SetCurrentJobBaseInfo(string jobId, string jobCode, string jobName)
        {
            SetCurrentJobBaseInfo(new JobBaseInfo
            {
                JobId = jobId,
                JobCode = jobCode,
                JobName = jobName,
            });
        }

        public static JobBaseInfo CurrentJobBaseInfo => CurrentJobBaseInfoAsyncLocal.Value;

        public static string JobId => CurrentJobBaseInfo?.JobId ?? "";

        public static string JobCode => CurrentJobBaseInfo?.JobCode ?? "";

        public static string JobName => CurrentJobBaseInfo?.JobName ?? "";
         

    }

}
