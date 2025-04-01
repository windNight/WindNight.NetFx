using System;
using System.Collections.Generic;

namespace WindNight.RabbitMq.Abstractions
{
    public class BasicProperties
    {
        private int _priority = -1;

        /// <summary>  标明消息的类型  </summary>
        public string ContentType { get; set; } = string.Empty;

        /// <summary>  标明消息的编码 </summary>
        public string ContentEncoding { get; set; } = string.Empty;

        /// <summary> 可扩展的信息对  </summary>
        public IDictionary<string, object> Headers { get; set; }

        /// <summary>  该消息是否需要被持久化支持. </summary>
        public bool Durable { get; set; }

        /// <summary> 该消息的权重（0-9）  </summary>
        public int Priority
        {
            get => _priority;
            set
            {
                if (value > 9 || value < 0)
                    value = 0; //如果无效值就设置为0，不抛出异常, 修复从本地文件读取的时候，存在-1值的bug

                // throw new Exception("无效的消息权重，消息权重的设置范围只能为[0,9]");
                _priority = Convert.ToByte(value);
            }
        }

        /// <summary>   用于"请求"与"响应"之间的匹配.</summary>
        public string CorrelationID { get; set; } = string.Empty;

        /// <summary>  "响应"的目标队列. </summary>
        public string ReplyTo { get; set; } = string.Empty;

        /// <summary>   消息有效期(单位为ms)  </summary>
        public long Expiration { get; set; } = -1;

        /// <summary> 消息的ID. </summary>
        public string MessageID { get; set; } = string.Empty;

        /// <summary>  时间戳(UnixTime)  </summary>
        public long Timestamp { get; set; } = -1;

        /// <summary> 消息的类型 </summary>
        public string Type { get; set; } = string.Empty;

        /// <summary>  用户的ID  </summary>
        public string UserID { get; set; } = string.Empty;

        /// <summary>   应用的ID </summary>
        public string AppID { get; set; } = string.Empty;

        /// <summary> 服务集群ID </summary>
        public string ClusterID { get; set; } = string.Empty;
    }
}
