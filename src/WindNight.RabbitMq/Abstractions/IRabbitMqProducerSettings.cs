namespace WindNight.RabbitMq.Abstractions
{
    public interface IRabbitMqProducerSettings
    {
        /// <summary> MQ协议链接 </summary>
        string RabbitMqUrl { get; set; }

        /// <summary> 交换机名称 </summary>
        string ExchangeName { get; set; }

        /// <summary> 是否持久化 </summary>
        bool ExchangeDurable { get; set; }

        /// <summary> 交换机类型 默认为 topic </summary>
        ExchangeTypeCodeEnum ExchangeTypeCode { get; set; }

        /// <summary> 生产者名称 </summary>
        string ProducerName { get; set; }
    }
}
