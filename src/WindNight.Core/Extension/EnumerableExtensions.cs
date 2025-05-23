using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using WindNight.Core.Abstractions;

namespace WindNight.Linq.Extensions.Expressions
{
    /// <summary>
    /// </summary>
    public static class EnumerableExtensions
    {

        public static bool IsNullOrEmpty<T>(this IPagedList<T>? items)
        {
            return items == null || items.RecordCount == 0 || items.List.IsNullOrEmpty();
        }

        public static bool IsNullOrEmpty<T>(this IEnumerable<T>? items)
        {
            return items == null || !items.Any();
        }

        public static IEnumerable<T> EmptyArray<T>()
        {
#if NET45
            return new List<T>();
#else
            return Array.Empty<T>();
#endif
        }


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
        public static IEnumerable<TSource> DistinctByItem<TSource, TKey>(this IEnumerable<TSource> items,
            Func<TSource, TKey> property)
        {
            var comparer = new GeneralPropertyComparer<TSource, TKey>(property);
            return items.Distinct(comparer);
        }

        /// <summary>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="bag"></param>
        public static void Clear<T>(this ConcurrentBag<T> bag)
        {
#if NETSTANDARD2_0
            while (!bag.IsEmpty)
            {
                try
                {
                    bag.TryTake(out _);
                }
                catch (Exception ex)
                {
                }
            }
#else
            bag.Clear();
#endif

            //if (!bag.IsEmpty)
            //{
            //    bag.TryTake(out _);
            //}
        }

        /// <summary>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="bag"></param>
        /// <param name="dataList"></param>
        public static void AddRange<T>(this ConcurrentBag<T> bag, IEnumerable<T> dataList)
        {
            foreach (var item in dataList)
            {
                bag.Add(item);
            }


        }

        /// <summary>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="bag"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static bool TryAdd<T>(this ConcurrentBag<T> bag, T data)
        {
            if (!bag.Contains(data))
            {
                try
                {
                    bag.Add(data);
                }
                catch (Exception ex)
                {
                    return false;
                }
            }

            return false;
        }

        public static List<T> ListToTree<T>(this List<T> list)
            where T : ITreeObject<T>, new()
        {
            if (list.IsNullOrEmpty()) return new List<T>();

            // list 去重
            var lookup = list.DistinctByItem(m => m.Id).ToDictionary(n => n.Id, n => n);

            var rootNodes = new List<T>();

            foreach (var node in list)
            {
                // if (node.ParentId > 0 && lookup.ContainsKey(node.ParentId))
                if (node.ParentId > 0 && lookup.TryGetValue(node.ParentId, out var parent))
                {
                    //add node to its parent
                    //T? parent = lookup[node.ParentId];

                    parent.Children.Add(node);
                }
                else
                {
                    rootNodes.Add(node);
                }
            }

            return rootNodes;
        }


        public static IEnumerable<int> ToIntList(this string s, char separator = ',', bool needDistinct = true)
        {
            if (s.IsNullOrEmpty()) return EmptyArray<int>();

            var list = s.Split(separator).Select(m => m.ToInt());

            // list 去重
            if (needDistinct)
            {
                list = list.DistinctByItem(m => m);
            }

            return list;
        }

        public static IEnumerable<long> ToLongList(this string s, char separator = ',', bool needDistinct = true)
        {
            if (s.IsNullOrEmpty()) return EmptyArray<long>();


            var list = s.Split(separator).Select(m => m.ToLong());
            // list 去重
            if (needDistinct)
            {
                list = list.DistinctByItem(m => m);
            }

            return list;
        }

        public static IEnumerable<string> StringToList(this string s, char separator = ',', bool needDistinct = true)
        {
            if (s.IsNullOrEmpty()) return EmptyArray<string>();

            var list = s.Split(separator);
            // list 去重
            if (needDistinct)
            {
                list = list.DistinctByItem(m => m).ToArray();
            }

            return list;
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
            // 解引用可能出现空引用。
            return leftProp.Equals(rightProp);
            // 解引用可能出现空引用。
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
