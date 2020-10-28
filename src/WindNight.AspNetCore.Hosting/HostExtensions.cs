﻿using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.WindNight.Hosting.Internals;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.Extensions.Hosting.WnExtensions
{
    public static class HostExtensions
    {
        public static async Task InjectionRSAsync(this IHost host, string buildType)
        {
            // Get the lifetime object from the DI container
            var applicationLifetime = host.Services.GetService<IHostApplicationLifetime>();

            // Create a new TaskCompletionSource called waitForStop
            var waitForStop = new TaskCompletionSource<object>(TaskCreationOptions.RunContinuationsAsynchronously);
            applicationLifetime.ApplicationStarted.Register(() => { LogHelper.LogRegisterInfo(buildType); }
            );

            // Register a callback with the ApplicationStopping cancellation token
            applicationLifetime.ApplicationStopping.Register(obj =>
            {
                var tcs = (TaskCompletionSource<object>)obj;

                //PUT YOUR CODE HERE 

                LogHelper.LogOfflineInfo(buildType);

                Thread.Sleep(200);
                // When the application stopping event is fired, set 
                // the result for the waitForStop task, completing it
                tcs.TrySetResult(null);
            }, waitForStop);

            // Await the Task. This will block until ApplicationStopping is triggered,
            // and TrySetResult(null) is called
            //await waitForStop.Task;

            // We're shutting down, so call StopAsync on IHost
            //await host.StopAsync();
        }
    }
}