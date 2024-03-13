using WindNight.Extension.Logger.DcLog.Abstractions;

namespace WindNight.Extension.Logger.DbLog.Abstractions
{
    public interface IDcLoggerProcessor : IDisposable
    {
        /// <summary>
        /// </summary>
        /// <param name="message"></param>
        void EnqueueMessage(SysLogs message);
    }


}
