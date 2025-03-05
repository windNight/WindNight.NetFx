using System;
using RabbitMQ.Client.Events;

namespace WindNight.RabbitMq.Abstractions
{
    public interface IRabbitMqConsumer
    {
        /// <summary>
        ///     接受mq消息并执行 <paramref name="func" />
        /// </summary>
        /// <param name="func">
        ///     Func{in string message,in ulong deliveryTag,int string routingKey,string messageMd5,out bool
        ///     execResult}
        /// </param>
        void ExecuteReceiveMsg(Func<string, ulong, string, string, bool> func);

        /// <summary>
        ///     接受mq消息并执行 <paramref name="func" />
        /// </summary>
        /// <param name="func"> Func{in string message,in ulong deliveryTag,int string routingKey,out bool execResult} </param>
        void ExecuteReceiveMsg(Func<string, ulong, string, bool> func);

        /// <summary>
        ///     接受mq消息并执行 <paramref name="func" />
        /// </summary>
        /// <param name="func"> Func{in string message,in ulong deliveryTag,out bool execResult} </param>
        void ExecuteReceiveMsg(Func<string, ulong, bool> func);

        /// <summary>
        ///     接受mq消息并执行 <paramref name="func" />
        /// </summary>
        /// <param name="func"> Func{in string message,in string messageMd5,out bool execResult} </param>
        void ExecuteReceiveMsg(Func<string, string, bool> func);

        /// <summary>
        ///     接受mq消息并执行 <paramref name="func" />
        /// </summary>
        /// <param name="func"> Func{in string message,in string messageMd5,in string routingKey,out bool execResult} </param>
        void ExecuteReceiveMsg(Func<string, string, string, bool> func);

        /// <summary>
        ///     接受mq消息并执行 <paramref name="func" />
        /// </summary>
        /// <param name="func"> Func{in string message,out bool execResult} </param>
        void ExecuteReceiveMsg(Func<string, bool> func);


        /// <summary>
        ///     接受mq消息并执行 <paramref name="func" />  使用 EventingBasicConsumer
        /// </summary>
        /// <param name="func">
        ///     Func{in string message,in ulong deliveryTag,int string routingKey,string messageMd5,out bool
        ///     execResult}
        /// </param>
        EventingBasicConsumer SetEventingConsumerWithFunc(Func<string, ulong, string, string, bool> func);

        /// <summary>
        ///     接受mq消息并执行 <paramref name="func" /> 使用 EventingBasicConsumer
        /// </summary>
        /// <param name="func"> Func{in string message,in ulong deliveryTag,int string routingKey,out bool execResult} </param>
        EventingBasicConsumer SetEventingConsumerWithFunc(Func<string, ulong, string, bool> func);

        /// <summary>
        ///     接受mq消息并执行 <paramref name="func" /> 使用 EventingBasicConsumer
        /// </summary>
        /// <param name="func"> Func{in string message,in ulong deliveryTag,out bool execResult} </param>
        EventingBasicConsumer SetEventingConsumerWithFunc(Func<string, ulong, bool> func);

        /// <summary>
        ///     接受mq消息并执行 <paramref name="func" /> 使用 EventingBasicConsumer
        /// </summary>
        /// <param name="func"> Func{in string message,in string messageMd5,out bool execResult} </param>
        EventingBasicConsumer SetEventingConsumerWithFunc(Func<string, string, bool> func);

        /// <summary>
        ///     接受mq消息并执行 <paramref name="func" /> 使用 EventingBasicConsumer
        /// </summary>
        /// <param name="func"> Func{in string message,in messageMd5,in string routingKey,out bool execResult} </param>
        EventingBasicConsumer SetEventingConsumerWithFunc(Func<string, string, string, bool> func);

        /// <summary>
        ///     接受mq消息并执行 <paramref name="func" /> 使用 EventingBasicConsumer
        /// </summary>
        /// <param name="func"> Func{in string message,out bool execResult} </param>
        EventingBasicConsumer SetEventingConsumerWithFunc(Func<string, bool> func);


        /// <summary>
        ///     同步更新配置
        /// </summary>
        /// <returns></returns>
        bool SyncMqConsumerSettings(IRabbitMqConsumerSettings settings);

        bool Ack(ulong deliveryTag, bool multiple);
        bool NoAck(ulong deliveryTag, bool multiple, bool requeue);
        bool Receive(out string message);
        bool Receive(out string message, out string routingKey);
        bool ReceiveNeedAck(out string message, out ulong deliveryTag);
        bool ReceiveNeedAck(out string message, out ulong deliveryTag, out string routingKey);
        EventingBasicConsumer SetConsumerActive(EventHandler<BasicDeliverEventArgs> e);
    }
}
