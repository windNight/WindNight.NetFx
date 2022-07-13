using WindNight.RabbitMq;
using WindNight.RabbitMq.Abstractions;

namespace RabbitMqDemos;

public interface ITestProducerService
{
    bool SendWithNotRetry(string message, string routingKey, bool isMessageDurable = true);

    bool SendWithNotRetry(byte[] messageBodyBytes, string routingKey, bool isMessageDurable = true);
    void Send(string message, string routingKey, bool isMessageDurable = true);
}

public class TestProducerService : BaseProducerService, ITestProducerService
{
    public TestProducerService(IRabbitMqProducerFactory producerFactory, IRabbitMqProducerSettings producerSettings)
        : base(producerFactory, producerSettings)
    {
    }

    protected override string ProducerName => "TestProducerSvr";
}