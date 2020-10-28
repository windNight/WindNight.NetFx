using System;

namespace Microsoft.AspNetCore.Mvc.WnExtensions.Abstractions.Attributes
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public class ClearResultAttribute : Attribute
    {
        public ClearResultAttribute(bool isclear = true)
        {
            IsClear = isclear;
        }

        public bool IsClear { get; }
    }
}