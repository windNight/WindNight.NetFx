using System.Threading.Tasks;
using Schedule.Abstractions;
using Schedule.@internal;
using WindNight.Core.Abstractions;

namespace Schedule
{
    public class DefaultScheduleNotice : IScheduleNotice
    {
        public void DoNotice(IJobBaseInfo jobBaseInfo, string message)
        {
            DoNoticeAsync(jobBaseInfo, message).GetAwaiter().GetResult();
        }

        public async Task DoNoticeAsync(IJobBaseInfo jobBaseInfo, string message)
        {
            await DingTalkNoticeHelper.DoNoticeAsync(jobBaseInfo, message);
        }
    }
}
