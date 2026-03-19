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
        /// <param name="channel">通道</param>
        /// <param name="configInfo">消息基础属性</param>
        /// <returns></returns>
        public static BasicProperties CreateBasicProperties(IChannel channel, BasicMqProperties configInfo)
        {
            var basicProperties = new BasicProperties(); // RabbitMQ.Client 7.x 中直接创建

            if (configInfo.AppId.IsNotNullOrEmpty())
            {
                basicProperties.AppId = configInfo.AppId;
            }

            if (configInfo.ClusterId.IsNotNullOrEmpty())
            {
                basicProperties.ClusterId = configInfo.ClusterId;
            }

            if (configInfo.ContentEncoding.IsNotNullOrEmpty())
            {
                basicProperties.ContentEncoding = configInfo.ContentEncoding;
            }

            if (configInfo.ContentType.IsNotNullOrEmpty())
            {
                basicProperties.ContentType = configInfo.ContentType;
            }

            if (configInfo.CorrelationId.IsNotNullOrEmpty())
            {
                basicProperties.CorrelationId = configInfo.CorrelationId;
            }

            basicProperties.DeliveryMode = configInfo.Durable ? (DeliveryModes.Persistent) : (DeliveryModes.Transient);


            if (configInfo.Expiration >0)
            {
                basicProperties.Expiration = configInfo.Expiration.ToString();
            }

            if (configInfo.Headers != null)
            {
                basicProperties.Headers = configInfo.Headers;
            }

            if (configInfo.MessageId.IsNotNullOrEmpty())
            {
                basicProperties.MessageId = configInfo.MessageId;
            }

            if (configInfo.Priority != -1)
            {
                basicProperties.Priority = Convert.ToByte(configInfo.Priority);
            }

            if (configInfo.ReplyTo.IsNotNullOrEmpty())
            {
                basicProperties.ReplyTo = configInfo.ReplyTo;
            }

            if (configInfo.Timestamp != -1)
            {
                basicProperties.Timestamp = new AmqpTimestamp(configInfo.Timestamp);
            }

            if (configInfo.Type.IsNotNullOrEmpty())
            {
                basicProperties.Type = configInfo.Type;
            }

            if (configInfo.UserId.IsNotNullOrEmpty())
            {
                basicProperties.UserId = configInfo.UserId;
            }

            return basicProperties;
        }

        /// <summary>
        ///     序列化成byte[]
        /// </summary>
        /// <returns></returns>
        public static byte[] BinarySerialize(string messageString)
        {
            if (messageString.IsNullOrEmpty())
            {
                return HardInfo.EmptyArrayList<byte>();
            }

            return messageString.ToBytes(Encoding.UTF8);
            // return Encoding.UTF8.GetBytes(messageString);
        }

        /// <summary>
        ///     byte[]反序列化
        /// </summary>
        /// <returns></returns>
        public static string BinaryDeserialize(byte[] bytes)
        {
            if (bytes == null || bytes.Length == 0)
            {
                return "";
            }

            return bytes.ToGetString(Encoding.UTF8);
            //return Encoding.UTF8.GetString(bytes);
        }
    }
}
