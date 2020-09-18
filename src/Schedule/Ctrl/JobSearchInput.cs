using System.Collections.Generic;
using Schedule.Model;
using Schedule.Model.Enums;

namespace Schedule.Ctrl
{
    public class JobSearchInput
    {
        /// <summary>
        /// </summary>
        public string JobName { set; get; }

        /// <summary>
        ///     只显示额外单次运行的任务
        /// </summary>
        public bool? ShowOnceJob { set; get; }

        /// <summary>
        ///     job状态
        /// </summary>
        public JobStateEnum? State { set; get; }

        /// <summary>
        ///     是否支持单次运行
        /// </summary>
        public bool? SupportOnceJob { set; get; }

        /// <summary>
        ///     页码
        /// </summary>
        public int PageCurrent { set; get; }

        /// <summary>
        ///     页大小
        /// </summary>
        public int PageSize { set; get; }
    }

    public class JobInfoOutput
    {
        /// <summary>
        ///     数据列表
        /// </summary>
        public List<JobMeta> DataList { set; get; }

        /// <summary>
        ///     当前页
        /// </summary>
        public int PageCurrent { set; get; }

        /// <summary>
        ///     数据总的数量
        /// </summary>
        public int Total { set; get; }
    }
}