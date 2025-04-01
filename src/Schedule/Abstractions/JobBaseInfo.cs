using System;

namespace Schedule.Abstractions
{
    public class JobBaseInfo
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
