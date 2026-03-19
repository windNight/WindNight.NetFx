using System.Security.Cryptography.Extensions;
using Newtonsoft.Json.Extension;
using RabbitMQ.Client;
using WindNight.RabbitMq.Abstractions;
using WindNight.RabbitMq.@internal;

namespace WindNight.RabbitMq
{
    /// <summary>
    ///     生产者类
    /// </summary>
    public class Producer : IAsyncDisposable, IDisposable
    {
        private readonly object lockObj;
        private readonly ProducerConfigInfo producerConfigInfo;
        private readonly Producer spareProduce;
        private readonly string uri;
        private BasicLibrary basicLibrary;
        private bool breakRepairLoop;
        private string encrypturi;
        private Thread loopRepairExceptionThread;
        private IChannel model;
        private IMessageWrapper wrapper;
        private readonly SemaphoreSlim _modelLock = new SemaphoreSlim(1, 1);
        private bool _disposed;

        private string EncryptUri
        {
            get
            {
                if (encrypturi.IsNullOrEmpty())
                {
                    encrypturi = RSAEncrypt(uri);
                }
                return encrypturi;
            }
        }

        /// <summary>
        ///     通道连接信息
        /// </summary>
        private async Task<IChannel> GetModelAsync()
        {
            if (model != null && model.IsOpen) return model;

            await _modelLock.WaitAsync().ConfigureAwait(false);
            try
            {
                if (model == null || !model.IsOpen)
                {
                    await CreateModelAsync().ConfigureAwait(false);
                }
            }
            finally
            {
                _modelLock.Release();
            }
            return model;
        }

        /// <summary>
        ///     初始化通道
        /// </summary>
        private async Task CreateModelAsync()
        {
            basicLibrary = new BasicLibrary(uri);
            model = await basicLibrary.CreateProducerChannelByConfigAsync(producerConfigInfo).ConfigureAwait(false);
        }

        #region 同步发送方法（保持接口兼容）

        /// <summary>
        ///     发送消息(不自动重试,不融断,实时返回发送结果)
        /// </summary>
        public bool SendWithNotRetry(string message, string routingKey, bool isMessageDurable = true)
        {
            var messageBodyBytes = CommonLibrary.BinarySerialize(message);
            return SendWithNotRetry(messageBodyBytes, routingKey, isMessageDurable);
        }

        /// <summary>
        ///     发送消息(不自动重试,不融断,实时返回发送结果)
        /// </summary>
        public bool SendWithNotRetry(byte[] messageBodyBytes, string routingKey, bool isMessageDurable = true)
        {
            try
            {
                var config = new BasicMqProperties { Durable = isMessageDurable };
                return SendWithNotRetryInternalAsync(messageBodyBytes, routingKey, config).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                var errLog = $"ExchangeName:{producerConfigInfo.ExchangeName},RoutingKey:{routingKey},发送失败";
                LogHelper.Error(errLog, ex);
                return false;
            }
        }

        /// <summary>
        ///     发送消息
        /// </summary>
        public bool Send(string message, string routingKey, bool isMessageDurable = true)
        {
            return Send(message, routingKey, new BasicMqProperties { Durable = isMessageDurable }, null);
        }

        /// <summary>
        ///     发送消息
        /// </summary>
        public bool Send(string message, string routingKey, BasicMqProperties basicProperties)
        {
            return Send(message, routingKey, basicProperties, null);
        }

        internal bool Send(string message, string routingKey, BasicMqProperties basicProperties,
            MessageLocal messageLocal)
        {
            try
            {
                return SendInternalAsync(message, routingKey, basicProperties, messageLocal).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                var log = spareProduce != null
                    ? $"发送失败:本次消息推送到中转服务处理 ExchangeName:{producerConfigInfo.ExchangeName},RoutingKey:{routingKey} "
                    : $"发送失败:本次消息记录到本地临时文件 ExchangeName:{producerConfigInfo.ExchangeName},RoutingKey:{routingKey} ";

                LogHelper.Error(log, ex);
                SaveException(message, routingKey, producerConfigInfo, messageLocal);
                return false;
            }
        }

