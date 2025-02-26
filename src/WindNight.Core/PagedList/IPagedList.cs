namespace System.Collections.Generic
{
    /// <summary>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IPagedList<T>
    {
        int IndexFrom { get; }

        int PageIndex { get; }

        int PageSize { get; }

        int RecordCount { get; }

        int PageCount { get; }

        IList<T> List { get; }

        bool HasPreviousPage { get; }

        bool HasNextPage { get; }
    }
}
