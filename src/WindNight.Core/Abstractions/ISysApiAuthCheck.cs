using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace WindNight.Core.Abstractions
{
    public interface ISysApiAuthCheck
    {
        /// <summary>
        ///  true 则验证 <see cref="SysApiAuth()"/> false 则验证 <see cref="ReqClientIpCheck(string)"/>
        /// </summary>
        bool OpenSysApiAuthCheck { get; }

        bool SysApiAuth();

        bool ReqClientIpCheck(string clientIp);



    }
}
