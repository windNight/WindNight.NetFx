using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindNight.DataSourceTestTool.RabbitMQ
{
    public class ConsumerConfigInfo
    {
        private bool queue_durable = true;
        private ushort prefetchCount = 200;

        public string QueueName { get; set; }

        public bool QueueDurable
        {
            get => this.queue_durable;
            set => this.queue_durable = value;
        }

        public ushort PrefetchCount
        {
            get => this.prefetchCount;
            set => this.prefetchCount = value;
        }
    }
}
