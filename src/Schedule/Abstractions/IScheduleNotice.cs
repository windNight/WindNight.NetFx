using System.Threading.Tasks;

namespace Schedule.Abstractions
{
    /// <summary>
    /// </summary>
    public interface IScheduleNotice
    {
        void DoNotice(JobBaseInfo jobBaseInfo, string message);
        Task DoNoticeAsync(JobBaseInfo jobBaseInfo, string message);
    }
}