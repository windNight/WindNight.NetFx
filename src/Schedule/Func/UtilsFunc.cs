using Newtonsoft.Json.Extension;
using Quartz;
using Schedule.Model;
using System;

namespace Schedule.Func
{
    public class UtilsFunc
    {
        /// <summary>
        ///     获取运行时jobkey
        /// </summary>
        /// <param name="name"></param>
        /// <param name="group"></param>
        /// <returns></returns>
        public static JobKey GenJobKey(string name, string? group = "")
        {
            return new JobKey($"{name}_running_jkey",
                group.IsNullOrEmpty() ? $"{name}_jgroup" : group);
        }

        /// <summary>
        ///     获取运行时trigger key
        /// </summary>
        /// <param name="name"></param>
        /// <param name="group"></param>
        /// <returns></returns>
        public static TriggerKey GenTriggerKey(string name, string? group = "")
        {
            return new TriggerKey($"{name}_running_tkey",
                group.IsNullOrEmpty() ? $"{name}_tgroup" : group);
        }

        /// <summary>
        ///     获取运行时listener name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string GenListenerName(string name)
        {
            return $"{name}_running_lname";
        }

        /// <summary>
        ///     序列化
        /// </summary>
        /// <param name="jobMeta"></param>
        /// <returns></returns>
        public static string JobMetaToString(JobMeta jobMeta)
        {
            return jobMeta.ToJsonStr();
        }

        /// <summary>
        ///     反序列化
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static JobMeta? StringToJobMeta(string str)
        {
            return str.To<JobMeta>();
        }
    }
}