using System;
using System.Security.Cryptography.Extensions;
using System.Text;
using System.Threading;
using Newtonsoft.Json.Extension;
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
                    QueueDurable = settings.QueueDurable,
                })
        {
            Settings = settings;
        }

        private IRabbitMqConsumerSettings Settings { get; set; }

        /// <inheritdoc />
        public bool SyncMqConsumerSettings(IRabbitMqConsumerSettings settings)
        {
            Settings = settings;

            return true;
        }


        #region ExecuteReceiveMsg

        /// <inheritdoc />
        public virtual void ExecuteReceiveMsg(Func<string, ulong, string, string, bool> func)
        {
            ReceiveMessageAndExecFunc(func.Invoke);
        }

        /// <inheritdoc />
        public virtual void ExecuteReceiveMsg(Func<string, ulong, string, bool> func)
        {
            ReceiveMessageAndExecFunc((message, deliveryTag, routingKey, messageMd5) =>
                func.Invoke(message, deliveryTag, routingKey));
        }

        /// <inheritdoc />
        public virtual void ExecuteReceiveMsg(Func<string, ulong, bool> func)
        {
            ReceiveMessageAndExecFunc((message, deliveryTag, routingKey, messageMd5) =>
                func.Invoke(message, deliveryTag));
        }

        /// <inheritdoc />
        public virtual void ExecuteReceiveMsg(Func<string, string, bool> func)
        {
            ReceiveMessageAndExecFunc(
                (message, deliveryTag, routingKey, messageMd5) => func.Invoke(message, messageMd5));
        }

        /// <inheritdoc />
        public virtual void ExecuteReceiveMsg(Func<string, string, string, bool> func)
        {
            ReceiveMessageAndExecFunc((message, deliveryTag, routingKey, messageMd5) =>
                func.Invoke(message, messageMd5, routingKey));
        }

        /// <inheritdoc />
        public virtual void ExecuteReceiveMsg(Func<string, bool> func)
        {
            ReceiveMessageAndExecFunc((message, deliveryTag, routingKey, messageMd5) => func.Invoke(message));
        }

        #endregion //end ExecuteReceiveMsg

        #region SetEventingConsumerWithFunc

        /// <inheritdoc />
        public virtual EventingBasicConsumer SetEventingConsumerWithFunc(Func<string, ulong, string, string, bool> func)
        {
            return SetConsumerWithFunc((message, deliveryTag, routingKey, messageMd5) =>
                func.Invoke(message, deliveryTag, routingKey, messageMd5));
        }

        /// <inheritdoc />
        public virtual EventingBasicConsumer SetEventingConsumerWithFunc(Func<string, ulong, string, bool> func)
        {
            return SetConsumerWithFunc((message, deliveryTag, routingKey, messageMd5) =>
                func.Invoke(message, deliveryTag, routingKey));
        }

        /// <inheritdoc />
        public virtual EventingBasicConsumer SetEventingConsumerWithFunc(Func<string, ulong, bool> func)
        {
            return SetConsumerWithFunc((message, deliveryTag, routingKey, messageMd5) =>
                func.Invoke(message, deliveryTag));
        }

        /// <inheritdoc />
        public virtual EventingBasicConsumer SetEventingConsumerWithFunc(Func<string, string, bool> func)
        {
            return SetConsumerWithFunc((message, deliveryTag, routingKey, messageMd5) =>
                func.Invoke(message, messageMd5));
        }

        /// <inheritdoc />
        public virtual EventingBasicConsumer SetEventingConsumerWithFunc(Func<string, string, string, bool> func)
        {
            return SetConsumerWithFunc((message, deliveryTag, routingKey, messageMd5) =>
                func.Invoke(message, messageMd5, routingKey));
        }

        /// <inheritdoc />
        public virtual EventingBasicConsumer SetEventingConsumerWithFunc(Func<string, bool> func)
        {
            return SetConsumerWithFunc((message, deliveryTag, routingKey, messageMd5) => func.Invoke(message));
        }

        #endregion //end SetEventingConsumerWithFunc

        #region Private

        /// <summary>
        /// </summary>
        /// <param name="func">
        ///     Func{in <paramref name="message" /> ,in <paramref name="deliveryTag" />, in <paramref name="routingKey" />,in
        ///     <paramref name="messageMd5" /> ,out bool execResult}
        /// </param>
        private void ReceiveMessageAndExecFunc(Func<string, ulong, string, string, bool> func)
        {
            while (true)
            {
                try
                {
                    var isSuccess = ReceiveNeedAck(out var message, out var deliveryTag, out var routingKey);
                    if (!isSuccess)
                    {
                        Thread.Sleep(TimeSpan.FromMilliseconds(Settings.SleepTime));
                        return;
                    }

                    ProcessOneMessage(func, message, deliveryTag, routingKey);
                }
                catch (Exception ex)
                {
                    LogHelper.Error($"{Settings.QueueName} handler error:{ex.Message}", ex);
                }

            }
        }

        /// <summary>
        /// </summary>
        /// <param name="func">
        ///     Func{in <paramref name="message" /> ,in <paramref name="deliveryTag" />, in <paramref name="routingKey" />,in
        ///     <paramref name="messageMd5" /> ,out bool execResult}
        /// </param>
        /// <returns></returns>
        private EventingBasicConsumer SetConsumerWithFunc(Func<string, ulong, string, string, bool> func)
        {
            var eventConsumer = SetConsumerActive((sender, received) =>
            {
                var message = received.Body.ToArray().ToGetString();
                var obj = new
                {
                    received.Body,
                    message,
                    received.DeliveryTag,
                    received.Exchange,
                    received.ConsumerTag,
                    received.Redelivered,
                    received.RoutingKey,
                };
                if (Settings.LogSwitch)
                {
                    LogHelper.Debug(
                        $"BasicDeliverEventArgs obj is {obj.ToJsonStr()} ,messageMd5 is {message.Md5Encrypt()}");
                }
                ProcessOneMessage(func, message, received.DeliveryTag, received.RoutingKey);
            });

            return eventConsumer;
        }

        /// <summary>
        /// </summary>
        /// <param name="func">
        ///     Func{in <paramref name="message" /> ,in <paramref name="deliveryTag" />, in <paramref name="routingKey" />, in
        ///     string messageMd5 ,out bool execResult}
        /// </param>
        /// <param name="message"></param>
        /// s
        /// <param name="deliveryTag"></param>
        /// <param name="routingKey"></param>
        private void ProcessOneMessage(Func<string, ulong, string, string, bool> func, string message,
            ulong deliveryTag, string routingKey)
        {
            var doAck = true;
            if (!string.IsNullOrEmpty(message))
            {
                try
                {
                    var messageMd5 = message.Md5Encrypt();
                    doAck = TimeWatcherHelper.TimeWatcherUnsafe(
                        () => func.Invoke(message, deliveryTag, routingKey, messageMd5),
                        $"process queue({Settings.QueueName}) with message:{message}",
                        warnMiSeconds: Settings.ProcessWarnMs);
                }
                catch (Exception ex)
                {
                    doAck = false;
                    LogHelper.Error($"{Settings.QueueName} handler error:{ex.Message}", ex);
                    throw;
                }
                finally
                {
                    if (doAck)
                    {
                        Ack(deliveryTag, false);
                    }
                    else
                    {
                        NoAck(deliveryTag, false, true);
                    }
                }

            }
        }

        #endregion //end Private
    }
}
