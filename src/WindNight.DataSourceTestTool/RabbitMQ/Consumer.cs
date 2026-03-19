using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace WindNight.DataSourceTestTool.RabbitMQ
{
    public class Consumer : IAsyncDisposable, IDisposable
    {
        private readonly SemaphoreSlim _channelLock;
        private readonly ConsumerConfigInfo _consumerConfigInfo;
        private readonly string _uri;
        private BasicLibrary _basicLibrary;
        private IChannel _channel;
        private AsyncEventingBasicConsumer _consumerPassive;
        private int _disposed;

        public Consumer(string uri, ConsumerConfigInfo consumerConfigInfo)
        {
            _uri = uri;
            _consumerConfigInfo = consumerConfigInfo;
            _channelLock = new SemaphoreSlim(1, 1);
        }

        private async Task<IChannel> GetChannelAsync()
        {
            // 快速检查是否已释放
            if (Volatile.Read(ref _disposed) == 1)
            {
                throw new ObjectDisposedException(nameof(Consumer));
            }

            if (_channel != null && _channel.IsOpen)
            {
                return _channel;
            }

            // 尝试获取锁，设置超时避免死锁
            var lockTaken = false;
            try
            {
                lockTaken = await _channelLock.WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
                if (!lockTaken)
                {
                    throw new TimeoutException("获取通道锁超时");
                }

                // 再次检查释放状态
                if (Volatile.Read(ref _disposed) == 1)
                {
                    throw new ObjectDisposedException(nameof(Consumer));
                }

                // 双重检查
                if (_channel == null || !_channel.IsOpen)
                {
                    await InitChannelAsync().ConfigureAwait(false);
                }
            }
            finally
            {
                if (lockTaken && Volatile.Read(ref _disposed) == 0)
                {
                    try
                    {
                        _channelLock.Release();
                    }
                    catch (ObjectDisposedException)
                    {
                        // 忽略释放后的异常
                    }
                    catch (SemaphoreFullException)
                    {
                        // 忽略信号量已满的异常
                    }
                }
            }

            return _channel;
        }

        #region 消息接收（返回值替代 out 参数）

        /// <summary>
        ///     接收消息（需要确认）- 返回包含所有信息的对象
        /// </summary>
        public async Task<ReceiveResult> ReceiveNeedAckAsync()
        {
            try
            {
                ThrowIfDisposed();
                var channel = await GetChannelAsync().ConfigureAwait(false);
                var result = await channel.BasicGetAsync(_consumerConfigInfo.QueueName, false).ConfigureAwait(false);

                if (result == null)
                {
                    await Task.Delay(500).ConfigureAwait(false);
                    return ReceiveResult.Empty;
                }

                var body = result.Body.ToArray();
                var message = CommonLibrary.BinaryDeserialize(body);

                return new ReceiveResult
                {
                    Success = true,
                    Message = message,
                    DeliveryTag = result.DeliveryTag,
                    RoutingKey = result.RoutingKey
                };
            }
            catch (ObjectDisposedException)
            {
                return ReceiveResult.Empty;
            }
            catch (Exception ex)
            {
                // LogHelper.Error($"QueueName:{_consumerConfigInfo.QueueName},ReceiveNeedAck", ex);
                await Task.Delay(500).ConfigureAwait(false);
                _ = DisposeAsync(); // 不等待释放完成
                return ReceiveResult.Empty;
            }
        }

        #endregion

        #region 通道状态检查

        /// <summary>
        ///     同步检查通道是否开启（不推荐使用）
        /// </summary>
        [Obsolete("请使用异步方法 IsChannelOpenAsync")]
        public bool IsChannelOpen()
        {
            if (Volatile.Read(ref _disposed) == 1)
            {
                return false;
            }

            return _channel != null && _channel.IsOpen;
        }

        /// <summary>
        ///     异步检查通道是否开启
        /// </summary>
        public async Task<bool> IsChannelOpenAsync()
        {
            if (Volatile.Read(ref _disposed) == 1)
            {
                return false;
            }

            // 如果通道存在且开启，直接返回
            if (_channel != null && _channel.IsOpen)
            {
                return true;
            }

            // 尝试获取通道但不抛出异常
            try
            {
                var channel = await GetChannelAsync().ConfigureAwait(false);
                return channel != null && channel.IsOpen;
            }
            catch (ObjectDisposedException)
            {
                return false;
            }
            catch (TimeoutException)
            {
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        #endregion

        #region 通道初始化

        private async Task InitChannelAsync()
        {
            ThrowIfDisposed();
            _basicLibrary = new BasicLibrary(_uri);
            _channel = await _basicLibrary.CreateConsumerChannelByConfigAsync(_consumerConfigInfo)
                .ConfigureAwait(false);
        }

        private void ThrowIfDisposed()
        {
            if (Volatile.Read(ref _disposed) == 1)
            {
                throw new ObjectDisposedException(nameof(Consumer));
            }
        }

        #endregion

        #region 资源释放

        public void Dispose()
        {
            DisposeAsync().AsTask().GetAwaiter().GetResult();
        }

        public async ValueTask DisposeAsync()
        {
            // 使用 Interlocked 确保只执行一次
            if (Interlocked.Exchange(ref _disposed, 1) == 1)
            {
                return;
            }

            var lockToDispose = _channelLock;

            // 等待任何正在进行的操作完成
            try
            {
                // 尝试获取锁，确保没有其他操作在进行
                if (await lockToDispose.WaitAsync(TimeSpan.FromSeconds(3)).ConfigureAwait(false))
                {
                    try
                    {
                        await DisposeResourcesAsync().ConfigureAwait(false);
                    }
                    finally
                    {
                        try
                        {
                            lockToDispose.Release();
                        }
                        catch
                        {
                            // 忽略释放异常
                        }
                    }
                }
                else
                {
                    // 超时，强制释放资源
                    await DisposeResourcesAsync().ConfigureAwait(false);
                }
            }
            catch
            {
                // 发生异常时也要尝试释放资源
                await DisposeResourcesAsync().ConfigureAwait(false);
            }
            finally
            {
                // 最后释放锁
                lockToDispose?.Dispose();
            }

            GC.SuppressFinalize(this);
        }

        private async ValueTask DisposeResourcesAsync()
        {
            var exceptions = new List<Exception>();

            if (_basicLibrary != null)
            {
                try
                {
                    await _basicLibrary.DisposeAsync().ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    exceptions.Add(ex);
                }
                finally
                {
                    _basicLibrary = null;
                }
            }

            if (_channel != null)
            {
                try
                {
                    await _channel.DisposeAsync().ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    exceptions.Add(ex);
                }
                finally
                {
                    _channel = null;
                }
            }

            _consumerPassive = null;

            if (exceptions.Count > 0)
            {
                // LogHelper.Error("Consumer.DisposeResourcesAsync", string.Join(", ", exceptions));
            }
        }

        ~Consumer()
        {
            if (Interlocked.Exchange(ref _disposed, 1) == 0)
            {
                _channelLock?.Dispose();
                _basicLibrary?.Dispose();
                _channel?.Dispose();
            }
        }

        #endregion

        #region 消费者创建

        private async Task<AsyncEventingBasicConsumer> CreateConsumerAckAsync(
            Func<object, BasicDeliverEventArgs, Task> asyncHandler)
        {
            ThrowIfDisposed();
            var channel = await GetChannelAsync().ConfigureAwait(false);
            var consumer = new AsyncEventingBasicConsumer(channel);

            consumer.ReceivedAsync += async (sender, eventArgs) =>
            {
                try
                {
                    await asyncHandler(sender, eventArgs).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    // LogHelper.Error($"消息处理失败: {ex.Message}", ex);
                    await channel.BasicNackAsync(eventArgs.DeliveryTag, false, true).ConfigureAwait(false);
                }
            };

            await channel.BasicQosAsync(0, _consumerConfigInfo.PrefetchCount, false).ConfigureAwait(false);
            await channel.BasicConsumeAsync(_consumerConfigInfo.QueueName, false, consumer).ConfigureAwait(false);

            return consumer;
        }

        public async Task<AsyncEventingBasicConsumer> SetConsumerActiveAsync(
            Func<object, BasicDeliverEventArgs, Task> asyncHandler)
        {
            ThrowIfDisposed();
            if (_consumerPassive == null)
            {
                _consumerPassive = await CreateConsumerAckAsync(asyncHandler).ConfigureAwait(false);
            }

            return _consumerPassive;
        }

        #endregion

        #region 消息确认

        public async Task<bool> AckAsync(ulong deliveryTag, bool multiple)
        {
            try
            {
                ThrowIfDisposed();
                var channel = await GetChannelAsync().ConfigureAwait(false);
                await channel.BasicAckAsync(deliveryTag, multiple).ConfigureAwait(false);
                return true;
            }
            catch (ObjectDisposedException)
            {
                return false;
            }
            catch (Exception ex)
            {
                // LogHelper.Error($"deliveryTag:{deliveryTag},AckAsync ", ex);
                _ = DisposeAsync(); // 不等待释放完成
                return false;
            }
        }

        public async Task<bool> NoAckAsync(ulong deliveryTag, bool multiple, bool requeue)
        {
            try
            {
                ThrowIfDisposed();
                var channel = await GetChannelAsync().ConfigureAwait(false);
                await channel.BasicNackAsync(deliveryTag, multiple, requeue).ConfigureAwait(false);
                return true;
            }
            catch (ObjectDisposedException)
            {
                return false;
            }
            catch (Exception ex)
            {
                // LogHelper.Error($"deliveryTag:{deliveryTag},NoAckAsync ", ex);
                _ = DisposeAsync(); // 不等待释放完成
                return false;
            }
        }

        #endregion

        #region 同步兼容方法（使用返回值）

        public bool ReceiveNeedAck(out string message, out ulong deliveryTag, out string routingKey)
        {
            var result = ReceiveNeedAckAsync().GetAwaiter().GetResult();
            message = result.Message;
            deliveryTag = result.DeliveryTag;
            routingKey = result.RoutingKey;
            return result.Success;
        }

        public void Ack(ulong deliveryTag, bool multiple)
        {
            AckAsync(deliveryTag, multiple).GetAwaiter().GetResult();
        }

        public void NoAck(ulong deliveryTag, bool multiple, bool requeue)
        {
            NoAckAsync(deliveryTag, multiple, requeue).GetAwaiter().GetResult();
        }

        [Obsolete("请使用异步方法 SetConsumerActiveAsync")]
        public AsyncEventingBasicConsumer SetConsumerActive(EventHandler<BasicDeliverEventArgs> e)
        {
            return SetConsumerActiveAsync(async (sender, args) => { await Task.Run(() => e(sender, args)); })
                .GetAwaiter().GetResult();
        }

        #endregion
    }

    /// <summary>
    ///     接收结果类
    /// </summary>
    public class ReceiveResult
    {
        public static readonly ReceiveResult Empty = new ReceiveResult { Success = false };

        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public ulong DeliveryTag { get; set; }
        public string RoutingKey { get; set; } = string.Empty;
    }
}
