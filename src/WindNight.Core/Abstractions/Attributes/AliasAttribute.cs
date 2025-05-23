using System;
using System.Xml.Linq;

namespace WindNight.Core.Attributes.Abstractions
{
    [AttributeUsage(AttributeTargets.All)]
    public class AliasAttribute : Attribute
    {
        /// <summary>
        ///     The constructor.
        /// </summary>
        /// <param name="name">The AliasName Of The Class.</param>
        public AliasAttribute(string name)
        {
            Name = name;
        }

        public string Name { get; set; }
    }





}



