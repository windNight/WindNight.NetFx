namespace WindNight.RabbitMq.Abstractions
{
    public class RabbitMqConsumerSettings : IRabbitMqConsumerSettings
    {
        /// <inheritdoc />
        public string RabbitMqUrl { get; set; }

        /// <inheritdoc />
        public string QueueName { get; set; }

        /// <inheritdoc />
        public bool QueueDurable { get; set; } = true;

        /// <inheritdoc />
        public ushort PrefetchCount { get; set; } = 200;

        /// <inheritdoc />
        public int SleepTime { get; set; } = 100;

        /// <inheritdoc />
        public bool LogSwitch { get; set; } = true;

        /// <inheritdoc />
        public int ProcessWarnMs { get; set; } = 1 * 1000;
    }
}
