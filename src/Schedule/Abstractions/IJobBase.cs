using System.Threading.Tasks;
using Quartz;

namespace WindNight.Core.Abstractions
{
    public interface IJobBase
    {
        bool CanRunTest { get; }

        string CurrentJobCode { get; }

        Task<bool> DoJobAsync(IJobExecutionContext context);


        //Task<bool> RunTestAtStartAsync();

        bool RunTestAtStart();

    }
}
