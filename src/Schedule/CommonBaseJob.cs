using Microsoft.Extensions.DependencyInjection.WnExtension;
using Quartz;
using Schedule.Abstractions;
using Schedule.@internal;
using Schedule.Model.Enums;
using WindNight.Core.Abstractions;
using WindNight.Extension;

namespace Schedule
{
    public abstract class CommonBaseJob<TJob> : CommonBaseJob
        where TJob : BaseJob, IJobBase, new()
    {
        public override string CurrentJobCode => typeof(TJob).Name;
    }

    public abstract class CommonBaseJob : BaseJob, IJobBase
    {
        protected bool OpenDebug => ConfigItems.OpenDebug;

        //public abstract Task<bool> DoJobAsync(IJobExecutionContext context);

        protected abstract int CurrentUserId { get; }

        protected abstract Func<bool, bool> PreTodo { get; }

        public override async Task<JobBusinessStateEnum> ExecuteWithResultAsync(IJobExecutionContext context)
        {
            return await DoBizJobAsync(context);
        }

        public abstract Task<JobBusinessStateEnum> DoBizJobAsync(IJobExecutionContext context);


        #region RunTest

        public virtual bool CanRunTest => CurrentJobMeta?.CanRunTest ?? false;


        public override bool RunTestAtStart(int delayS = 2)
        {
            if (!CanRunTest)
            {
                return false;
            }

            var userId = CurrentUserId;
            var traceId = CurrentItem.GetSerialNumber;
            CurrentItem.AddItem("CurrentJobUserId", userId);
            if (PreTodo != null)
            {
                var flag = PreTodo.Invoke(true);
                if (!flag)
                {
                    return false;
                }
            }

            var sc = Ioc.GetService<ICommandCtrl>();
            var res = sc?.StartJob(CurrentJobCode, HardInfo.Now.AddSeconds(delayS), "oncejob:RunTestAtStart", false);
            return true;
        }


        public override async Task<bool> RunTestAtStartAsync(int delayS = 2)
        {
            if (!CanRunTest)
            {
                return false;
            }

            var userId = CurrentUserId;
            var traceId = CurrentItem.GetSerialNumber;
            CurrentItem.AddItem("CurrentJobUserId", userId);
            if (PreTodo != null)
            {
                var flag = PreTodo.Invoke(true);
                if (!flag)
                {
                    return false;
                }
            }

            var sc = Ioc.GetService<ICommandCtrl>();
            var res = sc?.StartJob(CurrentJobCode, HardInfo.Now.AddSeconds(delayS), "oncejob:RunTestAtStart", false);
            return await Task.FromResult(true);
        }

        #endregion
    }
}
