using WindNight.Core.Abstractions;

namespace WindNight.Extension.Logger.DcLog.Abstractions
{
    public interface IDcLoggerProcessor : IDisposable
    {
        /// <summary>
        /// </summary>
        /// <param name="message"></param>
        void EnqueueMessage(SysLogs message);
    }


}
