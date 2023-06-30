using RabbitMQ.Client;
using System;
using System.Threading;

namespace WindNight.DataSourceTestTool.RabbitMQ
{
    public class Producer : IDisposable
    {
        private string uri;
        private IModel model;
        private object lockObj;
        private ProducerConfigInfo producerConfigInfo;
        private BasicLibrary basicLibrary;
        //  private ILocalMessageWapper wapper;
        private Producer.ExceptionStatus exceptionStatus;

        public Producer(string uri, ProducerConfigInfo producerConfigInfo)
        {
            this.lockObj = new object();
            this.uri = uri;
            this.producerConfigInfo = producerConfigInfo;
            // this.wapper = (ILocalMessageWapper)new DefaultLocalMessageWapper(producerConfigInfo.ExchangeName);
        }

        ~Producer()
        {
            this.Dispose();
            GC.Collect();
        }
        public bool IsChannelOpen => this.model != null && this.model.IsOpen;

        private IModel Channel
        {
            get
            {
                if (this.model == null || !this.model.IsOpen)
                {
                    lock (this.lockObj)
                    {
                        if (this.model != null && !this.model.IsOpen)
                            this.basicLibrary.Dispose();
                        this.InitChannel();
                    }
                }
                return this.model;
            }
        }

        private void InitChannel()
        {
            this.basicLibrary = new BasicLibrary(this.uri);
            this.model = this.basicLibrary.CreateProducerChannelByConfig(this.producerConfigInfo);
        }

        public void Send(string message, string routingKey, bool isMessageDurable = true)
            => this.Send(message, routingKey, new BasicPropertiesConfigInfo
            {
                Durable = isMessageDurable,

            });

        public bool SendWithNotRetry(string message, string routingKey, bool isMessageDurable = true)
        {
            try
            {
                IBasicProperties basicProperties = CommonLibrary.CreateBasicProperties(this.Channel, new BasicPropertiesConfigInfo()
                {
                    Durable = isMessageDurable
                });
                byte[] body = CommonLibrary.BinarySerialize(message);
                this.Channel.BasicPublish(this.producerConfigInfo.ExchangeName, routingKey, basicProperties, body);
                return true;
            }
            catch (Exception ex)
            {
                // RecordLog.WriteLog("ExchangeName:" + this.producerConfigInfo.ExchangeName + ",RoutingKey:" + routingKey + ",发送失败", this.producerConfigInfo.ExchangeName, routingKey, "", ex);
                this.Dispose();
                return false;
            }
        }

        public bool SendWithNotRetry(byte[] messageBodyBytes, string routingKey, bool isMessageDurable = true)
        {
            try
            {
                IBasicProperties basicProperties = CommonLibrary.CreateBasicProperties(this.Channel, new BasicPropertiesConfigInfo()
                {
                    Durable = isMessageDurable
                });
                this.Channel.BasicPublish(this.producerConfigInfo.ExchangeName, routingKey, basicProperties, messageBodyBytes);
                return true;
            }
            catch (Exception ex)
            {
                //   RecordLog.WriteLog("ExchangeName:" + this.producerConfigInfo.ExchangeName + ",RoutingKey:" + routingKey + ",发送失败", this.producerConfigInfo.ExchangeName, routingKey, "", ex);
                this.Dispose();
                return false;
            }
        }

        public bool Send(
          string message,
          string routingKey,
          BasicPropertiesConfigInfo basicPropertiesConfigInfo)
        {
            IBasicProperties basicProperties = basicPropertiesConfigInfo == null ? (IBasicProperties)null : CommonLibrary.CreateBasicProperties(this.Channel, basicPropertiesConfigInfo);
            byte[] body = CommonLibrary.BinarySerialize(message);
            //try
            //{
            this.Channel.BasicPublish(this.producerConfigInfo.ExchangeName, routingKey, basicProperties, body);
            this.RepairException();
            return true;
            //}
            //catch (Exception ex)
            //{
            //    //RecordLog.WriteLog("ExchangeName:" + this.producerConfigInfo.ExchangeName + ",RoutingKey:" + routingKey + ",发送失败:本次消息已记录到本地临时文件", this.producerConfigInfo.ExchangeName, routingKey, "", ex);
            //    this.SaveException(message, routingKey, this.producerConfigInfo, basicPropertiesConfigInfo);
            //    this.Dispose();
            //    return false;
            //}
        }

        public void Dispose()
        {
            //try
            //{
            if (this.basicLibrary != null)
                this.basicLibrary.Dispose();
            if (this.model == null)
                return;
            ((IDisposable)this.model).Dispose();
            //}
            //catch (Exception ex)
            //{
            //    // RecordLog.WriteLog("Producer Dispose ", "", "", "", ex);
            //}
        }

        private void SaveException(
          string message,
          string routingKey,
          ProducerConfigInfo producerConfigInfo,
          BasicPropertiesConfigInfo basicPropertiesConfigInfo)
        {
            this.exceptionStatus = Producer.ExceptionStatus.Exception;
            try
            {
                //this.wapper.Write(new MessageLocal()
                //{
                //    Message = message,
                //    RoutingKey = routingKey,
                //    BasicPropertiesConfigInfo = basicPropertiesConfigInfo,
                //    ProducerConfigInfo = producerConfigInfo
                //});
            }
            catch (Exception ex)
            {
                //  RecordLog.WriteLog("ExchangeName:" + producerConfigInfo.ExchangeName + ",RoutingKey:" + routingKey + ",Message:" + message + ",SaveException", producerConfigInfo.ExchangeName, routingKey, "", ex);
                throw ex;
            }
        }

        private void RepairException()
        {
            if (!this.CheckNeedRepair())
                return;
            //this.exceptionStatus = Producer.ExceptionStatus.Repired;
            //ThreadPool.QueueUserWorkItem((WaitCallback)(p =>
            //{
            //    Producer producer1 = (Producer)p;
            //    MessageLocal messageLocal;
            //    while ((messageLocal = producer1.wapper.ReadLine()) != null)
            //    {
            //        Producer producer2 = new Producer(this.uri, messageLocal.ProducerConfigInfo);
            //        producer1.Send(messageLocal.Message, messageLocal.RoutingKey);
            //        Thread.Sleep(1);
            //        producer2.Dispose();
            //    }
            //}), (object)this);
        }

        private bool CheckNeedRepair()
        {
            bool flag = false;
            if (this.exceptionStatus == Producer.ExceptionStatus.Exception)
                flag = true;
            //else if (this.exceptionStatus == Producer.ExceptionStatus.None)
            //    flag = this.wapper.Exist();
            return flag;
        }

        private enum ExceptionStatus
        {
            None,
            Exception,
            Repired,
        }
    }
}
