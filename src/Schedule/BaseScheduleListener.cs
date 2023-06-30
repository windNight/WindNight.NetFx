using Microsoft.Extensions.DependencyInjection.WnExtension;
using Quartz;
using Schedule.Abstractions;
using Schedule.Ctrl;
using Schedule.Func;
using Schedule.Model.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Extension;
using System.Threading;
using System.Threading.Tasks;

namespace Schedule
{
    public class BaseScheduleListener<T> : IScheduleListener where T : IJob
    {
        protected readonly IScheduleNotice _scheduleNotice;

        /// <summary>
        ///     监听器名称
        /// </summary>
        private string __listenerName = string.Empty;

        public BaseScheduleListener()
        {
            _scheduleNotice = Ioc.GetService<IScheduleNotice>();
        }

        public virtual string JobCode => typeof(T).Name.ToLower();

        /// <summary>
        ///     原始名称
        /// </summary>
        protected string OrigName => $"{JobCode}_listener";

        /// <summary> </summary>
        protected string JobId { get; private set; } = string.Empty;

        /// <summary> </summary>
        protected string JobName { get; private set; } = string.Empty;

        protected bool IsDoNotice { get; private set; } = true;

        protected JobBaseInfo JobBaseInfo => new JobBaseInfo { JobCode = JobCode, JobId = JobId, JobName = JobName };

        /// <summary>
        ///     Get the name of the Quartz.IJobListener and the Quartz.ITriggerListener.
        /// </summary>
        public string Name => __listenerName.IsNullOrEmpty() ? OrigName : __listenerName;

        /// <summary>
        ///     Set the name of the Quartz.IJobListener and the Quartz.ITriggerListener.
        /// </summary>
        /// <param name="name"></param>
        public void SetName(string name)
        {
            __listenerName = name;
        }

        #region ITriggerListener

        /// <summary>
        ///     当与监听器相关联的 Trigger 被触发，Job 上的 execute() 方法将要被执行时，Scheduler 就调用这个方法。在全局 TriggerListener 情况下，这个方法为所有 Trigger 被调用。
        ///     Called by the Quartz.IScheduler when a Quartz.ITrigger has fired, and it's associated
        ///     Quartz.IJobDetail is about to be executed.
        ///     It is called before the
        ///     Quartz.ITriggerListener.VetoJobExecution(Quartz.ITrigger,Quartz.IJobExecutionContext,System.Threading.CancellationToken)
        ///     method of this interface.
        /// </summary>
        /// <param name="trigger">The Quartz.ITrigger that was fired.</param>
        /// <param name="context">
        ///     The Quartz.IJobExecutionContext that will be passed to the
        ///     Quartz.IJob'sQuartz.IJob.Execute(Quartz.IJobExecutionContext) method.
        /// </param>
        /// <param name="cancellationToken">The cancellation instruction.</param>
        /// <returns></returns>
        public virtual async Task TriggerFired(ITrigger trigger, IJobExecutionContext context,
            CancellationToken cancellationToken = default)
        {
            SetJobId(context);
            Debug(nameof(ITriggerListener), nameof(TriggerFired));

            await DoNoticeAsync($"begin with TriggerFired_ITriggerListener:NowTicks({DateTime.Now.Ticks})");

            await Task.CompletedTask;
        }

        /// <summary>
        ///     Scheduler 调用这个方法是在 Trigger 错过触发时。如这个方法的 JavaDoc 所指出的，你应该关注此方法中持续时间长的逻辑：在出现许多错过触发的 Trigger
        ///     时，长逻辑会导致骨牌效应。你应当保持这上方法尽量的小。
        ///     Called by the Quartz.IScheduler when a Quartz.ITrigger has misfired.
        ///     Consideration should be given to how much time is spent in this method, as it
        ///     will affect all triggers that are misfiring. If you have lots of triggers misfiring
        ///     at once, it could be an issue it this method does a lot.
        /// </summary>
        /// <param name="trigger">The Quartz.ITrigger that has misfired.</param>
        /// <param name="cancellationToken">The cancellation instruction.</param>
        /// <returns></returns>
        public virtual async Task TriggerMisfired(ITrigger trigger,
            CancellationToken cancellationToken = default)
        {
            Debug(nameof(ITriggerListener), nameof(TriggerMisfired));
            await Task.CompletedTask;
        }