        #endregion

        #region 异步发送方法（推荐使用）

        /// <summary>
        ///     异步发送消息(不自动重试,不融断,实时返回发送结果)
        /// </summary>
        public async Task<bool> SendWithNotRetryAsync(string message, string routingKey, bool isMessageDurable = true)
        {
            var messageBodyBytes = CommonLibrary.BinarySerialize(message);
            return await SendWithNotRetryAsync(messageBodyBytes, routingKey, isMessageDurable).ConfigureAwait(false);
        }

        /// <summary>
        ///     异步发送消息(不自动重试,不融断,实时返回发送结果)
        /// </summary>
        public async Task<bool> SendWithNotRetryAsync(byte[] messageBodyBytes, string routingKey, bool isMessageDurable = true)
        {
            try
            {
                var config = new BasicMqProperties { Durable = isMessageDurable };
                return await SendWithNotRetryInternalAsync(messageBodyBytes, routingKey, config).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                var errLog = $"ExchangeName:{producerConfigInfo.ExchangeName},RoutingKey:{routingKey},发送失败";
                LogHelper.Error(errLog, ex);
                return false;
            }
        }

        /// <summary>
        ///     异步发送消息
        /// </summary>
        public async Task<bool> SendAsync(string message, string routingKey, bool isMessageDurable = true)
        {
            return await SendAsync(message, routingKey, new BasicMqProperties { Durable = isMessageDurable }, null).ConfigureAwait(false);
        }

        /// <summary>
        ///     异步发送消息
        /// </summary>
        public async Task<bool> SendAsync(string message, string routingKey, BasicMqProperties basicProperties)
        {
            return await SendAsync(message, routingKey, basicProperties, null).ConfigureAwait(false);
        }

        internal async Task<bool> SendAsync(string message, string routingKey, BasicMqProperties basicProperties,
            MessageLocal messageLocal)
        {
            try
            {
                return await SendInternalAsync(message, routingKey, basicProperties, messageLocal).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                var log = spareProduce != null
                    ? $"发送失败:本次消息推送到中转服务处理 ExchangeName:{producerConfigInfo.ExchangeName},RoutingKey:{routingKey} "
                    : $"发送失败:本次消息记录到本地临时文件 ExchangeName:{producerConfigInfo.ExchangeName},RoutingKey:{routingKey} ";

                LogHelper.Error(log, ex);
                SaveException(message, routingKey, producerConfigInfo, messageLocal);
                return false;
            }
        }

        #endregion

        #region 内部异步实现

        private async Task<bool> SendWithNotRetryInternalAsync(byte[] messageBodyBytes, string routingKey, BasicMqProperties config)
        {
            var currentModel = await GetModelAsync().ConfigureAwait(false);
            var configInfo = CommonLibrary.CreateBasicProperties(currentModel, config);

            await currentModel.BasicPublishAsync(
                exchange: producerConfigInfo.ExchangeName,
                routingKey: routingKey,
                mandatory: false,
                basicProperties: configInfo,
                body: messageBodyBytes
            ).ConfigureAwait(false);

            return true;
        }

        private async Task<bool> SendInternalAsync(string message, string routingKey, BasicMqProperties basicProperties,
            MessageLocal messageLocal)
        {
            var currentModel = await GetModelAsync().ConfigureAwait(false);
            var configInfo = basicProperties == null
                ? null
                : CommonLibrary.CreateBasicProperties(currentModel, basicProperties);
            var messageBodyBytes = CommonLibrary.BinarySerialize(message);

            await currentModel.BasicPublishAsync(
                exchange: producerConfigInfo.ExchangeName,
                routingKey: routingKey,
                mandatory: false,
                basicProperties: configInfo,
                body: messageBodyBytes
            ).ConfigureAwait(false);

            return true;
        }

        #endregion

        #region 加密

