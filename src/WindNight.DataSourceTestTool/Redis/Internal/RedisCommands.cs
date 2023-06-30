using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WindNight.DataSourceTestTool.Redis.Internal.Commands;

namespace WindNight.DataSourceTestTool.Redis.Internal
{
    internal static class RedisCommands
    {
        public static RedisObject Call(string command, params string[] args)
        {
            return new RedisObject(command, args);
        }

        //public static RedisStatus AsTransaction<T>(RedisCommand<T> command)
        //{
        //    return new RedisStatus(command.Command, command.Arguments);
        //}

        #region Connection

        public static RedisStatus Auth(string password)
        {
            return new RedisStatus("AUTH", password);
        }

        public static RedisString Echo(string message)
        {
            return new RedisString("ECHO", message);
        }

        public static RedisStatus Ping()
        {
            return new RedisStatus("PING");
        }

        public static RedisStatus Quit()
        {
            return new RedisStatus("QUIT");
        }

        public static RedisStatus Select(int index)
        {
            return new RedisStatus("SELECT", index);
        }

        #endregion

        #region Server

        public static RedisString ClientList()
        {
            return new RedisString("CLIENT LIST");
        } 

        //public static RedisStatus ClientPause(int milliseconds)
        //{
        //    return new RedisStatus("CLIENT PAUSE", milliseconds);
        //}

        public static RedisString Info(string section = null)
        {
            return new RedisString("INFO", section == null ? new string[0] : new[] { section });
        }


        public static RedisStatus Monitor()
        {
            return new RedisStatus("MONITOR");
        }


        #endregion


        #region Transactions

        //public static RedisStatus Discard()
        //{
        //    return new RedisStatus("DISCARD");
        //}

        //public static RedisArray Exec()
        //{
        //    return new RedisArray("EXEC");
        //}

        //public static RedisStatus Multi()
        //{
        //    return new RedisStatus("MULTI");
        //}


        #endregion



    }

    internal class RedisCommand
    {
        protected RedisCommand(string command, params object[] args)
        {
            Command = command;
            Arguments = args;
        }

        public string Command { get; }

        public object[] Arguments { get; }
    }

    internal abstract class RedisCommand<T> : RedisCommand
    {
        protected RedisCommand(string command, params object[] args)
            : base(command, args)
        {
        }

        public abstract T Parse(RedisReader reader);

        public override string ToString()
        {
            return $"{Command} {string.Join(" ", Arguments)}";
        }
    }
}
