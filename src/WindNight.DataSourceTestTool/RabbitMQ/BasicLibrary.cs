using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using RabbitMQ.Client;

namespace WindNight.DataSourceTestTool.RabbitMQ
{
     

    internal class BasicLibrary : IAsyncDisposable, IDisposable
    {
        private IChannel _channel;
        private IConnection _connection;
        private ConnectionFactory _factory;
        private readonly ushort _requestedHeartbeat = 30;
        private readonly Uri _uri;
        private readonly SemaphoreSlim _connectionLock = new SemaphoreSlim(1, 1);
        private bool _disposed;

        public BasicLibrary(string uri)
        {
            _uri = new Uri(uri);
        }

        public async Task<bool> IsAliveAsync()
        {
            return _connection != null && _connection.IsOpen;
        }

        #region 资源释放

        public void Dispose()
        {
            if (_disposed) return;

            try
            {
                _channel?.Dispose();
                _connection?.Dispose();
                _connectionLock?.Dispose();
            }
            catch (Exception ex)
            {
                //RecordLog.WriteLog("BasicLibrary Dispose ", "", "", "", ex);
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
                if (_channel != null)
                {
                    await _channel.DisposeAsync().ConfigureAwait(false);
                    _channel = null;
                }

                if (_connection != null)
                {
                    await _connection.DisposeAsync().ConfigureAwait(false);
                    _connection = null;
                }

                _connectionLock?.Dispose();
            }
            catch (Exception ex)
            {
                //RecordLog.WriteLog("BasicLibrary DisposeAsync ", "", "", "", ex);
            }
            finally
            {
                _disposed = true;
            }
            GC.SuppressFinalize(this);
        }

        ~BasicLibrary()
        {
            Dispose();
        }

        #endregion

        #region 工厂和连接创建

        private ConnectionFactory CreateFactory()
        {
            if (_factory == null)
            {
                _factory = new ConnectionFactory
                {
                    Uri = _uri,
                    AutomaticRecoveryEnabled = true,
                    NetworkRecoveryInterval = TimeSpan.FromSeconds(5),
                    TopologyRecoveryEnabled = true,
                    ConsumerDispatchConcurrency = 1 // 保持消息顺序
                };

                if (_requestedHeartbeat > 0)
                {
                    _factory.RequestedHeartbeat = TimeSpan.FromSeconds(_requestedHeartbeat);
                }
            }

            return _factory;
        }

        private async Task<IConnection> CreateConnectionAsync()
        {
            if (_connection != null && _connection.IsOpen) return _connection;

            await _connectionLock.WaitAsync().ConfigureAwait(false);
            try
            {
                if (_connection == null || !_connection.IsOpen)
                {
                    _connection = await CreateFactory().CreateConnectionAsync().ConfigureAwait(false);
                }
            }
            finally
            {
                _connectionLock.Release();
            }

            return _connection;
        }

        private async Task<IChannel> CreateChannelAsync()
        {
            if (_channel != null && _channel.IsOpen) return _channel;

            var connection = await CreateConnectionAsync().ConfigureAwait(false);
            _channel = await connection.CreateChannelAsync().ConfigureAwait(false);

            return _channel;
        }

        #endregion

        #region 公共方法

        /// <summary>
        /// 创建消费者通道
        /// </summary>
        public async Task<IChannel> CreateConsumerChannelByConfigAsync(ConsumerConfigInfo consumerConfigInfo)
        {
            var channel = await CreateChannelAsync().ConfigureAwait(false);

            await channel.QueueDeclareAsync(
                queue: consumerConfigInfo.QueueName,
                durable: consumerConfigInfo.QueueDurable,
                exclusive: false,
                autoDelete: false,
                arguments: null
            ).ConfigureAwait(false);

            return channel;
        }

        /// <summary>
        /// 创建生产者通道
        /// </summary>
        public async Task<IChannel> CreateProducerChannelByConfigAsync(ProducerConfigInfo producerConfigInfo)
        {
            var channel = await CreateChannelAsync().ConfigureAwait(false);

            await channel.ExchangeDeclareAsync(
                exchange: producerConfigInfo.ExchangeName,
                type: producerConfigInfo.ExchangeTypeCode.ToString().ToLower(),
                durable: producerConfigInfo.ExchangeDurable,
                autoDelete: false,
                arguments: null
            ).ConfigureAwait(false);

            return channel;
        }

        #endregion

        #region 同步方法兼容（不推荐使用）

        [Obsolete("请使用异步方法 CreateConsumerChannelByConfigAsync")]
        public IChannel CreateConsumerChannelByConfig(ConsumerConfigInfo consumerConfigInfo)
        {
            return CreateConsumerChannelByConfigAsync(consumerConfigInfo).GetAwaiter().GetResult();
        }

        [Obsolete("请使用异步方法 CreateProducerChannelByConfigAsync")]
        public IChannel CreateProducerChannelByConfig(ProducerConfigInfo producerConfigInfo)
        {
            return CreateProducerChannelByConfigAsync(producerConfigInfo).GetAwaiter().GetResult();
        }

        #endregion
    }
}
