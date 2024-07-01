using System;
using System.Collections.Generic;
using System.Text;

namespace WindNight.Core
{
    public class ConfigBaseInfo : ConfigBaseInfo<string>
    {

    }

    public class ConfigBaseInfo2 : ConfigBaseInfo<object>
    {

    }


    public class ConfigBaseInfo<T>
    {
        public string Path { get; set; }
        public string Key { get; set; }
        public T Value { get; set; }
    }
}