        private static string RSAEncrypt(string content)
        {
            var publicKey =
                "BgIAAACkAABSU0ExAAQAAAEAAQDhywxiz16bJ1YSx187lNqNz8ltNXhpivkt2WJGEpraUvHKhdF6h5rcses7gxOhAAg38/ZZlZq26Ssm2v791c8+DZ0CAkDfGG7GwbVmV2k2hLU6IB0Owof2IroMvR2mBkxGMPRcOfk/3JMasY451oOo7t3XlHmvtpTcZTrMDMDNzw==";
            return content.RSAEncrypt(publicKey);
        }

        #endregion

        #region 构造函数与资源释放

        ~Producer()
        {
            Dispose();
        }

        /// <summary>
        ///     生产者初始化
        /// </summary>
        /// <param name="uri">amqp地址</param>
        /// <param name="producerConfigInfo">生产者配置信息</param>
        /// <param name="messageWrapper">消息包装器</param>
        public Producer(string uri, ProducerConfigInfo producerConfigInfo, IMessageWrapper messageWrapper = null)
        {
            if (producerConfigInfo.FileName.IsNullOrEmpty())
            {
                producerConfigInfo.FileName = producerConfigInfo.ExchangeName;
            }

            this.uri = uri;
            lockObj = new object();
            this.producerConfigInfo = producerConfigInfo;
            wrapper = new DefaultMessageWrapper(producerConfigInfo.FileName);
            LoopRepairException();

            if (!producerConfigInfo.SpareMqUri.IsNullOrEmpty())
            {
                if (producerConfigInfo.SpareExchangeName.IsNullOrEmpty() ||
                    producerConfigInfo.SpareRoutingKey.IsNullOrEmpty())
                {
                    throw new ArgumentNullException("SpareExchangeName or SpareRoutingKey can not be empty");
                }

                spareProduce = new Producer(producerConfigInfo.SpareMqUri,
                    new ProducerConfigInfo
                    {
                        ExchangeDurable = true,
                        ExchangeName = producerConfigInfo.SpareExchangeName,
                        ExchangeTypeCode = ExchangeTypeCodeEnum.Topic,
                        FileName = $"{producerConfigInfo.ExchangeName}_{producerConfigInfo.SpareExchangeName}"
                    });
            }
        }

        public void Dispose()
        {
            if (_disposed) return;
            breakRepairLoop = true;
            DisposeResource();
            _disposed = true;
            GC.SuppressFinalize(this);
        }

        public async ValueTask DisposeAsync()
        {
            if (_disposed) return;
            breakRepairLoop = true;
            await DisposeResourceAsync().ConfigureAwait(false);
            _disposed = true;
            GC.SuppressFinalize(this);
        }

        private void DisposeResource()
        {
            try
            {
                loopRepairExceptionThread?.Join(TimeSpan.FromSeconds(5));
            }
            catch { }

            if (basicLibrary != null)
            {
                try
                {
                    basicLibrary.Dispose();
                }
                catch (Exception ex)
                {
                    LogHelper.Error("basicLibrary.Dispose()", ex);
                }
                basicLibrary = null;
            }

            if (wrapper != null)
            {
                try
                {
                    wrapper.Dispose();
                }
                catch (Exception ex)
                {
                    LogHelper.Error("wrapper.Dispose()", ex);
                }
                wrapper = null;
            }

            _modelLock?.Dispose();
        }

        private async ValueTask DisposeResourceAsync()
        {
            try
            {
                if (loopRepairExceptionThread?.IsAlive == true)
                    loopRepairExceptionThread.Join(TimeSpan.FromSeconds(5));
            }
            catch { }

            if (basicLibrary != null)
            {
                try
                {
                    await basicLibrary.DisposeAsync().ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    LogHelper.Error("basicLibrary.DisposeAsync()", ex);
                }
                basicLibrary = null;
            }

            if (wrapper != null)
            {
                try
                {
                    wrapper.Dispose();
                }
                catch (Exception ex)
                {
                    LogHelper.Error("wrapper.Dispose()", ex);
                }
                wrapper = null;
            }

            _modelLock?.Dispose();
        }

