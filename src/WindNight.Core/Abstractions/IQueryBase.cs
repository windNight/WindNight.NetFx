namespace WindNight.Core.Abstractions
{
    public interface IKeywordCondition
    {
        /// <summary>
        ///     关键字   各个业务自定义模糊查询条件
        /// </summary>
        string Keyword { get; set; }
    }


    public interface IDateCondition
    {
        /// <summary> 日期的时间戳 13位  </summary>
        long QueryTs { get; set; }
    }

    /// <summary>
    ///     DateInt("yyyyMMdd")
    /// </summary>
    public interface IDayCondition : IDateCondition
    {
        /// <summary> 后端处理 readonly </summary>
        int QueryDateInt { get; }
    }


    public interface IDateRangeCondition
    {
        /// <summary> 开始日期的时间戳 13位  </summary>
        long BeginTs { get; set; }

        /// <summary> 结束日期的时间戳 13位  </summary>
        long EndTs { get; set; }
    }

    /// <summary>
    ///     DateInt
    /// </summary>
    public interface IDayRangeCondition : IDateRangeCondition
    {
        /// <summary> 后端处理  readonly </summary>
        int BeginDateInt { get; }

        /// <summary> 后端处理  readonly </summary>
        int EndDateInt { get; }
    }


    /// <summary>
    ///     DateInt("yyyyMM")
    /// </summary>
    public interface IMonthCondition : IDateCondition
    {
        /// <summary>后端处理 readonly </summary>
        int QueryMonthInt { get; }
    }

    public interface IMonthRangeCondition : IDateRangeCondition
    {
        /// <summary>后端处理 readonly </summary>
        int BeginMonthInt { get; }

        /// <summary>后端处理 readonly </summary>
        int EndMonthInt { get; }
    }

    public interface IWeekRangeCondition
    {
        int BeginYear { get; set; }
        int BeginWeek { get; set; }
        int EndYear { get; set; }
        int EndWeek { get; set; }
    }

    public interface IWeekCondition
    {
        int QueryYear { get; set; }
        int QueryWeek { get; set; }

        int QueryYearWeek { get; }
    }

    public interface IYearCondition
    {
        //int YearInt { get; set; }
        int RecordYear { get; set; }
    }
}
