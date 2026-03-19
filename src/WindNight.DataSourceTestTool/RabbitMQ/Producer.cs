using System;
using System.Threading;
using System.Threading.Tasks;
using RabbitMQ.Client;

namespace WindNight.DataSourceTestTool.RabbitMQ
{
    public class Producer : IAsyncDisposable, IDisposable
    {
        private BasicLibrary _basicLibrary;
        private ExceptionStatus _exceptionStatus;
        private readonly SemaphoreSlim _channelLock = new SemaphoreSlim(1, 1);
        private IChannel _channel;
        private readonly ProducerConfigInfo _producerConfigInfo;
        private readonly string _uri;
        private bool _disposed;

        public Producer(string uri, ProducerConfigInfo producerConfigInfo)
        {
            _uri = uri;
            _producerConfigInfo = producerConfigInfo;
        }

        public bool IsChannelOpen()
        {
            return IsChannelOpenAsync().GetAwaiter().GetResult();

        }

        public async Task<bool> IsChannelOpenAsync()
        {
            var rlt = _channel != null && _channel.IsOpen;
            return await Task.FromResult(rlt);
        }

        private async Task<IChannel> GetChannelAsync()
        {
            if (_channel != null && _channel.IsOpen) return _channel;

            await _channelLock.WaitAsync().ConfigureAwait(false);
            try
            {
                if (_channel == null || !_channel.IsOpen)
                {
                    if (_channel != null && !_channel.IsOpen)
                    {
                        _basicLibrary?.Dispose();
                    }

                    await InitChannelAsync().ConfigureAwait(false);
                }
            }
            finally
            {
                _channelLock.Release();
            }

            return _channel;
        }

        #region 资源释放

        public void Dispose()
        {
            if (_disposed) return;

            try
            {
                _basicLibrary?.Dispose();
                _channel?.Dispose();
                _channelLock?.Dispose();
            }
            catch (Exception ex)
            {
                // RecordLog.WriteLog("Producer Dispose ", "", "", "", ex);
            }
            finally
            {
                _disposed = true;
            }
            GC.SuppressFinalize(this);
        }

        public async ValueTask DisposeAsync()
        {
            if (_disposed) return;

            try
            {
                if (_basicLibrary != null)
                {
                    await _basicLibrary.DisposeAsync().ConfigureAwait(false);
                }

                if (_channel != null)
                {
                    await _channel.DisposeAsync().ConfigureAwait(false);
                }

                _channelLock?.Dispose();
            }
            catch (Exception ex)
            {
                // RecordLog.WriteLog("Producer DisposeAsync", "", "", "", ex);
            }
            finally
            {
                _disposed = true;
            }
            GC.SuppressFinalize(this);
        }

        ~Producer()
        {
            Dispose();
        }

        #endregion

        #region 通道初始化

        private async Task InitChannelAsync()
        {
            _basicLibrary = new BasicLibrary(_uri);
            _channel = await _basicLibrary.CreateProducerChannelByConfigAsync(_producerConfigInfo).ConfigureAwait(false);
        }

        #endregion

        #region 发送方法

        public async Task SendAsync(string message, string routingKey, bool isMessageDurable = true)
        {
            await SendAsync(message, routingKey, new BasicPropertiesConfigInfo { Durable = isMessageDurable }).ConfigureAwait(false);
        }

        public async Task<bool> SendWithNotRetryAsync(string message, string routingKey, bool isMessageDurable = true)
        {
            return await SendWithNotRetryAsync(CommonLibrary.BinarySerialize(message), routingKey, isMessageDurable).ConfigureAwait(false);
        }

        public async Task<bool> SendWithNotRetryAsync(byte[] messageBodyBytes, string routingKey, bool isMessageDurable = true)
        {
            try
            {
                var channel = await GetChannelAsync().ConfigureAwait(false);
                var basicProperties = CommonLibrary.CreateBasicProperties(channel,
                    new BasicPropertiesConfigInfo { Durable = isMessageDurable });

                await channel.BasicPublishAsync(
                    exchange: _producerConfigInfo.ExchangeName,
                    routingKey: routingKey,
                    body: messageBodyBytes,
                    mandatory: false,
                    basicProperties: basicProperties
                ).ConfigureAwait(false);

                return true;
            }
            catch (Exception ex)
            {
                // RecordLog.WriteLog($"ExchangeName:{_producerConfigInfo.ExchangeName},RoutingKey:{routingKey},发送失败", _producerConfigInfo.ExchangeName, routingKey, "", ex);
                await DisposeAsync().ConfigureAwait(false);
                return false;
            }
        }

