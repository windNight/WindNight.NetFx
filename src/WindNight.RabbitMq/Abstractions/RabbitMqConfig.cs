using System.Collections.Generic;

namespace WindNight.RabbitMq.Abstractions
{
    public class RabbitMqOptions
    {
        public bool CanLogDebug { get; set; } = false;
        public List<RabbitMqConfigInfo> Items { get; set; } = new();
    }

    public class RabbitMqConfig
    {
        public List<RabbitMqConfigInfo> Items { get; set; } = new();
    }

    public class RabbitMqConfigInfo
    {
        public string Type { get; set; }
        public string RabbitMqUrl { get; set; }
        public string QueueName { get; set; }
        public string QueueTag { get; set; }

        /// <summary> 队列持久化 默认为true </summary>
        public bool QueueDurable { get; set; } = true;

        /// <summary> 预读数  默认为200，仅在主动模式(Need Ack)下有效  </summary>
        public ushort PrefetchCount { get; set; } = 200;

        /// <summary> 无数据时 Sleep 时间 (Milliseconds) </summary>
        public int SleepTime { get; set; } = 100;

        /// <summary> Debug 日志是否输出 </summary>
        public bool LogSwitch { get; set; } = true;

        public int ProcessWarnMs { get; set; } = 1 * 1000;


        public string ExchangeName { get; set; }

        public bool ExchangeDurable { get; set; } = true;

        public ExchangeTypeCodeEnum ExchangeTypeCode { get; set; } = ExchangeTypeCodeEnum.Topic;
        public string ProducerName { get; set; }
    }
}
