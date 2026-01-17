using System;
using System.Collections.Generic;
using System.Text;
using WindNight.Core.Abstractions;

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
