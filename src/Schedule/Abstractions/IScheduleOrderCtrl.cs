using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Schedule.Model.Enums;

namespace Schedule.Abstractions
{
    /// <summary>
    /// </summary>
    public interface IScheduleOrderCtrl
    {
        /// <summary>
        ///     判断是否所有需要等待的完成的job已经正确完成
        /// </summary>
        /// <param name="sourceJob"></param>
        /// <param name="jobNames"></param>
        /// <param name="jobStartTime"></param>
        /// <returns></returns>
        Task<JobRunStateEnum> WaitJobCompleted(string sourceJob, List<string> jobNames, DateTime jobStartTime);

        /// <summary>
        ///     开始一个job,确认的 need do try catch
        /// </summary>
        /// <param name="jobId"></param>
        /// <param name="jobName"></param>
        /// <param name="jobCode"></param>
        /// <param name="runParams"></param>
        /// <returns></returns>
        Task StartJobSafety(string jobId, string jobName, string jobCode, string runParams);


        /// <summary>
        ///     完成一个job,确认的 need do try catch
        /// </summary>
        /// <param name="jobId"></param>
        /// <param name="jobRunState"></param>
        /// <returns></returns>
        Task<bool> CompleteJobSafety(string jobId, JobRunStateEnum jobRunState);
    }
}