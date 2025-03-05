using System;
using RabbitMQ.Client;
using WindNight.RabbitMq.Abstractions;

namespace WindNight.RabbitMq.@internal
{
    internal class BasicLibrary : IDisposable
    {
        /// <summary>
        ///     心跳包60秒一次
        /// </summary>
        private readonly ushort requestedHeartbeat = 30;

        private readonly Uri uri;

        public BasicLibrary(string uri)
        {
            this.uri = new Uri(uri);
        }

        public void Dispose()
        {
            if (_model != null)
            {
                try
                {
                    _model.Dispose();
                    _model.Close();
                }
                catch (Exception ex)
                {
                    LogHelper.Error("model.Dispose()", ex);
                }

                _model = null;
            }

            if (conn != null)
            {
                try
                {
                    conn.Dispose();
                    conn.Close();
                }
                catch (Exception ex)
                {
                    LogHelper.Error("conn.Dispose() ", ex);
                }

                conn = null;
            }
        }

        ~BasicLibrary()
        {
            Dispose();
        }

        #region 通道创建

        private ConnectionFactory factory;
        private IConnection conn;
        private IModel _model;

        /// <summary>
        ///     创建链接工厂(ConnectionFactory)
        /// </summary>
        /// <returns></returns>
        private ConnectionFactory CreateFactory()
        {
            if (factory == null)
            {
                factory = new ConnectionFactory();
                if (requestedHeartbeat > 0)
                    factory.RequestedHeartbeat = TimeSpan.FromSeconds(requestedHeartbeat);

                factory.AutomaticRecoveryEnabled = true;

                factory.Uri = uri;
            }

            return factory;
        }

        /// <summary>
        ///     创建链接(Connection)
        /// </summary>
        /// <returns></returns>
        private IConnection CreateConnection()
        {
            if (conn == null || !conn.IsOpen)
                conn = CreateFactory().CreateConnection();
            // conn.AutoClose = false;
            return conn;
        }

        /// <summary>
        ///     创建通道(Model)
        /// </summary>
        /// <returns></returns>
        private IModel CreateModel()
        {
            if (_model == null || !_model.IsOpen) _model = CreateConnection().CreateModel();
            return _model;
        }

        /// <summary>
        ///     通过消费者配置创建通道
        /// </summary>
        /// <param name="consumerConfigInfo"></param>
        /// <returns></returns>
        public IModel CreateConsumerChannelByConfig(ConsumerConfigInfo consumerConfigInfo)
        {
            var model = CreateModel();
            model.QueueDeclare(consumerConfigInfo.QueueName, consumerConfigInfo.QueueDurable, false, false, null);
            return model;
        }

        /// <summary>
        ///     通过生产者配置创建通道
        /// </summary>
        /// <param name="producerConfigInfo"></param>
        /// <returns></returns>
        public IModel CreateProducerChannelByConfig(ProducerConfigInfo producerConfigInfo)
        {
            var model = CreateModel();
            model.ExchangeDeclare(producerConfigInfo.ExchangeName,
                producerConfigInfo.ExchangeTypeCode.ToString().ToLower(),
                producerConfigInfo.ExchangeDurable);
            return model;
        }

        #endregion
    }
}
