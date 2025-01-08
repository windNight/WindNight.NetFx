using System;
using System.Text.Extension;
using System.Threading.Tasks;
using Quartz;
using WindNight.Core.Abstractions;
using WindNight.Extension;

namespace Schedule
{
    public abstract class CommonBaseJob<TJob> : CommonBaseJob
        where TJob : BaseJob, IJobBase, new()
    {
        public CommonBaseJob() : base()
        {

        }

        public override string CurrentJobCode => typeof(TJob).Name;

    }

    public abstract class CommonBaseJob : BaseJob, IJobBase
    {
        public CommonBaseJob()
        {
        }

        public abstract Task<bool> DoJobAsync();

       
        protected abstract int CurrentUserId { get; }

        protected abstract Func<bool, bool> PreTodo { get; }

     

        public override async Task Execute(IJobExecutionContext context)
        {
            await base.Execute(context);

        }

        #region RunTest


        public virtual bool CanRunTest => CurrentJobMeta?.CanRunTest ?? false;

        public override async Task<bool> RunTestAtStartAsync()
        {
            if (!CanRunTest)
            {
                return false;
            }

            var userId = CurrentUserId;
            CurrentItem.AddItem("serialnumber", GuidHelper.GenerateOrderNumber());
            CurrentItem.AddItem("CurrentJobUserId", userId);
            if (PreTodo != null)
            {
                var flag = PreTodo.Invoke(true);
                if (!flag)
                {
                    return false;
                }
            }



            return await DoJobAsync();
        }

        #endregion



    }


}
