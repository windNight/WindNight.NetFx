using System;

namespace WindNight.RabbitMq.Abstractions
{
    /// <summary>
    ///     本地消息包装器
    /// </summary>
    public interface IMessageWrapper : IDisposable
    {
        /// <summary>
        ///     写入临时缓存
        /// </summary>
        /// <param name="msgLocal">临时消息对象</param>
        void Write(MessageLocal msgLocal);

        /// <summary>
        ///     读取一个临时消息
        /// </summary>
        /// <returns></returns>
        MessageLocal ReadLine();
    }
}
