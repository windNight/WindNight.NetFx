using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace Swashbuckle.AspNetCore.Extensions.@internal
{
    internal partial class XmlHelper
    {
        private static readonly Lazy<XmlHelper> LazyInstance = new(() => new XmlHelper());

        private XmlHelper()
        {
            RegisterHeartRun();
        }

        public static XmlHelper Instance => LazyInstance.Value;

        public List<string> DocumentFiles { get; set; } = new();

        public void Init()
        {
            DoSync();
        }
    }

    internal partial class XmlHelper
    {
        #region HeartRun

        private static Thread _keepAliveThread;
        protected bool IsStop { get; set; }
        private int SleepTime => 30;

        /// <summary>
        ///     注册心跳刷新
        /// </summary>
        private void RegisterHeartRun()
        {
            try
            {
                _keepAliveThread = new Thread(KeepAliveThread)
                {
                    Name = "KeepAliveThread:SyncXmls", IsBackground = true
                };

                _keepAliveThread.Start();
            }
            catch (Exception ex)
            {
                //   EgLogHelper.Error($"RegisterHeartRun handler error ,{ex.Message}", ex);
            }
        }

        public void KeepAliveThread()
        {
            var loopStartTimeTicks = DateTime.Now.Ticks;
            var loop = 0;


            while (true)
            {
                var flag = false;
                var isContinue = false;
                try
                {
                    loop++;
                    if (IsStop)
                        break;
                    var ttl = (int)TimeSpan.FromTicks(DateTime.Now.Ticks - loopStartTimeTicks).TotalMinutes;

                    if (ttl < SleepTime)
                    {
                        Thread.Sleep(100);
                        isContinue = true;
                        continue;
                    }

                    DoSync();

                    flag = true;
                    Thread.Sleep(20);
                }
                catch (Exception ex)
                {
                    // EgLogHelper.Error($"{_keepAliveThread.Name} 心跳包 HeartRun  handler error ,{ex.Message}", ex);
                }
                finally
                {
                    // 成功后重置
                    if (flag)
                        loopStartTimeTicks = DateTime.Now.Ticks;
                    // if (!isContinue)
                    //  EgLogHelper.Debug($"{_keepAliveThread.Name}  心跳包 HeartRun 执行 {(flag ? "成功" : "失败")}！ ReInit loopStartTimeTicks={DateTime.Now:yyyy-MM-dd HH:mm:ss}");
                }
            }
        }

        private void DoSync()
        {
            //TODO your job
            SyncXmlFiles();
        }

        #endregion //end HeartRun
    }

    internal partial class XmlHelper
    {
        public void SyncXmlFiles()
        {
            try
            {
                var list = new List<string>();
                var path = AppContext.BaseDirectory;
                foreach (var file in Directory.GetFiles(path))
                {
                    if (".xml".Equals(Path.GetExtension(file)))
                    {
                        list.Add(Path.GetFullPath(file));
                    }
                }


                if (!list.IsNullOrEmpty())
                {
                    DocumentFiles.Clear();
                    DocumentFiles = list;
                    GC.Collect();
                }
            }
            catch (Exception ex)
            {
            }
        }
    }
}
