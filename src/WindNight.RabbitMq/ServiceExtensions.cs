using Microsoft.Extensions.DependencyInjection;
using WindNight.RabbitMq.Abstractions;

namespace WindNight.RabbitMq;

public static class ServiceExtensions
{
    public static IServiceCollection AddRabbitMqConsumer(this IServiceCollection services)
    {
        services.AddSingleton<IRabbitMqConsumerSettings, RabbitMqConsumerSettings>();
        services.AddSingleton<IRabbitMqConsumer, DefaultRabbitMqConsumer>();
        services.AddSingleton<IRabbitMqConsumerFactory, DefaultRabbitMqConsumerFactory>();
        return services;
    }


    public static IServiceCollection AddRabbitMqProducer(this IServiceCollection services)
    {
        services.AddSingleton<IRabbitMqProducerSettings, RabbitMqProducerSettings>();
        services.AddSingleton<IRabbitMqProducer, DefaultRabbitMqProducer>();
        services.AddSingleton<IRabbitMqProducerFactory, DefaultRabbitMqProducerFactory>();
        return services;
    }
}