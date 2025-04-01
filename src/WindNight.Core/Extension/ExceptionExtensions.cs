using System;
using System.Linq;

namespace WindNight.Core.ExceptionExt
{
    public static class ExceptionExtensions
    {
        /// <summary>获取异常消息</summary>
        /// <param name="ex">异常</param>
        /// <returns></returns>
        public static string GetMessage(this Exception ex)
        {
            try
            {
                if (ex == null)
                {
                    return string.Empty;
                }

                var msg = ex.ToString();

                if (msg.IsNullOrEmpty())
                {
                    return string.Empty;
                }

                var ss = msg.Split(Environment.NewLine);
                var ns = ss.Where(e =>
                    !e.StartsWith("---") &&
                    !e.Contains("System.Runtime.ExceptionServices") &&
                    !e.Contains("System.Runtime.CompilerServices"));

                msg = ns.Join(Environment.NewLine);

                return msg;
            }
            catch
            {
                return string.Empty;
            }
        }
    }
}
