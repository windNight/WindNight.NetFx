using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Extension;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection.WnExtension;
using Schedule.Abstractions;
using Schedule.Attributes;
using Schedule.Func;
using Schedule.Model;
using Schedule.Model.Enums;

namespace Schedule.Ctrl
{
    [CommandArea("schedule")]
    public class ScheduleCtrl : ICommandCtrl
    {
        private readonly IJobEnvManager __JobEnvManager;

        public ScheduleCtrl()
        {
            __JobEnvManager = new CommJobEnvManager();
        }

        //public ScheduleCtrl(IJobEnvManager jobEnvManager)
        //{
        //    __JobEnvManager = jobEnvManager;
        //}

        /// <summary>
        ///     查询job信息
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        public JobInfoOutput GetJobInfo(JobSearchInput search)
        {
            var allJobInfo = GetAllJobInfo();
            if (search.JobName != null)
                allJobInfo = allJobInfo.Where(x => x.JobName.Equals(search.JobName)).ToList();
            if (search.State != null)
                allJobInfo = allJobInfo.Where(x => x.State == (search.State ?? JobStateEnum.Open)).ToList();
            if (search.SupportOnceJob != null)
                allJobInfo = allJobInfo.Where(x => x.SupportOnceJob == (search.SupportOnceJob ?? true)).ToList();
            if (search.ShowOnceJob != null)
                allJobInfo = allJobInfo.Where(x => !x.JobId.IsNullOrEmpty() == search.ShowOnceJob.Value).ToList();

            var totalJobsCount = allJobInfo.Count;
            allJobInfo = allJobInfo.Skip((search.PageCurrent - 1) * search.PageSize).Take(search.PageSize).ToList();

            return new JobInfoOutput
            {
                DataList = allJobInfo,
                PageCurrent = search.PageCurrent,
                Total = totalJobsCount
            };
        }

        private List<JobMeta> GetAllJobInfo()
        {
            var allJobEnv = __JobEnvManager.ReadAllJobEnv();
            var allJobInfo = ScheduleModConfig.Instance.Jobs.ToList();

            allJobInfo.RemoveAll(x =>
                allJobEnv.Select(y => y.JobName).Contains(x.JobName) && x.JobId.IsNullOrEmpty());
            allJobInfo.AddRange(allJobEnv);
            return allJobInfo;
        }

        /// <summary>
        ///     获取job详情
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public JobMeta GetDetail(string name)
        {
            var jSInput = new JobSearchInput();
            jSInput.JobName = name;
            jSInput.PageCurrent = 1;
            jSInput.PageSize = 1;
            return GetJobInfo(jSInput).DataList.FirstOrDefault();
        }

        /// <summary>
        ///     获取后台调度的任务信息
        /// </summary>
        /// <returns></returns>
        public List<JobMeta> GetBGJobInfo()
        {
            var allJobEnv = __JobEnvManager.ReadAllJobEnv();
            return allJobEnv.Where(x => x.JobId.IsNullOrEmpty()).ToList();
        }

        /// <summary>
        ///     单次运行job
        /// </summary>
        /// <param name="name"></param>
        /// <param name="execTime"></param>
        /// <param name="runParams"></param>
        /// <param name="autoClose"></param>
        /// <returns></returns>
        public JobActionRetEnum StartJob(string name, DateTime execTime, string runParams, bool autoClose)
        {
            //从原始配置中复制一份数据
            var job = Ioc.GetService<IJobCtrl>(name);
            var jobParams = job.ReadJobParam();

            if (jobParams.SupportOnceJob == false) return JobActionRetEnum.Conflict;

            //修改配置
            jobParams.RunParams = runParams;
            jobParams.StartTime = execTime;
            jobParams.JobId = GuidHelper.GetGuid();
            jobParams.JobName = $"{jobParams.JobName}_{jobParams.JobId}";
            jobParams.AutoClose = autoClose;

            //保存配置
            __JobEnvManager.SaveJobEnv(jobParams);

            return job.StartJob(jobParams, true) ? JobActionRetEnum.Success : JobActionRetEnum.Failed;
        }

        /// <summary>
        ///     停止单次运行的job
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public async Task<JobActionRetEnum> StopJobAsync(string name)
        {
            var jobParams = ScheduleModConfig.Instance.Jobs == null
                ? null
                : ScheduleModConfig.Instance.Jobs.FirstOrDefault(x => x.JobName.Equals(name));
            if (jobParams != null)
                //非单次运行的job不支持停止，只能暂停
                return JobActionRetEnum.Conflict;

            __JobEnvManager.DelJobFromEnv(name);

            var jobKey = UtilsFunc.GenJobKey(name);
            var listenerName = UtilsFunc.GenListenerName(name);

            //移除job, trigger listener and job self and its trigger
            return ScheduleModConfig.Instance.DefaultScheduler.ListenerManager.RemoveJobListener(listenerName) &&
                   ScheduleModConfig.Instance.DefaultScheduler.ListenerManager.RemoveTriggerListener(listenerName) &&
                await ScheduleModConfig.Instance.DefaultScheduler.DeleteJob(jobKey)
                ? JobActionRetEnum.Success
                : JobActionRetEnum.Failed;
        }

        /// <summary>
        ///     暂停非单次运行的job运行
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public JobActionRetEnum PauseJob(string name)
        {
            var jobParams = GetAllJobInfo()
                .FirstOrDefault(x => x.JobName.Equals(name) && x.JobId.IsNullOrEmpty());
            if (jobParams == null) return JobActionRetEnum.Failed;

            if (jobParams.State == JobStateEnum.Closed) return JobActionRetEnum.Conflict;
            if (jobParams.State == JobStateEnum.Pause) return JobActionRetEnum.Success;

            jobParams.State = JobStateEnum.Pause;
            __JobEnvManager.SaveJobEnv(jobParams);

            var jobCtrl = Ioc.GetService<IJobCtrl>(name);
            ScheduleModConfig.Instance.DefaultScheduler.PauseJob(jobCtrl.GetJobKey());
            return JobActionRetEnum.Success;
        }

        /// <summary>
        ///     恢复非单次运行job运行
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public JobActionRetEnum ResumeJob(string name)
        {
            var jobParams = GetAllJobInfo()
                .FirstOrDefault(x => x.JobName.Equals(name) && x.JobId.IsNullOrEmpty());
            if (jobParams == null) return JobActionRetEnum.Failed;

            if (jobParams.State == JobStateEnum.Closed) return JobActionRetEnum.Conflict;
            if (jobParams.State == JobStateEnum.Open) return JobActionRetEnum.Success;

            jobParams.State = JobStateEnum.Open;
            __JobEnvManager.SaveJobEnv(jobParams);

            var jobCtrl = Ioc.GetService<IJobCtrl>(name);
            ScheduleModConfig.Instance.DefaultScheduler.ResumeJob(jobCtrl.GetJobKey());
            return JobActionRetEnum.Success;
        }

        public string GetJobParamsDesc(string name)
        {
            var job = Ioc.GetService<IJobCtrl>(name);
            var jobParams = job.ReadJobParam();
            return jobParams.JobParamsDesc;
        }
    }
}