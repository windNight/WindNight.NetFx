using System;
using System.Collections.Generic;
using System.Text;
using MJsonIgnore = System.Text.Json.Serialization.JsonIgnoreAttribute;
using NJsonIgnore = Newtonsoft.Json.JsonIgnoreAttribute;

namespace WindNight.Core.Abstractions
{
    public interface ISvrRuntimeInfo
    {
        /// <summary>Gets the number of processors available to the current process.</summary>
        /// <returns>The 32-bit signed integer that specifies the number of processors that are available.</returns>
        int ProcessorCount { get; }


        /// <summary>Retrieves the value of an environment variable from the current process or from the Windows operating system registry key for the current user or local machine.</summary>
        /// <param name="variable">The name of an environment variable.</param>
        /// <param name="target">One of the <see cref="T:System.EnvironmentVariableTarget" /> values. Only <see cref="F:System.EnvironmentVariableTarget.Process" /> is supported on .NET running on Unix-like systems.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="variable" /> is <see langword="null" />.</exception>
        /// <exception cref="T:System.ArgumentException">
        /// <paramref name="target" /> is not a valid <see cref="T:System.EnvironmentVariableTarget" /> value.</exception>
        /// <exception cref="T:System.Security.SecurityException">The caller does not have the required permission to perform this operation.</exception>
        /// <returns>The value of the environment variable specified by the <paramref name="variable" /> and <paramref name="target" /> parameters, or <see langword="null" /> if the environment variable is not found.</returns>
        string GetEnvironmentVariable(string variable);

        /// <summary>Gets the unique identifier for the current process.</summary>
        /// <returns>A number that represents the unique identifier for the current process.</returns>
        int ProcessId { get; }

        /// <summary>Returns the path of the executable that started the currently executing process. Returns <see langword="null" /> when the path is not available.</summary>
        /// <returns>The path of the executable that started the currently executing process.</returns>
        string ProcessPath { get; }

        /// <summary>Gets the current platform identifier and version number.</summary>
        /// <exception cref="T:System.InvalidOperationException">This property was unable to obtain the system version.
        /// 
        /// -or-
        /// 
        /// The obtained platform identifier is not a member of <see cref="T:System.PlatformID" /></exception>
        /// <returns>The platform identifier and version number.</returns>
        OperatingSystem OSVersion { get; }

        /// <summary>Gets current stack trace information.</summary>
        /// <returns>A string containing stack trace information. This value can be <see cref="F:System.String.Empty" />.</returns>

        [MJsonIgnore, NJsonIgnore]
        string StackTrace { get; }

        /// <summary>Gets the number of bytes in the operating system's memory page.</summary>
        /// <returns>The number of bytes in the system memory page.</returns>
        int SystemPageSize { get; }

        /// <summary>Gets the number of milliseconds elapsed since the system started.</summary>
        /// <returns>A 32-bit signed integer containing the amount of time in milliseconds that has passed since the last time the computer was started.</returns>
        int TickCount { get; }

        /// <summary>Gets the NetBIOS name of this local computer.</summary>
        /// <exception cref="T:System.InvalidOperationException">The name of this computer cannot be obtained.</exception>
        /// <returns>The name of this computer.</returns>
        string RunMachineName { get; }

        /// <summary>Gets the user name of the person who is associated with the current thread.</summary>
        /// <returns>The user name of the person who is associated with the current thread.</returns>
        string RunMachineUserName { get; }

        /// <summary>Gets the network domain name associated with the current user.</summary>
        /// <exception cref="T:System.PlatformNotSupportedException">The operating system does not support retrieving the network domain name.</exception>
        /// <exception cref="T:System.InvalidOperationException">The network domain name cannot be retrieved.</exception>
        /// <returns>The network domain name associated with the current user.</returns>
        string RunMachineUserDomainName { get; }

        /// <summary>Gets or sets the fully qualified path of the current working directory.</summary>
        /// <exception cref="T:System.ArgumentException">Attempted to set to an empty string ("").</exception>
        /// <exception cref="T:System.ArgumentNullException">Attempted to set to <see langword="null" />.</exception>
        /// <exception cref="T:System.IO.IOException">An I/O error occurred.</exception>
        /// <exception cref="T:System.IO.DirectoryNotFoundException">Attempted to set a local path that cannot be found.</exception>
        /// <exception cref="T:System.Security.SecurityException">The caller does not have the appropriate permission.</exception>
        /// <returns>The directory path.</returns>
        string CurrentDirectory { get; }

        /// <summary>Gets the fully qualified path of the system directory.</summary>
        /// <returns>A string containing a directory path.</returns>
        string SystemDirectory { get; }

        /// <summary>Gets a value indicating whether the current process is running in user interactive mode.</summary>
        /// <returns>
        /// <see langword="true" /> if the current process is running in user interactive mode; otherwise, <see langword="false" />.</returns>
        bool UserInteractive { get; }

        /// <summary>Gets the amount of physical memory mapped to the process context.</summary>
        /// <returns>A 64-bit signed integer containing the number of bytes of physical memory mapped to the process context.</returns>
        long WorkingSet { get; }

        /// <summary>
        ///   <para>Get the CPU usage, including the process time spent running the application code, the process time spent running the operating system code, and the total time spent running both the application and operating system code.</para>
        /// </summary>
        IServerCpuUsageInfo CpuUsage { get; }

        /// <summary>Retrieves all environment variable names and their values from the current process.</summary>
        /// <exception cref="T:System.Security.SecurityException">The caller does not have the required permission to perform this operation.</exception>
        /// <exception cref="T:System.OutOfMemoryException">The buffer is out of memory.</exception>
        /// <returns>A dictionary that contains all environment variable names and their values; otherwise, an empty dictionary if no environment variables are found.</returns>
        IReadOnlyDictionary<object, object> GetEnvironmentVariables();


    }

    public interface IServerCpuUsageInfo
    {
        /// <summary>Gets the amount of time the associated process has spent running code inside the application portion of the process (not the operating system code).</summary>
        TimeSpan UserTime { get; }

        /// <summary>Gets the amount of time the process has spent running code inside the operating system code.</summary>
        TimeSpan PrivilegedTime { get; }

        /// <summary>Gets the amount of time the process has spent utilizing the CPU, including the process time spent in the application code and in the operating system code.</summary>
        TimeSpan TotalTime { get; }
    }
}