        /// <summary>
        ///     Trigger 被触发并且完成了 Job 的执行时，Scheduler 调用这个方法。这不是说这个 Trigger 将不再触发了，而仅仅是当前 Trigger 的触发(并且紧接着的 Job 执行) 结束时。这个 Trigger
        ///     也许还要在将来触发多次的。
        ///     Called by the Quartz.IScheduler when a Quartz.ITrigger has fired, it's associated
        ///     Quartz.IJobDetail has been executed, and it's Quartz.Spi.IOperableTrigger.Triggered(Quartz.ICalendar)
        ///     method has been called.
        /// </summary>
        /// <param name="trigger">The Quartz.ITrigger that was fired.</param>
        /// <param name="context">
        ///     The Quartz.IJobExecutionContext that was passed to the
        ///     Quartz.IJob'sQuartz.IJob.Execute(Quartz.IJobExecutionContext) method.
        /// </param>
        /// <param name="triggerInstructionCode">
        ///     The result of the call on the
        ///     Quartz.ITrigger'sQuartz.Spi.IOperableTrigger.Triggered(Quartz.ICalendar) method.
        /// </param>
        /// <param name="cancellationToken">The cancellation instruction.</param>
        /// <returns></returns>
        public virtual async Task TriggerComplete(ITrigger trigger, IJobExecutionContext context,
            SchedulerInstruction triggerInstructionCode,
            CancellationToken cancellationToken = default)
        {
            Debug(nameof(ITriggerListener), nameof(TriggerComplete));
            var beginTicks = context.GetJobBeginDateTimeTicks();
            var milliseconds = (long)TimeSpan.FromTicks(DateTime.Now.Ticks - beginTicks).TotalMilliseconds;

            if (milliseconds > 5 * 1000)
                JobLogHelper.Warn($"{JobInfo} 耗时{milliseconds} ms ", null, nameof(TriggerComplete));
            else if (milliseconds > 0)
                JobLogHelper.Info($"{JobInfo} 耗时{milliseconds} ms ", nameof(TriggerComplete));
            var bizState = context.GetJobBusinessState();
            var bizStatsStr = bizState.ToString();
            if (bizState == JobBusinessStateEnum.Success)
            {
                bizStatsStr = $"<font color=#20CE43>{bizState}</font>";
            }
            if (bizState == JobBusinessStateEnum.Fail)
            {
                bizStatsStr = $"<font color=#FF0000>{bizState}</font>";
            }
            await DoNoticeAsync($"耗时 {milliseconds} ms. 任务状态为 {bizStatsStr}", context.GetTempConfig("BizContent").ToString());

            await Task.CompletedTask;
        }

        /// <summary>
        ///     在Trigger 触发后，Job 将要被执行时由 Scheduler 调用这个方法。TriggerListener 给了一个选择去否决 Job 的执行。假如这个方法返回 true，这个 Job 将不会为此次 Trigger
        ///     触发而得到执行
        ///     Called by the Quartz.IScheduler when a Quartz.ITrigger has fired, and it's associated
        ///     Quartz.IJobDetail is about to be executed.
        ///     It is called after the
        ///     Quartz.ITriggerListener.TriggerFired(Quartz.ITrigger,Quartz.IJobExecutionContext,System.Threading.CancellationToken)
        ///     method of this interface. If the implementation vetoes the execution (via returning
        ///     true), the job's execute method will not be called.
        /// </summary>
        /// <param name="trigger"> The Quartz.ITrigger that has fired.</param>
        /// <param name="context">
        ///     The Quartz.IJobExecutionContext that will be passed to the
        ///     Quartz.IJob'sQuartz.IJob.Execute(Quartz.IJobExecutionContext) method.
        /// </param>
        /// <param name="cancellationToken"> The cancellation instruction.</param>
        /// <returns> Returns true if job execution should be vetoed, false otherwise. </returns>
        public virtual async Task<bool> VetoJobExecution(ITrigger trigger, IJobExecutionContext context,
            CancellationToken cancellationToken = default)
        {
            Debug(nameof(ITriggerListener), nameof(VetoJobExecution));
            var origJobName = context.GetJobCode();
            //检查依赖选项是否满足
            var isOnceJob = context.IsOnceJob();
            var depJobs = context.GetDepJobs(); //jobcodes
            var isContinueRun = isOnceJob || depJobs.IsNullOrEmpty() || await WaitJobCompleted(origJobName, depJobs.Split(',').ToList(), DateTime.Now.Date);
            context.JobDetail.SetContinueRunFlag(isContinueRun);

            if (!isContinueRun)
            {
                Debug(nameof(ITriggerListener), origJobName + "未满足运行条件");
                // Do Notice
                await DoNoticeAsync($"前置job：{depJobs} 未全部完成！ ");
                return await Task.FromResult(true);
            }

            return await Task.FromResult(false);
        }

        #endregion //end ITriggerListener

        #region IJobListener

