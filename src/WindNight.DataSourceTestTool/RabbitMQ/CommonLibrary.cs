using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindNight.DataSourceTestTool.RabbitMQ
{
    internal class CommonLibrary
    {
        public static IBasicProperties CreateBasicProperties(
          IModel channel,
          BasicPropertiesConfigInfo configInfo)
        {
            IBasicProperties basicProperties = channel.CreateBasicProperties();
            if (configInfo.AppID != string.Empty)
                basicProperties.AppId = configInfo.AppID;
            if (configInfo.ClusterID != string.Empty)
                basicProperties.ClusterId = configInfo.ClusterID;
            if (configInfo.ContentEncoding != string.Empty)
                basicProperties.ContentEncoding = configInfo.ContentEncoding;
            if (configInfo.ContentType != string.Empty)
                basicProperties.ContentType = configInfo.ContentType;
            if (configInfo.CorrelationID != string.Empty)
                basicProperties.CorrelationId = configInfo.CorrelationID;
            basicProperties.DeliveryMode = configInfo.Durable ? Convert.ToByte(2) : Convert.ToByte(1);
            if (configInfo.Expiration > 0L)
                basicProperties.Expiration = configInfo.Expiration.ToString();
            if (configInfo.Headers != null)
                basicProperties.Headers=(configInfo.Headers);
            if (configInfo.MessageID != string.Empty)
                basicProperties.MessageId = configInfo.MessageID;
            if (configInfo.Priority != -1)
                basicProperties.Priority = Convert.ToByte(configInfo.Priority);
            if (configInfo.ReplyTo != string.Empty)
                basicProperties.ReplyTo = configInfo.ReplyTo;
            if (configInfo.Timestamp != -1L)
                basicProperties.Timestamp = new AmqpTimestamp(configInfo.Timestamp);
            if (configInfo.Type != string.Empty)
                basicProperties.Type = configInfo.Type;
            if (configInfo.UserID != string.Empty)
                basicProperties.UserId = configInfo.UserID;
            return basicProperties;
        }

        public static byte[] BinarySerialize(string message_string) => string.IsNullOrEmpty(message_string) ? (byte[])null : Encoding.UTF8.GetBytes(message_string);

        public static string BinaryDeserialize(byte[] bytes) => bytes == null ? (string)null : Encoding.UTF8.GetString(bytes);
    }
}
