using System.Security.Cryptography.Extensions;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using WindNight.Core.Tools;
using WindNight.RabbitMq.Abstractions;
using WindNight.RabbitMq.@internal;

namespace WindNight.RabbitMq
{
    public class DefaultRabbitMqConsumer : Consumer, IRabbitMqConsumer
    {
        public DefaultRabbitMqConsumer(IRabbitMqConsumerSettings settings) :
            base(settings.RabbitMqUrl,
                new ConsumerConfigInfo
                {
                    QueueName = settings.QueueName,
                    PrefetchCount = settings.PrefetchCount,
                    QueueDurable = settings.QueueDurable
                })
        {
            Settings = settings;
        }

        public static string CurrentVersion => BuildInfo.BuildVersion;
        public static string CurrentCompileTime => BuildInfo.BuildTime;

        private IRabbitMqConsumerSettings Settings { get; set; }

        public bool SyncMqConsumerSettings(IRabbitMqConsumerSettings settings)
        {
            Settings = settings;
            return true;
        }

        #region ExecuteReceiveMsg 同步实现

        public void ExecuteReceiveMsg(Func<string, ulong, string, string, bool> func)
        {
            Task.Run(async () =>
                await ReceiveMessageLoopAsync(async (m, d, r, md5) => await Task.FromResult(func(m, d, r, md5))
                )).GetAwaiter().GetResult();
        }

        public void ExecuteReceiveMsg(Func<string, ulong, string, bool> func)
        {
            Task.Run(async () =>
                await ReceiveMessageLoopAsync(async (m, d, r, md5) => await Task.FromResult(func(m, d, r))
                )).GetAwaiter().GetResult();
        }

        public void ExecuteReceiveMsg(Func<string, ulong, bool> func)
        {
            Task.Run(async () => await ReceiveMessageLoopAsync(async (m, d, r, md5) => await Task.FromResult(func(m, d))
            )).GetAwaiter().GetResult();
        }

        public void ExecuteReceiveMsg(Func<string, string, bool> func)
        {
            Task.Run(async () =>
                await ReceiveMessageLoopAsync(async (m, d, r, md5) => await Task.FromResult(func(m, md5))
                )).GetAwaiter().GetResult();
        }

        public void ExecuteReceiveMsg(Func<string, string, string, bool> func)
        {
            Task.Run(async () =>
                await ReceiveMessageLoopAsync(async (m, d, r, md5) => await Task.FromResult(func(m, md5, r))
                )).GetAwaiter().GetResult();
        }

        public void ExecuteReceiveMsg(Func<string, bool> func)
        {
            Task.Run(async () => await ReceiveMessageLoopAsync(async (m, d, r, md5) => await Task.FromResult(func(m))
            )).GetAwaiter().GetResult();
        }

        #endregion

        #region SetEventingConsumerWithFunc 同步实现

        public AsyncEventingBasicConsumer SetEventingConsumerWithFunc(Func<string, ulong, string, string, bool> func)
        {
            return SetConsumerWithFuncAsync(async (m, d, r, md5) => await Task.FromResult(func(m, d, r, md5))
            ).GetAwaiter().GetResult();
        }

        public AsyncEventingBasicConsumer SetEventingConsumerWithFunc(Func<string, ulong, string, bool> func)
        {
            return SetConsumerWithFuncAsync(async (m, d, r, md5) => await Task.FromResult(func(m, d, r))
            ).GetAwaiter().GetResult();
        }

        public AsyncEventingBasicConsumer SetEventingConsumerWithFunc(Func<string, ulong, bool> func)
        {
            return SetConsumerWithFuncAsync(async (m, d, r, md5) => await Task.FromResult(func(m, d))
            ).GetAwaiter().GetResult();
        }

        public AsyncEventingBasicConsumer SetEventingConsumerWithFunc(Func<string, string, bool> func)
        {
            return SetConsumerWithFuncAsync(async (m, d, r, md5) => await Task.FromResult(func(m, md5))
            ).GetAwaiter().GetResult();
        }

        public AsyncEventingBasicConsumer SetEventingConsumerWithFunc(Func<string, string, string, bool> func)
        {
            return SetConsumerWithFuncAsync(async (m, d, r, md5) => await Task.FromResult(func(m, md5, r))
            ).GetAwaiter().GetResult();
        }

        public AsyncEventingBasicConsumer SetEventingConsumerWithFunc(Func<string, bool> func)
        {
            return SetConsumerWithFuncAsync(async (m, d, r, md5) => await Task.FromResult(func(m))
            ).GetAwaiter().GetResult();
        }

        #endregion





        #region 同步接收方法

        public bool Receive(out string message)
        {
            return Receive(out message, out _);
        }

        public bool Receive(out string message, out string routingKey)
        {
            var result = ReceiveInternalAsync(true).GetAwaiter().GetResult();
            message = result.Message;
            routingKey = result.RoutingKey;
            return result.Success;
        }

        public bool ReceiveNeedAck(out string message, out ulong deliveryTag)
        {
            return ReceiveNeedAck(out message, out deliveryTag, out _);
        }

        public bool ReceiveNeedAck(out string message, out ulong deliveryTag, out string routingKey)
        {
            var result = ReceiveInternalAsync(false).GetAwaiter().GetResult();
            message = result.Message;
            deliveryTag = result.DeliveryTag;
            routingKey = result.RoutingKey;
            return result.Success;
        }