        /// <summary>
        ///     在Job执行前调用
        ///     Called by the Quartz.IScheduler when a Quartz.IJobDetail is about to be executed
        ///     (an associated Quartz.ITrigger has occurred).
        ///     This method will not be invoked if the execution of the Job was vetoed by a Quartz.ITriggerListener.
        /// </summary>
        /// <param name="context">
        ///     The Quartz.IJobExecutionContext that will be passed to the
        ///     Quartz.IJob'sQuartz.IJob.Execute(Quartz.IJobExecutionContext) method.
        /// </param>
        /// <param name="cancellationToken"> The cancellation instruction.</param>
        /// <returns></returns>
        public virtual async Task JobToBeExecuted(IJobExecutionContext context,
            CancellationToken cancellationToken = default)
        {
            Debug(nameof(IJobListener), nameof(JobToBeExecuted));
            try
            {
                var jobName = context.GetJobName();
                var jobCode = context.GetJobCode();
                var autoClose = context.GetAutoClose();
                var runParams = context.GetJobRunParams();

                var msg = $"{JobInfo} begin time:{DateTime.Now:yyyy-MM-dd HH:mm:sss}, autoClose:{autoClose}, runParams:{runParams}";
                JobLogHelper.Info(msg, nameof(JobToBeExecuted));

                // 检查依赖选项是否满足
                //  var isOnceJob = JobContextFunc.IsOnceJob(context);
                //  var depJobs = JobContextFunc.GetDepJobs(context);
                // var isContinueRun = (isOnceJob || string.IsNullOrEmpty(depJobs)) ? true : WaitJobCompleted(origJobName, depJobs.Split(',').ToList(), DateTime.Now.Date);
                //JobContextFunc.SetContinueRunFlag(context.JobDetail, isContinueRun);


                //记录任务开始执行
                // var jobParam = GetJobMeta(JobContextFunc.GetOrigJobName(context));

                var ctrl = Ioc.GetService<IScheduleOrderCtrl>();
                if (ctrl != null)
                    await ctrl.StartJobSafety(JobId, jobName, jobCode, runParams);

                /*
                 
                //TODO do job run log
                ScheduleOrderCtrl ctrl = new ScheduleOrderCtrl();
                var jobId = ctrl.StartJobSafety(jobName, origJobName, runParams);
                JobContextFunc.SetJobDbId(context.JobDetail, jobId.ToString());
                */

                //remove to TriggerFired do this.
                // JobContextFunc.SetJobDbId(context.JobDetail, GuidHelper.GetGuid()); 
            }
            catch (Exception ex)
            {
                JobLogHelper.Error($"{JobInfo}:JobToBeExecuted Error {ex}", ex, nameof(JobToBeExecuted));
            }

            // return TaskUtil.CompletedTask;
            await Task.CompletedTask;
        }

        /// <summary>
        ///     Job被否决运行后调用
        ///     Called by the Quartz.IScheduler when a Quartz.IJobDetail was about to be executed
        ///     (an associated Quartz.ITrigger has occurred), but a Quartz.ITriggerListener vetoed
        ///     it's execution.
        /// </summary>
        /// <param name="context">
        ///     The Quartz.IJobExecutionContext that will be passed to the
        ///     Quartz.IJob'sQuartz.IJob.Execute(Quartz.IJobExecutionContext) method.
        /// </param>
        /// <param name="cancellationToken">The cancellation instruction.</param>
        /// <returns></returns>
        public virtual async Task JobExecutionVetoed(IJobExecutionContext context,
            CancellationToken cancellationToken = default)
        {
            //TODO Add Notice 
            Debug(nameof(IJobListener), nameof(JobExecutionVetoed));
            await Task.CompletedTask;
            //return Task.FromResult(true);
        }

        protected void Debug(string interfaceName, string actionName)
        {
            JobLogHelper.Debug($"{JobInfo}:{actionName}:{interfaceName}:NowTicks({DateTime.Now.Ticks})", actionName);
        }

