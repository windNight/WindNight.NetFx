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
        Conflict = 30
    }
}