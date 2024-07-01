using System;
using System.Collections.Generic;
using System.Text;

namespace WindNight.Config.Abstractions
{
    public interface IConfigCenterAuth
    {

        bool OpenConfigCenterAuth { get; set; }

        bool ConfigCenterApiAuth();



    }
}
