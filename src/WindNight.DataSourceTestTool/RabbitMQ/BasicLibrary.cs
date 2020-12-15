using RabbitMQ.Client;
using System;
using System.Collections.Generic;

namespace WindNight.DataSourceTestTool.RabbitMQ
{
    internal class BasicLibrary : IDisposable
    {
        private Uri uri;
        private ushort requestedHeartbeat = 30;
        private ConnectionFactory factory;
        private IConnection conn;
        private IModel channel;

        public BasicLibrary(string uri) => this.uri = new Uri(uri);

        public bool IsAlive => this.conn != null && this.conn.IsOpen;

        private ConnectionFactory CreateFactory()
        {
            if (this.factory == null)
            {
                this.factory = new ConnectionFactory();
                if (this.requestedHeartbeat > (ushort)0)
                    this.factory.RequestedHeartbeat = TimeSpan.FromSeconds(this.requestedHeartbeat);
                this.factory.Uri = (this.uri);
            }
            return this.factory;
        }

        private IConnection CreateConnection()
        {
            if (this.conn == null || !this.conn.IsOpen)
                this.conn = this.CreateFactory().CreateConnection();
            return this.conn;
        }

        private IModel CreateChannel()
        {
            if (this.channel == null || !this.channel.IsOpen)
            {
                this.channel = this.CreateConnection().CreateModel();
                //    this.conn.AutoClose = false;
            }
            return this.channel;
        }

        public IModel CreateConsumerChannelByConfig(ConsumerConfigInfo consumerConfigInfo)
        {
            IModel channel = this.CreateChannel();
            channel.QueueDeclare(consumerConfigInfo.QueueName, consumerConfigInfo.QueueDurable, false, false, (IDictionary<string, object>)null);
            return channel;
        }

        public IModel CreateProducerChannelByConfig(ProducerConfigInfo producerConfigInfo)
        {
            IModel channel = this.CreateChannel();
            IModelExensions.ExchangeDeclare(channel, producerConfigInfo.ExchangeName, producerConfigInfo.ExchangeTypeCode, producerConfigInfo.ExchangeDurable, false, (IDictionary<string, object>)null);
            return channel;
        }

        public void Dispose()
        {
            try
            {
                if (this.channel != null)
                    ((IDisposable)this.channel).Dispose();
                if (this.conn == null)
                    return;
                ((IDisposable)this.conn).Dispose();
            }
            catch (Exception ex)
            {
                //RecordLog.WriteLog("BasicLibrary Dispose ", "", "", "", ex);
            }
        }

        ~BasicLibrary() => this.Dispose();
    }
}
