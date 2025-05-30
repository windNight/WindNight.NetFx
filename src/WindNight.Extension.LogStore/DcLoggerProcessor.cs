using System.Collections.Concurrent;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Microsoft.Extensions.DependencyInjection.WnExtension;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Extension;
using WindNight.Core.Abstractions;
using WindNight.Extension.Logger.DcLog.Abstractions;
using WindNight.Extension.Logger.DcLog.@internal;

namespace WindNight.Extension.Logger.DcLog
{
    public class DcLoggerProcessor : IDcLoggerProcessor
    {
        private const int OpenGZipLimit = 150_000;
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

        bool CheckLogLevel(SysLogs message)
        {
            try
            {
                var dcLogOption = Ioc.GetService<IOptionsMonitor<DcLogOptions>>().CurrentValue;
                if (message.LevelType == (int)LogLevels.None)
                {
                    return false;
                }

                if (message.LevelType >= (int)dcLogOption.MinLogLevel || message.IsForce)
                {
                    return true;
                }

                if (message.LevelType >= (int)ConfigItems.GlobalMiniLogLevel)
                {
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {

                return true;
            }


        }

        protected bool IsStop { get; set; }

        /// <summary> </summary>
        public virtual void EnqueueMessage(SysLogs message)
        {
            if (!CheckLogLevel(message))
            {
                return;
            }
            // if (message.ToLower().Contains($"{nameof(SysLogs)}".ToLower())) return;
            if (DcLogOptions.IsOpenDebug)
            {
                Console.WriteLine($"EnqueueMessage({message.ToJsonStr()})");
            }
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
            {
                Console.WriteLine("start processlog to sender");
            }
            Thread.CurrentThread.Name = "DbLoggerProcessor-sender";
            while (true)
            {
                if (IsStop) break;
                try
                {
                    if (MessageQueue.TryDequeue(out var message))
                    {
                        if (!CheckLogLevel(message))
                        {
                            continue;
                        }
                        ProcessLog(message);
                    }
                    else
                    {
                        Thread.Sleep(100);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    Thread.Sleep(200);
                }
            }
        }

        /// <summary> </summary>
        protected virtual void BackupThread()
        {
            if (DcLogOptions.IsConsoleLog)
            {
                Console.WriteLine("start backupThread to sender batch");
            }
            Thread.CurrentThread.Name = "DbLoggerProcessor-backup-sender";
            while (true)
            {
                if (IsStop)
                {
                    if (MessageQueue.Count > 0)
                    {
                        if (DcLogOptions.IsConsoleLog)
                        {
                            Console.WriteLine("start backupThread Before Stop to sender batch");
                        }
                        ProcessBackupLogs();
                    }

                    break;
                }

                try
                {
                    if (_stopwatch.ElapsedMilliseconds >= 1000 * 60 * 5 && MessageQueue.Count >= DcLogOptions.QueuedMaxMessageCount)
                    {
                        if (DcLogOptions.IsConsoleLog)
                        {
                            Console.WriteLine("start backupThread to sender batch");
                        }
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
            {
                Console.WriteLine($"ProcessLog({message})");
            }

            using (var udpClient = new UdpClient())
            {
                try
                {
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
            {
                Console.WriteLine($"ProcessLog({list.ToJsonStr()})");
            }

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
