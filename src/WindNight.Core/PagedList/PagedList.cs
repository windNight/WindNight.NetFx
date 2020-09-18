﻿using System.Linq;

namespace System.Collections.Generic
{
    public class PagedListExtension
    {
        public static IPagedList<T> GeneratorPagedList<T>(int pageIndex, int pageSize, int indexFrom, int recordCount,
            int pageCount, IList<T> list)
        {
            return new PagedList<T>
            {
                PageIndex = pageIndex,
                PageSize = pageSize,
                IndexFrom = indexFrom,
                RecordCount = recordCount,
                PageCount = pageCount,
                List = list
            };
        }

        public static IPagedList<TResult> GeneratorPagedList<TSource, TResult>(IPagedList<TSource> source,
            Func<IEnumerable<TSource>, IEnumerable<TResult>> converter)
        {
            return new PagedList<TResult>
            {
                PageIndex = source.PageIndex,
                PageSize = source.PageSize,
                IndexFrom = source.IndexFrom,
                RecordCount = source.RecordCount,
                PageCount = source.PageCount,
                List = new List<TResult>(converter(source.List))
            };
        }
    }


    public class PagedList<T> : IPagedList<T>
    {
        internal PagedList(IEnumerable<T> source, int pageIndex, int pageSize, int indexFrom)
        {
            if (indexFrom > pageIndex)
                throw new ArgumentException(
                    $"indexFrom: {indexFrom} > pageIndex: {pageIndex}, must indexFrom <= pageIndex");

            if (source is IQueryable<T> querable)
            {
                PageIndex = pageIndex;
                PageSize = pageSize;
                IndexFrom = indexFrom;
                RecordCount = querable.Count();
                PageCount = (int) Math.Ceiling(RecordCount / (double) PageSize);

                List = querable.Skip((PageIndex - IndexFrom) * PageSize).Take(PageSize).ToList();
            }
            else
            {
                PageIndex = pageIndex;
                PageSize = pageSize;
                IndexFrom = indexFrom;
                RecordCount = source.Count();
                PageCount = (int) Math.Ceiling(RecordCount / (double) PageSize);

                List = source.Skip((PageIndex - IndexFrom) * PageSize).Take(PageSize).ToList();
            }
        }

        internal PagedList()
        {
            List = new T[0];
        }

        public int PageIndex { get; set; }

        public int PageSize { get; set; }

        public int RecordCount { get; set; }

        public int PageCount { get; set; }

        public int IndexFrom { get; set; }

        public IList<T> List { get; set; }

        public bool HasPreviousPage => PageIndex - IndexFrom > 0;

        public bool HasNextPage => PageIndex - IndexFrom + 1 < PageCount;
    }

    internal class PagedList<TSource, TResult> : IPagedList<TResult>
    {
        public PagedList(IEnumerable<TSource> source, Func<IEnumerable<TSource>, IEnumerable<TResult>> converter,
            int pageIndex, int pageSize, int indexFrom)
        {
            if (indexFrom > pageIndex)
                throw new ArgumentException(
                    $"indexFrom: {indexFrom} > pageIndex: {pageIndex}, must indexFrom <= pageIndex");

            if (source is IQueryable<TSource> querable)
            {
                PageIndex = pageIndex;
                PageSize = pageSize;
                IndexFrom = indexFrom;
                RecordCount = querable.Count();
                PageCount = (int) Math.Ceiling(RecordCount / (double) PageSize);

                var items = querable.Skip((PageIndex - IndexFrom) * PageSize).Take(PageSize).ToArray();

                List = new List<TResult>(converter(items));
            }
            else
            {
                PageIndex = pageIndex;
                PageSize = pageSize;
                IndexFrom = indexFrom;
                RecordCount = source.Count();
                PageCount = (int) Math.Ceiling(RecordCount / (double) PageSize);

                var items = source.Skip((PageIndex - IndexFrom) * PageSize).Take(PageSize).ToArray();

                List = new List<TResult>(converter(items));
            }
        }

        public PagedList(IPagedList<TSource> source, Func<IEnumerable<TSource>, IEnumerable<TResult>> converter)
        {
            PageIndex = source.PageIndex;
            PageSize = source.PageSize;
            IndexFrom = source.IndexFrom;
            RecordCount = source.RecordCount;
            PageCount = source.PageCount;

            List = new List<TResult>(converter(source.List));
        }

        public int PageIndex { get; }

        public int PageSize { get; }

        public int RecordCount { get; }

        public int PageCount { get; }

        public int IndexFrom { get; }

        public IList<TResult> List { get; }

        public bool HasPreviousPage => PageIndex - IndexFrom > 0;

        public bool HasNextPage => PageIndex - IndexFrom + 1 < PageCount;
    }

    public static class PagedList
    {
        public static IPagedList<T> Empty<T>()
        {
            return new PagedList<T>();
        }

        public static IPagedList<TResult> From<TResult, TSource>(IPagedList<TSource> source,
            Func<IEnumerable<TSource>, IEnumerable<TResult>> converter)
        {
            return new PagedList<TSource, TResult>(source, converter);
        }
    }
}