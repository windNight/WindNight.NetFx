namespace WindNight.RabbitMq.Abstractions;

/// <summary>  交换机类型枚举 </summary>
public enum ExchangeTypeCodeEnum
{
    Direct = 0,
    Fanout = 1,
    Topic = 3,

    Headers = 4
    //    public const string Direct = "direct";

    //public const string Fanout = "fanout";

    //public const string Headers = "headers";

    //public const string Topic = "topic";
}