using System.Threading.Tasks;
using WindNight.Core.Abstractions;

namespace Schedule.Abstractions
{
    /// <summary>
    /// </summary>
    public interface IScheduleNotice
    {
        void DoNotice(IJobBaseInfo jobBaseInfo, string message);
        Task DoNoticeAsync(IJobBaseInfo jobBaseInfo, string message);
    }
}
