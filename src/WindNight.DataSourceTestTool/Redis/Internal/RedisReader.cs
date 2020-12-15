using System;
using System.IO;
using System.Text;

namespace WindNight.DataSourceTestTool.Redis.Internal
{
    internal class RedisReader
    {
        private readonly RedisIO _io;

        public RedisReader(RedisIO io)
        {
            _io = io;
        }

        public RedisMessage ReadType()
        {
            var type = (RedisMessage) _io.ReadByte();
            //Console.WriteLine($"ReadType: {type}");
            if (type == RedisMessage.Error)
                throw new RedisException(ReadStatus(false));
            return type;
        }

        public string ReadStatus(bool checkType = true)
        {
            if (checkType)
                ExpectType(RedisMessage.Status);
            return ReadLine();
        }

        public long ReadInt(bool checkType = true)
        {
            if (checkType)
                ExpectType(RedisMessage.Int);

            var line = ReadLine();
            return long.Parse(line);
        }

        public object ReadBulk(bool checkType = true, bool asString = false)
        {
            if (asString)
                return ReadBulkString(checkType);
            return ReadBulkBytes(checkType);
        }

        public byte[] ReadBulkBytes(bool checkType = true)
        {
            if (checkType)
                ExpectType(RedisMessage.Bulk);

            var size = (int) ReadInt(false);
            if (size == -1)
                return null;

            var bulk = new byte[size];
            var bytes_read = 0;
            var bytes_remaining = size;

            while (bytes_read < size)
                bytes_read += _io.Read(bulk, bytes_read, size - bytes_read);

            //Console.WriteLine($"ReadBulkBytes1: {Encoding.UTF8.GetString(bulk)}");
            ExpectBytesRead(size, bytes_read);
            ReadCRLF();
            return bulk;
        }

        public void ReadBulkBytes(Stream destination, int bufferSize, bool checkType = true)
        {
            if (checkType)
                ExpectType(RedisMessage.Bulk);
            var size = (int) ReadInt(false);
            if (size == -1)
                return;

            var buffer = new byte[bufferSize];
            var position = 0;
            while (position < size)
            {
                var bytes_to_buffer = Math.Min(buffer.Length, size - position);
                var bytes_read = 0;
                while (bytes_read < bytes_to_buffer)
                {
                    var bytes_to_read = Math.Min(bytes_to_buffer - bytes_read, size - position);
                    bytes_read += _io.Read(buffer, bytes_read, bytes_to_read);
                }

                position += bytes_read;
                destination.Write(buffer, 0, bytes_read);
            }

            //Console.WriteLine($"ReadBulkBytes2: {Encoding.UTF8.GetString(buffer)}");
            ExpectBytesRead(size, position);
            ReadCRLF();
        }

        public string ReadBulkString(bool checkType = true)
        {
            var bulk = ReadBulkBytes(checkType);
            if (bulk == null)
                return null;
            return _io.Encoding.GetString(bulk);
        }

        public void ExpectType(RedisMessage expectedType)
        {
            var type = ReadType();
            if ((int) type == -1)
            {
                var alldata = _io.ReadAll();
                try
                {
                    _io.Dispose();
                }
                catch
                {
                }

                throw new EndOfStreamException(
                    $"Unexpected end of stream; expected type '{expectedType}'; data = '{Encoding.UTF8.GetString(alldata)}'");
            }

            if (type != expectedType)
                throw new RedisProtocolException($"Unexpected response type: {type} (expecting {expectedType})");
        }

        public void ExpectMultiBulk(long expectedSize)
        {
            ExpectType(RedisMessage.MultiBulk);
            ExpectSize(expectedSize);
        }

        public void ExpectSize(long expectedSize)
        {
            var size = ReadInt(false);
            ExpectSize(expectedSize, size);
        }

        public void ExpectSize(long expectedSize, long actualSize)
        {
            if (actualSize != expectedSize)
                throw new RedisProtocolException("Expected " + expectedSize + " elements; got " + actualSize);
        }

        public void ReadCRLF() // TODO: remove hardcoded
        {
            var r = _io.ReadByte();
            var n = _io.ReadByte();
            //Console.WriteLine($"ReadCRLF: {r} {n}");
            if (r != 13 && n != 10)
                throw new RedisProtocolException(string.Format("Expecting CRLF; got bytes: {0}, {1}", r, n));
        }

        public object[] ReadMultiBulk(bool checkType = true, bool bulkAsString = false)
        {
            if (checkType)
                ExpectType(RedisMessage.MultiBulk);
            var count = ReadInt(false);
            if (count == -1)
                return null;

            var lines = new object[count];
            for (var i = 0; i < count; i++)
                lines[i] = Read(bulkAsString);
            return lines;
        }

        public object Read(bool bulkAsString = false)
        {
            var type = ReadType();
            switch (type)
            {
                case RedisMessage.Bulk:
                    return ReadBulk(false, bulkAsString);

                case RedisMessage.Int:
                    return ReadInt(false);

                case RedisMessage.MultiBulk:
                    return ReadMultiBulk(false, bulkAsString);

                case RedisMessage.Status:
                    return ReadStatus(false);

                case RedisMessage.Error:
                    throw new RedisException(ReadStatus(false));

                default:
                    throw new RedisProtocolException("Unsupported response type: " + type);
            }
        }

        private string ReadLine()
        {
            var sb = new StringBuilder();
            char c;
            var should_break = false;
            while (true)
            {
                c = (char) _io.ReadByte();
                if (c == '\r') // TODO: remove hardcoded
                {
                    should_break = true;
                }
                else if (c == '\n' && should_break)
                {
                    break;
                }
                else
                {
                    sb.Append(c);
                    should_break = false;
                }
            }

            //Console.WriteLine($"ReadLine: {sb.ToString()}");
            return sb.ToString();
        }

        private void ExpectBytesRead(long expecting, long actual)
        {
            if (actual != expecting)
                throw new RedisProtocolException(string.Format("Expecting {0} bytes; got {1} bytes", expecting,
                    actual));
        }
    }
}