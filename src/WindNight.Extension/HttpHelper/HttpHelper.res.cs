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
        public static T GetResponse<T>(string domain, string path,
            Dictionary<string, object> queries,
            Dictionary<string, string> headerDict = null,
            int warnMiSeconds = 200, int timeOut = 1000 * 60 * 20,
            Func<IRestResponse, bool> errStatusFunc = null) //where T : new()
        {
            return TimeWatcherHelper.TimeWatcher(() =>
                {
                    var request = GenGetRequest(path, headerDict, queries);

                    return ExecuteHttpClient3<T>(domain, request, headerDict, timeOut, errStatusFunc);
                }, $"HttpGet({domain}{path}) with params {queries.ToJsonStr()}", warnMiSeconds: warnMiSeconds);
        }


        public static T GetResponse<T>(string domain, string path,
            object queries,
            Dictionary<string, string> headerDict = null,
            int warnMiSeconds = 200, int timeOut = 1000 * 60 * 20, Func<IRestResponse, bool> errStatusFunc = null)
        {
            var queryDict = queries.GenQueryDict();
            return GetResponse<T>(domain, path, queryDict, headerDict, warnMiSeconds, timeOut, errStatusFunc);
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
        public static async Task<T> GetResponseAsync<T>(string domain, string path,
            Dictionary<string, object> queries,
            Dictionary<string, string> headerDict = null,
            int warnMiSeconds = 200, int timeOut = 1000 * 60 * 20,
            CancellationToken token = default, Func<IRestResponse, bool> errStatusFunc = null) //where T : new()
        {
            return await TimeWatcherHelper.TimeWatcher(async () =>
                {
                    var request = GenGetRequest(path, headerDict, queries);

                    return await ExecuteHttpClientAsync3<T>(domain, request, headerDict, token,
                        errStatusFunc: errStatusFunc);
                }, $"HttpGetAsync({domain}{path}) with params {queries.ToJsonStr()}", warnMiSeconds: warnMiSeconds);
        }


        public static async Task<T> GetResponseAsync<T>(string domain, string path,
            object queries,
            Dictionary<string, string> headerDict = null,
            int warnMiSeconds = 200, int timeOut = 1000 * 60 * 20,
            CancellationToken token = default, Func<IRestResponse, bool> errStatusFunc = null)
        {
            var queryDict = queries.GenQueryDict();
            return await GetResponseAsync<T>(domain, path, queryDict, headerDict, warnMiSeconds, timeOut, token,
                errStatusFunc);
        }


        /// <summary>
        ///     HttpPost 同步请求
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="domain"></param>
        /// <param name="path"></param>
        /// <param name="bodyObjects"></param>
        /// <param name="headerDict"></param>
        /// <param name="warnMiSeconds"></param>
        /// <param name="timeOut">Timeout in milliseconds to be used for the request</param>
        /// <param name="isJsonBody"></param>
        /// <returns></returns>
        public static T PostResponse<T>(string domain, string path, object bodyObjects,
            Dictionary<string, string> headerDict = null, int warnMiSeconds = 200,
            int timeOut = 1000 * 60 * 20, bool isJsonBody = true,
            Func<IRestResponse, bool> errStatusFunc = null) //where T : new()
        {
            return TimeWatcherHelper.TimeWatcher(() =>
                {
                    var request = GenPostRequest(path, headerDict, bodyObjects, isJsonBody);

                    return ExecuteHttpClient3<T>(domain, request, headerDict, timeOut, errStatusFunc);
                }, $"HttpPost({domain}{path}) with params={bodyObjects.ToJsonStr()} , header={headerDict?.ToJsonStr()}",
                warnMiSeconds: warnMiSeconds);
        }

        /// <summary>
        ///     HttpPost 同步请求
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="domain"></param>
        /// <param name="path"></param>
        /// <param name="bodyObjects"></param>
        /// <param name="headerDict"></param>
        /// <param name="warnMiSeconds"></param>
        /// <param name="timeOut">Timeout in milliseconds to be used for the request</param>
        /// <param name="token"></param>
        /// <param name="isJsonBody"></param>
        /// <returns></returns>
        public static async Task<T> PostResponseAsync<T>(string domain, string path, object bodyObjects,
            Dictionary<string, string> headerDict = null, int warnMiSeconds = 200,
            int timeOut = 1000 * 60 * 20,
            CancellationToken token = default,
            bool isJsonBody = true, Func<IRestResponse, bool> errStatusFunc = null) //where T : new()
        {
            return await TimeWatcherHelper.TimeWatcher(async () =>
                {
                    var request = GenPostRequest(path, headerDict, bodyObjects, isJsonBody);

                    return await ExecuteHttpClientAsync3<T>(domain, request, headerDict, token, timeOut, errStatusFunc);
                },
                $"HttpPostAsync({domain}{path}) with params={bodyObjects.ToJsonStr()} , header={headerDict?.ToJsonStr()}",
                warnMiSeconds: warnMiSeconds);
        }
    }
}
