namespace WindNight.DataSourceTestTool.RabbitMQ
{
    public class ProducerConfigInfo
    {
        private bool exchange_durable = true;
        private string exchangeTypeCode = "topic";

        public string ExchangeName { get; set; }

        public string ExchangeTypeCode
        {
            get => this.exchangeTypeCode;
            set => this.exchangeTypeCode = value;
        }

        public bool ExchangeDurable
        {
            get => this.exchange_durable;
            set => this.exchange_durable = value;
        }
    }
}
