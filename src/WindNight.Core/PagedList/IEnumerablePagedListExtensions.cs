using WindNight.Core.SQL.Abstractions;

namespace System.Collections.Generic
{
    /// <summary>
    /// </summary>
    public static class IEnumerablePagedListExtensions
    {
        /// <summary>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="pagedInfo"></param>
        /// <returns></returns>
        public static IPagedList<T> ToPagedList<T>(this IEnumerable<T> source, IQueryPageBase pagedInfo)
        {
            return new PagedList<T>(source, pagedInfo);
        }

        public static IPagedList<TResult> ToPagedList<TSource, TResult>(this IEnumerable<TSource> source,
            Func<IEnumerable<TSource>, IEnumerable<TResult>> converter, IQueryPageBase pagedInfo)
        {
            return new PagedList<TSource, TResult>(source, converter, pagedInfo);
        }

        /// <summary>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="indexFrom"></param>
        /// <returns></returns>
        public static IPagedList<T> ToPagedList<T>(this IEnumerable<T> source, int pageIndex, int pageSize,
            int indexFrom = 1)
        {
            return new PagedList<T>(source, pageIndex, pageSize, indexFrom);
        }

        /// <summary>
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="source"></param>
        /// <param name="converter"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="indexFrom"></param>
        /// <returns></returns>
        public static IPagedList<TResult> ToPagedList<TSource, TResult>(this IEnumerable<TSource> source,
            Func<IEnumerable<TSource>, IEnumerable<TResult>> converter, int pageIndex, int pageSize, int indexFrom = 1)
        {
            return new PagedList<TSource, TResult>(source, converter, pageIndex, pageSize, indexFrom);
        }
    }
}
