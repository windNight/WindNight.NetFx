using System;
using System.Collections.Generic;
using System.Text;

namespace WindNight.Core.Abstractions
{
    public interface ISvrCenterReportRes
    {
        bool Success { get; }
        string SvrToken { get; }
        long ServerTs { get; }
        long SvrTokenExpireTs { get; }
        IEnumerable<string> ServerIps { get; }
    }

    public interface ISvrCenterRegisteredInfo : ISvrCenterReportInfo
    {


    }

    public interface ISvrCenterReportInfo
    {
        string AppId { get; }
        string AppName { get; }
        string AppCode { get; }
        string SvrToken { get; }
        long SvrTokenExpireTs { get; }

        IEnumerable<string> ServerIps { get; }
        ISvrCenterReportRes RegisteredRes { get; }
        void UpdateRegisteredRes(ISvrCenterReportRes res);
    }

}
