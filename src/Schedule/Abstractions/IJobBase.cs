using System.Threading.Tasks;
using Quartz;

namespace WindNight.Core.Abstractions
{
    public interface IJobBase
    {
        bool CanRunTest { get; }

        string CurrentJobCode { get; }

        //Task<bool> DoJobAsync(IJobExecutionContext context);
        Task<bool> ExecuteWithResultAsync(IJobExecutionContext context);


        Task<bool> RunTestAtStartAsync(int delayS = 2);

        bool RunTestAtStart(int delayS = 2);

    }
}
