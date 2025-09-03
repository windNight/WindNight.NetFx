using System.Reflection;
using Quartz;
using Quartz.Impl.Matchers;
using Schedule.Abstractions;
using Schedule.Func;
using Schedule.@internal;
using Schedule.Model;
using Schedule.Model.Enums;
using WindNight.Core.Attributes.Abstractions;

namespace Schedule
{
    public class BaseJobCtrl<T, LT> : IJobCtrl
        where T : IJob
        where LT : IScheduleListener, new()
    {
        protected static string CurrentPluginVersion => BuildInfo.BuildVersion;

        protected static string CurrentPluginCompileTime => BuildInfo.BuildTime;

        public virtual string JobCode => typeof(T).Name.ToLower();


        public virtual bool JobCanSkip()
        {
            try
            {
                var canSkip = typeof(T).GetCustomAttribute<ScheduleJobCanSkipAttribute>();
                if (canSkip == null)
                {
                    return false;
                }

                return canSkip.CanSkip;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        ///     开启job
        /// </summary>
        /// <param name="jobParam">job执行参数</param>
        /// <param name="onceJob">是否执行计划外的任务</param>
        /// <returns></returns>
        public bool StartJob(JobMeta jobParam, bool onceJob = false)
        {
            try
            {
                //var triggerKey = onceJob ? UtilsFunc.GenTriggerKey(jobParam.JobName) : GetTriggerKey();
                //var trigger = GenTrigger(triggerKey, jobParam.StartTime, jobParam.Interval, jobParam.CronExpression,
                //    onceJob);
                //if (trigger == null)
                //{
                //    JobLogHelper.Error(
                //        $"创建{jobParam.JobName}的trigger失败，参数为{onceJob},{jobParam.StartTime},{jobParam.Interval},{jobParam.CronExpression}",
                //        null, nameof(StartJob));
                //    return false;
                //}

                //var jobkey = onceJob ? UtilsFunc.GenJobKey(jobParam.JobName) : GetJobKey();
                //var job = JobBuilder.Create<T>()
                //    .WithIdentity(jobkey)
                //    .Build();

                //job.SetJobName(jobParam.JobName);
                //job.SetJobCode(jobParam.JobCode);
                //job.SetOnceJobFlag(onceJob);
                //job.SetDepJobs(jobParam.DepJobs);
                //job.SetJobRunParams(jobParam.RunParams);
                //job.SetAutoClose(jobParam.AutoClose);
                //job.SetIsDoNotice(jobParam.IsDoNotice);
                //job.SetIsLogJobLC(jobParam.IsLogJobLC);

                //if (onceJob)
                //{
                //    ScheduleModConfig.Instance.InitOnceJobScheduler();
                //    ScheduleModConfig.Instance.OnceJobScheduler.ScheduleJob(job, trigger);

                //    var jobListener = new LT();
                //    if (onceJob) jobListener.SetName(UtilsFunc.GenListenerName(jobParam.JobName));
                //    ScheduleModConfig.Instance.OnceJobScheduler.ListenerManager.AddJobListener(jobListener,
                //        KeyMatcher<JobKey>.KeyEquals(jobkey));
                //    ScheduleModConfig.Instance.OnceJobScheduler.ListenerManager.AddTriggerListener(jobListener,
                //        KeyMatcher<TriggerKey>.KeyEquals(triggerKey));
                //    if (jobParam.State == JobStateEnum.Pause)
                //    {
                //        ScheduleModConfig.Instance.OnceJobScheduler.PauseJob(jobkey);
                //    }
                //}
                //else
                //{
                var triggerKey = onceJob ? UtilsFunc.GenTriggerKey(jobParam.JobName) : GetTriggerKey();
                var trigger = GenTrigger(triggerKey, jobParam.StartTime, jobParam.Interval, jobParam.CronExpression,
                    onceJob);
                if (trigger == null)
                {
                    JobLogHelper.Error(
                        $"创建{jobParam.JobName}的trigger失败，参数为{onceJob},{jobParam.StartTime},{jobParam.Interval},{jobParam.CronExpression}",
                        null, nameof(StartJob));
                    return false;
                }

                var jobkey = onceJob ? UtilsFunc.GenJobKey(jobParam.JobName) : GetJobKey();

                var job = BuildJobDetail(jobParam, jobkey, onceJob);

                ScheduleModConfig.Instance.DefaultScheduler.ScheduleJob(job, trigger);
                var jobListener = new LT();
                if (onceJob)
                {
                    jobListener.SetName(UtilsFunc.GenListenerName(jobParam.JobName));
                }

                ScheduleModConfig.Instance.DefaultScheduler.ListenerManager.AddJobListener(jobListener,
                    KeyMatcher<JobKey>.KeyEquals(jobkey));

                ScheduleModConfig.Instance.DefaultScheduler.ListenerManager.AddTriggerListener(jobListener,
                    KeyMatcher<TriggerKey>.KeyEquals(triggerKey));

                if (jobParam.State == JobStateEnum.Pause)
                {
                    ScheduleModConfig.Instance.DefaultScheduler.PauseJob(jobkey);
                }
                //  }

                return true;
            }
            catch (Exception ex)
            {
                JobLogHelper.Error(ex.Message, ex, nameof(StartJob));
                return false;
            }
        }

        /// <summary>
        ///     开启job
        /// </summary>
        /// <param name="jobParam">job执行参数</param>
        /// <param name="onceJob">是否执行计划外的任务</param>
        /// <returns></returns>
        public async Task<bool> StartJobAsync(JobMeta jobParam, bool onceJob = false)
        {
            try
            {
                var triggerKey = onceJob ? UtilsFunc.GenTriggerKey(jobParam.JobName) : GetTriggerKey();
                var trigger = GenTrigger(triggerKey, jobParam.StartTime, jobParam.Interval, jobParam.CronExpression,
                    onceJob);
                if (trigger == null)
                {
                    JobLogHelper.Error(
                        $"创建{jobParam.JobName}的trigger失败，参数为{onceJob},{jobParam.StartTime},{jobParam.Interval},{jobParam.CronExpression}",
                        null, nameof(StartJob));
                    return false;
                }

                //var job = JobBuilder.Create<T>()
                //    .WithIdentity(jobkey)
                //    .Build();

                //job.SetJobName(jobParam.JobName);
                //job.SetJobCode(jobParam.JobCode);
                //job.SetOnceJobFlag(onceJob);
                //job.SetDepJobs(jobParam.DepJobs);
                //job.SetJobRunParams(jobParam.RunParams);
                //job.SetAutoClose(jobParam.AutoClose);
                //job.SetIsDoNotice(jobParam.IsDoNotice);
                //job.SetIsLogJobLC(jobParam.IsLogJobLC);

                //if (onceJob)
                //{
                //    await ScheduleModConfig.Instance.InitOnceJobSchedulerAsync();
                //    await ScheduleModConfig.Instance.OnceJobScheduler.ScheduleJob(job, trigger);

                //    var jobListener = new LT();
                //    if (onceJob) jobListener.SetName(UtilsFunc.GenListenerName(jobParam.JobName));
                //    ScheduleModConfig.Instance.OnceJobScheduler.ListenerManager.AddJobListener(jobListener,
                //        KeyMatcher<JobKey>.KeyEquals(jobkey));
                //    ScheduleModConfig.Instance.OnceJobScheduler.ListenerManager.AddTriggerListener(jobListener,
                //        KeyMatcher<TriggerKey>.KeyEquals(triggerKey));
                //    if (jobParam.State == JobStateEnum.Pause)
                //    {
                //        await ScheduleModConfig.Instance.OnceJobScheduler.PauseJob(jobkey);
                //    }
                //}
                //else
                //{
                var jobkey = onceJob ? UtilsFunc.GenJobKey(jobParam.JobName) : GetJobKey();
                var job = BuildJobDetail(jobParam, jobkey, onceJob);

                await ScheduleModConfig.Instance.DefaultScheduler.ScheduleJob(job, trigger);

                var jobListener = new LT();
                if (onceJob)
                {
                    jobListener.SetName(UtilsFunc.GenListenerName(jobParam.JobName));
                }

                ScheduleModConfig.Instance.DefaultScheduler.ListenerManager.AddJobListener(jobListener,
                    KeyMatcher<JobKey>.KeyEquals(jobkey));

                ScheduleModConfig.Instance.DefaultScheduler.ListenerManager.AddTriggerListener(jobListener,
                    KeyMatcher<TriggerKey>.KeyEquals(triggerKey));

                if (jobParam.State == JobStateEnum.Pause)
                {
                    await ScheduleModConfig.Instance.DefaultScheduler.PauseJob(jobkey);
                }

                //  }

                return true;
            }
            catch (Exception ex)
            {
                JobLogHelper.Error(ex.Message, ex, nameof(StartJob));
                return false;
            }
        }

        public JobMeta ReadJobParam()
        {
            var jobCode = GetJobKey().Name;
            //var jobMeta = ConfigItems.JobsConfig?.Items?.FirstOrDefault(m =>
            //    string.Equals(m.JobCode, jobCode, StringComparison.InvariantCultureIgnoreCase));
            var jobMeta = ConfigItems.JobsConfig.FetchJobConfig(jobCode);

            if (jobMeta == null)
            {
                throw new ArgumentNullException("JobCode", $"JobCode({jobCode}) 缺少配置项");
            }

            return new JobMeta
            {
                JobId = string.Empty,
                Group = string.Empty,
                RunParams = jobMeta.RunParams,
                JobName = jobMeta.JobName,
                JobCode = jobMeta.JobCode,
                Title = jobMeta.Title,
                Description = jobMeta.Description,
                StartTime = jobMeta.StartTime,
                Interval = jobMeta.Interval,
                CronExpression = jobMeta.CronExpression,
                State = jobMeta.State,
                SupportOnceJob = jobMeta.SupportOnceJob,
                DepJobs = jobMeta.DepJobs,
                AutoClose = jobMeta.AutoClose,
                JobParamsDesc = jobMeta.JobParamsDesc,
                IsDoNotice = jobMeta.IsDoNotice,
                IsLogJobLC = jobMeta.IsLogJobLC,
                CanRunTest = jobMeta.CanRunTest,
            };
        }

        public virtual JobKey GetJobKey()
        {
            return JobKey.Create(JobCode, $"{JobCode}_group");
        }


        private IJobDetail BuildJobDetail(JobMeta jobParam, JobKey jobKey, bool onceJob = false)
        {
            var job = JobBuilder.Create<T>().WithIdentity(jobKey).Build();

            job.SetJobName(jobParam.JobName);
            job.SetJobCode(jobParam.JobCode);
            job.SetOnceJobFlag(onceJob);
            job.SetDepJobs(jobParam.DepJobs);
            job.SetJobRunParams(jobParam.RunParams);
            job.SetAutoClose(jobParam.AutoClose);
            job.SetIsDoNotice(jobParam.IsDoNotice);
            job.SetIsLogJobLC(jobParam.IsLogJobLC);
            job.SetIsStoreJobLC(jobParam.IsStoreJobLC);
            job.SetJobWarnTs(jobParam.JobWarnTs);

            return job;
        }

        public virtual TriggerKey GetTriggerKey()
        {
            return new TriggerKey($"{JobCode}_trigger", $"{JobCode}_group");
        }

        private ITrigger? GenTrigger(TriggerKey triggerKey, DateTime startTime, uint interval, string cronExpression,
            bool onceJob = false)
        {
            if (onceJob && startTime != default)
            {
                Console.WriteLine($"=={HardInfo.NowString}== will start job at time: {startTime}");
                return TriggerBuilder.Create()
                    .WithIdentity(triggerKey)
                    .StartAt(new DateTimeOffset(new DateTime(startTime.Year, startTime.Month, startTime.Day,
                        startTime.Hour, startTime.Minute, startTime.Second)))
                    .WithSimpleSchedule(x => x.WithMisfireHandlingInstructionNextWithRemainingCount())
                    .Build();
            }

            if (startTime != default)
            {
                return TriggerBuilder.Create()
                    .WithIdentity(triggerKey)
                    .StartNow()
                    .WithSchedule(
                        CronScheduleBuilder.DailyAtHourAndMinute(startTime.Hour,
                            startTime.Minute)) //未指定错过执行时间的处理方式，要求job里面的逻辑支持可重入
                    .Build();
            }

            if (interval != 0)
            //pause后重新resume,执行类似misfire job的原因
            //http://stackoverflow.com/questions/1933676/quartz-java-resuming-a-job-excecutes-it-many-times
            {
                return TriggerBuilder.Create()
                    .WithIdentity(triggerKey)
                    .WithSimpleSchedule(x => x
                            .WithIntervalInSeconds((int)interval)
                            .RepeatForever()
                            .WithMisfireHandlingInstructionNextWithRemainingCount() //discard misfire trigger
                    )
                    .StartNow()
                    .Build();
            }

            if (cronExpression.IsNotNullOrEmpty())
            {
                return TriggerBuilder.Create()
                    .WithIdentity(triggerKey)
                    .StartNow()
                    .WithCronSchedule(cronExpression)
                    .Build();
            }

            return null;
        }
    }
}
