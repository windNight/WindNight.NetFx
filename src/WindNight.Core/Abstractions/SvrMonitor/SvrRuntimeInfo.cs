using System;
using System.Collections.Generic;
using System.Text;

namespace WindNight.Core.Abstractions
{    /// <summary>
     /// 
     /// </summary>
     /// <inheritdoc /> 
    public class SvrRuntimeInfo : ISvrRuntimeInfo
    {

        private SvrRuntimeInfo()
        {

        }

        public static ISvrRuntimeInfo Gen()
        {
            return new SvrRuntimeInfo();
        }

        public virtual int ProcessorCount
        {
            get
            {
                try
                {
                    return Environment.ProcessorCount;
                }
                catch
                {
                    return -1;
                }
            }
        }

        public virtual int ProcessId
        {
            get
            {
                try
                {
                    return System.Diagnostics.Process.GetCurrentProcess().Id;
                }
                catch
                {
                    return -1;
                }
            }
        }

        public virtual string ProcessPath
        {
            get
            {
                try
                {
                    return System.Diagnostics.Process.GetCurrentProcess().MainModule?.FileName ?? "";
                }
                catch
                {
                    return "";
                }
            }
        }

        public virtual OperatingSystem OSVersion
        {
            get
            {
                try
                {
                    return Environment.OSVersion;
                }
                catch
                {
                    return null;
                }
            }
        }

        public virtual string StackTrace
        {
            get
            {
                try
                {
                    return Environment.StackTrace ?? "";
                }
                catch
                {
                    return "";
                }
            }
        }

        public virtual int SystemPageSize
        {

            get
            {
                try
                {
                    return Environment.SystemPageSize;
                }
                catch
                {
                    return -1;
                }
            }
        }

        public virtual int TickCount
        {

            get
            {
                try
                {
                    return Environment.TickCount;
                }
                catch
                {
                    return -1;
                }
            }
        }

        public virtual string RunMachineName
        {
            get
            {
                try
                {
                    return Environment.MachineName ?? "";
                }
                catch
                {
                    return "";
                }
            }
        }

        public virtual string RunMachineUserName
        {
            get
            {
                try
                {
                    return Environment.UserName ?? "";
                }
                catch
                {
                    return "";
                }
            }
        }

        public virtual string RunMachineUserDomainName
        {
            get
            {
                try
                {
                    return Environment.UserDomainName ?? "";
                }
                catch
                {
                    return "";
                }
            }
        }

        public virtual string CurrentDirectory
        {
            get
            {
                try
                {
                    return Environment.CurrentDirectory ?? "";
                }
                catch
                {
                    return "";
                }
            }
        }

        public virtual string SystemDirectory
        {
            get
            {
                try
                {
                    return Environment.SystemDirectory ?? "";

                }
                catch
                {
                    return "";
                }
            }
        }

        public virtual bool UserInteractive
        {
            get
            {
                try
                {
                    return Environment.UserInteractive;

                }
                catch (Exception e)
                {
                    return false;
                }
            }
        }

        public virtual long WorkingSet
        {
            get
            {
                try
                {
                    return Environment.WorkingSet;
                }
                catch
                {
                    return -1L;
                }
            }

        }

        public virtual IServerCpuUsageInfo CpuUsage
        {
            get
            {
                var model = new ServerCpuUsageInfo();
                try
                {

                    // 例如：通过 System.Diagnostics.Process 获取当前进程的 CPU 时间
                    var process = System.Diagnostics.Process.GetCurrentProcess();
                    var userTime = process.UserProcessorTime;
                    var privilegedTime = process.PrivilegedProcessorTime;
                    var totalTime = process.TotalProcessorTime;
                    model = new ServerCpuUsageInfo(userTime, privilegedTime, totalTime);
                }
                catch (Exception ex)
                {

                }

                return model;
            }
        }

        public virtual IReadOnlyDictionary<object, object> GetEnvironmentVariables()
        {
            var envDict = new Dictionary<object, object>();
            try
            {
                var dict = Environment.GetEnvironmentVariables();
                foreach (System.Collections.DictionaryEntry item in dict)
                {
                    if (!envDict.ContainsKey(item.Key))
                    {
                        envDict.Add(item.Key, item.Value);
                    }
                }
            }
            catch (Exception e)
            {

            }

            return envDict;
        }

        public virtual string GetEnvironmentVariable(string variable)
        {

            try
            {
                return Environment.GetEnvironmentVariable(variable) ?? "";

            }
            catch
            {
                return "";
            }

        }

    }


}
