using System;
using System.Collections.Generic;
using System.Text;
using WindNight.Extension.Logger.DbLog.Abstractions;

namespace WindNight.Extension.Logger.DbLog.Abstractions
{
    public interface ISystemLogsProcess
    {

        bool Insert(SysLogs entity);
        Task<bool> InsertAsync(SysLogs entity);

        bool BatchInsert(List<SysLogs> entities);
        Task<bool> BatchInsertAsync(List<SysLogs> entities);


    }
}
