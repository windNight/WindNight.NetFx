using System;
using Newtonsoft.Json.Extension;
using Quartz;
using Schedule.Abstractions;
using Schedule.Model.Enums;
using WindNight.Core.Extension;

namespace Schedule.Func
{
    /// <summary>  </summary>
    public static class JobContextFunc
    {
        public const string BizContentKey = "bizContent";

        static JobDataMap DataMap(this IJobDetail jobDetail) => jobDetail.JobDataMap;


        /// <summary>
        ///     设置原始job name
        /// </summary>
        /// <param name="jobDetail"></param>
        /// <param name="origName"></param>
        public static void SetJobCode(this IJobDetail jobDetail, string origName)
        {
            if (!origName.IsNullOrEmpty())
                jobDetail.JobDataMap["jobCode"] = origName;
        }

        /// <summary>
        ///     获取原始job name
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static string GetJobCode(this IJobExecutionContext context)
        {
            return context.JobDetail.JobDataMap.ContainsKey("jobCode")
                ? context.JobDetail.JobDataMap["jobCode"].ToString()
                : string.Empty;
        }

        /// <summary>
        ///     设置当前用于标识的job name
        /// </summary>
        /// <param name="jobDetail"></param>
        /// <param name="name"></param>
        public static void SetJobName(this IJobDetail jobDetail, string name)
        {
            if (!name.IsNullOrEmpty())
                jobDetail.JobDataMap["jobName"] = name;
        }

        /// <summary>
        ///     获取当前用于标识的job name
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static string GetJobName(this IJobExecutionContext context)
        {
            return context.JobDetail.JobDataMap.ContainsKey("jobName")
                ? context.JobDetail.JobDataMap["jobName"].ToString()
                : string.Empty;
        }

        /// <summary>
        ///     设置job运行参数
        /// </summary>
        /// <param name="jobDetail"></param>
        /// <param name="runParams"></param>
        public static void SetJobRunParams(this IJobDetail jobDetail, string runParams)
        {
            if (!runParams.IsNullOrEmpty())
                jobDetail.JobDataMap["runParams"] = runParams;
        }

        /// <summary>
        ///     获取job运行参数
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static string GetJobRunParams(this IJobExecutionContext context)
        {
            return context.JobDetail.JobDataMap.ContainsKey("runParams")
                ? context.JobDetail.JobDataMap["runParams"].ToString()
                : string.Empty;
        }

        /// <summary>
        ///     设置job是否是单词运行
        /// </summary>
        /// <param name="jobDetail"></param>
        /// <param name="isOnceFlag"></param>
        public static void SetOnceJobFlag(this IJobDetail jobDetail, bool isOnceFlag)
        {
            jobDetail.JobDataMap["onceJob"] = isOnceFlag;
        }

        /// <summary>
        ///     获取job是否是单次执行
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static bool IsOnceJob(this IJobExecutionContext context)
        {
            return context.JobDetail.JobDataMap.ContainsKey("onceJob")
                ? bool.Parse(context.JobDetail.JobDataMap["onceJob"].ToString())
                : false;
        }

        /// <summary>
        ///     设置是否继续执行
        /// </summary>
        /// <param name="jobDetail"></param>
        /// <param name="isContinueFlag"></param>
        public static void SetContinueRunFlag(this IJobDetail jobDetail, bool isContinueFlag)
        {
            jobDetail.JobDataMap.Add("JobContinue", isContinueFlag);
        }

        /// <summary>
        ///     设置是否继续执行
        /// </summary>
        /// <param name="context"></param>
        public static bool IsContinueRun(this IJobExecutionContext context)
        {
            return context.JobDetail.JobDataMap.ContainsKey("JobContinue")
                ? bool.Parse(context.JobDetail.JobDataMap["JobContinue"].ToString())
                : false;
        }

        /// <summary>
        ///     设置依赖jobs
        /// </summary>
        /// <param name="jobDetail"></param>
        /// <param name="depJobs"></param>
        public static void SetDepJobs(this IJobDetail jobDetail, string depJobs)
        {
            if (!depJobs.IsNullOrEmpty())
                jobDetail.JobDataMap.Add("depJobs", depJobs);
        }

        /// <summary>
        ///     获取依赖jobs
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static string GetDepJobs(this IJobExecutionContext context)
        {
            return context.JobDetail.JobDataMap.ContainsKey("depJobs")
                ? context.JobDetail.JobDataMap["depJobs"].ToString()
                : string.Empty;
        }

        /// <summary>
        ///     设置job db id
        /// </summary>
        /// <param name="jobDetail"></param>
        /// <param name="jobDbId"></param>
        public static void SetJobDbId(this IJobDetail jobDetail, string jobDbId)
        {
            if (!jobDbId.IsNullOrEmpty())
                jobDetail.JobDataMap.Add("jobId", jobDbId);
        }

        /// <summary>
        ///     获取job db id
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static string GetJobDbId(this IJobExecutionContext context)
        {
            return context.JobDetail.JobDataMap.ContainsKey("jobId")
                ? context.JobDetail.JobDataMap["jobId"].ToString()
                : string.Empty;
        }

        /// <summary>
        ///     设置job完成后是否退出程序
        /// </summary>
        /// <param name="jobDetail"></param>
        /// <param name="autoClose"></param>
        public static void SetAutoClose(this IJobDetail jobDetail, bool autoClose)
        {
            jobDetail.JobDataMap["autoClose"] = autoClose;
        }