        #endregion

        #region 异常数据处理

        private bool CheckLocalMessage(MessageLocal messageLocal, IMessageWrapper wrapper)
        {
            return !messageLocal.NeedGiveUp;
        }

        private void LoopRepairException()
        {
            loopRepairExceptionThread = new Thread(p =>
            {
                var sleepTicks = 1;
                var sender = (Producer)p;

                while (!breakRepairLoop)
                {
                    try
                    {
                        var localFile = false;
                        var repairCount = 0;
                        MessageLocal msgLocal;

                        while ((msgLocal = sender.wrapper.ReadLine()) != null)
                        {
                            localFile = true;

                            if (!CheckLocalMessage(msgLocal, sender.wrapper))
                            {
                                continue;
                            }

                            if (!sender.Send(msgLocal.Message, msgLocal.RoutingKey, null, msgLocal))
                            {
                                break;
                            }

                            repairCount++;
                            Thread.Sleep(1);
                        }

                        sleepTicks = repairCount > 0 ? 1 :
                            localFile ? Math.Min(sleepTicks + 1, 10) : Math.Min(sleepTicks + 1, 20);

                        if (repairCount > 0)
                        {
                            RecordLog.Debug($"修复线程: {sender.producerConfigInfo.ExchangeName} 修复本地文件数量:{repairCount}");
                        }

                        RecordLog.Debug($"修复线程: {sender.producerConfigInfo.ExchangeName} 延时:{sleepTicks}秒");
                        Thread.Sleep(sleepTicks * 1000);
                    }
                    catch (ThreadAbortException)
                    {
                        breakRepairLoop = true;
                    }
                    catch (Exception ex)
                    {
                        LogHelper.Error("LoopRepairException", ex);
                    }
                }

                if (breakRepairLoop)
                {
                    DisposeResource();
                }
            });
            loopRepairExceptionThread.Start(this);
        }

        private void SaveException(string message, string routingKey, ProducerConfigInfo producerConfigInfo,
            MessageLocal messageLocal)
        {
            if (spareProduce != null)
            {
                SaveExceptionSpareProcess(message, routingKey, producerConfigInfo);
            }
            else
            {
                SaveExceptionLocalFile(message, routingKey, producerConfigInfo, messageLocal);
            }
        }

        private void SaveExceptionSpareProcess(string message, string routingKey, ProducerConfigInfo producerConfigInfo)
        {
            var msg = new
            {
                Exchange = producerConfigInfo.ExchangeName,
                RoutingKey = routingKey,
                Msg = message,
                Uri = EncryptUri
            }.ToJsonStr();
            spareProduce.Send(msg, producerConfigInfo.SpareRoutingKey);
        }

        private void SaveExceptionLocalFile(string message, string routingKey, ProducerConfigInfo producerConfigInfo,
            MessageLocal messageLocal)
        {
            try
            {
                var now = HardInfo.Now;
                var msgLocal = new MessageLocal
                {
                    Message = message,
                    RoutingKey = routingKey,
                    IsEncrypt = true,
                    CreateTime = now.ConvertToUnixTime(),
                    ProducerConfigInfo = producerConfigInfo
                };

                if (messageLocal != null)
                {
                    msgLocal.CreateTime = messageLocal.CreateTime;
                    msgLocal.RetryNum = messageLocal.RetryNum;
                }

                msgLocal.RetryNum++;
                msgLocal.LastRetryTime = now.ConvertToUnixTime();
                wrapper.Write(msgLocal);
            }
            catch (Exception ex)
            {
                var errLog =
                    $"保存本地文件异常 ExchangeName:{producerConfigInfo.ExchangeName},RoutingKey:{routingKey},Message:{message}";
                LogHelper.Error(errLog, ex);
                throw;
            }
        }

        #endregion
    }
}
