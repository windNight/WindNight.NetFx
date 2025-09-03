using System.Text;
using RabbitMQ.Client;
using WindNight.Linq.Extensions.Expressions;
using WindNight.RabbitMq.Abstractions;

namespace WindNight.RabbitMq.@internal
{
    internal class CommonLibrary
    {
        /// <summary>
        ///     创建消息基础属性
        /// </summary>
        /// <param name="model">通道</param>
        /// <param name="configInfo">消息基础属性</param>
        /// <returns></returns>
        public static IBasicProperties CreateBasicProperties(IModel model, BasicProperties configInfo)
        {
            var basicProperties = model.CreateBasicProperties();
            if (configInfo.AppID != string.Empty)
            {
                basicProperties.AppId = configInfo.AppID;
            }

            if (configInfo.ClusterID != string.Empty)
            {
                basicProperties.ClusterId = configInfo.ClusterID;
            }

            if (configInfo.ContentEncoding != string.Empty)
            {
                basicProperties.ContentEncoding = configInfo.ContentEncoding;
            }

            if (configInfo.ContentType != string.Empty)
            {
                basicProperties.ContentType = configInfo.ContentType;
            }

            if (configInfo.CorrelationID != string.Empty)
            {
                basicProperties.CorrelationId = configInfo.CorrelationID;
            }

            basicProperties.DeliveryMode = configInfo.Durable ? Convert.ToByte(2) : Convert.ToByte(1);
            if (configInfo.Expiration > 0)
            {
                basicProperties.Expiration = configInfo.Expiration.ToString();
            }

            if (configInfo.Headers != null)
            {
                basicProperties.Headers = configInfo.Headers;
            }

            if (configInfo.MessageID != string.Empty)
            {
                basicProperties.MessageId = configInfo.MessageID;
            }

            if (configInfo.Priority != -1)
            {
                basicProperties.Priority = Convert.ToByte(configInfo.Priority);
            }

            if (configInfo.ReplyTo != string.Empty)
            {
                basicProperties.ReplyTo = configInfo.ReplyTo;
            }

            if (configInfo.Timestamp != -1)
            {
                basicProperties.Timestamp = new AmqpTimestamp(configInfo.Timestamp);
            }

            if (configInfo.Type != string.Empty)
            {
                basicProperties.Type = configInfo.Type;
            }

            if (configInfo.UserID != string.Empty)
            {
                basicProperties.UserId = configInfo.UserID;
            }

            return basicProperties;
        }

        /// <summary>
        ///     序列化成byte[]
        /// </summary>
        /// <returns></returns>
        public static byte[] BinarySerialize(string message_string)
        {
            if (message_string.IsNullOrEmpty())
            {
                return null;
            }

            return message_string.ToBytes();
        }

        /// <summary>
        ///     byte[]反序列化
        /// </summary>
        /// <returns></returns>
        public static string BinaryDeserialize(byte[] bytes)
        {
            if (bytes.IsNullOrEmpty())
            {
                return null;
            }

            return bytes.ToGetString();
        }
    }
}
