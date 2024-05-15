using System;
using System.Collections.Generic;
using System.Text;

namespace WindNight.Core.Abstractions
{
    public class RemoteFileInfo
    {
        public string ETag { get; set; }
        public string FileName { get; set; }
        public long ContentLength { get; set; }
        public virtual bool IsExist { get; set; }



    }
}
