using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json.Extension;
using WindNight.RabbitMq.Abstractions;
using WindNight.RabbitMq.Internal;

namespace WindNight.RabbitMq;

public abstract class BaseProducerService
{
    protected readonly IRabbitMqProducer Producer;

    public BaseProducerService(IRabbitMqProducerFactory producerFactory, IRabbitMqProducerSettings producerSettings)
    {
        if (producerSettings == null || string.IsNullOrEmpty(producerSettings.ExchangeName))
            producerSettings = DefaultRabbitMqProducerSettings;
        LogHelper.Info($" IRabbitMqProducerSettings is {producerSettings.ToJsonStr()}");
        Producer = producerFactory.GetRabbitMqProducer(producerSettings);
    }

    protected abstract string ProducerName { get; }

    protected IRabbitMqProducerSettings DefaultRabbitMqProducerSettings
    {
        get
        {
            var config = ConfigItems.RabbitMqConfig.Items.FirstOrDefault(m => m.ProducerName == ProducerName);
            if (config == null)
                throw new ArgumentNullException($"RabbitMqConfig({ProducerName}) Can not Get from config");
            return new RabbitMqProducerSettings
            {
                RabbitMqUrl = config.RabbitMqUrl,
                ExchangeName = config.ExchangeName,
                ExchangeTypeCode = config.ExchangeTypeCode,
                ExchangeDurable = config.ExchangeDurable,
                ProducerName = config.ProducerName
            };
        }
    }


    public virtual bool SendWithNotRetry(string message, string routingKey, bool isMessageDurable = true)
    {
        return Producer.SendWithNotRetry(message, routingKey, isMessageDurable);
    }

    public virtual bool SendWithNotRetry(byte[] messageBodyBytes, string routingKey, bool isMessageDurable = true)
    {
        return Producer.SendWithNotRetry(messageBodyBytes, routingKey, isMessageDurable);
    }

    public virtual void Send(string message, string routingKey, bool isMessageDurable = true)
    {
        Producer.Send(message, routingKey, isMessageDurable);
    }
}

public abstract class BaseProducerBackgroundService : BackgroundService
{
    protected readonly IRabbitMqProducer Producer;

    public BaseProducerBackgroundService(IRabbitMqProducerFactory producerFactory,
        IRabbitMqProducerSettings producerSettings)
    {
        if (producerSettings == null || string.IsNullOrEmpty(producerSettings.ExchangeName))
            producerSettings = DefaultRabbitMqProducerSettings;
        LogHelper.Info($" IRabbitMqProducerSettings is {producerSettings.ToJsonStr()}");
        Producer = producerFactory.GetRabbitMqProducer(producerSettings);
    }

    protected abstract string ProducerName { get; }

    protected IRabbitMqProducerSettings DefaultRabbitMqProducerSettings
    {
        get
        {
            var config = ConfigItems.RabbitMqConfig.Items.FirstOrDefault(m => m.ProducerName == ProducerName);
            if (config == null)
                throw new ArgumentNullException($"RabbitMqConfig({ProducerName}) Can not Get from config");
            return new RabbitMqProducerSettings
            {
                RabbitMqUrl = config.RabbitMqUrl,
                ExchangeName = config.ExchangeName,
                ExchangeTypeCode = config.ExchangeTypeCode,
                ExchangeDurable = config.ExchangeDurable,
                ProducerName = config.ProducerName
            };
        }
    }


    public virtual bool SendWithNotRetry(string message, string routingKey, bool isMessageDurable = true)
    {
        return Producer.SendWithNotRetry(message, routingKey, isMessageDurable);
    }

    public virtual bool SendWithNotRetry(byte[] messageBodyBytes, string routingKey, bool isMessageDurable = true)
    {
        return Producer.SendWithNotRetry(messageBodyBytes, routingKey, isMessageDurable);
    }

    public virtual void Send(string message, string routingKey, bool isMessageDurable = true)
    {
        Producer.Send(message, routingKey, isMessageDurable);
    }

    protected abstract void DoBackgroundWork(CancellationToken stoppingToken);

    //  Stop when stoppingToken is IsCancellationRequested ?
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        Task.Run(() => { DoBackgroundWork(stoppingToken); }, stoppingToken);
        return Task.CompletedTask;
    }
}