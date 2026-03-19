using RabbitMQ.Client;
using WindNight.RabbitMq.Abstractions;

namespace WindNight.RabbitMq.@internal
{
    internal class BasicLibrary : IAsyncDisposable, IDisposable
    {
        private readonly ushort requestedHeartbeat = 30;
        private readonly Uri uri;
        private IChannel _channel;
        private IConnection _connection;
        private bool _disposed;
        private ConnectionFactory _factory;

        public BasicLibrary(string uri)
        {
            this.uri = new Uri(uri);
        }

        public async ValueTask DisposeAsync()
        {
            if (_disposed)
            {
                return;
            }

            try
            {
                if (_channel != null)
                {
                    try
                    {
                        if (_channel.IsOpen)
                        {
                            await _channel.CloseAsync().ConfigureAwait(false);
                        }

                        await _channel.DisposeAsync().ConfigureAwait(false);
                    }
                    catch (Exception ex)
                    {
                        LogHelper.Error("channel.DisposeAsync()", ex);
                    }

                    _channel = null;
                }

                if (_connection != null)
                {
                    try
                    {
                        if (_connection.IsOpen)
                        {
                            await _connection.CloseAsync().ConfigureAwait(false);
                        }

                        await _connection.DisposeAsync().ConfigureAwait(false);
                    }
                    catch (Exception ex)
                    {
                        LogHelper.Error("connection.DisposeAsync()", ex);
                    }

                    _connection = null;
                }
            }
            finally
            {
                _disposed = true;
            }

            GC.SuppressFinalize(this);
        }

        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            try
            {
                _channel?.CloseAsync().GetAwaiter().GetResult();
                _channel?.Dispose();
                _connection?.CloseAsync().GetAwaiter().GetResult();
                _connection?.Dispose();
            }
            catch (Exception ex)
            {
                LogHelper.Error("Dispose error", ex);
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

        private ConnectionFactory CreateFactory()
        {
            if (_factory == null)
            {
                _factory = new ConnectionFactory
                {
                    Uri = uri,
                    AutomaticRecoveryEnabled = true,
                    NetworkRecoveryInterval = TimeSpan.FromSeconds(5),
                    ConsumerDispatchConcurrency = 1, // 保持消息顺序
                    TopologyRecoveryEnabled = true,
                    RequestedHeartbeat = TimeSpan.FromSeconds(requestedHeartbeat)
                };
            }

            return _factory;
        }

        private async Task<IConnection> CreateConnectionAsync()
        {
            if (_connection == null || !_connection.IsOpen)
            {
                _connection = await CreateFactory().CreateConnectionAsync().ConfigureAwait(false);
            }

            return _connection;
        }

        private async Task<IChannel> CreateChannelAsync()
        {
            if (_channel == null || !_channel.IsOpen)
            {
                var connection = await CreateConnectionAsync().ConfigureAwait(false);
                _channel = await connection.CreateChannelAsync().ConfigureAwait(false);
            }

            return _channel;
        }

        public async Task<IChannel> CreateConsumerChannelByConfigAsync(ConsumerConfigInfo consumerConfigInfo)
        {
            var channel = await CreateChannelAsync().ConfigureAwait(false);

            await channel.QueueDeclareAsync(
                consumerConfigInfo.QueueName,
                consumerConfigInfo.QueueDurable,
                false,
                false,
                null
            ).ConfigureAwait(false);

            return channel;
        }

        public async Task<IChannel> CreateProducerChannelByConfigAsync(ProducerConfigInfo producerConfigInfo)
        {
            var channel = await CreateChannelAsync().ConfigureAwait(false);

            await channel.ExchangeDeclareAsync(
                producerConfigInfo.ExchangeName,
                producerConfigInfo.ExchangeTypeCode.ToString().ToLower(),
                producerConfigInfo.ExchangeDurable,
                false,
                null
            ).ConfigureAwait(false);

            return channel;
        }
    }
}