        /// <summary>
        ///     获取job完成后是否退出程序
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static bool GetAutoClose(this IJobExecutionContext context)
        {
            return context.JobDetail.JobDataMap.ContainsKey("autoClose")
                ? bool.Parse(context.JobDetail.JobDataMap["autoClose"].ToString())
                : false;
        }


        /// <summary>
        ///     设置当前用于标识的jobBusinessState
        /// </summary>
        /// <param name="jobDetail"></param>
        /// <param name="state"></param>
        public static void SetJobBusinessState(this IJobDetail jobDetail, JobBusinessStateEnum state)
        {
            jobDetail.JobDataMap["jobBusinessState"] = state;
        }

        /// <summary>
        ///     获取当前用于标识的jobBusinessState
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static JobBusinessStateEnum GetJobBusinessState(this IJobExecutionContext context)
        {
            return context.JobDetail.JobDataMap.ContainsKey("jobBusinessState")
                ? (JobBusinessStateEnum)context.JobDetail.JobDataMap["jobBusinessState"]
                : JobBusinessStateEnum.Unknown;
        }

        public static void SetJobBeginDateTimeTicks(this IJobDetail jobDetail)
        {
            jobDetail.JobDataMap.Add("jobbeginticks", HardInfo.Now.Ticks);
        }

        public static long GetJobBeginDateTimeTicks(this IJobExecutionContext context)
        {
            return context.JobDetail.JobDataMap.ContainsKey("jobbeginticks")
                ? context.JobDetail.JobDataMap["jobbeginticks"].ToLong()
                : 0L;
        }

        /// <summary>
        /// </summary>
        /// <param name="jobDetail"></param>
        /// <param name="isDoNotice"></param>
        public static void SetIsDoNotice(this IJobDetail jobDetail, bool isDoNotice)
        {
            jobDetail.JobDataMap["isDoNotice"] = isDoNotice;
        }

        /// <summary>
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static bool GetIsDoNotice(this IJobExecutionContext context)
        {
            return context.JobDetail.JobDataMap.ContainsKey("isDoNotice") && bool.Parse(context.JobDetail.JobDataMap["isDoNotice"].ToString());
        }

        /// <summary>
        /// </summary>
        /// <param name="jobDetail"></param>
        /// <param name="isLogJobLC"></param>
        public static void SetIsLogJobLC(this IJobDetail jobDetail, bool isLogJobLC)
        {
            jobDetail.JobDataMap["isLogJobLC"] = isLogJobLC;
        }

        /// <summary>
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static bool GetIsLogJobLC(this IJobExecutionContext context)
        {
            return context.JobDetail.JobDataMap.ContainsKey("isLogJobLC") && bool.Parse(context.JobDetail.JobDataMap["isLogJobLC"].ToString());
        }

        /// <summary>
        ///     设置 job 被否决的原因
        /// </summary>
        /// <param name="jobDetail"></param>
        /// <param name="reason"></param>
        public static void SetVotedReason(this IJobDetail jobDetail, string reason)
        {
            if (!reason.IsNullOrEmpty())
                jobDetail.JobDataMap.Add("vetoedReason", reason);
        }

        /// <summary>
        ///     获取job 被否决的原因 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static string GetVotedReason(this IJobExecutionContext context)
        {
            return context.JobDetail.JobDataMap.ContainsKey("vetoedReason")
                ? context.JobDetail.JobDataMap["vetoedReason"].ToString()
                : string.Empty;
        }



        public static void SetTempConfig(this IJobExecutionContext context, string key, object value)
        {
            if (context.JobDetail.JobDataMap.ContainsKey(key))
                context.JobDetail.JobDataMap[key] = value;
            else
                context.JobDetail.JobDataMap.Add(key, value);
        }

        public static object GetTempConfig(this IJobExecutionContext context, string key)
        {
            if (context.JobDetail.JobDataMap.ContainsKey(key))
                return context.JobDetail.JobDataMap[key];
            return string.Empty;
        }

        /// <summary>
        ///      
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static string GetBizContent(this IJobExecutionContext context)
        {
            return context.GetTempConfig(BizContentKey).ToString();
        }


        /// <summary>
        ///     设置当前用于标识的job name
        /// </summary>
        /// <param name="jobDetail"></param>
        /// <param name="bizContent"></param>
        public static void SetBizContent(this IJobDetail jobDetail, string bizContent)
        {
            if (!bizContent.IsNullOrEmpty())
                jobDetail.JobDataMap[BizContentKey] = bizContent;
        }

        /// <summary>
        ///      
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static JobBaseInfo GetJobBaseInfo(this IJobExecutionContext context)
        {
            var jobBaseInfo = context.JobDetail.JobDataMap.SafeGetValue("jobBaseInfo")?.ToString() ?? "";
            if (!jobBaseInfo.IsNullOrEmpty())
            {
                return jobBaseInfo.To<JobBaseInfo>();
            }

            return null;
        }
        /// <summary>
        ///     设置原始job baseInfo
        /// </summary>
        /// <param name="jobDetail"></param>
        /// <param name="jobInfo"></param>
        public static void SetJobBaseInfo(this IJobDetail jobDetail, JobBaseInfo jobInfo)
        {
            if (jobInfo != null && !jobInfo.JobId.IsNullOrEmpty() && jobInfo.JobExecTs > 0)
            {
                jobDetail.JobDataMap["jobBaseInfo"] = jobInfo.ToJsonStr();
            }
        }


    }
}
