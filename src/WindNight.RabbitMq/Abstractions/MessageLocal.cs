using System;

namespace WindNight.RabbitMq.Abstractions;

/// <summary>
///     当连接异常时，临时存储到本地的消息
/// </summary>
public class MessageLocal
{
    //private long DefaultTimeOut => TimeSpan.FromHours(36).TotalSeconds;//129600

    private const long DefaultGiveUpTimeOut = 129600L;

    /// <summary>
    ///     构造函数
    /// </summary>
    public MessageLocal()
    {
    }

    /// <summary>  消息内容的类型 Type全名  </summary>
    public ProducerConfigInfo ProducerConfigInfo { get; set; }

    ///// <summary>
    ///// 基础属性
    ///// </summary>
    //public BasicProperties basicProperties { set; get; }

    /// <summary> 消息 </summary>
    public string Message { get; set; }

    /// <summary>  路由键  </summary>
    public string RoutingKey { get; set; }

    /// <summary> 是否加密 </summary>
    public bool IsEncrypt { set; get; }

    /// <summary> 重试次数 </summary>
    public int RetryNum { set; get; }

    /// <summary>   创建时间 </summary>
    public long CreateTime { set; get; }

    /// <summary> 最后重试时间  </summary>
    public long LastRetryTime { set; get; }

    /// <summary> 是否需要放弃 </summary>
    public bool NeedGiveUp
    {
        get
        {
            // 时间超过36小时
            if (DateTime.Now.ConvertToUnixTime() - CreateTime > DefaultGiveUpTimeOut) return true;
            return false;
        }
    }
}