using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace WindNight.DataSourceTestTool.Redis
{

    internal class RedisSocket : IRedisSocket
    {
        private readonly bool _ssl;
        internal Socket _socket;
        private EndPoint _remote;

        public bool Connected => _socket == null ? false : _socket.Connected;

        public int ReceiveTimeout
        {
            get => _socket.ReceiveTimeout;
            set => _socket.ReceiveTimeout = value;
        }

        public int SendTimeout
        {
            get => _socket.SendTimeout;
            set => _socket.SendTimeout = value;
        }

        public RedisSocket(bool ssl)
        {
            _ssl = ssl;
        }

        public void Connect(EndPoint endpoint, int timeout)
        {
            InitSocket(endpoint);

            var result = _socket.BeginConnect(endpoint, null, null);
            if (!result.AsyncWaitHandle.WaitOne(timeout, true))
                throw new RedisSocketException("Connect to server timeout");
        }

        TaskCompletionSource<bool> connectTcs;
        public Task<bool> ConnectAsync(EndPoint endpoint)
        {
            InitSocket(endpoint);

            if (connectTcs != null) connectTcs.TrySetCanceled();
            connectTcs = new TaskCompletionSource<bool>();

            _socket.BeginConnect(endpoint, asyncResult =>
            {
                try
                {
                    _socket.EndConnect(asyncResult);
                    connectTcs.TrySetResult(true);
                }
                catch (Exception ex)
                {
                    connectTcs.TrySetException(ex);
                }
            }, null);
            return connectTcs.Task;
        }

        public Stream GetStream()
        {
            Stream netStream = new NetworkStream(_socket, true);

            if (!_ssl) return netStream;

            var sslStream = new SslStream(netStream, true);
            sslStream.AuthenticateAsClient(GetHostForAuthentication());
            sslStream.AuthenticateAsClientAsync(GetHostForAuthentication()).Wait();
            return sslStream;
        }

        private bool isDisposed;

        public void Dispose()
        {
            if (isDisposed) return;
            isDisposed = true;
            try
            {
                _socket.Shutdown(SocketShutdown.Both);
            }
            catch
            {
            }

            try
            {
                _socket.Close();
            }
            catch
            {
            }

            try
            {
                _socket.Dispose();
            }
            catch
            {
            }
        }

        private void InitSocket(EndPoint endpoint)
        {
            if (_socket != null)
            {
                try
                {
                    _socket.Shutdown(SocketShutdown.Both);
                }
                catch
                {
                }

                try
                {
                    _socket.Close();
                }
                catch
                {
                }

                try
                {
                    _socket.Dispose();
                }
                catch
                {
                }
            }

            isDisposed = false;
            _socket = endpoint.AddressFamily == AddressFamily.InterNetworkV6
                ? new Socket(AddressFamily.InterNetworkV6, SocketType.Stream, ProtocolType.Tcp)
                : new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _remote = endpoint;
        }

        private string GetHostForAuthentication()
        {
            if (_remote == null)
                throw new ArgumentNullException("Remote endpoint is not set");
            if (_remote is DnsEndPoint)
                return (_remote as DnsEndPoint).Host;
            if (_remote is IPEndPoint)
                return (_remote as IPEndPoint).Address.ToString();

            throw new InvalidOperationException("Cannot get remote host");
        }
         
    }
}
