using Microsoft.Extensions.Options;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Sockets;
using System.Net;
using System.Text;
using WindNight.Extension.Logger.DbLog.Abstractions;

namespace WindNight.Extension.Logger.DbLog
{
    public class DbLoggerProcessor : IDbLoggerProcessor
    {


        private const int OpenGZipLimit = 15_00;
        private const string GZipFlagStr = "gzip@";
        private readonly Stopwatch _stopwatch = new Stopwatch();

        /// <summary> </summary>
        protected readonly DbLogOptions DbLogOptions;

        /// <summary> </summary>
        protected readonly ConcurrentQueue<string> MessageQueue;

        /// <summary>
        /// </summary>
        /// <param name="lyLogOptions"></param>
        public DbLoggerProcessor(IOptionsMonitor<DbLogOptions> options)
        {
            DbLogOptions = options.CurrentValue;
            _stopwatch.Start();
            MessageQueue = new ConcurrentQueue<string>();

            Task.Run(ProcessLogQueueThread);
            Task.Run(BackupThread);
        }


        protected bool IsStop { get; set; }

        /// <summary> </summary>
        public virtual void EnqueueMessage(string message)
        {
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
            if (DbLogOptions.IsConsoleLog)
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
            if (DbLogOptions.IsConsoleLog)
                Console.WriteLine("start backupThread to sender batch");
            Thread.CurrentThread.Name = "DbLoggerProcessor-backup-sender";
            while (true)
            {
                if (IsStop)
                {
                    if (MessageQueue.Count > 0)
                    {
                        if (DbLogOptions.IsConsoleLog)
                            Console.WriteLine("start backupThread Before Stop to sender batch");
                        ProcessBackupLogs();
                    }

                    break;
                }

                try
                {
                    if (_stopwatch.ElapsedMilliseconds >= 1000 * 60 * 5 &&
                        MessageQueue.Count >= DbLogOptions.QueuedMaxMessageCount)
                    {
                        if (DbLogOptions.IsConsoleLog)
                            Console.WriteLine("start backupThread to sender batch");
                        Debug.Assert(MessageQueue.Count >= DbLogOptions.QueuedMaxMessageCount,
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

        private void ProcessLog(params string[] messages)
        {
            // using (var udpClient = new UdpClient())
            // {
            foreach (var message in messages)
                try
                {
                    //  var data = Encoding.UTF8.GetBytes(message);
                    // var sendData = FixSenDbontent(data);
                    //  udpClient.Send(sendData, sendData.Length, EndPoint);
                    if (DbLogOptions.IsConsoleLog)
                    {
                        //Console.WriteLine($"send msg success {EndPoint.Address}:{EndPoint.Port} :{message}, Current Length In Queue is {MessageQueue.Count}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Send Msg:{message} Handler Error {ex.Message}");
                }
            //  }
        }

        private byte[] FixSenDbontent(byte[] originBytes)
        {
            if (DbLogOptions.OpenGZip && originBytes.Length > OpenGZipLimit) return MergerGZipFlag(originBytes);

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
            var oldQueue = new string[MessageQueue.Count];
            MessageQueue.CopyTo(oldQueue, 0);
            ClearQueue();
            ProcessLog(oldQueue);
            oldQueue = null;
        }

    }
}
