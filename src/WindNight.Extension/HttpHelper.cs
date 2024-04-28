using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Policy;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection.WnExtension;
using Newtonsoft.Json.Extension;
using Newtonsoft.Json.Linq;
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
        class ConfigItems// : ConfigItemsBase
        {
            public static bool DebugIsOpen
            {
                get
                {
                    var config = Ioc.Instance.CurrentConfigService;// Ioc.GetService<IConfigService>();
                    if (config == null) return false;
                    return config.GetAppSetting("DebugIsOpen", false, false);
                }
            }

        }


        /// <summary>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url"></param>
        /// <param name="headerDict"></param>
        /// <param name="warnMiSeconds"></param>
        /// <param name="timeOut">Timeout in milliseconds to be used for the request </param>
        /// <returns></returns>
        public static T Get<T>(string url, Dictionary<string, string> headerDict = null, int warnMiSeconds = 200, int timeOut = 1000 * 60 * 20) //where T : new()
        {
            return TimeWatcherHelper.TimeWatcher(() =>
            {
                var request = new RestRequest(Method.GET);
                headerDict = GeneratorHeaderDict(headerDict);

                foreach (var header in headerDict) request.AddHeader(header.Key, header.Value);
                return ExecuteHttpClient<T>(url, request, timeOut);
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
        /// <returns></returns>
        public static T Get<T>(string domain, string path,
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

                return ExecuteHttpClient<T>(domain, request, timeOut);
            }, $"HttpGet({domain}{path}) with params {queries.ToJsonStr()}", warnMiSeconds: warnMiSeconds);
        }

        /// <summary>
        ///     HttpGet 同步请求
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="domain"></param>
        /// <param name="path"></param>
        /// <param name="queries"></param>
        /// <param name="headerDict"></param>
        /// <param name="warnMiSeconds"></param>
        /// <param name="timeOut">Timeout in milliseconds to be used for the request</param>
        /// <returns></returns>
        public static async Task<T> GetAsync<T>(string domain, string path,
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

                return await ExecuteHttpClientAsync<T>(domain, request);
            }, $"HttpGetAsync({domain}{path}) with params {queries.ToJsonStr()}", warnMiSeconds: warnMiSeconds);
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
        /// <returns></returns>
        public static T Post<T>(string domain, string path, object bodyObjects,
            Dictionary<string, string> headerDict = null, int warnMiSeconds = 200, int timeOut = 1000 * 60 * 20) //where T : new()
        {
            return TimeWatcherHelper.TimeWatcher(() =>
            {
                var request = new RestRequest(path, Method.POST);

                headerDict = GeneratorHeaderDict(headerDict);

                foreach (var header in headerDict) request.AddHeader(header.Key, header.Value);

                request.AddJsonBody(bodyObjects);

                return ExecuteHttpClient<T>(domain, request, timeOut);
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
        /// <returns></returns>
        public static async Task<T> PostAsync<T>(string domain, string path, object bodyObjects,
            Dictionary<string, string> headerDict = null, int warnMiSeconds = 200, int timeOut = 1000 * 60 * 20) //where T : new()
        {
            return await TimeWatcherHelper.TimeWatcher(async () =>
            {
                var request = new RestRequest(path, Method.POST);

                headerDict = GeneratorHeaderDict(headerDict);

                foreach (var header in headerDict) request.AddHeader(header.Key, header.Value);

                request.AddJsonBody(bodyObjects);

                return await ExecuteHttpClientAsync<T>(domain, request, timeOut: timeOut);
            },
                $"HttpPostAsync({domain}{path}) with params={bodyObjects.ToJsonStr()} , header={headerDict?.ToJsonStr()}",
                warnMiSeconds: warnMiSeconds);
        }

        static RestRequest GenHeadRequest(string url, Dictionary<string, string> headerDict = null)
        {
            var request = new RestRequest(url, Method.HEAD);
            request.AddHeader("Accept", "*/*");
            headerDict = GeneratorHeaderDict(headerDict);

            foreach (var header in headerDict) request.AddHeader(header.Key, header.Value);

            return request;
        }

        static RestRequest GenDownloadRequest(string url, Dictionary<string, string> headerDict = null)
        {
            var request = new RestRequest(url, Method.GET);
            request.AddHeader("Accept", "*/*");
            headerDict = GeneratorHeaderDict(headerDict);

            foreach (var header in headerDict) request.AddHeader(header.Key, header.Value);
            request.AlwaysMultipartFormData = true;
            return request;
        }

        public static bool CheckRemoteFile(string url, Dictionary<string, string> headerDict = null, int warnMiSeconds = 200, int timeOut = 1000 * 60 * 20)
        {
            return TimeWatcherHelper.TimeWatcher(() =>
                {
                    var request = GenHeadRequest(url, headerDict);

                    var client = new RestClient(url)
                    {
                        Proxy = null,
                        Timeout = timeOut,// 1000 * 60 * 20
                    };

                    var response = client.Execute(request);
                    var isOk = response.StatusCode == HttpStatusCode.OK;
                    return isOk;
                },
                $"HttpHead({url})   , header={headerDict?.ToJsonStr()}",
                warnMiSeconds: warnMiSeconds);
        }


        public static async Task<bool> CheckRemoteFileAsync(string url, Dictionary<string, string> headerDict = null, int warnMiSeconds = 200, int timeOut = 1000 * 60 * 20)
        {
            return await TimeWatcherHelper.TimeWatcher(async () =>
                {
                    var request = GenHeadRequest(url, headerDict);

                    var client = new RestClient(url)
                    {
                        Proxy = null,
                        Timeout = timeOut,// 1000 * 60 * 20
                    };

#if NET45
            var response = await client.ExecuteTaskAsync(request);
#else
                    var response = await client.ExecuteAsync(request, default(CancellationToken));
#endif
                    var isOk = response.StatusCode == HttpStatusCode.OK;
                    return isOk;
                },
                $"HttpHead({url})   , header={headerDict?.ToJsonStr()}",
                warnMiSeconds: warnMiSeconds);
        }



        public static RemoteFileInfo HttpHead(string url, Dictionary<string, string> headerDict = null, int warnMiSeconds = 200, int timeOut = 1000 * 60 * 20)
        {
            return TimeWatcherHelper.TimeWatcher(() =>
                {
                    var request = GenHeadRequest(url, headerDict);

                    var client = new RestClient(url)
                    {
                        Proxy = null,
                        Timeout = timeOut,// 1000 * 60 * 20

                    };
                    var fileName = Path.GetFileName(url);
                    var remoteInfo = new RemoteFileInfo
                    {
                        FileName = fileName,
                    };
                    var response = client.Execute(request);

                    var isOk = response.StatusCode == HttpStatusCode.OK;
                    remoteInfo.IsExist = isOk;
                    if (isOk)
                    {
                        remoteInfo.ContentLength = response.ContentLength;
                        var eTagHeader = response.Headers.FirstOrDefault(m =>
                            string.Equals(m.Name, "ETag", StringComparison.OrdinalIgnoreCase));
                        if (eTagHeader != null)
                        {

                            var etag = eTagHeader?.Value?.ToString() ?? "";
                            remoteInfo.ETag = etag;
                        }
                    }

                    return remoteInfo;
                },
                $"HttpHead({url})   , header={headerDict?.ToJsonStr()}",
                warnMiSeconds: warnMiSeconds);

        }

        public static async Task<RemoteFileInfo> HttpHeadAsync(string url, Dictionary<string, string> headerDict = null, int warnMiSeconds = 200, int timeOut = 1000 * 60 * 20)
        {
            return await TimeWatcherHelper.TimeWatcher(async () =>
                {
                    var request = GenHeadRequest(url, headerDict);

                    var client = new RestClient(url)
                    {
                        Proxy = null,
                        Timeout = timeOut,// 1000 * 60 * 20
                    };
                    var fileName = Path.GetFileName(url);
                    var remoteInfo = new RemoteFileInfo
                    {
                        FileName = fileName,
                    };
#if NET45
            var response = await client.ExecuteTaskAsync(request);
#else
                    var response = await client.ExecuteAsync(request, default(CancellationToken));
#endif

                    var isOk = response.StatusCode == HttpStatusCode.OK;
                    remoteInfo.IsExist = isOk;
                    if (isOk)
                    {
                        remoteInfo.ContentLength = response.ContentLength;
                        var eTagHeader = response.Headers.FirstOrDefault(m =>
                            string.Equals(m.Name, "ETag", StringComparison.OrdinalIgnoreCase));
                        if (eTagHeader != null)
                        {

                            var etag = eTagHeader?.Value?.ToString() ?? "";
                            remoteInfo.ETag = etag;
                        }
                    }

                    return remoteInfo;
                },
                $"HttpHead({url})   , header={headerDict?.ToJsonStr()}",
                warnMiSeconds: warnMiSeconds);

        }



        public static byte[] HttpDownload(string url, Dictionary<string, string> headerDict = null, int warnMiSeconds = 200, int timeOut = 1000 * 60 * 20, bool checkExist = true)
        {

            return TimeWatcherHelper.TimeWatcher(() =>
                {
                    if (checkExist)
                    {
                        var isExist = CheckRemoteFile(url, headerDict, warnMiSeconds, timeOut);
                        if (!isExist)
                        {
                            return null;
                        }
                    }

                    var request = GenDownloadRequest(url, headerDict);

                    var client = new RestClient(url)
                    {
                        Proxy = null,
                        Timeout = timeOut,// 1000 * 60 * 20

                    };
                    var bytes = client.DownloadData(request);
                    return bytes;
                },
                $"HttpDownload({url})   , header={headerDict?.ToJsonStr()}, checkExist={checkExist} ",
                warnMiSeconds: warnMiSeconds);

        }



        private static T ExecuteHttpClient<T>(string domain, IRestRequest request, int timeOut = 1000 * 60 * 20)
        {
            var client = new RestClient(domain)
            {
                Proxy = null,
                Timeout = timeOut,
            };
            var response = client.Execute(request);
            return DeserializeResponse<T>(response);
        }


        private static async Task<T> ExecuteHttpClientAsync<T>(string domain, IRestRequest request, CancellationToken token = default(CancellationToken), int timeOut = 1000 * 60 * 20)
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
            return DeserializeResponse<T>(response);


            // return DeserializeResponse<T>(response);
        }

        private static T DeserializeResponse<T>(IRestResponse response)
        {
            if (response.StatusCode == HttpStatusCode.OK)
            {
                if (ConfigItems.DebugIsOpen)
                    LogHelper.Debug($" response.Content is {response.Content} ", appendMessage: false);
                return response.Content.To<T>();
            }

            LogHelper.Warn($" ResponseStatus is {response.ResponseStatus} ", appendMessage: false);
            return default;
        }

        private static Dictionary<string, string> GeneratorHeaderDict(Dictionary<string, string> headerDict = null)
        {
            if (headerDict == null)
            {
                if (CurrentItem.Items != null)
                    headerDict = new Dictionary<string, string>
                    {
                        {Consts.SERIZLNUMBER, CurrentItem.GetSerialNumber}
                    };
            }
            else if (!headerDict.ContainsKey(Consts.SERIZLNUMBER))
            {
                if (CurrentItem.Items != null) headerDict.Add(Consts.SERIZLNUMBER, CurrentItem.GetSerialNumber);
            }

            return headerDict ?? new Dictionary<string, string>();
        }


    }

}
