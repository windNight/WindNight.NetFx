using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Extension;
using RestSharp;
using WindNight.Core.Tools;

namespace WindNight.Extension
{
    public static partial class HttpHelper
    {
        /// <summary>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url"></param>
        /// <param name="headerDict"></param>
        /// <param name="warnMiSeconds"></param>
        /// <param name="timeOut">Timeout in milliseconds to be used for the request </param>
        /// <param name="convertFunc"></param>
        /// <returns></returns>
        public static T Get<T>(string url, Dictionary<string, string> headerDict = null,
            int warnMiSeconds = 200, int timeOut = 1000 * 60 * 20,
            Func<string, T> convertFunc = null,
            Func<IRestResponse, bool> errStatusFunc = null) //where T : new()
        {
            return TimeWatcherHelper.TimeWatcher(() =>
                {
                    var request = GenGetRequest(url, headerDict);

                    return ExecuteHttpClient(url, request, headerDict, timeOut, convertFunc, errStatusFunc);
                }, $"HttpGet({url})", warnMiSeconds: warnMiSeconds);
        }

        /// <summary>
        ///     HttpGet 同步请求
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="domain"></param>
        /// <param name="path"></param>
        /// <param name="queries"></param>
        /// <param name="headerDict"></param>
        /// <param name="warnMiSeconds">Timeout in milliseconds to be used for the request</param>
        /// <param name="timeOut"></param>
        /// <param name="convertFunc"></param>
        /// <returns></returns>
        public static T Get<T>(string domain, string path,
            Dictionary<string, object> queries,
            Dictionary<string, string> headerDict = null,
            int warnMiSeconds = 200,
            int timeOut = 1000 * 60 * 20,
            Func<string, T> convertFunc = null,
            Func<IRestResponse, bool> errStatusFunc = null) //where T : new()
        {
            return TimeWatcherHelper.TimeWatcher(() =>
                {
                    var request = GenGetRequest(path, headerDict, queries);

                    return ExecuteHttpClient(domain, request, headerDict, timeOut, convertFunc, errStatusFunc);
                }, $"HttpGet({domain}{path}) with params {queries.ToJsonStr()}", warnMiSeconds: warnMiSeconds);
        }


        public static T Get<T>(string domain, string path,
            object queries,
            Dictionary<string, string> headerDict = null,
            int warnMiSeconds = 200,
            int timeOut = 1000 * 60 * 20,
            Func<string, T> convertFunc = null,
            Func<IRestResponse, bool> errStatusFunc = null)
        {
            var queryDict = queries.GenQueryDict();
            return Get<T>(domain, path, queryDict, headerDict, warnMiSeconds, timeOut, errStatusFunc: errStatusFunc);
        }


        /// <summary>
        ///     HttpGet 异步请求
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="domain"></param>
        /// <param name="path"></param>
        /// <param name="queries"></param>
        /// <param name="headerDict"></param>
        /// <param name="warnMiSeconds"></param>
        /// <param name="timeOut">Timeout in milliseconds to be used for the request</param>
        /// <param name="token"></param>
        /// <param name="convertFunc"></param>
        /// <returns></returns>
        public static async Task<T> GetAsync<T>(string domain, string path,
            Dictionary<string, object> queries,
            Dictionary<string, string> headerDict = null,
            int warnMiSeconds = 200,
            int timeOut = 1000 * 60 * 20,
            Func<string, T> convertFunc = null,
            CancellationToken token = default,
            Func<IRestResponse, bool> errStatusFunc = null) //where T : new()
        {
            return await TimeWatcherHelper.TimeWatcher(async () =>
                {
                    var request = GenGetRequest(path, headerDict, queries);

                    return await ExecuteHttpClientAsync(domain, request, headerDict, convertFunc, token,
                        errStatusFunc: errStatusFunc);
                }, $"HttpGetAsync({domain}{path}) with params {queries.ToJsonStr()}", warnMiSeconds: warnMiSeconds);
        }


        public static async Task<T> GetAsync<T>(string domain, string path,
            object queries,
            Dictionary<string, string> headerDict = null,
            int warnMiSeconds = 200,
            int timeOut = 1000 * 60 * 20,
            Func<string, T> convertFunc = null,
            CancellationToken token = default,
            Func<IRestResponse, bool> errStatusFunc = null)
        {
            var queryDict = queries.GenQueryDict();
            return await GetAsync(domain, path, queryDict, headerDict, warnMiSeconds, timeOut, convertFunc, token,
                errStatusFunc);
        }
    }
}
