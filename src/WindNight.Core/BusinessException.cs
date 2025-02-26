using System;

namespace WindNight.Core
{
    public class BusinessException : Exception
    {
        public BusinessException(int code, string msg) : base(msg)
        {
            BusinessCode = code;
        }

        public BusinessException(int code, string msg, Exception innerException) : base(msg, innerException)
        {
            BusinessCode = code;
        }

        public int BusinessCode { get; set; }
    }
}
