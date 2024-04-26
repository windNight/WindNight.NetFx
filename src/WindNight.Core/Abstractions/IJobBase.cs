using System.Threading.Tasks;

namespace WindNight.Core.Abstractions
{
    public interface IJobBase
    {
        bool CanRunTest { get; }

        string CurrentJobCode { get; }

        Task<bool> DoJobAsync();


        Task<bool> RunTestAtStartAsync();
    }
}
