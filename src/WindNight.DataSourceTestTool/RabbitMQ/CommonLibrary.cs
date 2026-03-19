using System;
using System.Text;
using RabbitMQ.Client;

namespace WindNight.DataSourceTestTool.RabbitMQ
{
    internal class CommonLibrary
    {
        public static BasicProperties CreateBasicProperties(IChannel channel, BasicPropertiesConfigInfo configInfo)
        {
            var basicProperties = new BasicProperties();// channel.CreateBasicProperties();
            if (configInfo.AppId != string.Empty)
            {
                basicProperties.AppId = configInfo.AppId;
            }

            if (configInfo.ClusterId != string.Empty)
            {
                basicProperties.ClusterId = configInfo.ClusterId;
            }

            if (configInfo.ContentEncoding != string.Empty)
            {
                basicProperties.ContentEncoding = configInfo.ContentEncoding;
            }

            if (configInfo.ContentType != string.Empty)
            {
                basicProperties.ContentType = configInfo.ContentType;
            }

            if (configInfo.CorrelationId != string.Empty)
            {
                basicProperties.CorrelationId = configInfo.CorrelationId;
            }

            //  basicProperties.DeliveryMode = configInfo.Durable ? Convert.ToByte(2) : Convert.ToByte(1);
            basicProperties.DeliveryMode = configInfo.Durable ? (DeliveryModes.Persistent) : (DeliveryModes.Transient);

            if (configInfo.Expiration > 0L)
            {
                basicProperties.Expiration = configInfo.Expiration.ToString();
            }

            if (configInfo.Headers != null)
            {
                basicProperties.Headers = configInfo.Headers;
            }

            if (configInfo.MessageId != string.Empty)
            {
                basicProperties.MessageId = configInfo.MessageId;
            }

            if (configInfo.Priority != -1)
            {
                basicProperties.Priority = Convert.ToByte(configInfo.Priority);
            }

            if (configInfo.ReplyTo != string.Empty)
            {
                basicProperties.ReplyTo = configInfo.ReplyTo;
            }

            if (configInfo.Timestamp != -1L)
            {
                basicProperties.Timestamp = new AmqpTimestamp(configInfo.Timestamp);
            }

            if (configInfo.Type != string.Empty)
            {
                basicProperties.Type = configInfo.Type;
            }

            if (configInfo.UserId != string.Empty)
            {
                basicProperties.UserId = configInfo.UserId;
            }

            return basicProperties;
        }

        public static byte[] BinarySerialize(string message_string)
        {
            return string.IsNullOrEmpty(message_string) ? null : Encoding.UTF8.GetBytes(message_string);
        }

        public static string BinaryDeserialize(byte[] bytes)
        {
            return bytes == null ? null : Encoding.UTF8.GetString(bytes);
        }
    }
}
