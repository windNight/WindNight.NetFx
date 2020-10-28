using System;

namespace Microsoft.AspNetCore.Mvc.WnExtensions.Abstractions.Attributes
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public class NonAuthAttribute : Attribute
    {
        public NonAuthAttribute(bool noauth = true)
        {
            NoAuth = noauth;
        }

        public bool NoAuth { get; }
    }
}