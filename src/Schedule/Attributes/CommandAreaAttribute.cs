using System;

namespace Schedule.Attributes
{
    /// <summary> 指令区域属性  </summary>
    public class CommandAreaAttribute : Attribute
    {
        /// <summary>
        /// </summary>
        /// <param name="name"></param>
        public CommandAreaAttribute(string name)
        {
            Name = name;
        }

        /// <summary>
        ///     名称
        /// </summary>
        public string Name { get; }
    }
}