using System;
using Quartz;
using Schedule.Model.Enums;

namespace Schedule.Func
{
    /// <summary>  </summary>
    public static class JobContextFunc
    {
        /// <summary>
        ///     设置原始job name
        /// </summary>
        /// <param name="jobDetail"></param>
        /// <param name="origName"></param>
        public static void SetJobCode(IJobDetail jobDetail, string origName)
        {
            if (!string.IsNullOrEmpty(origName))
                jobDetail.JobDataMap["jobCode"] = origName;
        }

        /// <summary>
        ///     获取原始job name
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static string GetJobCode(IJobExecutionContext context)
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
        public static void SetJobName(IJobDetail jobDetail, string name)
        {
            if (!string.IsNullOrEmpty(name))
                jobDetail.JobDataMap["jobName"] = name;
        }

        /// <summary>
        ///     获取当前用于标识的job name
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static string GetJobName(IJobExecutionContext context)
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
        public static void SetJobRunParams(IJobDetail jobDetail, string runParams)
        {
            if (!string.IsNullOrEmpty(runParams))
                jobDetail.JobDataMap["runParams"] = runParams;
        }

        /// <summary>
        ///     获取job运行参数
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static string GetJobRunParams(IJobExecutionContext context)
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
        public static void SetOnceJobFlag(IJobDetail jobDetail, bool isOnceFlag)
        {
            jobDetail.JobDataMap["onceJob"] = isOnceFlag;
        }

        /// <summary>
        ///     获取job是否是单次执行
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static bool IsOnceJob(IJobExecutionContext context)
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
        public static void SetContinueRunFlag(IJobDetail jobDetail, bool isContinueFlag)
        {
            jobDetail.JobDataMap.Add("JobContinue", isContinueFlag);
        }

        /// <summary>
        ///     设置是否继续执行
        /// </summary>
        /// <param name="context"></param>
        public static bool IsContinueRun(IJobExecutionContext context)
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
        public static void SetDepJobs(IJobDetail jobDetail, string depJobs)
        {
            if (!string.IsNullOrEmpty(depJobs))
                jobDetail.JobDataMap.Add("depJobs", depJobs);
        }

        /// <summary>
        ///     获取依赖jobs
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static string GetDepJobs(IJobExecutionContext context)
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
        public static void SetJobDbId(IJobDetail jobDetail, string jobDbId)
        {
            if (!string.IsNullOrEmpty(jobDbId))
                jobDetail.JobDataMap.Add("jobId", jobDbId);
        }

        /// <summary>
        ///     获取job db id
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static string GetJobDbId(IJobExecutionContext context)
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
        public static void SetAutoClose(IJobDetail jobDetail, bool autoClose)
        {
            jobDetail.JobDataMap["autoClose"] = autoClose;
        }

        /// <summary>
        ///     获取job完成后是否退出程序
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static bool GetAutoClose(IJobExecutionContext context)
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
        public static void SetJobBusinessState(IJobDetail jobDetail, JobBusinessStateEnum state)
        {
            jobDetail.JobDataMap["jobBusinessState"] = state;
        }

        /// <summary>
        ///     获取当前用于标识的jobBusinessState
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static JobBusinessStateEnum GetJobBusinessState(IJobExecutionContext context)
        {
            return context.JobDetail.JobDataMap.ContainsKey("jobBusinessState")
                ? (JobBusinessStateEnum) context.JobDetail.JobDataMap["jobBusinessState"]
                : JobBusinessStateEnum.Unknown;
        }

        public static void SetJobBeginDateTimeTicks(IJobDetail jobDetail)
        {
            jobDetail.JobDataMap.Add("jobbeginticks", DateTime.Now.Ticks);
        }

        public static long GetJobBeginDateTimeTicks(IJobExecutionContext context)
        {
            return context.JobDetail.JobDataMap.ContainsKey("jobbeginticks")
                ? context.JobDetail.JobDataMap["jobbeginticks"].ToString().ToLong()
                : 0L;
        }

        /// <summary>
        /// </summary>
        /// <param name="jobDetail"></param>
        /// <param name="isDoNotice"></param>
        public static void SetIsDoNotice(IJobDetail jobDetail, bool isDoNotice)
        {
            jobDetail.JobDataMap["isDoNotice"] = isDoNotice;
        }

        /// <summary>
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static bool GetIsDoNotice(IJobExecutionContext context)
        {
            return context.JobDetail.JobDataMap.ContainsKey("isDoNotice")
                ? bool.Parse(context.JobDetail.JobDataMap["isDoNotice"].ToString())
                : false;
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
    }
}