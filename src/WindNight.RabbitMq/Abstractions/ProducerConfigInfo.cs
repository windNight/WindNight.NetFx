namespace WindNight.RabbitMq.Abstractions;

/// <summary>
///     生产者配置类
/// </summary>
public class ProducerConfigInfo
{
    /// <summary>
    ///     交换机名称
    /// </summary>
    public string ExchangeName { get; set; }

    /// <summary>
    ///     交换模式(默认为topic模式，具体查看CtRabbitMQ.ExchangeTypeCode类)
    /// </summary>
    public ExchangeTypeCodeEnum ExchangeTypeCode { get; set; } = ExchangeTypeCodeEnum.Topic; //.ToString().ToLower();

    /// <summary>
    ///     交换机持久化(默认为true，请根据当前实际交换机配置情况做相应配置)
    /// </summary>
    public bool ExchangeDurable { get; set; } = true;

    /// <summary>
    ///     备用mq地址
    /// </summary>
    public string SpareMqUri { set; get; }

    /// <summary>
    ///     备用交换机名称
    /// </summary>
    public string SpareExchangeName { get; set; } = "mqtransport.topic";

    /// <summary>
    ///     路由键名称
    /// </summary>
    public string SpareRoutingKey { set; get; } = "mq.1";

    /// <summary>
    ///     文件名称
    /// </summary>
    internal string FileName { set; get; }
}