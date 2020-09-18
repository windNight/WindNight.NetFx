using System.Collections.Generic;
using Schedule.Model;

namespace Schedule.Abstractions
{
    /// <summary> </summary>
    public interface IJobEnvManager
    {
        /// <summary>
        ///     保存job运行环境
        /// </summary>
        /// <param name="jobParams"></param>
        /// <returns></returns>
        bool SaveJobEnv(JobMeta jobParams);

        /// <summary>
        ///     从job运行环境中删除任务
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        bool DelJobFromEnv(string name);

        /// <summary>
        ///     读取job运行环境
        /// </summary>
        /// <param name="jobKey"></param>
        /// <returns></returns>
        JobMeta ReadJobEnv(string jobKey);

        /// <summary>
        ///     读取所有job运行环境
        /// </summary>
        /// <returns></returns>
        List<JobMeta> ReadAllJobEnv();
    }
}