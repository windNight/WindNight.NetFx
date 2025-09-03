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
        ///     HttpPut 同步请求
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="domain"></param>
        /// <param name="path"></param>
        /// <param name="bodyObjects"></param>
        /// <param name="headerDict"></param>
        /// <param name="warnMiSeconds"></param>
        /// <param name="timeOut">Timeout in milliseconds to be used for the request</param>
        /// <param name="convertFunc"></param>
        /// <param name="isJsonBody"></param>
        /// <returns></returns>
        public static T Put<T>(string domain, string path, object bodyObjects,
            Dictionary<string, string> headerDict = null, int warnMiSeconds = 200,
            int timeOut = 1000 * 60 * 20,
            Func<string, T> convertFunc = null, bool isJsonBody = true,
            Func<IRestResponse, bool> errStatusFunc = null) //where T : new()
        {
            return TimeWatcherHelper.TimeWatcher(() =>
                {
                    var request = GenPutRequest(path, headerDict, bodyObjects, isJsonBody);

                    return ExecuteHttpClient(domain, request, headerDict, timeOut, convertFunc, errStatusFunc);
                }, $"HttpPut({domain}{path}) with params={bodyObjects.ToJsonStr()} , header={headerDict?.ToJsonStr()}",
                warnMiSeconds: warnMiSeconds);
        }

        /// <summary>
        ///     HttpPut 同步请求
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="domain"></param>
        /// <param name="path"></param>
        /// <param name="bodyObjects"></param>
        /// <param name="headerDict"></param>
        /// <param name="warnMiSeconds"></param>
        /// <param name="timeOut">Timeout in milliseconds to be used for the request</param>
        /// <param name="convertFunc"></param>
        /// <param name="token"></param>
        /// <param name="isJsonBody"></param>
        /// <returns></returns>
        public static async Task<T> PutAsync<T>(string domain, string path, object bodyObjects,
            Dictionary<string, string> headerDict = null, int warnMiSeconds = 200,
            int timeOut = 1000 * 60 * 20, Func<string, T> convertFunc = null,
            CancellationToken token = default,
            bool isJsonBody = true, Func<IRestResponse, bool> errStatusFunc = null) //where T : new()
        {
            return await TimeWatcherHelper.TimeWatcher(async () =>
                {
                    var request = GenPutRequest(path, headerDict, bodyObjects, isJsonBody);


                    return await ExecuteHttpClientAsync(domain, request, timeOut: timeOut, token: token,
                        convertFunc: convertFunc, errStatusFunc: errStatusFunc);
                },
                $"HttpPutAsync({domain}{path}) with params={bodyObjects.ToJsonStr()} , header={headerDict?.ToJsonStr()}",
                warnMiSeconds: warnMiSeconds);
        }




    }
}
