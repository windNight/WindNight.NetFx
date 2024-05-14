using Microsoft.Extensions.Hosting;
using WindNight.LogExtension;

namespace RabbitMqDemos;

public class TestProducerBackgroundService : BackgroundService
{
    private readonly ITestProducerService _testProducer;

    public TestProducerBackgroundService(ITestProducerService testProducer)
    {
        _testProducer = testProducer;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        Task.Run(() =>
        {
            while (!stoppingToken.IsCancellationRequested)
                try
                {
                    var message = $"TestProducerSvr:sendtestmsg:{HardInfo.Now:yyyy-MM-dd HH:mm:ss}";
                    var routingKey = $"engrid.test.testproducersvr.{HardInfo.Now:yyyMMdd}";
                    _testProducer.Send(message, routingKey);
                    LogHelper.Info($"publish:message->{message} \r\n routingKey->{routingKey}");
                    Thread.Sleep(TimeSpan.FromSeconds(5));
                }
                catch (Exception ex)
                {
                    LogHelper.Error($"TestProducerSvr:DoBackgroundWork:Send Handler Error {ex.Message}", ex);
                    Thread.Sleep(TimeSpan.FromSeconds(30));
                }
        }, stoppingToken);
        return Task.CompletedTask;
    }
}