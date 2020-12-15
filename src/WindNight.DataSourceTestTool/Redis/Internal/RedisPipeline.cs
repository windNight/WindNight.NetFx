using System;
using System.Collections.Concurrent;
using System.IO;

namespace WindNight.DataSourceTestTool.Redis.Internal
{
    internal class RedisPipeline : IDisposable
    {
        private readonly MemoryStream _buffer;
        private readonly object _bufferLock = new object();
        private readonly RedisIO _io;
        private readonly ConcurrentQueue<Func<object>> _parsers;

        internal RedisPipeline(RedisIO io)
        {
            _io = io;
            _buffer = new MemoryStream();
            _parsers = new ConcurrentQueue<Func<object>>();
        }

        public bool Active { get; private set; }

        public void Dispose()
        {
            _buffer.Dispose();
        }

        public T Write<T>(RedisCommand<T> command)
        {
            var data = _io.Writer.Prepare(command);
            lock (_bufferLock)
            {
                _buffer.Write(data, 0, data.Length);
                _parsers.Enqueue(() => command.Parse(_io.Reader));
            }

            return default;
        }

        public void Begin()
        {
            Active = true;
        }

        public object[] Flush()
        {
            try
            {
                var results = new object[0];
                if (_parsers.IsEmpty == false)
                    lock (_bufferLock)
                    {
                        if (_parsers.IsEmpty == false)
                        {
                            _buffer.Position = 0;
                            //Console.WriteLine(Encoding.UTF8.GetString(_buffer.ToArray()));
                            _io.Write(_buffer);

                            _buffer.SetLength(0);

                            results = new object[_parsers.Count];
                        }
                    }

                for (var i = 0; i < results.Length; i++)
                    if (_parsers.TryDequeue(out var func))
                        try
                        {
                            results[i] = func();
                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }

                return results;
            }
            finally
            {
                Active = false;
            }
        }
    }
}