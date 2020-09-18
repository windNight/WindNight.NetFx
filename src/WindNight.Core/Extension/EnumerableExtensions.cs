using System.Collections;
using System.Collections.Generic;

namespace System.Linq.Expressions
{
    /// <summary>
    /// </summary>
    public static class EnumerableExtensions
    {
        /// <summary>
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="items"></param>
        /// <param name="property"></param>
        /// <returns></returns>
        /// <example>
        ///     <code lang="c#">
        /// <![CDATA[ 
        /// public class MenuCodeAndName   
        /// {
        ///    public string MenuCode { get; set; }
        ///    public string MenuName { get; set; }
        /// }
        /// 
        /// public class A{
        ///     public IEnumerable<MenuCodeAndName> Method1{
        ///         Func<Method1, string> property=m=> m.MenuCode; 
        ///         return items.DistinctBy(property);
        ///     }
        /// } 
        /// ]]>
        /// </code>
        /// </example>
        public static IEnumerable<TSource> DistinctBy<TSource, TKey>(this IEnumerable<TSource> items,
            Func<TSource, TKey> property)
        {
            var comparer = new GeneralPropertyComparer<TSource, TKey>(property);
            return items.Distinct(comparer);
        }
    }

    /// <summary>
    ///     Impl <see cref="IEqualityComparer" /> ,can be use in
    ///     <see cref="Enumerable.Distinct{TSource}(IEnumerable{TSource}, IEqualityComparer{TSource}) " />
    ///     or just use
    ///     <see cref="EnumerableExtensions.DistinctBy{TSource, TKey}(IEnumerable{TSource}, Func{TSource, TKey})" /> also use
    ///     this method.
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    /// <typeparam name="TKey"></typeparam>
    /// <example>
    ///     <code lang="c#">
    /// <![CDATA[ 
    /// public class MenuCodeAndName   
    /// {
    ///    public string MenuCode { get; set; }
    ///    public string MenuName { get; set; }
    /// }
    /// 
    /// public class A{
    ///     public IEnumerable<MenuCodeAndName> Method1{
    ///         Func<Method1, string> property=m=> m.MenuCode;
    ///         var comparer = new GeneralPropertyComparer<Method1, string>(property);
    ///         return items.Distinct(comparer);
    ///     }
    /// } 
    /// ]]>
    /// </code>
    /// </example>
    public class GeneralPropertyComparer<TSource, TKey> : IEqualityComparer<TSource>
    {
        /// <summary>
        /// </summary>
        /// <param name="expr"></param>
        public GeneralPropertyComparer(Func<TSource, TKey> expr)
        {
            this.expr = expr;
        }

        private Func<TSource, TKey> expr { get; }

        /// <summary>
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public bool Equals(TSource left, TSource right)
        {
            var leftProp = expr.Invoke(left);
            var rightProp = expr.Invoke(right);
            if (leftProp == null && rightProp == null)
                return true;
            if ((leftProp == null) ^ (rightProp == null)) //逻辑或位 XOR(异或)。 通常可以将此运算符与整数类型和 enum 类型一起使用
                return false;
            return leftProp.Equals(rightProp);
        }

        /// <summary>
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public int GetHashCode(TSource obj)
        {
            var prop = expr.Invoke(obj);
            return prop == null ? 0 : prop.GetHashCode();
        }
    }
}