using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using WindNight.RabbitMq.Abstractions;
using WindNight.RabbitMq.@internal;

namespace WindNight.RabbitMq
{
    /// <summary>
    ///     消费者类
    /// </summary>
    public class Consumer : IAsyncDisposable, IDisposable
    {
        private readonly SemaphoreSlim _modelLock = new SemaphoreSlim(1, 1);
        protected readonly ConsumerConfigInfo consumerConfigInfo;
        protected readonly string uri;
        private bool _disposed;

        private BasicLibrary basicLibrary;

        protected AsyncEventingBasicConsumer consumerPassive;
        protected IChannel model;

        public Consumer(string uri, ConsumerConfigInfo consumerConfigInfo)
        {
            this.uri = uri;
            this.consumerConfigInfo = consumerConfigInfo;
        }

        public async ValueTask DisposeAsync()
        {
            if (_disposed)
            {
                return;
            }

            try
            {
                if (basicLibrary != null)
                {
                    await basicLibrary.DisposeAsync().ConfigureAwait(false);
                }

                _modelLock?.Dispose();
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
                basicLibrary?.Dispose();
                _modelLock?.Dispose();
            }
            finally
            {
                _disposed = true;
            }

            GC.SuppressFinalize(this);
        }

        protected async Task<IChannel> GetModelAsync()
        {
            if (model != null && model.IsOpen)
            {
                return model;
            }

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

        private async Task CreateModelAsync()
        {
            basicLibrary = new BasicLibrary(uri);
            model = await basicLibrary.CreateConsumerChannelByConfigAsync(consumerConfigInfo).ConfigureAwait(false);
        }

        public async Task<AsyncEventingBasicConsumer> SetConsumerActiveAsync(Func<object, BasicDeliverEventArgs, Task> asyncHandler)
        {
            try
            {
                if (consumerPassive == null)
                {
                    var currentModel = await GetModelAsync().ConfigureAwait(false);
                    var consumer = new AsyncEventingBasicConsumer(currentModel);

                    consumer.ReceivedAsync += async (sender, eventArgs) =>
                    {
                        try
                        {
                            await asyncHandler(sender, eventArgs).ConfigureAwait(false);
                        }
                        catch (Exception ex)
                        {
                            LogHelper.Error($"消息处理失败: {ex.Message}", ex);
                            await currentModel.BasicNackAsync(eventArgs.DeliveryTag, false, true).ConfigureAwait(false);
                        }
                    };

                    await currentModel.BasicQosAsync(0, consumerConfigInfo.PrefetchCount, false).ConfigureAwait(false);
                    await currentModel.BasicConsumeAsync(consumerConfigInfo.QueueName, false, consumer)
                        .ConfigureAwait(false);

                    consumerPassive = consumer;
                }

                return consumerPassive;
            }
            catch (Exception ex)
            {
                LogHelper.Error($"SetConsumerActiveAsync Error {ex.Message}", ex);
                return null;
            }
        }

        public async Task<bool> AckAsync(ulong deliveryTag, bool multiple)
        {
            try
            {
                var currentModel = await GetModelAsync().ConfigureAwait(false);
                await currentModel.BasicAckAsync(deliveryTag, multiple).ConfigureAwait(false);
                return true;
            }
            catch (Exception ex)
            {
                LogHelper.Error($"AckAsync error: {ex.Message}", ex);
                return false;
            }
        }

        public async Task<bool> NackAsync(ulong deliveryTag, bool multiple, bool requeue)
        {
            try
            {
                var currentModel = await GetModelAsync().ConfigureAwait(false);
                await currentModel.BasicNackAsync(deliveryTag, multiple, requeue).ConfigureAwait(false);
                return true;
            }
            catch (Exception ex)
            {
                LogHelper.Error($"NackAsync error: {ex.Message}", ex);
                return false;
            }
        }

        protected async Task<(bool Success, string Message, ulong DeliveryTag, string RoutingKey)> ReceiveInternalAsync(
            bool autoAck)
        {
            try
            {
                var currentModel = await GetModelAsync().ConfigureAwait(false);
                var result = await currentModel.BasicGetAsync(consumerConfigInfo.QueueName, autoAck)
                    .ConfigureAwait(false);

                if (result == null)
                {
                    await Task.Delay(200).ConfigureAwait(false);
                    return (false, string.Empty, 0, string.Empty);
                }

                var bytes = result.Body.ToArray();
                var message = Encoding.UTF8.GetString(bytes);
                return (true, message, result.DeliveryTag, result.RoutingKey);
            }
            catch (Exception ex)
            {
                LogHelper.Error($"ReceiveInternalAsync error: {ex.Message}", ex);
                await Task.Delay(500).ConfigureAwait(false);
                return (false, string.Empty, 0, string.Empty);
            }
        }
    }
}
