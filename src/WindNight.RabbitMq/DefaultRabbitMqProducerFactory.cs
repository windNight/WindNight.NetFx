using System;
using System.Collections.Concurrent;
using WindNight.RabbitMq.Abstractions;

namespace WindNight.RabbitMq
{
    public class DefaultRabbitMqProducerFactory : IRabbitMqProducerFactory, IDisposable
    {
        public static string CurrentVersion => BuildInfo.BuildVersion;// _version.ToString();

        public static string CurrentCompileTime => BuildInfo.BuildTime;

        private static readonly object objectLock = new();

        private readonly ConcurrentDictionary<string, IRabbitMqProducer> ProducerDict = new();

        public DefaultRabbitMqProducerFactory()
        {
            if (ProducerDict == null)
            {
                ProducerDict = new ConcurrentDictionary<string, IRabbitMqProducer>();
            }
        }


        public void Dispose()
        {
            ProducerDict?.Clear();
        }

        public IRabbitMqProducer GetRabbitMqProducer(IRabbitMqProducerSettings settings)
        {
            IRabbitMqProducer producer;
            // 是否使用settings.ExchangeName 更合适
            if (!ProducerDict.TryGetValue(settings.ProducerName, out producer))
            {
                lock (objectLock)
                {
                    if (!ProducerDict.TryGetValue(settings.ProducerName, out producer))
                    {
                        producer = new DefaultRabbitMqProducer(settings);
                        ProducerDict.TryAdd(settings.ProducerName, producer);
                    }
                }

            }

            return producer;
        }

        ~DefaultRabbitMqProducerFactory()
        {
            ProducerDict?.Clear();
        }
    }
}
