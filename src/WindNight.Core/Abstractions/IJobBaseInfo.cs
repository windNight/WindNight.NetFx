using System;
using System.Collections.Generic;
using System.Text;

namespace WindNight.Core.Abstractions
{
    public interface IJobBaseInfo
    {
        string JobId { get; set; }
        string JobName { get; set; }
        string JobCode { get; set; }
        long JobExecTs { get; set; }
        string ExecTag { get; set; }
        string ToString(bool needAppend);
    }
}
