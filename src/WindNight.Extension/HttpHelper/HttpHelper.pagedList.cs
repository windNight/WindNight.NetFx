using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Extension;
using WindNight.Core.Tools;

namespace WindNight.Extension
{
    public static partial class HttpHelper
    {

        public static IPagedList<T> GetPagedList<T>(string url, Dictionary<string, string> headerDict = null, int warnMiSeconds = 200, int timeOut = 1000 * 60 * 20) //where T : new()
        {
            return TimeWatcherHelper.TimeWatcher(() =>
                {
                    var request = GenGetRequest(url, headerDict);
                    //var request = new RestRequest(Method.GET);
                    //headerDict = GeneratorHeaderDict(headerDict);

                    //foreach (var header in headerDict) request.AddHeader(header.Key, header.Value);
                    return ExecuteHttpClient2<T>(url, request, timeOut);
                }, $"HttpGet({url})", warnMiSeconds: warnMiSeconds);
        }

        public static async Task<IPagedList<T>> GetPagedListAsync<T>(string domain, string path,
            Dictionary<string, object> queries,
            Dictionary<string, string> headerDict = null,
            int warnMiSeconds = 200, int timeOut = 1000 * 60 * 20, CancellationToken token = default(CancellationToken)) //where T : new()
        {
            return await TimeWatcherHelper.TimeWatcher(async () =>
                {
                    var request = GenGetRequest(path, headerDict, queries);
                    //var request = new RestRequest(path, Method.GET);

                    //headerDict = GeneratorHeaderDict(headerDict);

                    //foreach (var header in headerDict) request.AddHeader(header.Key, header.Value);
                    //if (queries != null)
                    //    foreach (var query in queries)
                    //        request.AddParameter(query.Key, query.Value);

                    return await ExecuteHttpClientAsync2<T>(domain, request, timeOut: timeOut, token: token);
                },
                $"GetListAsync({domain}{path}) with params {queries.ToJsonStr()}",
                warnMiSeconds: warnMiSeconds);

        }

        public static async Task<IPagedList<T>> GetPagedListAsync<T>(string domain, string path,
            object queries,
            Dictionary<string, string> headerDict = null,
            int warnMiSeconds = 200, int timeOut = 1000 * 60 * 20)
        {
            var queryDict = queries.GenQueryDict();
            return await GetPagedListAsync<T>(domain, path, queryDict, headerDict, warnMiSeconds, timeOut);
        }


        public static IPagedList<T> GetPagedList<T>(string domain, string path,
            Dictionary<string, object> queries,
            Dictionary<string, string> headerDict = null,
            int warnMiSeconds = 200, int timeOut = 1000 * 60 * 20) //where T : new()
        {
            return TimeWatcherHelper.TimeWatcher(() =>
                {
                    var request = GenGetRequest(path, headerDict, queries);
                    //var request = new RestRequest(path, Method.GET);

                    //headerDict = GeneratorHeaderDict(headerDict);

                    //foreach (var header in headerDict) request.AddHeader(header.Key, header.Value);
                    //if (queries != null)
                    //    foreach (var query in queries)
                    //        request.AddParameter(query.Key, query.Value);

                    return ExecuteHttpClient2<T>(domain, request);
                }, $"GetList({domain}{path}) with params {queries.ToJsonStr()}", warnMiSeconds: warnMiSeconds);
        }

        public static IPagedList<T> GetPagedList<T>(string domain, string path,
           object queries,
            Dictionary<string, string> headerDict = null,
            int warnMiSeconds = 200, int timeOut = 1000 * 60 * 20)
        {
            var queryDict = queries.GenQueryDict();
            return GetPagedList<T>(domain, path, queryDict, headerDict, warnMiSeconds, timeOut);
        }


        public static IPagedList<T> PostPagedList<T>(string domain, string path, object bodyObjects,
            Dictionary<string, string> headerDict = null, int warnMiSeconds = 200,
            int timeOut = 1000 * 60 * 20, bool isJsonBody = true) //where T : new()
        {
            return TimeWatcherHelper.TimeWatcher(() =>
                {
                    var request = GenPostRequest(path, headerDict, bodyObjects, isJsonBody);
                    //var request = new RestRequest(path, Method.POST);

                    //headerDict = GeneratorHeaderDict(headerDict);

                    //foreach (var header in headerDict) request.AddHeader(header.Key, header.Value);

                    //request.AddJsonBody(bodyObjects);

                    return ExecuteHttpClient2<T>(domain, request, timeOut: timeOut);
                },
                $"PostPageList({domain}{path}) with params={bodyObjects.ToJsonStr()} , header={headerDict?.ToJsonStr()}",
                warnMiSeconds: warnMiSeconds);
        }


        public static async Task<IPagedList<T>> PostPagedListAsync<T>(string domain, string path, object bodyObjects,
            Dictionary<string, string> headerDict = null, int warnMiSeconds = 200,
            int timeOut = 1000 * 60 * 20, CancellationToken token = default(CancellationToken), bool isJsonBody = true) //where T : new()
        {
            return await TimeWatcherHelper.TimeWatcher(async () =>
                {
                    var request = GenPostRequest(path, headerDict, bodyObjects, isJsonBody);
                    //var request = new RestRequest(path, Method.POST);

                    //headerDict = GeneratorHeaderDict(headerDict);

                    //foreach (var header in headerDict) request.AddHeader(header.Key, header.Value);

                    //request.AddJsonBody(bodyObjects);

                    return await ExecuteHttpClientAsync2<T>(domain, request, timeOut: timeOut, token: token);
                },
                $"PostPageListAsync({domain}{path}) with params={bodyObjects.ToJsonStr()} , header={headerDict?.ToJsonStr()}",
                warnMiSeconds: warnMiSeconds);
        }






    }

    internal class PagedList<T> : IPagedList<T>
    {
        public PagedList()
        {
            List = new List<T>();// Array.Empty<T>();
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


}
