using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace Schedule
{
    /// <summary>  </summary>
    public class ScheduleModBackgroundService : BackgroundService
    {
        /// <summary>
        ///     This method is called when the <see cref="IHostedService" /> starts. The implementation should return a task that
        ///     represents
        ///     the lifetime of the long running operation(s) being performed.
        /// </summary>
        /// <param name="stoppingToken">Triggered when <see cref="IHostedService.StopAsync(CancellationToken)" /> is called.</param>
        /// <returns>A <see cref="Task" /> that represents the long running operations.</returns>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await ScheduleModIniter.Instance.InitAsync(stoppingToken: stoppingToken);
        }

        /// <summary>
        ///     Triggered when the application host is performing a graceful shutdown.
        /// </summary>
        /// <param name="cancellationToken">ndicates that the shutdown process should no longer be graceful.</param>
        /// <returns></returns>
        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            if (ScheduleModConfig.Instance.DefaultScheduler != null)
            {
                await ScheduleModConfig.Instance.DefaultScheduler.Shutdown(true, cancellationToken);
            }
            await base.StopAsync(cancellationToken);

        }
    }
}
