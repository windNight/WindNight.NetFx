using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection.WnExtension;
using Newtonsoft.Json.Extension;
using RestSharp;
using RestSharp.Extensions;
using WindNight.Core.Abstractions;
using WindNight.Core.Tools;
using WindNight.Extension.Internals;
//using WindNight.NetCore.Extension;

namespace WindNight.Extension
{
    public static class HttpHelper
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
        /// <returns></returns>
        public static T Get<T>(string url, Dictionary<string, string> headerDict = null, int warnMiSeconds = 200) //where T : new()
        {
            return TimeWatcherHelper.TimeWatcher(() =>
            {
                var request = new RestRequest(Method.GET);
                headerDict = GeneratorHeaderDict(headerDict);

                foreach (var header in headerDict) request.AddHeader(header.Key, header.Value);
                return ExecuteHttpClient<T>(url, request);
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
        /// <param name="warnMiSeconds"></param>
        /// <returns></returns>
        public static T Get<T>(string domain, string path,
            Dictionary<string, object> queries,
            Dictionary<string, string> headerDict = null,
            int warnMiSeconds = 200) //where T : new()
        {
            return TimeWatcherHelper.TimeWatcher(() =>
            {
                var request = new RestRequest(path, Method.GET);

                headerDict = GeneratorHeaderDict(headerDict);

                foreach (var header in headerDict) request.AddHeader(header.Key, header.Value);
                if (queries != null)
                    foreach (var query in queries)
                        request.AddParameter(query.Key, query.Value);

                return ExecuteHttpClient<T>(domain, request);
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
        /// <returns></returns>
        public static async Task<T> GetAsync<T>(string domain, string path,
            Dictionary<string, object> queries,
            Dictionary<string, string> headerDict = null,
            int warnMiSeconds = 200) //where T : new()
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
        /// <returns></returns>
        public static T Post<T>(string domain, string path, object bodyObjects,
            Dictionary<string, string> headerDict = null, int warnMiSeconds = 200) //where T : new()
        {
            return TimeWatcherHelper.TimeWatcher(() =>
            {
                var request = new RestRequest(path, Method.POST);

                headerDict = GeneratorHeaderDict(headerDict);

                foreach (var header in headerDict) request.AddHeader(header.Key, header.Value);

                request.AddJsonBody(bodyObjects);

                return ExecuteHttpClient<T>(domain, request);
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
        /// <returns></returns>
        public static async Task<T> PostAsync<T>(string domain, string path, object bodyObjects,
            Dictionary<string, string> headerDict = null, int warnMiSeconds = 200) //where T : new()
        {
            return await TimeWatcherHelper.TimeWatcher(async () =>
            {
                var request = new RestRequest(path, Method.POST);

                headerDict = GeneratorHeaderDict(headerDict);

                foreach (var header in headerDict) request.AddHeader(header.Key, header.Value);

                request.AddJsonBody(bodyObjects);

                return await ExecuteHttpClientAsync<T>(domain, request);
            },
                $"HttpPostAsync({domain}{path}) with params={bodyObjects.ToJsonStr()} , header={headerDict?.ToJsonStr()}",
                warnMiSeconds: warnMiSeconds);
        }


        private static T ExecuteHttpClient<T>(string domain, IRestRequest request)
        {
            var client = new RestClient(domain)
            {
                Proxy = null,
                Timeout = 1000 * 60 * 20
            };
            var response = client.Execute(request);
            return DeserializeResponse<T>(response);
        }


        private static async Task<T> ExecuteHttpClientAsync<T>(string domain, IRestRequest request, CancellationToken token = default(CancellationToken))
        {
            var client = new RestClient(domain)
            {
                Proxy = null,
                Timeout = 1000 * 60 * 20
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
