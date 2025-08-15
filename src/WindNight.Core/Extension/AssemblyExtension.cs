using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using WindNight.Core.Abstractions;

namespace System
{
    public static class AssemblyExtension
    {

        public static IAssemblyInfoDto Analysis(this Assembly assembly)
        {
            var model = new AssemblyInfoDto();
            if (assembly == null)
            {
                return model;
            }
            try
            {
                model.AssemblyCodeBase = assembly.CodeBase;
                model.AssemblyName = assembly.ManifestModule.Name;
                model.AssemblyFullName = assembly.FullName;
                model.AssemblyLocation = assembly.Location;
                model.AssemblyVersion = assembly.GetName()?.Version?.ToString();
                model.AssemblyLastCreateTime = File.GetCreationTime(assembly.Location).FormatDateTimeFullString();
                model.AssemblyLastModifyTime = File.GetLastWriteTime(assembly.Location).FormatDateTimeFullString();
                model.IsSuccess = true;
            }
            catch
            {

            }

            return model;

        }


    }
}
