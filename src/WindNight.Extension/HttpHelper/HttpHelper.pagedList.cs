using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Extension;
using RestSharp;
using WindNight.Core.Tools;
using WindNight.Linq.Extensions.Expressions;

namespace WindNight.Extension
{
    public static partial class HttpHelper
    {
        public static IPagedList<T> GetPagedList<T>(string url, Dictionary<string, string> headerDict = null,
            int warnMiSeconds = 200, int timeOut = 1000 * 60 * 20,
            Func<IRestResponse, bool> errStatusFunc = null) //where T : new()
        {
            return TimeWatcherHelper.TimeWatcher(() =>
                {
                    var request = GenGetRequest(url, headerDict);

                    return ExecuteHttpClient2<T>(url, request, headerDict, timeOut, errStatusFunc);
                }, $"HttpGet({url})", warnMiSeconds: warnMiSeconds);
        }

        public static async Task<IPagedList<T>> GetPagedListAsync<T>(string domain, string path,
            Dictionary<string, object> queries,
            Dictionary<string, string> headerDict = null,
            int warnMiSeconds = 200, int timeOut = 1000 * 60 * 20,
            CancellationToken token = default,
            Func<IRestResponse, bool> errStatusFunc = null) //where T : new()
        {
            return await TimeWatcherHelper.TimeWatcher(async () =>
                {
                    var request = GenGetRequest(path, headerDict, queries);

                    return await ExecuteHttpClientAsync2<T>(domain, request, headerDict, token, timeOut, errStatusFunc);
                },
                $"GetPagedListAsync({domain}{path}) with params {queries.ToJsonStr()}",
                warnMiSeconds: warnMiSeconds);
        }

        public static async Task<IPagedList<T>> GetPagedListAsync<T>(string domain, string path,
            object queries,
            Dictionary<string, string> headerDict = null,
            int warnMiSeconds = 200,
            int timeOut = 1000 * 60 * 20, Func<IRestResponse, bool> errStatusFunc = null)
        {
            var queryDict = queries.GenQueryDict();
            return await GetPagedListAsync<T>(domain, path, queryDict, headerDict, warnMiSeconds, timeOut,
                errStatusFunc: errStatusFunc);
        }


        public static IPagedList<T> GetPagedList<T>(string domain, string path,
            Dictionary<string, object> queries,
            Dictionary<string, string> headerDict = null,
            int warnMiSeconds = 200,
            int timeOut = 1000 * 60 * 20, Func<IRestResponse, bool> errStatusFunc = null) //where T : new()
        {
            return TimeWatcherHelper.TimeWatcher(() =>
                {
                    var request = GenGetRequest(path, headerDict, queries);

                    return ExecuteHttpClient2<T>(domain, request, headerDict, errStatusFunc: errStatusFunc);
                }, $"GetList({domain}{path}) with params {queries.ToJsonStr()}", warnMiSeconds: warnMiSeconds);
        }

        public static IPagedList<T> GetPagedList<T>(string domain, string path,
            object queries,
            Dictionary<string, string> headerDict = null,
            int warnMiSeconds = 200, int timeOut = 1000 * 60 * 20,
            Func<IRestResponse, bool> errStatusFunc = null)
        {
            var queryDict = queries.GenQueryDict();
            return GetPagedList<T>(domain, path, queryDict, headerDict, warnMiSeconds, timeOut, errStatusFunc);
        }


        public static IPagedList<T> PostPagedList<T>(string domain, string path, object bodyObjects,
            Dictionary<string, string> headerDict = null, int warnMiSeconds = 200,
            int timeOut = 1000 * 60 * 20, bool isJsonBody = true,
            Func<IRestResponse, bool> errStatusFunc = null) //where T : new()
        {
            return TimeWatcherHelper.TimeWatcher(() =>
                {
                    var request = GenPostRequest(path, headerDict, bodyObjects, isJsonBody);

                    return ExecuteHttpClient2<T>(domain, request, headerDict, timeOut, errStatusFunc);
                },
                $"PostPageList({domain}{path}) with params={bodyObjects.ToJsonStr()} , header={headerDict?.ToJsonStr()}",
                warnMiSeconds: warnMiSeconds);
        }


        public static async Task<IPagedList<T>> PostPagedListAsync<T>(string domain, string path, object bodyObjects,
            Dictionary<string, string> headerDict = null, int warnMiSeconds = 200,
            int timeOut = 1000 * 60 * 20,
            CancellationToken token = default, bool isJsonBody = true,
            Func<IRestResponse, bool> errStatusFunc = null) //where T : new()
        {
            return await TimeWatcherHelper.TimeWatcher(async () =>
                {
                    var request = GenPostRequest(path, headerDict, bodyObjects, isJsonBody);


                    return await ExecuteHttpClientAsync2<T>(domain, request, headerDict, token, timeOut, errStatusFunc);
                },
                $"PostPageListAsync({domain}{path}) with params={bodyObjects.ToJsonStr()} , header={headerDict?.ToJsonStr()}",
                warnMiSeconds: warnMiSeconds);
        }
    }

    internal class PagedList<T> : IPagedList<T>
    {
        public PagedList()
        {
            List = new List<T>(); // Array.Empty<T>();
        }

        public int PageIndex { get; set; }

        public int PageSize { get; set; }

        public int RecordCount { get; set; }

        public int PageCount { get; set; }

        public int IndexFrom { get; set; }

        public IList<T> List { get; set; }

        public bool HasPreviousPage => PageIndex - IndexFrom > 0;

        public bool HasNextPage => PageIndex - IndexFrom + 1 < PageCount;
        public bool IsEmpty()
        {
            return this.IsNullOrEmpty();
        }
    }
}
