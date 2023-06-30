using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindNight.DataSourceTestTool.Redis.Internal.Commands
{
    internal class RedisStatus : RedisCommand<string>
    {
        public RedisStatus(string command, params object[] args)
            : base(command, args)
        {
        }

        public override string Parse(RedisReader reader)
        {
            return reader.ReadStatus();
        }

        //public class Empty : RedisCommand<string>
        //{
        //    public Empty(string command, params object[] args)
        //        : base(command, args)
        //    {
        //    }

        //    public override string Parse(RedisReader reader)
        //    {
        //        var type = reader.ReadType();
        //        if ((int)type == -1)
        //            return string.Empty;
        //        if (type == RedisMessage.Error)
        //            throw new RedisException(reader.ReadStatus(false));

        //        throw new RedisProtocolException("Unexpected type: " + type);
        //    }
        //}

        //public class Nullable : RedisCommand<string>
        //{
        //    public Nullable(string command, params object[] args)
        //        : base(command, args)
        //    {
        //    }

        //    public override string Parse(RedisReader reader)
        //    {
        //        var type = reader.ReadType();
        //        if (type == RedisMessage.Status)
        //            return reader.ReadStatus(false);

        //        var result = reader.ReadMultiBulk(false);
        //        if (result != null)
        //            throw new RedisProtocolException("Expecting null MULTI BULK response. Received: " + result);

        //        return null;
        //    }
        //}
    }
}
