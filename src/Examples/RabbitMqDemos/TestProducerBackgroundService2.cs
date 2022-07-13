using WindNight.LogExtension;
using WindNight.RabbitMq;
using WindNight.RabbitMq.Abstractions;

namespace RabbitMqDemos;

public class TestProducerBackgroundService2 : BaseProducerBackgroundService
{
    public TestProducerBackgroundService2(IRabbitMqProducerFactory producerFactory,
        IRabbitMqProducerSettings producerSettings)
        : base(producerFactory, producerSettings)
    {
    }

    protected override string ProducerName => "TestProducerSvr2";

    protected override void DoBackgroundWork(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
            try
            {
                var message = $"{ProducerName}:sendtestmsg:{DateTime.Now:yyyy-MM-dd HH:mm:ss}";
                var routingKey = $"engrid.test.{ProducerName.ToLower()}.{DateTime.Now:yyyMMdd}";
                Send(message, routingKey);
                LogHelper.Info($"publish:message->{message} \r\n routingKey->{routingKey}");
                Thread.Sleep(TimeSpan.FromSeconds(15));
            }
            catch (Exception ex)
            {
                LogHelper.Error($"{ProducerName}:DoBackgroundWork:Send Handler Error {ex.Message}", ex);
                Thread.Sleep(TimeSpan.FromSeconds(30));
            }
    }
}