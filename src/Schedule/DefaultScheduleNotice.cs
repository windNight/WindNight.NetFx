using System.Threading.Tasks;
using Schedule.Abstractions;
using Schedule.Internal;

namespace Schedule
{
    public class DefaultScheduleNotice : IScheduleNotice
    { 
        public void DoNotice(JobBaseInfo jobBaseInfo, string message)
        {
            DoNoticeAsync(jobBaseInfo, message).GetAwaiter().GetResult();
        }

        public async Task DoNoticeAsync(JobBaseInfo jobBaseInfo, string message)
        {
            await DingTalkNoticeHelper.DoNoticeAsync(jobBaseInfo, message);
        }
    }
}