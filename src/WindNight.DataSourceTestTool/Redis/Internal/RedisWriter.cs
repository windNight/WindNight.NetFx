using System.Globalization;
using System.IO;
using System.Text;


namespace WindNight.DataSourceTestTool.Redis.Internal
{
    internal class RedisWriter
    {
        private const char Bulk = (char)RedisMessage.Bulk;
        private const char MultiBulk = (char)RedisMessage.MultiBulk;
        private const string EOL = "\r\n";

        private readonly RedisIO _io;

        public RedisWriter(RedisIO io)
        {
            _io = io;
        }
         
        public byte[] Prepare(RedisCommand command)
        {
            var parts = command.Command.Split(' ');
            var length = parts.Length + command.Arguments.Length;
            var sb = new StringBuilder();
            sb.Append(MultiBulk).Append(length).Append(EOL);

            foreach (var part in parts)
                sb.Append(Bulk).Append(_io.Encoding.GetByteCount(part)).Append(EOL).Append(part).Append(EOL);

            var ms = new MemoryStream();
            var data = _io.Encoding.GetBytes(sb.ToString());
            ms.Write(data, 0, data.Length);

            foreach (var arg in command.Arguments)
                if (arg != null && arg.GetType() == typeof(byte[]))
                {
                    data = arg as byte[];
                    var data2 = _io.Encoding.GetBytes($"{Bulk}{data.Length}{EOL}");
                    ms.Write(data2, 0, data2.Length);
                    ms.Write(data, 0, data.Length);
                    ms.Write(new byte[] { 13, 10 }, 0, 2);
                }
                else
                {
                    var str = string.Format(CultureInfo.InvariantCulture, "{0}", arg);
                    data = _io.Encoding.GetBytes($"{Bulk}{_io.Encoding.GetByteCount(str)}{EOL}{str}{EOL}");
                    ms.Write(data, 0, data.Length);
                }
            
            return ms.ToArray();
        }
    }
}
