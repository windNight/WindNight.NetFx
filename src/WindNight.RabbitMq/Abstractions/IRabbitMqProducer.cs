namespace WindNight.RabbitMq.Abstractions;

public interface IRabbitMqProducer
{
    bool SendWithNotRetry(string message, string routingKey, bool isMessageDurable = true);
    bool SendWithNotRetry(byte[] messageBodyBytes, string routingKey, bool isMessageDurable = true);

    bool Send(string message, string routingKey, bool isMessageDurable = true);
}