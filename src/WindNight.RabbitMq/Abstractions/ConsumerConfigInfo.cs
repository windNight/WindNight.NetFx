namespace WindNight.RabbitMq.Abstractions;

/// <summary>
///     消费者配置实体类
/// </summary>
public class ConsumerConfigInfo
{
    /// <summary 队列名称
    /// </summary>
    public string QueueName { get; set; }

    /// <summary>  队列是否持久化(默认为true)  </summary>
    public bool QueueDurable { get; set; } = true;

    /// <summary>    预读数(默认为200，仅在Ack模式下有效) </summary>
    public ushort PrefetchCount { get; set; } = 200;
}