namespace WindNight.RabbitMq.Abstractions;

/// <inheritdoc />
public class RabbitMqProducerSettings : IRabbitMqProducerSettings
{
    public string RabbitMqUrl { get; set; }

    public string ExchangeName { get; set; }

    public bool ExchangeDurable { get; set; } = true;

    public ExchangeTypeCodeEnum ExchangeTypeCode { get; set; } = ExchangeTypeCodeEnum.Topic;

    public string ProducerName { get; set; }
}