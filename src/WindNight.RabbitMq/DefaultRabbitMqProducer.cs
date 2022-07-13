using WindNight.RabbitMq.Abstractions;

namespace WindNight.RabbitMq;

public class DefaultRabbitMqProducer : Producer, IRabbitMqProducer
{
    public DefaultRabbitMqProducer(IRabbitMqProducerSettings settings) :
        base(settings.RabbitMqUrl, new ProducerConfigInfo
        {
            ExchangeName = settings.ExchangeName,
            ExchangeDurable = settings.ExchangeDurable,
            ExchangeTypeCode = settings.ExchangeTypeCode
        })
    {
        _settings = settings;
    }

    private IRabbitMqProducerSettings _settings { get; }
}