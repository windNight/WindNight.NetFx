using Microsoft.Extensions.Options;
using Newtonsoft.Json.Extension;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.WnExtension;
using WindNight.Extension.Logger.DcLog.Abstractions;
using System.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WindNight.Core.Abstractions;
using WindNight.Extension.Logger.DcLog.Internal;

namespace WindNight.Extension.Logger.DcLog
{
    public class DcLoggerProcessor : IDcLoggerProcessor
    {


        private const int OpenGZipLimit = 15_00;
        private const string GZipFlagStr = "gzip@";
        private readonly Stopwatch _stopwatch = new Stopwatch();
        //private ISystemLogsProcess _repo => Ioc.GetService<ISystemLogsProcess>();
        /// <summary> </summary>
        protected readonly DcLogOptions DcLogOptions;
        private IPEndPoint EndPoint { get; set; }

        /// <summary> </summary>
        protected readonly ConcurrentQueue<SysLogs> MessageQueue;


        /// <summary>
        /// </summary>
        /// <param name="options"></param>
        public DcLoggerProcessor(IOptionsMonitor<DcLogOptions> options)
        {
            DcLogOptions = options.CurrentValue;
            _stopwatch.Start();
            MessageQueue = new ConcurrentQueue<SysLogs>();
            EndPoint = new IPEndPoint(Dns.GetHostAddresses(DcLogOptions.HostName)[0], DcLogOptions.Port);

            Task.Run(ProcessLogQueueThread);
            Task.Run(BackupThread);

        }


        protected bool IsStop { get; set; }

        /// <summary> </summary>
        public virtual void EnqueueMessage(SysLogs message)
        {
            // if (message.ToLower().Contains($"{nameof(SysLogs)}".ToLower())) return;
            if (DcLogOptions.IsOpenDebug)
                Console.WriteLine($"EnqueueMessage({message.ToJsonStr()})");
            MessageQueue.Enqueue(message);
        }

        /// <summary>
        /// </summary>
        public void Dispose()
        {
            ClearQueue();
            IsStop = true;
        }

        /// <summary> </summary>
        protected virtual void ProcessLogQueueThread()
        {
            if (DcLogOptions.IsConsoleLog)
                Console.WriteLine("start processlog to sender");
            Thread.CurrentThread.Name = "DbLoggerProcessor-sender";
            while (true)
            {
                if (IsStop) break;
                try
                {
                    if (MessageQueue.TryDequeue(out var message))
                        ProcessLog(message);
                    else
                        Thread.Sleep(100);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    Thread.Sleep(100);
                }
            }
        }

        /// <summary> </summary>
        protected virtual void BackupThread()
        {
            if (DcLogOptions.IsConsoleLog)
                Console.WriteLine("start backupThread to sender batch");
            Thread.CurrentThread.Name = "DbLoggerProcessor-backup-sender";
            while (true)
            {
                if (IsStop)
                {
                    if (MessageQueue.Count > 0)
                    {
                        if (DcLogOptions.IsConsoleLog)
                            Console.WriteLine("start backupThread Before Stop to sender batch");
                        ProcessBackupLogs();
                    }

                    break;
                }

                try
                {
                    if (_stopwatch.ElapsedMilliseconds >= 1000 * 60 * 5 &&
                        MessageQueue.Count >= DcLogOptions.QueuedMaxMessageCount)
                    {
                        if (DcLogOptions.IsConsoleLog)
                            Console.WriteLine("start backupThread to sender batch");
                        Debug.Assert(MessageQueue.Count >= DcLogOptions.QueuedMaxMessageCount,
                            $"Current Length In Queue is {MessageQueue.Count}");
                        ProcessBackupLogs();
                        _stopwatch.Restart();
                    }
                    else
                    {
                        Thread.Sleep(1000);
                    }
                }
                catch
                {
                    Thread.Sleep(1000);
                }
            }
        }

        private void ClearQueue()
        {
#if !NETSTANDARD2_0
            MessageQueue.Clear();
#else
            if (!MessageQueue.IsEmpty)
                for (var i = 0; i < MessageQueue.Count; i++)
                    MessageQueue.TryDequeue(out var msg);
#endif
        }

