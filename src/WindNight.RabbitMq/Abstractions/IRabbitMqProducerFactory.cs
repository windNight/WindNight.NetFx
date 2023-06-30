namespace WindNight.RabbitMq.Abstractions;

public interface IRabbitMqProducerFactory
{
    IRabbitMqProducer GetRabbitMqProducer(IRabbitMqProducerSettings settings);
}