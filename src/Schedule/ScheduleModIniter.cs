using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.WnExtension;
using Quartz.Impl;
using Schedule.Abstractions;
using Schedule.Ctrl;
using Schedule.Model;
using Schedule.Model.Enums;

namespace Schedule
{
    public class ScheduleModIniter : IDisposable
    {
        public void Dispose()
        {
            if (ScheduleModConfig.Instance.DefaultScheduler != null)
                ScheduleModConfig.Instance.DefaultScheduler.Shutdown(true);
        }

        /// <summary>
        ///     初始化调度context
        /// </summary>
        public bool Init(JobMeta? config = null)
        {
            var jobConfig = ConfigItems.JobsConfig;
            var properties = new NameValueCollection
            {
                ["quartz.jobStore.misfireThreshold"] = (10 * 1000).ToString(), //修改misfire的时间为10秒
                ["quartz.scheduler.instanceName"] = jobConfig.JobInstanceName,
            };

            if (jobConfig.JobRemoteIsOpen && jobConfig.JobRemotingConfig.JobRemotePort > 0)
            {
                var jobRemotingConfig = jobConfig.JobRemotingConfig;
                // set remoting expoter
                properties["quartz.scheduler.exporter.type"] = "Quartz.Simpl.RemotingSchedulerExporter, Quartz";
                properties["quartz.scheduler.exporter.port"] = jobRemotingConfig.JobRemotePort.ToString();
                properties["quartz.scheduler.exporter.bindName"] = jobRemotingConfig.JobRemoteBindName;
                properties["quartz.scheduler.exporter.channelType"] = jobRemotingConfig.JobRemoteChannelType;
            }

            ScheduleModConfig.Instance.DefaultScheduler =
                new StdSchedulerFactory(properties).GetScheduler().GetAwaiter().GetResult();

            ScheduleModConfig.Instance.DefaultScheduler.Start();

            if (config != null) //在当天的用户指定时间运行一次指定的任务
            {
                //TODO 暂不支持 OnceJob
                var sc = new ScheduleCtrl();
                sc.StartJob(config.JobName, config.StartTime, config.RunParams, config.AutoClose);
                return true;
            }
            else
            {
                //加载配置文件，并且运行状态为open的任务
                var allJobs = Ioc.GetServices<IJobCtrl>().ToList();
                ScheduleModConfig.Instance.Jobs = new List<JobMeta>(allJobs.Count());
                var sc = new ScheduleCtrl();
                var cacheJobs = sc.GetBGJobInfo();

                foreach (var job in allJobs)
                {
                    var jobParam = job.ReadJobParam();
                    jobParam = cacheJobs.FirstOrDefault(x => x.JobName.Equals(jobParam.JobName)) == null
                        ? jobParam
                        : cacheJobs.FirstOrDefault(x => x.JobName.Equals(jobParam.JobName));

                    ScheduleModConfig.Instance.Jobs.Add(jobParam);
                    if (jobParam.State.Equals(JobStateEnum.Open) || jobParam.State.Equals(JobStateEnum.Pause))
                        job.StartJob(jobParam);
                }
            }

            return true;
        }

        /// <summary>
        ///     初始化调度context
        /// </summary>
        public async Task<bool> InitAsync(JobMeta? config = null)
        {
            var jobConfig = ConfigItems.JobsConfig;
            var properties = new NameValueCollection
            {
                ["quartz.jobStore.misfireThreshold"] = (10 * 1000).ToString(), //修改misfire的时间为10秒
                ["quartz.scheduler.instanceName"] = jobConfig.JobInstanceName,
            };

            if (jobConfig.JobRemoteIsOpen && jobConfig.JobRemotingConfig.JobRemotePort > 0)
            {
                var jobRemotingConfig = jobConfig.JobRemotingConfig;
                // set remoting expoter
                properties["quartz.scheduler.exporter.type"] = "Quartz.Simpl.RemotingSchedulerExporter, Quartz";
                properties["quartz.scheduler.exporter.port"] = jobRemotingConfig.JobRemotePort.ToString();
                properties["quartz.scheduler.exporter.bindName"] = jobRemotingConfig.JobRemoteBindName;
                properties["quartz.scheduler.exporter.channelType"] = jobRemotingConfig.JobRemoteChannelType;
            }

            ScheduleModConfig.Instance.DefaultScheduler = await new StdSchedulerFactory(properties).GetScheduler();
            await ScheduleModConfig.Instance.DefaultScheduler.Start();

            if (config != null) //在当天的用户指定时间运行一次指定的任务
            {
                //TODO 暂不支持 OnceJob
                var sc = new ScheduleCtrl();
                sc.StartJob(config.JobName, config.StartTime, config.RunParams, config.AutoClose);
                return true;
            }
            else
            {
                //加载配置文件，并且运行状态为open的任务
                var allJobs = Ioc.GetServices<IJobCtrl>().ToList();
                ScheduleModConfig.Instance.Jobs = new List<JobMeta>(allJobs.Count());
                var sc = new ScheduleCtrl();
                var cacheJobs = sc.GetBGJobInfo();

                foreach (var job in allJobs)
                {
                    var jobParam = job.ReadJobParam();
                    jobParam = cacheJobs.FirstOrDefault(x => x.JobName.Equals(jobParam.JobName)) == null
                        ? jobParam
                        : cacheJobs.FirstOrDefault(x => x.JobName.Equals(jobParam.JobName));

                    ScheduleModConfig.Instance.Jobs.Add(jobParam);
                    if (jobParam.State.Equals(JobStateEnum.Open) || jobParam.State.Equals(JobStateEnum.Pause))
                        job.StartJob(jobParam);
                }
            }

            return true;
        }
    }
}