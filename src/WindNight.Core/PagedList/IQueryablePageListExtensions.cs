using System.Linq;
using System.Threading;
using WindNight.Core.SQL.Abstractions;

namespace System.Collections.Generic
{
    /// <summary>
    /// </summary>
    public static class IQueryablePageListExtensions
    {
        /// <summary>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="indexFrom"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static IPagedList<T> ToPagedList<T>(this IQueryable<T> source, int pageIndex, int pageSize,
            int indexFrom = 0, CancellationToken cancellationToken = default)
        {
            if (indexFrom > pageIndex)
                throw new ArgumentException(
                    $"indexFrom: {indexFrom} > pageIndex: {pageIndex}, must indexFrom <= pageIndex");

            var count = source.Count();
            var items = source.Skip((pageIndex - indexFrom) * pageSize)
                .Take(pageSize).ToList();

            var pagedList = new PagedList<T>
            {
                PageIndex = pageIndex,
                PageSize = pageSize,
                IndexFrom = indexFrom,
                RecordCount = count,
                List = items,
                PageCount = (int)Math.Ceiling(count / (double)pageSize),
            };

            return pagedList;
        }


        /// <summary>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="pagedInfo"></param>
        /// <param name="indexFrom"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static IPagedList<T> ToPagedList<T>(this IQueryable<T> source, IQueryPageBase pagedInfo,
            int indexFrom = 0, CancellationToken cancellationToken = default)
        {
            var pageIndex = pagedInfo.PageIndex;
            var pageSize = pagedInfo.PageSize;

            if (indexFrom > pageIndex)
                throw new ArgumentException(
                    $"indexFrom: {indexFrom} > pageIndex: {pageIndex}, must indexFrom <= pageIndex");

            var count = source.Count();
            var items = source.Skip((pageIndex - indexFrom) * pageSize)
                .Take(pageSize).ToList();

            var pagedList = new PagedList<T>
            {
                PageIndex = pageIndex,
                PageSize = pageSize,
                IndexFrom = indexFrom,
                RecordCount = count,
                List = items,
                PageCount = (int)Math.Ceiling(count / (double)pageSize),
            };

            return pagedList;
        }
    }
}
