namespace WindNight.RabbitMq.Abstractions;

public interface IRabbitMqConsumerFactory
{
    IRabbitMqConsumer GetRabbitMqConsumer(string queueName, IRabbitMqConsumerSettings settings);
}