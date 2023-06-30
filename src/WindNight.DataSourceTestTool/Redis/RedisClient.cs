using System;
using System.Net;
using WindNight.DataSourceTestTool.Redis.Internal;

namespace WindNight.DataSourceTestTool.Redis
{
    /// <summary>
    ///     Common properties of the RedisClient
    /// </summary>
    public interface IRedisClient : IDisposable
    {

    }

    /// <summary>
    ///     Represents a client connection to a Redis server instance
    /// </summary>
    public partial class RedisClient : IRedisClient
    {
        public RedisClient(RedisOption op) : this(op.Host, op.Port)
        {

        }

        /// <summary>
        ///     Create a new RedisClient using default port and encoding
        /// </summary>
        /// <param name="host">Redis server hostname</param>
        public RedisClient(string host)
            : this(host, DefaultPort)
        {
        }

        /// <summary>
        ///     Create a new RedisClient
        /// </summary>
        /// <param name="host">Redis server hostname</param>
        /// <param name="port">Redis server port</param>
        public RedisClient(string host, int port)
            : this(host, port, DefaultSSL)
        {
        }

        /// <summary>
        ///     Create a new RedisClient
        /// </summary>
        /// <param name="host">Redis server hostname</param>
        /// <param name="port">Redis server port</param>
        /// <param name="ssl">Set to true if remote Redis server expects SSL</param>
        public RedisClient(string host, int port, bool ssl)
            : this(host, port, ssl, DefaultConcurrency, DefaultBufferSize)
        {
        }

        /// <summary>
        ///     Create a new RedisClient
        /// </summary>
        /// <param name="endpoint">Redis server</param>
        public RedisClient(EndPoint endpoint)
            : this(endpoint, DefaultSSL)
        {
        }

        /// <summary>
        ///     Create a new RedisClient
        /// </summary>
        /// <param name="endpoint">Redis server</param>
        /// <param name="ssl">Set to true if remote Redis server expects SSL</param>
        public RedisClient(EndPoint endpoint, bool ssl)
            : this(endpoint, ssl, DefaultConcurrency, DefaultBufferSize)
        {
        }

        /// <summary>
        ///     Create a new RedisClient with specific async concurrency settings
        /// </summary>
        /// <param name="host">Redis server hostname</param>
        /// <param name="port">Redis server port</param>
        /// <param name="asyncConcurrency">Max concurrent threads (default 1000)</param>
        /// <param name="asyncBufferSize">Async thread buffer size (default 10240 bytes)</param>
        public RedisClient(string host, int port, int asyncConcurrency, int asyncBufferSize)
            : this(host, port, DefaultSSL, asyncConcurrency, asyncBufferSize)
        {
        }

        /// <summary>
        ///     Create a new RedisClient with specific async concurrency settings
        /// </summary>
        /// <param name="host">Redis server hostname</param>
        /// <param name="port">Redis server port</param>
        /// <param name="ssl">Set to true if remote Redis server expects SSL</param>
        /// <param name="asyncConcurrency">Max concurrent threads (default 1000)</param>
        /// <param name="asyncBufferSize">Async thread buffer size (default 10240 bytes)</param>
        public RedisClient(string host, int port, bool ssl, int asyncConcurrency, int asyncBufferSize)
            : this(new DnsEndPoint(host, port), ssl, asyncConcurrency, asyncBufferSize)
        {
        }

        /// <summary>
        ///     Create a new RedisClient with specific async concurrency settings
        /// </summary>
        /// <param name="endpoint">Redis server</param>
        /// <param name="asyncConcurrency">Max concurrent threads (default 1000)</param>
        /// <param name="asyncBufferSize">Async thread buffer size (default 10240 bytes)</param>
        public RedisClient(EndPoint endpoint, int asyncConcurrency, int asyncBufferSize)
            : this(endpoint, DefaultSSL, asyncConcurrency, asyncBufferSize)
        {
        }

