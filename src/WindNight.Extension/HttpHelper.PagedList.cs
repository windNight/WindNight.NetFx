using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection.WnExtension;
using Newtonsoft.Json.Extension;
using RestSharp;
using RestSharp.Extensions;
using WindNight.Core;
using WindNight.Core.Abstractions;
using WindNight.Core.Tools;
using WindNight.Extension.Internals;
//using WindNight.NetCore.Extension;

namespace WindNight.Extension
{
    public static partial class HttpHelper
    {

        public static IPagedList<T> GetPagedList<T>(string url, Dictionary<string, string> headerDict = null, int warnMiSeconds = 200, int timeOut = 1000 * 60 * 20) //where T : new()
        {
            return TimeWatcherHelper.TimeWatcher(() =>
                {
                    var request = new RestRequest(Method.GET);
                    headerDict = GeneratorHeaderDict(headerDict);

                    foreach (var header in headerDict) request.AddHeader(header.Key, header.Value);
                    return ExecuteHttpClient2<T>(url, request, timeOut);
                }, $"HttpGet({url})", warnMiSeconds: warnMiSeconds);
        }

        public static async Task<IPagedList<T>> GetPagedListAsync<T>(string domain, string path,
            Dictionary<string, object> queries,
            Dictionary<string, string> headerDict = null,
            int warnMiSeconds = 200, int timeOut = 1000 * 60 * 20) //where T : new()
        {
            return await TimeWatcherHelper.TimeWatcher(async () =>
                {
                    var request = new RestRequest(path, Method.GET);

                    headerDict = GeneratorHeaderDict(headerDict);

                    foreach (var header in headerDict) request.AddHeader(header.Key, header.Value);
                    if (queries != null)
                        foreach (var query in queries)
                            request.AddParameter(query.Key, query.Value);

                    return await ExecuteHttpClientAsync2<T>(domain, request, timeOut: timeOut);
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
                    var request = new RestRequest(path, Method.GET);

                    headerDict = GeneratorHeaderDict(headerDict);

                    foreach (var header in headerDict) request.AddHeader(header.Key, header.Value);
                    if (queries != null)
                        foreach (var query in queries)
                            request.AddParameter(query.Key, query.Value);

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
            Dictionary<string, string> headerDict = null, int warnMiSeconds = 200, int timeOut = 1000 * 60 * 20) //where T : new()
        {
            return TimeWatcherHelper.TimeWatcher(() =>
                {
                    var request = new RestRequest(path, Method.POST);

                    headerDict = GeneratorHeaderDict(headerDict);

                    foreach (var header in headerDict) request.AddHeader(header.Key, header.Value);

                    request.AddJsonBody(bodyObjects);

                    return ExecuteHttpClient2<T>(domain, request, timeOut: timeOut);
                },
                $"PostPageList({domain}{path}) with params={bodyObjects.ToJsonStr()} , header={headerDict?.ToJsonStr()}",
                warnMiSeconds: warnMiSeconds);
        }


        public static async Task<IPagedList<T>> PostPagedListAsync<T>(string domain, string path, object bodyObjects,
            Dictionary<string, string> headerDict = null, int warnMiSeconds = 200, int timeOut = 1000 * 60 * 20) //where T : new()
        {
            return await TimeWatcherHelper.TimeWatcher(async () =>
                {
                    var request = new RestRequest(path, Method.POST);

                    headerDict = GeneratorHeaderDict(headerDict);

                    foreach (var header in headerDict) request.AddHeader(header.Key, header.Value);

                    request.AddJsonBody(bodyObjects);

                    return await ExecuteHttpClientAsync2<T>(domain, request, timeOut: timeOut);
                },
                $"PostPageListAsync({domain}{path}) with params={bodyObjects.ToJsonStr()} , header={headerDict?.ToJsonStr()}",
                warnMiSeconds: warnMiSeconds);
        }



        private static IPagedList<T> DeserializeResponse2PageList<T>(IRestResponse response)
        {
            if (response.StatusCode == HttpStatusCode.OK)
            {
                if (ConfigItems.DebugIsOpen)
                    LogHelper.Debug($" response.Content is {response.Content} ", appendMessage: false);
                var res = response.Content.To<ResponseResult<PagedList<T>>>();
                return res.Data;
            }

            LogHelper.Warn($" ResponseStatus is {response.ResponseStatus} ", appendMessage: false);
            return default;
        }

        private static IPagedList<T> ExecuteHttpClient2<T>(string domain, IRestRequest request, int timeOut = 1000 * 60 * 20)
        {
            var client = new RestClient(domain)
            {
                Proxy = null,
                Timeout = timeOut,
            };
            var response = client.Execute(request);
            return DeserializeResponse2PageList<T>(response);
        }

        private static async Task<IPagedList<T>> ExecuteHttpClientAsync2<T>(string domain, IRestRequest request, CancellationToken token = default(CancellationToken), int timeOut = 1000 * 60 * 20)
        {
            var client = new RestClient(domain)
            {
                Proxy = null,
                Timeout = timeOut,// 1000 * 60 * 20
            };
#if  NET45
            var response = await client.ExecuteTaskAsync(request);
#else
            var response = await client.ExecuteAsync(request, token);
#endif
            return DeserializeResponse2PageList<T>(response);


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
