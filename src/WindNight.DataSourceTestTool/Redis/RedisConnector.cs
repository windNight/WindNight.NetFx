using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WindNight.DataSourceTestTool.Redis.Internal;

namespace WindNight.DataSourceTestTool.Redis
{
    internal class RedisConnector
    {
        //   private readonly int _concurrency;
        //   private readonly int _bufferSize;
        internal readonly IRedisSocket _redisSocket;
        private readonly RedisIO _io;

        public event EventHandler Connected;

        public bool IsConnected => _redisSocket.Connected;
        public EndPoint EndPoint { get; }

        //    public bool IsPipelined => _io.IsPipelined;

        public int ReconnectAttempts { get; set; }
        public int ReconnectWait { get; set; }




        public RedisConnector(EndPoint endPoint, IRedisSocket socket, int concurrency, int bufferSize)
        {
            //  _concurrency = concurrency;
            //  _bufferSize = bufferSize;
            EndPoint = endPoint;
            _redisSocket = socket;
            _io = new RedisIO();
            // _autoPipeline = new AutoPipelineOption(_io);
        }

        public bool Connect(int timeout)
        {
            _redisSocket.Connect(EndPoint, timeout);

            if (_redisSocket.Connected)
                OnConnected();

            return _redisSocket.Connected;
        }


        public T Call<T>(RedisCommand<T> command)
        {
            ConnectIfNotConnected();

            try
            {
                _io.Write(_io.Writer.Prepare(command));
                return command.Parse(_io.Reader);
            }
            catch (IOException)
            {
                if (ReconnectAttempts == 0)
                    throw;
                Reconnect();
                return Call(command);
            }
            catch (RedisException ex)
            {
                throw new RedisException($"{ex.Message}\r\nCommand: {command}", ex);
            }
        }


        public async Task<T> CallAsync<T>(RedisCommand<T> command)
        {
            //if (_autoPipeline.IsEnabled)
            //	return _autoPipeline.EnqueueAsync(command);

            //await _io.Writer.WriteAsync(command, _io.Stream);
            await _io.WriteAsync(command);
            //_io.Stream.BeginRead()
            return command.Parse(_io.Reader);
        }


        public void Write(RedisCommand command)
        {
            ConnectIfNotConnected();

            try
            {
                // _io.Writer.Write(command, _io.Stream);
                _io.Write(_io.Writer.Prepare(command));
            }
            catch (IOException)
            {
                if (ReconnectAttempts == 0)
                    throw;
                Reconnect();
                Write(command);
            }
        }

        public T Read<T>(Func<RedisReader, T> func)
        {
            ExpectConnected();

            try
            {
                return func(_io.Reader);
            }
            catch (IOException)
            {
                if (ReconnectAttempts == 0)
                    throw;
                Reconnect();
                return Read(func);
            }
        }

        public void Read(Stream destination, int bufferSize)
        {
            ExpectConnected();

            try
            {
                _io.Reader.ExpectType(RedisMessage.Bulk);
                _io.Reader.ReadBulkBytes(destination, bufferSize, false);
            }
            catch (IOException)
            {
                if (ReconnectAttempts == 0)
                    throw;
                Reconnect();
                Read(destination, bufferSize);
            }
        }


        public object[] EndPipe()
        {
            ExpectConnected();

            try
            {
                return _io.Pipeline.Flush();
            }
            catch (IOException)
            {
                if (ReconnectAttempts == 0)
                    throw;
                Reconnect();
                return EndPipe();
            }
        }

        public void Dispose()
        {
            _io.Dispose();

            if (_redisSocket != null)
                _redisSocket.Dispose();
        }

        private void Reconnect()
        {
            var attempts = 0;
            while (attempts++ < ReconnectAttempts || ReconnectAttempts == -1)
            {
                if (Connect(-1))
                    return;

                Thread.Sleep(TimeSpan.FromMilliseconds(ReconnectWait));
            }

            throw new IOException("Could not reconnect after " + attempts + " attempts");
        }

        private void OnConnected()
        {
            _io.SetStream(_redisSocket.GetStream());
            if (Connected != null)
                Connected(this, new EventArgs());
        }


        private void ConnectIfNotConnected()
        {
            if (!IsConnected)
                Connect(-1);
        }

        private void ExpectConnected()
        {
            if (!IsConnected)
                throw new RedisClientException("Client is not connected");
        }
    }
}