        /// <summary>
        ///     Create a new RedisClient with specific async concurrency settings
        /// </summary>
        /// <param name="endpoint">Redis server</param>
        /// <param name="ssl">Set to true if remote Redis server expects SSL</param>
        /// <param name="asyncConcurrency">Max concurrent threads (default 1000)</param>
        /// <param name="asyncBufferSize">Async thread buffer size (default 10240 bytes)</param>
        public RedisClient(EndPoint endpoint, bool ssl, int asyncConcurrency, int asyncBufferSize)
            : this(new RedisSocket(ssl), endpoint, asyncConcurrency, asyncBufferSize)
        {
        }

        internal RedisClient(IRedisSocket socket, EndPoint endpoint)
            : this(socket, endpoint, DefaultConcurrency, DefaultBufferSize)
        {
        }

        internal RedisClient(IRedisSocket socket, EndPoint endpoint, int asyncConcurrency, int asyncBufferSize)
        {
            _connector = new RedisConnector(endpoint, socket, asyncConcurrency, asyncBufferSize);
            //   _transaction = new RedisTransaction(_connector);
            //   _monitor = new MonitorListener(_connector);

            _connector.Connected += OnConnectionConnected;
        }

        /// <summary>
        ///     Dispose all resources used by the current RedisClient
        /// </summary>
        public void Dispose()
        {
            _connector.Dispose();
        }


        private void OnConnectionConnected(object sender, EventArgs args)
        {
            if (Connected != null)
                Connected(this, args);
        }

        private const int DefaultPort = 6379;
        private const bool DefaultSSL = false;
        private const int DefaultConcurrency = 1000;
        private const int DefaultBufferSize = 10240;
        private readonly RedisConnector _connector;
        //   private readonly MonitorListener _monitor;
        //  private readonly RedisTransaction _transaction;
        //  private bool _streaming;


        /// <summary>
        ///     Occurs when the connection has sucessfully reconnected
        /// </summary>
        public event EventHandler Connected;


        /// <summary>
        ///     Get a value indicating whether the Redis client is connected to the server
        /// </summary>
        public bool IsConnected => _connector.IsConnected;

        /// <summary>
        ///     Connect to the remote host
        /// </summary>
        /// <param name="timeout">Connection timeout in milliseconds</param>
        /// <returns>True if connected</returns>
        public virtual bool Connect(int timeout)
        {
            return _connector.Connect(timeout);
        }

        /// <summary>
        ///     Call arbitrary Redis command
        /// </summary>
        /// <param name="command">Command name</param>
        /// <param name="args">Command arguments</param>
        /// <returns>Redis object</returns>
        public virtual object Call(string command, params string[] args)
        {
            return Write(RedisCommands.Call(command, args));
        }

        private T Write<T>(RedisCommand<T> command)
        {
            //if (_transaction.Active) return _transaction.Write(command);

            //if (_monitor.Listening) return default;

            //if (_streaming)
            //{
            //    _connector.Write(command);
            //    return default;
            //}

            return _connector.Call(command);
        }

        /// <summary>
        ///     Authenticate to the server
        /// </summary>
        /// <param name="password">Redis server password</param>
        /// <returns>Status message</returns>
        public virtual string Auth(string password)
        {
            return Write(RedisCommands.Auth(password));
        }


        /// <summary>
        ///     Change the selected database for the current connection
        /// </summary>
        /// <param name="index">Zero-based database index</param>
        /// <returns>Status message</returns>
        public virtual string Select(int index)
        {
            return Write(RedisCommands.Select(index));
        }

        /// <summary>
        ///     Close the connection
        /// </summary>
        /// <returns>Status message</returns>
        public virtual string Quit()
        {
            var response = Write(RedisCommands.Quit());
            _connector.Dispose();
            return response;
        }

        /// <summary>
        ///     Get the list of client connections
        /// </summary>
        /// <returns>Formatted string of clients</returns>
        public virtual string ClientList()
        {
            return Write(RedisCommands.ClientList());
        }

        /// <summary>
        ///     Get information and statistics about the server
        /// </summary>
        /// <param name="section">all|default|server|clients|memory|persistence|stats|replication|cpu|commandstats|cluster|keyspace</param>
        /// <returns>Formatted string</returns>
        public virtual string Info(string section = null)
        {
            return Write(RedisCommands.Info(section));
        }
    }
}
