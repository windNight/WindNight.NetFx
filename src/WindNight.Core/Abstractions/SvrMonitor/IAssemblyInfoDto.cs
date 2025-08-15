using System;
using System.Collections.Generic;
using System.Text;

namespace WindNight.Core.Abstractions
{
    public interface IAssemblyInfoDto
    {
        bool IsSuccess { get; }

        /// <summary>
        ///  GetName().Version  the major, minor, build, and revision numbers of the assembly.
        /// </summary>
        string AssemblyVersion { get; }
        /// <summary>
        ///  The display name of the assembly.
        /// </summary>
        string AssemblyFullName { get; }
        /// <summary> <see cref="System.Reflection.Module"/> The module name with no path.</summary>
        string AssemblyName { get; }
        string AssemblyCodeBase { get; }

        /// <summary>Gets the full path or UNC location of the loaded file that contains the manifest.</summary>
        /// <exception cref="T:System.NotSupportedException">The current assembly is a dynamic assembly, represented by an <see cref="T:System.Reflection.Emit.AssemblyBuilder" /> object.</exception>
        /// <returns>The location of the loaded file that contains the manifest. If the assembly is loaded from a byte array, such as when using <see cref="M:System.Reflection.Assembly.Load(System.Byte[])" />, the value returned is an empty string ("").</returns>
        string AssemblyLocation { get; }
        string AssemblyLastCreateTime { get; }
        string AssemblyLastModifyTime { get; }


    }
}