        public bool Ack(ulong deliveryTag, bool multiple)
        {
            return AckAsync(deliveryTag, multiple).GetAwaiter().GetResult();
        }

        public bool NoAck(ulong deliveryTag, bool multiple, bool requeue)
        {
            return NackAsync(deliveryTag, multiple, requeue).GetAwaiter().GetResult();
        }

        public AsyncEventingBasicConsumer SetConsumerActive(EventHandler<BasicDeliverEventArgs> e)
        {
            return SetConsumerActiveAsync(async (sender, args) => { await Task.Run(() => e(sender, args)); })
                .GetAwaiter().GetResult();
        }
       

        #endregion

        #region 私有异步方法

        private async Task ReceiveMessageLoopAsync(Func<string, ulong, string, string, Task<bool>> asyncFunc)
        {
            while (true)
            {
                try
                {
                    var (success, message, deliveryTag, routingKey) =
                        await ReceiveInternalAsync(false).ConfigureAwait(false);

                    if (!success)
                    {
                        await Task.Delay(TimeSpan.FromMilliseconds(Settings.SleepTime)).ConfigureAwait(false);
                        continue;
                    }

                    await ProcessOneMessageAsync(asyncFunc, message, deliveryTag, routingKey).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    LogHelper.Error($"{Settings.QueueName} handler error:{ex.Message}", ex);
                    await Task.Delay(TimeSpan.FromMilliseconds(Settings.SleepTime)).ConfigureAwait(false);
                }
            }
        }

        private async Task<AsyncEventingBasicConsumer> SetConsumerWithFuncAsync(
            Func<string, ulong, string, string, Task<bool>> asyncFunc)
        {
            return await SetConsumerActiveAsync(async (sender, received) =>
            {
                try
                {
                    var message = Encoding.UTF8.GetString(received.Body.ToArray());

                    if (Settings.LogSwitch)
                    {
                        var messageMd5 = message.Md5Encrypt();
                        LogHelper.Debug(
                            $"BasicDeliverEventArgs: DeliveryTag={received.DeliveryTag}, RoutingKey={received.RoutingKey}, MessageMd5={messageMd5}");
                    }

                    await ProcessOneMessageAsync(asyncFunc, message, received.DeliveryTag, received.RoutingKey)
                        .ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    LogHelper.Error($"{Settings.QueueName} consumer error:{ex.Message}", ex);
                    if (sender is AsyncEventingBasicConsumer consumer && consumer.Channel is IChannel channel)
                    {
                        await channel.BasicNackAsync(received.DeliveryTag, false, true).ConfigureAwait(false);
                    }
                }
            }).ConfigureAwait(false);
        }

        private async Task ProcessOneMessageAsync(
            Func<string, ulong, string, string, Task<bool>> asyncFunc,
            string message,
            ulong deliveryTag,
            string routingKey)
        {
            var doAck = true;

            try
            {
                if (!string.IsNullOrEmpty(message))
                {
                    var messageMd5 = message.Md5Encrypt();

                    doAck = await TimeWatcherHelper.TimeWatcherUnsafe(
                        async () => await asyncFunc(message, deliveryTag, routingKey, messageMd5).ConfigureAwait(false),
                        $"process queue({Settings.QueueName}) with message:{message}",
                        warnMiSeconds: Settings.ProcessWarnMs).ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                doAck = false;
                LogHelper.Error($"{Settings.QueueName} handler error:{ex.Message}", ex);
            }
            finally
            {
                if (doAck)
                {
                    await AckAsync(deliveryTag, false).ConfigureAwait(false);
                }
                else
                {
                    await NackAsync(deliveryTag, false, true).ConfigureAwait(false);
                }
            }
        }

        #endregion




        #region SetEventingConsumerWithFunc 异步实现

        public async Task<AsyncEventingBasicConsumer> SetEventingConsumerWithFuncAsync(Func<string, ulong, string, string, Task<bool>> func)
        {
            return await SetConsumerWithFuncAsync(async (m, d, r, md5) => await func(m, d, r, md5)).ConfigureAwait(false);
        }

        public async Task<AsyncEventingBasicConsumer> SetEventingConsumerWithFuncAsync(Func<string, ulong, string, Task<bool>> func)
        {
            return await SetConsumerWithFuncAsync(async (m, d, r, md5) => await func(m, d, r)
            ).ConfigureAwait(false);
        }

        public async Task<AsyncEventingBasicConsumer> SetEventingConsumerWithFuncAsync(Func<string, ulong, Task<bool>> func)
        {
            return await SetConsumerWithFuncAsync(async (m, d, r, md5) => await func(m, d)
            ).ConfigureAwait(false);
        }

        public async Task<AsyncEventingBasicConsumer> SetEventingConsumerWithFuncAsync(Func<string, string, Task<bool>> func)
        {
            return await SetConsumerWithFuncAsync(async (m, d, r, md5) => await func(m, md5)
            ).ConfigureAwait(false);
        }

        public async Task<AsyncEventingBasicConsumer> SetEventingConsumerWithFuncAsync(Func<string, string, string, Task<bool>> func)
        {
            return await SetConsumerWithFuncAsync(async (m, d, r, md5) => await func(m, md5, r)
            ).ConfigureAwait(false);
        }

        public async Task<AsyncEventingBasicConsumer> SetEventingConsumerWithFuncAsync(Func<string, Task<bool>> func)
        {
            return await SetConsumerWithFuncAsync(async (m, d, r, md5) => await func(m)
            ).ConfigureAwait(false);
        }

        #endregion










    }
}
