using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.WindNight.Hosting.@internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.WnExtension;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json.Extension;
using WindNight.Core.Abstractions;
using WindNight.Core.Abstractions.SvrMonitor;

namespace Microsoft.AspNetCore.Hosting.WnExtensions
{
    internal static class SvrMonitorExtension
    {
        public static IServiceCollection AddSvrMonitorService(this IServiceCollection services, IConfiguration configuration)
        {
            if (ConfigItems.SvrMonitorOpen)
            {
                services.AddHostedService<SvrMonitorBackgroundService>();
            }

            return services;
        }

    }

    internal partial class SvrMonitorBackgroundService : BackgroundService
    {

        public SvrMonitorBackgroundService()
        {

        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {

            RegisterHeartRun();
            return Task.CompletedTask;

        }

        /// <summary>
        /// Triggered when the application host is ready to start the service.
        /// </summary>
        /// <param name="cancellationToken">Indicates that the start process has been aborted.</param>
        /// <returns>A <see cref="T:System.Threading.Tasks.Task" /> that represents the asynchronous Start operation.</returns>
        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            // LogHelper.LogRegisterInfo("");
            SvrCenterHelper.PushSvrRegisterInfo();

            await base.StartAsync(cancellationToken);
        }

        /// <summary>
        /// Triggered when the application host is performing a graceful shutdown.
        /// </summary>
        /// <param name="cancellationToken">Indicates that the shutdown process should no longer be graceful.</param>
        /// <returns>A <see cref="T:System.Threading.Tasks.Task" /> that represents the asynchronous Stop operation.</returns>
        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            //   LogHelper.LogOfflineInfo("");
            SvrCenterHelper.PushSvrOfflineInfo();
            await base.StopAsync(cancellationToken);

        }



    }

    internal partial class SvrMonitorBackgroundService
    {
        protected virtual void DoMonitor()
        {
            try
            {
                SvrCenterHelper.PushSvrHeartInfo();
            }
            catch (Exception ex)
            {
                LogHelper.Error($"PushSvrHeartInfo Handler Error {ex.Message}", ex);
            }

        }
    }

    internal partial class SvrMonitorBackgroundService
    {
        protected volatile Thread _keepAliveThread;
        /// <summary>分钟</summary>
        protected virtual int SleepTime { get; } = 30;

        protected bool OpenDebug => ConfigItems.OpenDebug;

        /// <summary>注册心跳刷新</summary>
        protected void RegisterHeartRun()
        {
            try
            {
                _keepAliveThread = new Thread(KeepAliveThread)
                {
                    Name = "KeepAliveThread:SvrMonitor",
                    IsBackground = true,
                };
                _keepAliveThread.Start();
            }
            catch (Exception ex)
            {
                LogHelper.Error("RegisterHeartRun handler error ," + ex.Message, ex);
            }
        }

        protected virtual void KeepAliveThread()
        {
            var ticks = DateTime.Now.Ticks;
            var loop = 0L;
            while (true)
            {
                var flag = false;
                var isContinue = false;
                try
                {
                    ++loop;

                    if ((int)TimeSpan.FromTicks(DateTime.Now.Ticks - ticks).TotalMinutes < SleepTime)
                    {
                        Thread.Sleep(100);
                        isContinue = true;
                        continue;
                    }


                    DoMonitor();

                    flag = true;
                    Thread.Sleep(20);
                }
                catch (Exception ex)
                {
                    LogHelper.Error(_keepAliveThread.Name + " 心跳包 HeartRun  handler error ," + ex.Message, ex);
                }
                finally
                {
                    if (loop > long.MaxValue - 100)
                    {
                        loop = 0L;
                    }

                    if (flag)
                    {
                        ticks = DateTime.Now.Ticks;
                    }

                    if (!isContinue && OpenDebug)
                    {
                        LogHelper.Debug($"{_keepAliveThread.Name}  心跳包 HeartRun 执行 {(flag ? "成功" : "失败")}！ ReInit loopStartTimeTicks={DateTime.Now:yyyy-MM-dd HH:mm:ss} loop({loop}) ");
                    }

                    if (!flag)
                    {
                        Thread.Sleep(61000);
                    }
                }
            }
        }

    }

    public class SvrCenterContextHelper
    {
        private SvrCenterContextHelper()
        {

        }

        private static readonly Lazy<SvrCenterContextHelper> LazyInstance = new(() => new SvrCenterContextHelper());

        public static SvrCenterContextHelper Instance => LazyInstance.Value;


        public ISvrCenterRegisteredInfo SvrCenterRegisterInfo { get; private set; }

        public string SvrToken => SvrCenterRegisterInfo?.SvrToken ?? "";
        public long SvrTokenExpireTs => SvrCenterRegisterInfo?.SvrTokenExpireTs ?? 0;

        public IEnumerable<string> ServerIps => SvrCenterRegisterInfo?.ServerIps ?? new List<string>();

        public void Registered2SvrCenter(ISvrCenterReportRes res)
        {
            if (res == null)
            {
                return;
            }
            var info = new SvrCenterRegisteredInfo
            {
                RegisteredRes = res,
                AppId = ConfigItems.SystemAppId,
                AppCode = ConfigItems.SystemAppCode,
                AppName = ConfigItems.SystemAppName,

            };
            SvrCenterRegisterInfo = info;

        }

        public void UpdateSvrHeartInfo(ISvrCenterReportRes res)
        {
            if (res == null)
            {
                return;
            }
            if (res.Success)
            {
                if (res.SvrToken.IsNotNullOrEmpty())
                {
                    if (SvrCenterRegisterInfo == null)
                    {
                        Registered2SvrCenter(res);
                        return;
                    }
                    if (!res.SvrToken.Equals(SvrToken, StringComparison.OrdinalIgnoreCase))
                    {
                        SvrCenterRegisterInfo.UpdateRegisteredRes(res);
                        return;
                    }

                    var limitTs = HardInfo.Now.AddMinutes(60).ConvertToUnixTime();
                    if (SvrTokenExpireTs <= limitTs)
                    {
                        SvrCenterRegisterInfo.UpdateRegisteredRes(res);
                        return;
                    }
                }
            }

        }

    }




    internal class SvrCenterRegisteredInfo : ISvrCenterRegisteredInfo
    {
        public string AppId { get; set; } = string.Empty;
        public string AppName { get; set; } = string.Empty;
        public string AppCode { get; set; } = string.Empty;
        public string SvrToken => RegisteredRes?.SvrToken ?? "";
        public long SvrTokenExpireTs => RegisteredRes?.SvrTokenExpireTs ?? 0;

        public IEnumerable<string> ServerIps => RegisteredRes?.ServerIps ?? new List<string>();

        public ISvrCenterReportRes RegisteredRes { get; set; }

        public void UpdateRegisteredRes(ISvrCenterReportRes res)
        {
            RegisteredRes = res;
        }

    }

    internal class SvrCenterHelper
    {
        static ISvrCenterMonitorApp _svrCenterMonitorApp => Ioc.GetService<ISvrCenterMonitorApp>();
        public static bool PushSvrRegisterInfo()
        {
            Task.Run(() =>
            {
                try
                {
                    var svrInfo = HardInfo.GenSvrMonitorInfo(SvrMonitorTypeEnum.Register);
                    if (_svrCenterMonitorApp != null)
                    {
                        var res = _svrCenterMonitorApp.PushSvrRegisterInfo(svrInfo);

                        var flag = res?.Success ?? false;
                        if (flag)
                        {
                            SvrCenterContextHelper.Instance.Registered2SvrCenter(res);
                        }
                    }
                    else
                    {
                        LogHelper.Warn($" ISvrCenterMonitorApp NOT Impl  SvrRegisterInfo is {svrInfo.ToJsonStr()}");

                    }
                }
                catch (Exception ex)
                {

                    LogHelper.Error($"PushSvrRegisterInfo handler Error {ex.Message}", ex);
                }
            });

            return true;
        }

        public static bool PushSvrOfflineInfo()
        {
            Task.Run(() =>
            {
                try
                {
                    var svrInfo = HardInfo.GenSvrMonitorInfo(SvrMonitorTypeEnum.Offline);
                    if (_svrCenterMonitorApp != null)
                    {

                        var res = _svrCenterMonitorApp.PushSvrOfflineInfo(svrInfo);
                    }
                    else
                    {
                        LogHelper.Warn($" ISvrCenterMonitorApp NOT Impl SvrOfflineInfo is {svrInfo.ToJsonStr()}");
                    }
                    //return res.Success;
                }
                catch (Exception ex)
                {
                    LogHelper.Error($"PushSvrOfflineInfo handler Error {ex.Message}", ex);
                }

            });
            return true;
        }

        public static bool PushSvrHeartInfo()
        {
            try
            {
                if (_svrCenterMonitorApp != null)
                {

                    var svrInfo = HardInfo.GenSvrMonitorInfo(SvrMonitorTypeEnum.Heartbeat);
                    var res = _svrCenterMonitorApp.PushSvrHeartInfo(svrInfo);
                    var flag = res?.Success ?? false;
                    if (flag)
                    {
                        SvrCenterContextHelper.Instance.UpdateSvrHeartInfo(res);
                    }
                    return flag;
                }

            }
            catch (Exception ex)
            {
                LogHelper.Error($"PushSvrHeartInfo handler Error {ex.Message}", ex);
                return false;
            }
            return false;
        }
    }
}
