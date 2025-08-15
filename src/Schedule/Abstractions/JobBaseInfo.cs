using System;

namespace Schedule.Abstractions
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

    public class JobBaseInfo : IJobBaseInfo
    {
        public string JobId { get; set; } = "";
        public string JobName { get; set; } = "";
        public string JobCode { get; set; } = "";
        public long JobExecTs { get; set; }
        public string ExecTag { get; set; } = "";

        public override string ToString()
        {
            return $"{JobName} ({JobCode}) :{JobId} ";
        }

        public string ToString(bool needAppend)
        {
            if (needAppend)
            {
                return $"{JobName} ({JobCode}) :{JobId} execTag:{ExecTag} {JobExecTs.ConvertToTimeFormatUseUnix()} ";
            }
            return this.ToString();
        }
    }
}
