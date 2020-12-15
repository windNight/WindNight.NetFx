using System;

namespace WindNight.DataSourceTestTool.Redis
{ 
    /// <summary>
    ///     Provides data for the event that is raised when a transaction command has been processed by the server
    /// </summary>
    public class RedisTransactionQueuedEventArgs : EventArgs
    {
        internal RedisTransactionQueuedEventArgs(string status, string command, object[] arguments)
        {
            Status = status;
            Command = command;
            Arguments = arguments;
        }

        /// <summary>
        ///     The status code of the transaction command
        /// </summary>
        public string Status { get; }

        /// <summary>
        ///     The command that was queued
        /// </summary>
        public string Command { get; }

        /// <summary>
        ///     The arguments of the queued command
        /// </summary>
        public object[] Arguments { get; }
    }

    /// <summary>
    ///     Provides data for the event that is raised when a Redis MONITOR message is received
    /// </summary>
    public class RedisMonitorEventArgs : EventArgs
    {
        internal RedisMonitorEventArgs(object message)
        {
            Message = message;
        }

        /// <summary>
        ///     Monitor output
        /// </summary>
        public object Message { get; }
    }
}