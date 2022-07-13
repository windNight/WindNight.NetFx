using WindNight.LogExtension;
using WindNight.RabbitMq;
using WindNight.RabbitMq.Abstractions;

namespace RabbitMqDemos;

public class TestConsumerBackgroundService : BaseConsumerBackgroundService
{
    public TestConsumerBackgroundService(IRabbitMqConsumerFactory consumerFactory,
        IRabbitMqConsumerSettings consumerSettings) : base(consumerFactory, consumerSettings)
    {
    }

    protected override string QueueTag => "TestConsumerSvr";

    protected override bool HandleOneMessage(string message, string messageMd5)
    {
        LogHelper.Info($"{QueueTag}-> Receive message is {message}");
        return true;
    }
}