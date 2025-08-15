using System;

namespace Schedule.Model.Enums
{
    public enum JobActionRetEnum
    {
        Unknown = 0,

        /// <summary> 执行成功 </summary>
        Success = 10,

        /// <summary> 执行失败 </summary>
        Failed = 20,

        /// <summary> 操作不支持  </summary>
        Conflict = 30,
        /// <summary> 缺少配置项或者配置项不合法  </summary>
        NoConfig = 90,
    }
}

namespace Schedule.Model.Enums.Ex
{
    public static class EnumEx
    {

        public static string ToName(this JobActionRetEnum status)
        {
            var name = status switch
            {
                JobActionRetEnum.Success => "执行成功",
                JobActionRetEnum.Failed => "执行失败",
                JobActionRetEnum.Conflict => "操作不支持",
                JobActionRetEnum.Unknown => "未知",
                _ => "未知",
            };
            return name;
        }
    }
}
