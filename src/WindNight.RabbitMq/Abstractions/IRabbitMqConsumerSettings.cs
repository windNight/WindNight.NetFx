namespace WindNight.RabbitMq.Abstractions;

public interface IRabbitMqConsumerSettings
{
    /// <summary> MQ协议链接 </summary>
    string RabbitMqUrl { get; set; }

    /// <summary> 队列名 </summary>
    string QueueName { get; set; }

    /// <summary> 队列持久化 默认为true </summary>
    bool QueueDurable { get; set; }

    /// <summary> 预读数  默认为200，仅在主动模式(Need Ack)下有效  </summary>
    ushort PrefetchCount { get; set; }

    /// <summary> 无数据时 Sleep 时间 (Milliseconds) default is 100 </summary>
    int SleepTime { get; set; }

    /// <summary> Debug 日志是否输出 </summary>
    bool LogSwitch { get; set; }

    /// <summary> 消息处理告警时间（ms）  default is 1000 </summary>
    int ProcessWarnMs { get; set; }
}