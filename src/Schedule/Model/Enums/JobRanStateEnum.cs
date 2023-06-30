namespace Schedule.Model.Enums
{
    public enum JobRunStateEnum
    {
        Unknown = 0,

        /// <summary>  job运行正常结束 </summary>
        Ok = 10,

        /// <summary> job运行逻辑错误 </summary>
        Error = 20,

        /// <summary> job运行异常  </summary>
        Exception = 30,

        /// <summary> 正在运行  </summary>
        Running = 40,

        /// <summary> 未开始运行 </summary>
        NotStarted = 50,

        /// <summary> 业务错误</summary>
        BusinessError = 60
    }
}