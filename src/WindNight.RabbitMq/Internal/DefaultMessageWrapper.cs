using System;
using System.IO;
using System.Security.Cryptography.Extensions;
using System.Threading;
using System.Xml;
using Newtonsoft.Json.Extension;
using WindNight.RabbitMq.Abstractions;

namespace WindNight.RabbitMq.@internal
{
    internal class DefaultMessageWrapper : IMessageWrapper
    {
        #region private varibles

        private readonly object lockObj = new(); //锁对象
        private readonly string _filePath;

        private long lastReadEmptyTime;

        /// <summary>
        ///     内存队列
        /// </summary>
        private readonly ConcurrentQueue<MessageLocal> queueList;


        private Thread loopFlushThread;

        private const string TMP_MSG = "tmpMsg";

        #endregion

        #region 构造函数与析构函数

        /// <summary>
        ///     析构函数，回收资源
        /// </summary>
        ~DefaultMessageWrapper()
        {
            try
            {
                Dispose();
            }
            catch
            {
            }

            GC.Collect();
        }

        /// <summary>
        ///     构造函数
        /// </summary>
        /// <param name="exchangeName">交换机</param>
        public DefaultMessageWrapper(string exchangeName)
        {
            if (exchangeName.IsNullOrEmpty()) throw new ArgumentNullException("主题名称不能为空");
            _filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, TMP_MSG);
            _filePath = Path.Combine(_filePath, exchangeName);

            queueList = new ConcurrentQueue<MessageLocal>();

            LoopFlush();
        }

        /// <summary>
        ///     定期写入文件
        /// </summary>
        private void LoopFlush()
        {
            loopFlushThread = new Thread(p =>
            {
                while (true)
                    try
                    {
                        Thread.Sleep(30000);
                        lock (lockObj)
                        {
                            WriteFile();
                        }
                    }
                    catch (ThreadAbortException ex)
                    {
                        LogHelper.Error($"DefaultMessageWrapper:LoopFlush Handler Error {ex.Message}", ex);
                        // RecordLog.Err("DefaultMessageWrapper:LoopFlush", ex);
                        break;
                    }
                    catch
                    {
                    }
            });
            loopFlushThread.Start(this);
        }

        #endregion

        #region private methods

        /// <summary>
        ///     创建临时存储文件
        /// </summary>
        private void CreateFile()
        {
            if (!File.Exists(_filePath))
            {
                var directory = Path.GetDirectoryName(_filePath);
                if (!Directory.Exists(directory)) Directory.CreateDirectory(directory);
                if (File.Exists(_filePath)) return;
                using (var sw = File.CreateText(_filePath))
                {
                    sw.WriteLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
                    sw.WriteLine("<Root>");
                    sw.WriteLine("</Root>");
                }
            }
        }


        /// <summary>
        ///     写入文件
        /// </summary>
        private void WriteFile()
        {
            if (queueList.Count == 0) return;

            var doc = new XmlDocument();
            if (!Exist()) CreateFile();
            doc.Load(_filePath);
            var root = doc.DocumentElement.SelectSingleNode("//Root");
            while (queueList.TryDequeue(out var msgLocal))
            {
                if (msgLocal.IsEncrypt) msgLocal.Message = msgLocal.Message.Base64Encrypt();

                var node = doc.CreateElement("item");
                node.InnerText = msgLocal.ToJsonStr();
                root.AppendChild(node);
            }

            doc.Save(_filePath);

            RecordLog.Debug("写入文件");
        }


        /// <summary>
        ///     读取所有
        /// </summary>
        private void ReadAll()
        {
            if (queueList.Count > 0)
                return;

            lock (lockObj)
            {
                var now = HardInfo.Now.ConvertToUnixTime();
                if (now - lastReadEmptyTime > 30) //30秒内不重复读空文件
                {
                    ReadFromFile();

                    if (queueList.Count == 0)
                        lastReadEmptyTime = now;
                }
            }
        }

        /// <summary>
        ///     从临时存储文件中读取
        /// </summary>
        private void ReadFromFile()
        {
            if (!File.Exists(_filePath))
                return;

            var doc = new XmlDocument();
            doc.Load(_filePath);
            var nodes = doc.SelectNodes("//Root/item");
            foreach (XmlNode node in nodes)
            {
                var msgLocal = node.InnerText.Trim().To<MessageLocal>();
                if (msgLocal.IsEncrypt) msgLocal.Message = msgLocal.Message.Base64Decrypt();

                queueList.Enqueue(msgLocal);
            }

            File.Delete(_filePath);

            RecordLog.Debug("读取文件");
        }

        #endregion

        #region 接口方法实现

        /// <summary>
        ///     写入
        /// </summary>
        /// <param name="msgLocal">临时本地缓存对象</param>
        public void Write(MessageLocal msgLocal)
        {
            lock (lockObj)
            {
                queueList.Enqueue(msgLocal);
                if (queueList.Count >= 300)
                    WriteFile();
            }
        }

        /// <summary>
        ///     逐行读取
        /// </summary>
        /// <returns></returns>
        public MessageLocal ReadLine()
        {
            try
            {
                ReadAll();
                MessageLocal msgLocal;
                if (!queueList.TryDequeue(out msgLocal))
                    return null;

                return msgLocal;
            }
            catch (Exception ex)
            {
                LogHelper.Error($"{nameof(DefaultMessageWrapper)}:ReadLine Handler Error {ex.Message}", ex);
                //  RecordLog.Err("ReadLine ", ex);
                return null;
            }
        }

        /// <summary>
        ///     回收资源
        /// </summary>
        public void Dispose()
        {
            lock (lockObj)
            {
                WriteFile();
            }
        }

        /// <summary>
        ///     是否存在临时存储文件
        /// </summary>
        /// <returns></returns>
        public bool Exist()
        {
            return File.Exists(_filePath);
        }

        #endregion
    }
}
