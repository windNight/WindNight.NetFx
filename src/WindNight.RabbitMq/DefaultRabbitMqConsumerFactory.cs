using System;
using System.Collections.Concurrent;
using WindNight.RabbitMq.Abstractions;

namespace WindNight.RabbitMq
{
    public class DefaultRabbitMqConsumerFactory : IRabbitMqConsumerFactory, IDisposable
    {
        public static string CurrentVersion => BuildInfo.BuildVersion;

        public static string CurrentCompileTime => BuildInfo.BuildTime;

        private static readonly object objectLock = new();

        private readonly ConcurrentDictionary<string, IRabbitMqConsumer> ConsumerDict = new();

        public DefaultRabbitMqConsumerFactory()
        {
            if (ConsumerDict == null)
            {
                ConsumerDict = new ConcurrentDictionary<string, IRabbitMqConsumer>();
            }
        }


        public void Dispose()
        {
            ConsumerDict?.Clear();
        }


        public IRabbitMqConsumer GetRabbitMqConsumer(string queueName, IRabbitMqConsumerSettings settings)
        {
            IRabbitMqConsumer consumer;
            if (!ConsumerDict.TryGetValue(queueName, out consumer))
            {
                lock (objectLock)
                {
                    if (!ConsumerDict.TryGetValue(queueName, out consumer))
                    {
                        consumer = new DefaultRabbitMqConsumer(settings);
                        ConsumerDict.TryAdd(queueName, consumer);
                    }
                }
            }

            return consumer;
        }

        ~DefaultRabbitMqConsumerFactory()
        {
            ConsumerDict?.Clear();
        }
    }
}
