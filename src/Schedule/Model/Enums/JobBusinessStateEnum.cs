namespace Schedule.Model.Enums
{
    public enum JobBusinessStateEnum
    {
        Unknown = 0,
        /// <summary> 进行中 </summary>
        Processing = 10,
        /// <summary> 成功  </summary>
        Success = 20,
        /// <summary> 失败 </summary>
        Fail = 30,
        /// <summary> 否决 </summary>
        Vetoed = 40,
        /// <summary> 空跑的任务 </summary>
        EmptyRun = 50,
        /// <summary> Crashed </summary>
        Crashed = 99,

    }
}