        private void ProcessLog(SysLogs message)
        {
            if (DcLogOptions.IsOpenDebug)
                Console.WriteLine($"ProcessLog({message})");

            using (var udpClient = new UdpClient())
            {
                try
                {
                    //var obj = new
                    //{
                    //    AppCode = message.LogAppCode,
                    //    Items = message,
                    //};
                    //var data = obj.ToJsonStr().ToBytes();// Encoding.UTF8.GetBytes();

                    if (message.LogAppCode.IsNullOrEmpty())
                    {
                        var config = Ioc.GetService<IConfigService>();
                        if (config != null)
                        {
                            message.LogAppCode = config.SystemAppCode;
                            message.LogAppName = config.SystemAppName;
                        }

                        if (message.LogAppCode.IsNullOrEmpty())
                        {
                            var s = new Microsoft.Extensions.DependencyInjection.ServiceCollection();
                            var config1 = s.BuildServiceProvider().GetService<IConfiguration>();

                            var d1 = config1?.GetAppConfigValue("AppId", 0, false) ?? 0;
                            var d2 = config1?.GetAppConfigValue("AppCode", "", false) ?? "";
                            var d3 = config1?.GetAppConfigValue("AppName", "", false) ?? "";

                            return;
                        }

                    }
                    var data = message.ToJsonStr().ToBytes();// Encoding.UTF8.GetBytes();
                    var sendData = FixSendContent(data);
                    var count = udpClient.Send(sendData, sendData.Length, EndPoint);
                    if (count != sendData.Length)
                    {
                        Console.WriteLine($"Send Msg:{message}  Count ({count})");
                    }
                    if (DcLogOptions.IsConsoleLog)
                    {
                        Console.WriteLine($"send msg success {EndPoint.Address}:{EndPoint.Port} :{message.ToJsonStr()}, Current Length In Queue is {MessageQueue.Count}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Send Msg:{message} Handler Error {ex.Message}");
                }
            }

        }


        private void ProcessLog(params SysLogs[] messages)
        {
            var list = messages.ToList();
            if (DcLogOptions.IsOpenDebug)
                Console.WriteLine($"ProcessLog({list.ToJsonStr()})");

            using (var udpClient = new UdpClient())
            {
                foreach (var message in messages)
                {

                    try
                    {
                        //var obj = new
                        //{
                        //    AppCode = message.LogAppCode,
                        //    Items = message,
                        //};
                        //var data = obj.ToJsonStr().ToBytes();// Encoding.UTF8.GetBytes();
                        var data = message.ToJsonStr().ToBytes();// Encoding.UTF8.GetBytes();
                        var sendData = FixSendContent(data);
                        udpClient.Send(sendData, sendData.Length, EndPoint);
                        if (DcLogOptions.IsConsoleLog)
                        {
                            Console.WriteLine($"send msg success {EndPoint.Address}:{EndPoint.Port} :{message.ToJsonStr()}, Current Length In Queue is {MessageQueue.Count}");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Send Msg:{message} Handler Error {ex.Message}");
                    }
                }

            }

        }

        private byte[] FixSendContent(byte[] originBytes)
        {
            if (DcLogOptions.OpenGZip && originBytes.Length > OpenGZipLimit)
                return MergerGZipFlag(originBytes);

            return originBytes;
        }

        private byte[] MergerGZipFlag(byte[] originBytes)
        {
            var afterGZip = originBytes.DoCompress();
            var header = GZipFlagStr.ToBytes();

            return Combine(header, afterGZip);
        }

        public byte[] Combine(params byte[][] arrays)
        {
            var ret = new byte[arrays.Sum(x => x.Length)];
            var offset = 0;
            foreach (var data in arrays)
            {
                Buffer.BlockCopy(data, 0, ret, offset, data.Length);
                offset += data.Length;
            }

            return ret;
        }


        private void ProcessBackupLogs()
        {
            var oldQueue = new SysLogs[MessageQueue.Count];
            MessageQueue.CopyTo(oldQueue, 0);
            ClearQueue();
            ProcessLog(oldQueue);
            oldQueue = null;
        }

    }
}
