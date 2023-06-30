using System;
using System.IO;
using System.Net.Security;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace WindNight.DataSourceTestTool.Redis.Internal
{
    internal class RedisIO : IDisposable
    {
        private RedisReader _reader;
        private RedisPipeline _pipeline;
        private Stream _stream;
        private readonly object _streamLock = new object();

        public Stream Stream => GetOrThrow(_stream);
        public RedisWriter Writer { get; }

        public RedisReader Reader => GetOrThrow(_reader);
        public Encoding Encoding { get; set; }
        public RedisPipeline Pipeline => GetOrThrow(_pipeline);
        public bool IsPipelined => _pipeline == null ? false : _pipeline.Active;

        public RedisIO()
        {
            Writer = new RedisWriter(this);
            Encoding = new UTF8Encoding(false);
        }

        public void SetStream(Stream stream)
        {
            if (_stream != null)
            {
                try
                {
                    _stream.Close();
                }
                catch
                {
                }

                try
                {
                    _stream.Dispose();
                }
                catch
                {
                }
            }

            _stream = stream;
            _reader = new RedisReader(this);
            _pipeline = new RedisPipeline(this);
        }


        public async Task<int> WriteAsync(RedisCommand command)
        {
            var data = Writer.Prepare(command);
            await Stream.WriteAsync(data, 0, data.Length);
            return data.Length;
            //var tcs = new TaskCompletionSource<int>();
            //lock (_streamLock)
            //{
            //    Stream.BeginWrite(data, 0, data.Length, asyncResult =>
            //    {
            //        try
            //        {
            //            _stream.EndWrite(asyncResult);
            //            tcs.TrySetResult(data.Length);
            //        }
            //        catch (Exception ex)
            //        {
            //            tcs.TrySetException(ex);
            //        }
            //    }, null);
            //    Stream.Flush();
            //}
            //return tcs.Task;
        }

        public void Write(byte[] data)
        {
            lock (_streamLock)
            {
                Stream.Write(data, 0, data.Length);
                Stream.Flush();
            }
        }

        public void Write(Stream stream)
        {
            lock (_streamLock)
            {
                stream.CopyTo(Stream);
                Stream.Flush();
            }
        }

        public int ReadByte()
        {
            lock (_streamLock)
            {
                return Stream.ReadByte();
            }
        }

        public int Read(byte[] data, int offset, int count)
        {
            lock (_streamLock)
            {
                return Stream.Read(data, offset, count);
            }
        }

        public byte[] ReadAll()
        {
            var ns = _stream as NetworkStream;
            if (ns != null)
                using (var ms = new MemoryStream())
                {
                    try
                    {
                        var data = new byte[1024];
                        while (ns.DataAvailable && ns.CanRead)
                        {
                            var numBytesRead = 0;
                            lock (_streamLock)
                            {
                                numBytesRead = ns.Read(data, 0, data.Length);
                            }

                            if (numBytesRead <= 0) break;
                            ms.Write(data, 0, numBytesRead);
                            if (numBytesRead < data.Length) break;
                        }
                    }
                    catch
                    {
                    }

                    return ms.ToArray();
                }

            var ss = _stream as SslStream;
            if (ss != null)
                using (var ms = new MemoryStream())
                {
                    try
                    {
                        var data = new byte[1024];
                        while (ss.CanRead)
                        {
                            var numBytesRead = 0;
                            lock (_streamLock)
                            {
                                numBytesRead = ss.Read(data, 0, data.Length);
                            }

                            if (numBytesRead <= 0) break;
                            ms.Write(data, 0, numBytesRead);
                            if (numBytesRead < data.Length) break;
                        }
                    }
                    catch
                    {
                    }

                    return ms.ToArray();
                }

            return new byte[0];
        }

        public void Dispose()
        {
            if (_pipeline != null) _pipeline.Dispose();
            if (_stream != null)
            {
                try
                {
                    _stream.Close();
                }
                catch
                {
                }

                try
                {
                    _stream.Dispose();
                }
                catch
                {
                }
            }
        }

        private static T GetOrThrow<T>(T obj)
        {
            if (obj == null)
                throw new RedisClientException("Connection was not opened");
            return obj;
        }
    }
}
