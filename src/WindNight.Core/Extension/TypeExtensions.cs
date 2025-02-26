namespace System.Reflection
{
    public static class TypeExtensions
    {
        /// <summary>Retrieves a custom attribute of a specified type that is applied to a specified assembly.</summary>
        /// <param name="element">The assembly to inspect.</param>
        /// <param name="attributeType">The type of attribute to search for.</param>
        /// <exception cref="T:System.ArgumentNullException">
        ///     <paramref name="element" /> or <paramref name="attributeType" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="T:System.ArgumentException">
        ///     <paramref name="attributeType" /> is not derived from <see cref="T:System.Attribute" />.
        /// </exception>
        /// <exception cref="T:System.Reflection.AmbiguousMatchException">More than one of the requested attributes was found.</exception>
        /// <returns>
        ///     A custom attribute that matches <paramref name="attributeType" />, or <see langword="null" /> if no such
        ///     attribute is found.
        /// </returns>
        public static bool HasAttribute(this Assembly element, Type attributeType)
        {
            var attr = element.GetCustomAttribute(attributeType);

            return attr is not null;
        }

        /// <summary>Retrieves a custom attribute of a specified type that is applied to a specified module.</summary>
        /// <param name="element">The module to inspect.</param>
        /// <param name="attributeType">The type of attribute to search for.</param>
        /// <exception cref="T:System.ArgumentNullException">
        ///     <paramref name="element" /> or <paramref name="attributeType" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="T:System.ArgumentException">
        ///     <paramref name="attributeType" /> is not derived from <see cref="T:System.Attribute" />.
        /// </exception>
        /// <exception cref="T:System.Reflection.AmbiguousMatchException">More than one of the requested attributes was found.</exception>
        /// <returns>
        ///     A custom attribute that matches <paramref name="attributeType" />, or <see langword="null" /> if no such
        ///     attribute is found.
        /// </returns>
        public static bool HasAttribute(this Module element, Type attributeType)
        {
            var attr = element.GetCustomAttribute(attributeType);

            return attr is not null;
        }


        /// <summary>Retrieves a custom attribute of a specified type that is applied to a specified member.</summary>
        /// <param name="element">The member to inspect.</param>
        /// <param name="attributeType">The type of attribute to search for.</param>
        /// <exception cref="T:System.ArgumentNullException">
        ///     <paramref name="element" /> or <paramref name="attributeType" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="T:System.ArgumentException">
        ///     <paramref name="attributeType" /> is not derived from <see cref="T:System.Attribute" />.
        /// </exception>
        /// <exception cref="T:System.NotSupportedException">
        ///     <paramref name="element" /> is not a constructor, method, property, event, type, or field.
        /// </exception>
        /// <exception cref="T:System.Reflection.AmbiguousMatchException">More than one of the requested attributes was found.</exception>
        /// <exception cref="T:System.TypeLoadException">A custom attribute type cannot be loaded.</exception>
        /// <returns>
        ///     A custom attribute that matches <paramref name="attributeType" />, or <see langword="null" /> if no such
        ///     attribute is found.
        /// </returns>
        public static bool HasAttribute(this MemberInfo element, Type attributeType)
        {
            var attr = element.GetCustomAttribute(attributeType);

            return attr is not null;
        }

        /// <summary>Retrieves a custom attribute of a specified type that is applied to a specified parameter.</summary>
        /// <param name="element">The parameter to inspect.</param>
        /// <param name="attributeType">The type of attribute to search for.</param>
        /// <exception cref="T:System.ArgumentNullException">
        ///     <paramref name="element" /> or <paramref name="attributeType" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="T:System.ArgumentException">
        ///     <paramref name="attributeType" /> is not derived from <see cref="T:System.Attribute" />.
        /// </exception>
        /// <exception cref="T:System.Reflection.AmbiguousMatchException">More than one of the requested attributes was found.</exception>
        /// <exception cref="T:System.TypeLoadException">A custom attribute type cannot be loaded.</exception>
        /// <returns>
        ///     A custom attribute that matches <paramref name="attributeType" />, or <see langword="null" /> if no such
        ///     attribute is found.
        /// </returns>
        public static bool HasAttribute(this ParameterInfo element, Type attributeType)
        {
            var attr = element.GetCustomAttribute(attributeType);

            return attr is not null;
        }

        /// <summary>Retrieves a custom attribute of a specified type that is applied to a specified assembly.</summary>
        /// <param name="element">The assembly to inspect.</param>
        /// <typeparam name="T">The type of attribute to search for.</typeparam>
        /// <exception cref="T:System.ArgumentNullException">
        ///     <paramref name="element" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="T:System.Reflection.AmbiguousMatchException">More than one of the requested attributes was found.</exception>
        /// <returns>
        ///     A custom attribute that matches <paramref name="T" />, or <see langword="null" /> if no such attribute is
        ///     found.
        /// </returns>
        public static bool HasAttribute<T>(this Assembly element) where T : Attribute
        {
            var attr = element.GetCustomAttribute<T>();

            return attr is not null;
        }

        /// <summary>Retrieves a custom attribute of a specified type that is applied to a specified module.</summary>
        /// <param name="element">The module to inspect.</param>
        /// <typeparam name="T">The type of attribute to search for.</typeparam>
        /// <exception cref="T:System.ArgumentNullException">
        ///     <paramref name="element" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="T:System.Reflection.AmbiguousMatchException">More than one of the requested attributes was found.</exception>
        /// <returns>
        ///     A custom attribute that matches <paramref name="T" />, or <see langword="null" /> if no such attribute is
        ///     found.
        /// </returns>
        public static bool HasAttribute<T>(this Module element) where T : Attribute
        {
            var attr = element.GetCustomAttribute<T>();

            return attr is not null;
        }


        /// <summary>Retrieves a custom attribute of a specified type that is applied to a specified member.</summary>
        /// <param name="element">The member to inspect.</param>
        /// <typeparam name="T">The type of attribute to search for.</typeparam>
        /// <exception cref="T:System.ArgumentNullException">
        ///     <paramref name="element" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="T:System.NotSupportedException">
        ///     <paramref name="element" /> is not a constructor, method, property, event, type, or field.
        /// </exception>
        /// <exception cref="T:System.Reflection.AmbiguousMatchException">More than one of the requested attributes was found.</exception>
        /// <exception cref="T:System.TypeLoadException">A custom attribute type cannot be loaded.</exception>
        /// <returns>
        ///     A custom attribute that matches <paramref name="T" />, or <see langword="null" /> if no such attribute is
        ///     found.
        /// </returns>
        public static bool HasAttribute<T>(this MemberInfo element) where T : Attribute
        {
            var attr = element.GetCustomAttribute<T>();

            return attr is not null;
        }


        /// <summary>Retrieves a custom attribute of a specified type that is applied to a specified parameter.</summary>
        /// <param name="element">The parameter to inspect.</param>
        /// <typeparam name="T">The type of attribute to search for.</typeparam>
        /// <exception cref="T:System.ArgumentNullException">
        ///     <paramref name="element" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="T:System.NotSupportedException">
        ///     <paramref name="element" /> is not a constructor, method, property, event, type, or field.
        /// </exception>
        /// <exception cref="T:System.Reflection.AmbiguousMatchException">More than one of the requested attributes was found.</exception>
        /// <exception cref="T:System.TypeLoadException">A custom attribute type cannot be loaded.</exception>
        /// <returns>
        ///     A custom attribute that matches <paramref name="T" />, or <see langword="null" /> if no such attribute is
        ///     found.
        /// </returns>
        public static bool HasAttribute<T>(this ParameterInfo element) where T : Attribute
        {
            var attr = element.GetCustomAttribute<T>();

            return attr is not null;
        }

        public static bool IsDefined<T>(this MemberInfo element)
        {
            var hasAlias = Attribute.IsDefined(element, typeof(T));
            return hasAlias;
        }

        public static bool IsDefined<T>(this ParameterInfo element)
        {
            var hasAlias = Attribute.IsDefined(element, typeof(T));
            return hasAlias;
        }

        public static bool IsDefined<T>(this Module element)
        {
            var hasAlias = Attribute.IsDefined(element, typeof(T));
            return hasAlias;
        }

        public static bool IsDefined<T>(this Assembly element)
        {
            var hasAlias = Attribute.IsDefined(element, typeof(T));
            return hasAlias;
        }
    }
}
