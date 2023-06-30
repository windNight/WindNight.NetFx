using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Threading;
namespace WindNight.DataSourceTestTool.RabbitMQ
{
    public class Consumer : IDisposable
    {
        private IModel model;
        private string uri;
        private object lockObj;
        private ConsumerConfigInfo consumerConfigInfo;
        private EventingBasicConsumer consumerPassive;
        private BasicLibrary basicLibrary;

        private EventingBasicConsumer CreateConsumerAck(
          EventHandler<BasicDeliverEventArgs> e)
        {
            EventingBasicConsumer eventingBasicConsumer = new EventingBasicConsumer(this.Channel);
            eventingBasicConsumer.Received += (e);
            this.Channel.BasicQos(0U, this.consumerConfigInfo.PrefetchCount, false);
            this.Channel.BasicConsume(this.consumerConfigInfo.QueueName, false, (IBasicConsumer)eventingBasicConsumer);
            return eventingBasicConsumer;
        }

        public EventingBasicConsumer SetConsumerActive(
          EventHandler<BasicDeliverEventArgs> e)
        {
            if (this.consumerPassive == null || !this.consumerPassive.IsRunning)
                this.consumerPassive = this.CreateConsumerAck(e);
            return this.consumerPassive;
        }

        private IModel Channel
        {
            get
            {
                if (this.model == null || !this.model.IsOpen)
                {
                    lock (this.lockObj)
                    {
                        if (this.model != null)
                        {
                            if (this.model.IsOpen)
                                goto label_8;
                        }
                        this.InitChannel();
                    }
                }
            label_8:
                return this.model;
            }
        }

        public bool IsChannelOpen => this.model != null && this.model.IsOpen;

        private void InitChannel()
        {
            this.basicLibrary = new BasicLibrary(this.uri);
            this.model = this.basicLibrary.CreateConsumerChannelByConfig(this.consumerConfigInfo);
        }

        public Consumer(string uri, ConsumerConfigInfo consumerConfigInfo)
        {
            this.lockObj = new object();
            this.uri = uri;
            this.consumerConfigInfo = consumerConfigInfo;
        }

        //public bool Receive(out string message) => this.Receive(out message, out string _);

        //public bool Receive(out string message, out string routingKey)
        //{
        //    message = string.Empty;
        //    routingKey = string.Empty;
        //    try
        //    {
        //        BasicGetResult basicGetResult = this.Channel.BasicGet(this.consumerConfigInfo.QueueName, true);
        //        if (basicGetResult == null)
        //        {
        //            Thread.Sleep(500);
        //            return false;
        //        }
        //        byte[] body = basicGetResult.Body.ToArray();
        //        message = CommonLibrary.BinaryDeserialize(body);
        //        routingKey = basicGetResult.RoutingKey;
        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        //RecordLog.WriteLog("QueueName:" + this.consumerConfigInfo.QueueName + ",Receive ", "", "", this.consumerConfigInfo.QueueName, ex);
        //        Thread.Sleep(500);
        //        this.Dispose();
        //        return false;
        //    }
        //}

        // public bool ReceiveNeedAck(out string message, out ulong deliveryTag) => this.ReceiveNeedAck(out message, out deliveryTag, out string _);

        public bool ReceiveNeedAck(out string message, out ulong deliveryTag, out string routingKey)
        {
            message = string.Empty;
            routingKey = string.Empty;
            deliveryTag = 0UL;
            try
            {
                BasicGetResult basicGetResult = this.Channel.BasicGet(this.consumerConfigInfo.QueueName, false);
                if (basicGetResult == null)
                {
                    Thread.Sleep(500);
                    return false;
                }
                byte[] body = basicGetResult.Body.ToArray();
                message = CommonLibrary.BinaryDeserialize(body);
                routingKey = basicGetResult.RoutingKey;
                deliveryTag = basicGetResult.DeliveryTag;
                return true;
            }
            catch (Exception ex)
            {
                // RecordLog.WriteLog("QueueName:" + this.consumerConfigInfo.QueueName + ",ReceiveNeedAck", "", "", this.consumerConfigInfo.QueueName, ex);
                Thread.Sleep(500);
                this.Dispose();
                return false;
            }
        }

        public void Ack(ulong deliveryTag, bool multiple)
        {
            try
            {
                this.Channel.BasicAck(deliveryTag, multiple);
            }
            catch (Exception ex)
            {
                // RecordLog.WriteLog(string.Format("deliveryTag:{0},Ack ", (object)deliveryTag), "", "", "", ex);
                this.Dispose();
            }
        }

        public void NoAck(ulong deliveryTag, bool multiple, bool requeue)
        {
            //try
            //{
            this.Channel.BasicNack(deliveryTag, multiple, requeue);
            //}
            //catch (Exception ex)
            //{
            //    // RecordLog.WriteLog(string.Format("deliveryTag:{0},NAck ", (object)deliveryTag), "", "", "", ex);
            //    this.Dispose();
            //}
        }

        public void Dispose()
        {

            if (this.basicLibrary != null)
                this.basicLibrary.Dispose();
            if (this.model == null)
                return;
            ((IDisposable)this.model).Dispose();

        }
    }
}

