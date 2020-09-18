﻿using System;

namespace Mongodb.Driver.Extensions.Attributes
{
    /// <summary>
    ///     This attribute allows you to specify of the name of the collection.
    ///     Added at commit c117bf2a7fee378f1e02199dea9b2023a7089ee2 by https://github.com/Etchelon
    ///     who has included the CollectionName attribute into the repo to give another choice to the user on how
    ///     to name their collections.
    ///     The attribute takes precedence of course, and if not present the library will fall back to your Pluralize method.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class CollectionNameAttribute : Attribute
    {
        /// <summary>
        ///     The constructor.
        /// </summary>
        /// <param name="name">The name of the collection.</param>
        public CollectionNameAttribute(string name)
        {
            Name = name;
        }

        /// <summary>
        ///     The name of the collection in which your documents are stored.
        /// </summary>
        public string Name { get; set; }
    }
}