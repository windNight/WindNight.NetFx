using System;
using System.Threading;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using WindNight.RabbitMq.Abstractions;
using WindNight.RabbitMq.@internal;

namespace WindNight.RabbitMq
{
    /// <summary>
    ///     消费者类
    /// </summary>
    public class Consumer //: IDisposable
    {
        private readonly ConsumerConfigInfo consumerConfigInfo;

        private readonly object lockObj;

        private readonly string uri;
        private BasicLibrary basicLibrary;

        private EventingBasicConsumer consumerPassive;


        private IModel model;

        /// <summary>
        ///     消费者操作
        /// </summary>
        /// <param name="uri">amqp地址</param>
        /// <param name="consumerConfigInfo">消费者通道配置信息</param>
        public Consumer(string uri, ConsumerConfigInfo consumerConfigInfo)
        {
            lockObj = new object();
            this.uri = uri;
            this.consumerConfigInfo = consumerConfigInfo;
        }

        /// <summary>
        ///     渠道连接信息
        /// </summary>
        private IModel Model
        {
            get
            {
                if (model == null)
                    lock (lockObj)
                    {
                        if (model == null) CreateModel();
                    }

                return model;
            }
        }

        /// <summary>
        ///     渠道连接是否为开启状态
        /// </summary>
        public bool IsChannelOpen
        {
            get
            {
                if (!model.IsOpen) return false;

                return true;
            }
        }


        /// <summary>
        ///     创建被动接收（主动应答式的消费者）事件
        /// </summary>
        /// <returns></returns>
        public EventingBasicConsumer SetConsumerActive(EventHandler<BasicDeliverEventArgs> e)
        {
            try
            {
                if (consumerPassive == null)
                {
                    var consumer = new EventingBasicConsumer(Model);
                    consumer.Received += e;

                    Model.BasicQos(0, consumerConfigInfo.PrefetchCount, false);
                    Model.BasicConsume(consumerConfigInfo.QueueName, false, consumer);

                    consumerPassive = consumer;
                }

                return consumerPassive;
            }
            catch (Exception ex)
            {
                LogHelper.Error($"SetConsumerActive Handler Error {ex.Message}", ex);
                return default;
            }
        }

        /// <summary>
        ///     创建连接渠道
        /// </summary>
        /// <returns></returns>
        private void CreateModel()
        {
            basicLibrary = new BasicLibrary(uri);
            model = basicLibrary.CreateConsumerChannelByConfig(consumerConfigInfo);
        }


        ~Consumer()
        {
            Dispose();
            GC.Collect();
        }

        /// <summary>
        ///     接受
        /// </summary>
        /// <remarks>效率高，但在断开前一刻，消息队列中如果有未被读取的消息会被清空，在断开后有新消息时还是会保留在MQ中，可以在下次连接时获取，所以不保证完整性</remarks>
        /// <param name="message">获取消息</param>
        /// <returns></returns>
        public bool Receive(out string message)
        {
            string routingKey;
            return Receive(out message, out routingKey);
        }

        /// <summary>
        ///     无需ack
        /// </summary>
        /// <param name="message">获取消息</param>
        /// <param name="routingKey">路由键</param>
        /// <returns></returns>
        public bool Receive(out string message, out string routingKey)
        {
            message = string.Empty;
            routingKey = string.Empty;
            try
            {
                var res = Model.BasicGet(consumerConfigInfo.QueueName, true);
                if (res == null)
                {
                    Thread.Sleep(200);
                    return false;
                }

                var bytes = res.Body.ToArray();
                message = CommonLibrary.BinaryDeserialize(bytes);
                routingKey = res.RoutingKey;
                return true;
            }
            catch (Exception ex)
            {
                var errLog = $"QueueName:{consumerConfigInfo.QueueName},Receive ";
                LogHelper.Error(errLog, ex);
                Thread.Sleep(500);
                //Dispose();
                return false;
            }
        }

        /// <summary>
        ///     接受并且需要调用Ack方法告知MQ已完成接收
        /// </summary>
        /// <param name="message">获取消息</param>
        /// <remarks>可确保消息的完整性，效率，未完成的数据将需重新连接MQ获取</remarks>
        /// <param name="deliveryTag">该条消息识别码，调用Ack方法时使用</param>
        /// <returns></returns>
        public bool ReceiveNeedAck(out string message, out ulong deliveryTag)
        {
            string routingKey;
            return ReceiveNeedAck(out message, out deliveryTag, out routingKey);
        }

        /// <summary>
        ///     接受并且需要调用Ack方法告知MQ已完成接收
        /// </summary>
        /// <param name="message">获取消息</param>
        /// <param name="deliveryTag">该条消息识别码，调用Ack方法时使用</param>
        /// <param name="routingKey">路由键</param>
        /// <returns></returns>
        public bool ReceiveNeedAck(out string message, out ulong deliveryTag, out string routingKey)
        {
            message = string.Empty;
            routingKey = string.Empty;
            deliveryTag = 0;
            try
            {
                var res = Model.BasicGet(consumerConfigInfo.QueueName, false);
                if (res == null)
                {
                    Thread.Sleep(200);
                    return false;
                }

                var bytes = res.Body.ToArray();
                message = CommonLibrary.BinaryDeserialize(bytes);
                routingKey = res.RoutingKey;
                deliveryTag = res.DeliveryTag;
                return true;
            }
            catch (Exception ex)
            {
                var errLog = $"QueueName:{consumerConfigInfo.QueueName},ReceiveNeedAck";
                LogHelper.Error(errLog, ex);
                Thread.Sleep(500);
                //Dispose();
                return false;
            }
        }

        /// <summary>
        ///     告知服务已经收到该消息
        /// </summary>
        /// <param name="deliveryTag">消息识别码</param>
        /// <param name="multiple">是否通知多条消息(一般情况下为false)</param>
        /// <returns></returns>
        public bool Ack(ulong deliveryTag, bool multiple)
        {
            try
            {
                Model.BasicAck(deliveryTag, multiple);
                return true;
            }
            catch (Exception ex)
            {
                var errLog = $"deliveryTag:{deliveryTag},Ack ";
                LogHelper.Error(errLog, ex);
                //Dispose();
                return false;
            }
        }

        /// <summary>
        ///     告知服务已经收到该消息
        /// </summary>
        /// <param name="deliveryTag">消息识别码</param>
        /// <param name="multiple">是否通知多条消息(一般情况下为false)</param>
        /// <param name="requeue">是否扔回队列</param>
        /// <returns></returns>
        public bool NoAck(ulong deliveryTag, bool multiple, bool requeue)
        {
            try
            {
                Model.BasicNack(deliveryTag, multiple, requeue);
                return true;
            }
            catch (Exception ex)
            {
                var errLog = $"deliveryTag:{deliveryTag},NAck ";
                LogHelper.Error(errLog, ex);
                //Dispose();
                return false;
            }
        }

        /// <summary>
        ///     释放资源
        /// </summary>
        public void Dispose()
        {
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
        }
    }
}
