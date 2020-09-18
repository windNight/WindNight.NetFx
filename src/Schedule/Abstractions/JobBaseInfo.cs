using System;
using System.Collections.Generic;
using System.Text;

namespace Schedule.Abstractions
{
    public class JobBaseInfo
    {
        public string JobId { get; set; }
        public string JobName { get; set; }
        public string JobCode { get; set; }
        public override string ToString()
        {
            return $"{JobName} ({JobCode}) :{JobId}";
        }
    }
}
