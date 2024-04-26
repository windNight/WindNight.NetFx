using System;
using Newtonsoft.Json;

namespace WindNight.Core.Abstractions
{
    public class QueryDateRangeCondition : IDateRangeCondition
    {
        public long BeginTs { get; set; }
        public long EndTs { get; set; }
    }


    public class QueryDateCondition : IDateCondition
    {
        /// <summary> 日期的时间戳 13位  </summary>
        public long QueryTs { get; set; }
    }


    public class QueryDayCondition : QueryDateCondition, IDayCondition
    {
        /// <summary> 服务端处理，前端不用传值  </summary>
        [JsonIgnore]
        public virtual int QueryDateInt => QueryTs == 0 ? HardInfo.YesterdayDateInt : QueryTs.ConvertToTimeIntUseUnix();
    }



    public class QueryDayRangeCondition : QueryDateRangeCondition, IDayRangeCondition
    {
        /// <summary>后端处理 </summary>
        [JsonIgnore]
        public virtual int BeginDateInt => BeginTs > 0 ? BeginTs.ConvertToTimeIntUseUnix() : HardInfo.YesterdayDateInt;

        /// <summary>后端处理 </summary>
        [JsonIgnore]
        public virtual int EndDateInt => EndTs > 0 ? EndTs.ConvertToTimeIntUseUnix() : HardInfo.YesterdayDateInt;

    }


    public class QueryMonthCondition : QueryDateCondition, IMonthCondition, IYearCondition
    {
        /// <summary> 服务端处理，前端不用传值  </summary>
        [JsonIgnore]
        public virtual int QueryMonthInt => QueryTs == 0 ? -1 : QueryTs.ConvertToTimeUseUnix().ToDateInt("yyyyMM");

        public int YearInt { get; set; }
    }


    public class QueryMonthRangeCondition : QueryDateRangeCondition, IMonthRangeCondition, IYearCondition
    {
        /// <summary>后端处理 </summary>
        [JsonIgnore]
        public virtual int BeginMonthInt => BeginTs == 0 ? -1 : BeginTs.ConvertToTimeUseUnix().ToDateInt("yyyyMM");

        /// <summary>后端处理 </summary>
        [JsonIgnore]
        public virtual int EndMonthInt => EndTs == 0 ? -1 : EndTs.ConvertToTimeUseUnix().ToDateInt("yyyyMM");

        public int YearInt { get; set; }

    }





    public class QueryWeekRangeCondition : IWeekRangeCondition
    {
        public int BeginYear { get; set; }
        public int BeginWeek { get; set; }
        public int EndYear { get; set; }
        public int EndWeek { get; set; }

    }

    public class QueryWeekCondition : IWeekCondition
    {
        public int QueryYear { get; set; }
        public int QueryWeek { get; set; }

        /// <summary>
        ///  后端处理
        /// </summary>
        public virtual int QueryYearWeek => $"{QueryYear}{QueryWeek:00}".ToInt();

    }


    public class QueryYearCondition : IYearCondition
    {
        /// <summary>
        /// yyyy 2024
        /// </summary>
        public int YearInt { get; set; }
    }


}
