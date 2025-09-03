using System;
using Newtonsoft.Json.Extension;
using Schedule.Model.Enums;

namespace Schedule.Model
{
    public class JobMeta
    {
        /// <summary>
        ///     临时JobId
        /// </summary>
        public string JobId { set; get; } = "";

        /// <summary>
        ///     Job Name
        /// </summary>
        public string JobName { set; get; } = "";

        /// <summary>
        ///     Job Group
        /// </summary>
        public string Group { set; get; } = "";

        /// <summary>
        ///     JobCode from config
        /// </summary>
        public string JobCode { set; get; } = "";

        /// <summary>
        ///     job标题
        /// </summary>
        public string Title { set; get; } = "";

        /// <summary>
        ///     job描述
        /// </summary>
        public string Description { set; get; } = "";

        /// <summary>
        ///     job开始时间
        /// </summary>
        public DateTime StartTime { set; get; } = default;

        /// <summary>
        ///     job运行间隔
        /// </summary>
        public uint Interval { set; get; }

        /// <summary>
        ///     calendar运行时间
        /// </summary>
        public string CronExpression { set; get; } = "";

        /// <summary>
        ///     job状态
        /// </summary>
        public JobStateEnum State { set; get; }

        /// <summary>
        ///     是否支持单次运行
        /// </summary>
        public bool SupportOnceJob { set; get; }

        /// <summary>
        ///     运行参数
        /// </summary>
        public string RunParams { set; get; } = "";

        /// <summary>
        ///     向上依赖的jobs jobCodes ,分割
        /// </summary>
        public string DepJobs { set; get; } = "";

        /// <summary>
        ///     运行完成后是否退出程序
        /// </summary>
        public bool AutoClose { set; get; }

        /// <summary>
        /// </summary>
        public string JobParamsDesc { set; get; } = "";

        /// <summary>
        ///  是否需要通知 默认 true
        /// </summary>
        public bool IsDoNotice { set; get; } = true;

        /// <summary>
        ///  是否日志记录job的生命周期 默认true
        /// </summary>
        public bool IsLogJobLC { set; get; } = true;

        /// <summary>
        ///   是否持久化记录job的生命周期 默认true
        /// </summary>
        public bool IsStoreJobLC { set; get; } = true;

        /// <summary>
        ///  耗时  n 秒后 报警 默认 300秒    
        /// </summary>
        public long JobWarnTs { set; get; } = 300L;

        /// <summary>
        ///  是否可以进行冒烟测试,默认 false, 只是个标记 如果需要执行冒烟测试 测试内容由业务方实现
        /// </summary>
        public bool CanRunTest { set; get; } = false;

        public override string ToString()
        {
            return this.ToJsonStr();
        }
    }
}
