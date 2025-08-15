using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Schedule.Abstractions;
using Schedule.Func;
using Schedule.Model;

namespace Schedule.Ctrl
{
    public class CommJobEnvManager : IJobEnvManager
    {
        public bool SaveJobEnv(JobMeta jobParams)
        {
            var allJobsEnv = ReadAllJobEnv();
            allJobsEnv.RemoveAll(x => x.JobId.Equals(jobParams.JobId) && x.JobName.Equals(jobParams.JobName));
            allJobsEnv.Add(jobParams);
            return __Save(allJobsEnv);
        }

        public bool DelJobFromEnv(string name)
        {
            var allJobsEnv = ReadAllJobEnv();
            allJobsEnv.RemoveAll(x => x.JobName.Equals(name));
            return __Save(allJobsEnv);
        }

        public JobMeta ReadJobEnv(string name)
        {
            var allJobsEnv = ReadAllJobEnv();
            return allJobsEnv.FirstOrDefault(x => x.JobName.Equals(name));
        }

        public List<JobMeta> ReadAllJobEnv()
        {
            if (!File.Exists("jobrunningenv.cache"))
            {
                return new List<JobMeta>(0);
            }

            var allJobStr = File.ReadAllLines("jobrunningenv.cache", Encoding.UTF8);
            var jobMetaList = new List<JobMeta>(allJobStr.Length);
            foreach (var item in allJobStr)
            {
                jobMetaList.Add(UtilsFunc.StringToJobMeta(item));
            }
            return jobMetaList;
        }

        private bool __Save(List<JobMeta> jobMetas)
        {
            var encodingJobMetas = new List<string>();
            foreach (var item in jobMetas)
            {
                encodingJobMetas.Add(UtilsFunc.JobMetaToString(item));
            }
            File.WriteAllLines("jobrunningenv.cache", encodingJobMetas, Encoding.UTF8);
            return true;
        }
    }
}