        public async Task<bool> SendAsync(
            string message,
            string routingKey,
            BasicPropertiesConfigInfo basicPropertiesConfigInfo)
        {
            try
            {
                var channel = await GetChannelAsync().ConfigureAwait(false);
                var basicProperties = basicPropertiesConfigInfo == null
                    ? null
                    : CommonLibrary.CreateBasicProperties(channel, basicPropertiesConfigInfo);
                var body = CommonLibrary.BinarySerialize(message);

                await channel.BasicPublishAsync(
                    exchange: _producerConfigInfo.ExchangeName,
                    routingKey: routingKey,
                    body: body,
                    mandatory: false,
                    basicProperties: basicProperties
                ).ConfigureAwait(false);

                await RepairExceptionAsync().ConfigureAwait(false);
                return true;
            }
            catch (Exception ex)
            {
                // RecordLog.WriteLog($"ExchangeName:{_producerConfigInfo.ExchangeName},RoutingKey:{routingKey},发送失败:本次消息已记录到本地临时文件", _producerConfigInfo.ExchangeName, routingKey, "", ex);
                SaveException(message, routingKey, _producerConfigInfo, basicPropertiesConfigInfo);
                await DisposeAsync().ConfigureAwait(false);
                return false;
            }
        }

        #endregion

        #region 同步兼容方法

        public void Send(string message, string routingKey, bool isMessageDurable = true)
        {
            SendAsync(message, routingKey, isMessageDurable).GetAwaiter().GetResult();
        }

        public bool SendWithNotRetry(string message, string routingKey, bool isMessageDurable = true)
        {
            return SendWithNotRetryAsync(message, routingKey, isMessageDurable).GetAwaiter().GetResult();
        }

        public bool SendWithNotRetry(byte[] messageBodyBytes, string routingKey, bool isMessageDurable = true)
        {
            return SendWithNotRetryAsync(messageBodyBytes, routingKey, isMessageDurable).GetAwaiter().GetResult();
        }

        public bool Send(
            string message,
            string routingKey,
            BasicPropertiesConfigInfo basicPropertiesConfigInfo)
        {
            return SendAsync(message, routingKey, basicPropertiesConfigInfo).GetAwaiter().GetResult();
        }

        #endregion

        #region 异常处理

        private void SaveException(
            string message,
            string routingKey,
            ProducerConfigInfo producerConfigInfo,
            BasicPropertiesConfigInfo basicPropertiesConfigInfo)
        {
            _exceptionStatus = ExceptionStatus.Exception;
            try
            {
                // 实现本地文件保存逻辑
                // this.wapper.Write(new MessageLocal()
                // {
                //     Message = message,
                //     RoutingKey = routingKey,
                //     BasicPropertiesConfigInfo = basicPropertiesConfigInfo,
                //     ProducerConfigInfo = producerConfigInfo
                // });
            }
            catch (Exception ex)
            {
                // RecordLog.WriteLog($"ExchangeName:{producerConfigInfo.ExchangeName},RoutingKey:{routingKey},Message:{message},SaveException", producerConfigInfo.ExchangeName, routingKey, "", ex);
                throw;
            }
        }

        private async Task RepairExceptionAsync()
        {
            if (!CheckNeedRepair())
            {
                return;
            }

            _exceptionStatus = ExceptionStatus.Repired;

            await Task.Run(() =>
            {
                // 实现异常修复逻辑
                // while ((messageLocal = wapper.ReadLine()) != null)
                // {
                //     using var producer = new Producer(_uri, messageLocal.ProducerConfigInfo);
                //     producer.Send(messageLocal.Message, messageLocal.RoutingKey);
                //     Thread.Sleep(1);
                // }
            }).ConfigureAwait(false);
        }

        private bool CheckNeedRepair()
        {
            return _exceptionStatus == ExceptionStatus.Exception;
        }

        private enum ExceptionStatus
        {
            None,
            Exception,
            Repired
        }

        #endregion
    }
}
