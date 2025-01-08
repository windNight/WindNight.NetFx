using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Schedule.Ctrl;
using Schedule.Model;
using Schedule.Model.Enums;

namespace Schedule.Abstractions
{
    /// <summary>
    ///     指令基础类
    /// </summary>
    public interface ICommandCtrl
    {
        /// <summary>
        ///     查询job信息
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        JobInfoOutput GetJobInfo(JobSearchInput search);


        /// <summary>
        ///     获取job详情
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        JobMeta GetDetail(string name);

        /// <summary>
        ///     获取后台调度的任务信息
        /// </summary>
        /// <returns></returns>
        List<JobMeta> GetBGJobInfo();

        /// <summary>
        ///     单次运行job
        /// </summary>
        /// <param name="name"></param>
        /// <param name="execTime"></param>
        /// <param name="runParams"></param>
        /// <param name="autoClose"></param>
        /// <returns></returns>
        JobActionRetEnum StartJob(string name, DateTime execTime, string runParams, bool autoClose);

        /// <summary>
        ///     停止单次运行的job
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        Task<JobActionRetEnum> StopJobAsync(string name);

        /// <summary>
        ///     暂停非单次运行的job运行
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        JobActionRetEnum PauseJob(string name);

        /// <summary>
        ///     恢复非单次运行job运行
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        JobActionRetEnum ResumeJob(string name);

        string GetJobParamsDesc(string name);

    }
}