        /// <summary>
        ///     Job完成后调用
        ///     Called by the Quartz.IScheduler after a Quartz.IJobDetail has been executed,
        ///     and be for the associated Quartz.Spi.IOperableTrigger's Quartz.Spi.IOperableTrigger.Triggered(Quartz.ICalendar)
        ///     method has been called.
        /// </summary>
        /// <param name="context">
        ///     The Quartz.IJobExecutionContext that will be passed to the
        ///     Quartz.IJob'sQuartz.IJob.Execute(Quartz.IJobExecutionContext) method.
        /// </param>
        /// <param name="cancellationToken">The cancellation instruction.</param>
        /// <param name="jobException"></param>
        /// <returns></returns>
        public virtual async Task JobWasExecuted(IJobExecutionContext context, JobExecutionException jobException,
            CancellationToken cancellationToken = default)
        {
            Debug(nameof(IJobListener), nameof(JobWasExecuted));
            var retCode = 0;
            try
            {
                var jobName = context.GetJobName();
                // var jobCode = JobContextFunc.GetJobCode(context);

                var autoClose = context.GetAutoClose();
                var runParams = context.GetJobRunParams();

                // 如果是单次运行，则删除job 
                // OnceJob not support now 
                if (context.IsOnceJob())
                {
                    var delJobRet = await new ScheduleCtrl().StopJobAsync(jobName);
                    if (delJobRet != JobActionRetEnum.Success)
                        JobLogHelper.Error(
                            $"{JobInfo}:单次运行job结束后删除job失败，job ：{JobContext.CurrentJobBaseInfo}，返回结果：{delJobRet }", null, nameof(JobWasExecuted));
                }

                var jobId = JobId;

                var jobRunState = JobRunStateEnum.Ok;

                if (jobException != null)
                {
                    jobRunState = JobRunStateEnum.Exception;
                    JobLogHelper.Error($"{JobInfo} 发生异常",
                        jobException, nameof(JobWasExecuted));
                }
                else
                {
                    var jobBusinessState = context.GetJobBusinessState();
                    switch (jobBusinessState)
                    {
                        case JobBusinessStateEnum.Unknown: //基本上是没有继承BaseJob的
                            JobLogHelper.Info($"{JobInfo} 获取到Job的业务状态为 {JobBusinessStateEnum.Unknown},可能是该Job 未继承BaseJob ", nameof(JobWasExecuted));
                            break;
                        case JobBusinessStateEnum.Success:
                            break;
                        case JobBusinessStateEnum.Fail:
                            jobRunState = JobRunStateEnum.BusinessError;
                            break;
                        default:
                            jobRunState = JobRunStateEnum.Error;
                            JobLogHelper.Error(
                                $"{JobInfo} 发生异常，无法获取到Job的业务状态", null, nameof(JobWasExecuted));
                            break;
                    }
                }

                var msg =
                    $"{JobInfo} end time:{DateTime.Now:yyyy-MM-dd HH:mm:sss}, jobRunState:{jobRunState},autoClose:{autoClose}, runParams:{runParams}";
                JobLogHelper.Info(msg, nameof(JobWasExecuted));

                // DoJobLog 
                var ctrl = Ioc.GetService<IScheduleOrderCtrl>();
                if (ctrl != null)
                    await ctrl.CompleteJobSafety(JobId, jobRunState);
            }
            catch (Exception ex)
            {
                JobLogHelper.Info($"{JobInfo}: JobWasExecuted Error {ex}", nameof(JobWasExecuted));
                retCode = 1;
            }
            finally
            {
                var autoClose = JobContextFunc.GetAutoClose(context);
                if (autoClose)
                    // TODO 通知注册中心下线 
                    Environment.Exit(retCode);
            }

            await Task.CompletedTask;
            //  return TaskUtil.CompletedTask;
        }

        #endregion //end IJobListener

        protected string JobInfo => $"{JobName} ({JobCode}) :{JobId}";

        /// <summary>
        ///     等待job结束
        /// </summary>
        /// <param name="sourceJob"></param>
        /// <param name="jobCodes"></param>
        /// <param name="jobStartTime"></param>
        /// <returns>返回true未等待成功，false为等待失败</returns>
        private async Task<bool> WaitJobCompleted(string sourceJob, List<string> jobCodes, DateTime jobStartTime)
        {
            var ctrl = Ioc.GetService<IScheduleOrderCtrl>();
            if (ctrl == null) return await Task.FromResult(true);

            while (true)
            {
                var allJobRunState = await ctrl.WaitJobCompleted(sourceJob, jobCodes, jobStartTime);

                if (allJobRunState == JobRunStateEnum.Ok) return await Task.FromResult(true);

                if (allJobRunState == JobRunStateEnum.Running)
                {
                    Thread.Sleep(10000);
                    continue;
                }

                return await Task.FromResult(false);
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        protected void SetJobId(IJobExecutionContext context)
        {
            var jobId = GuidHelper.GetGuid();
            context.JobDetail.SetJobDbId(jobId);
            context.JobDetail.SetJobBeginDateTimeTicks();
            JobId = jobId;
            JobName = context.GetJobName();
            IsDoNotice = context.GetIsDoNotice();
            JobContext.SetCurrentJobBaseInfo(JobId, JobCode, JobName);
        }

        protected async Task DoNoticeAsync(string message, string extendInfo = "")
        {
            if (_scheduleNotice != null && IsDoNotice)
            {
                await _scheduleNotice.DoNoticeAsync(JobBaseInfo, $"{message}{(extendInfo.IsNullOrEmpty() ? "" : $"\n\n{extendInfo}")}");
            }

            await Task.CompletedTask;
        }




    }
}