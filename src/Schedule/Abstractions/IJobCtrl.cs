using Quartz;
using Schedule.Model;

namespace Schedule.Abstractions
{
    /// <summary>
    ///     任务控制接口
    /// </summary>
    public interface IJobCtrl
    {
        /// <summary>
        ///     获取job key
        /// </summary>
        /// <returns></returns>
        JobKey GetJobKey();

        /// <summary>
        ///     开始执行任务
        /// </summary>
        /// <returns></returns>
        bool StartJob(JobMeta jobParam, bool onceJob = false);

        /// <summary>
        ///     读取配置文件
        /// </summary>
        /// <returns></returns>
        JobMeta ReadJobParam();
    }
}