using System;
using System.Collections.Generic;
using System.Text;

namespace WindNight.Extension.Logger.DbLog.Abstractions
{
    public interface IDbLoggerProcessor : IDisposable
    {
        /// <summary>
        /// </summary>
        /// <param name="message"></param>
        void EnqueueMessage(SysLogs message);
    }


}
