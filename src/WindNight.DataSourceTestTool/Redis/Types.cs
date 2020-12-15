using System;
using System.Globalization;
using System.Runtime.Serialization;

namespace WindNight.DataSourceTestTool.Redis
{ 
    /// <summary>
    ///     Redis unified message prefix
    /// </summary>
    public enum RedisMessage
    {
        /// <summary>
        ///     Error message
        /// </summary>
        Error = '-',

        /// <summary>
        ///     Status message
        /// </summary>
        Status = '+',

        /// <summary>
        ///     Bulk message
        /// </summary>
        Bulk = '$',

        /// <summary>
        ///     Multi bulk message
        /// </summary>
        MultiBulk = '*',

        /// <summary>
        ///     Int message
        /// </summary>
        Int = ':'
    }



    /// <summary>
    ///     Represents a Redis subscription response
    /// </summary>
    public class RedisSubscriptionResponse
    {
        internal RedisSubscriptionResponse(string type, string channel, string pattern)
        {
            Type = type;
            Channel = channel;
            Pattern = pattern;
        }

        /// <summary>
        ///     Get the subscription channel name
        /// </summary>
        public string Channel { get; }

        /// <summary>
        ///     Get the subscription pattern
        /// </summary>
        public string Pattern { get; }

        /// <summary>
        ///     Get the message type
        /// </summary>
        public string Type { get; }
    }

    /// <summary>
    ///     Represents a Redis subscription channel
    /// </summary>
    public class RedisSubscriptionChannel : RedisSubscriptionResponse
    {
        internal RedisSubscriptionChannel(string type, string channel, string pattern, long count)
            : base(type, channel, pattern)
        {
            Count = count;
        }

        /// <summary>
        ///     Get the count of active subscriptions
        /// </summary>
        public long Count { get; }
    }

    /// <summary>
    ///     Represents a Redis subscription message
    /// </summary>
    public class RedisSubscriptionMessage : RedisSubscriptionResponse
    {
        internal RedisSubscriptionMessage(string type, string channel, string body)
            : base(type, channel, null)
        {
            Body = body;
        }

        internal RedisSubscriptionMessage(string type, string pattern, string channel, string body)
            : base(type, channel, pattern)
        {
            Body = body;
        }

        /// <summary>
        ///     Get the subscription message
        /// </summary>
        public string Body { get; }
    }





}