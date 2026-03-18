using RestSharp;
using WindNight.Core;
using WindNight.Core.Extension;

namespace WindNight.Extension
{
    public static partial class HttpHelper
    {
        private static RestClient GenRestClient(string domain, int timeOut,
            Dictionary<string, string> headerDict = null)
        {
            var userAgent = headerDict.SafeGetValue(UserAgentKey) ?? null;
            var options = new RestClientOptions(domain)
            {
                // 代理设置
                Proxy = null,

                // 超时设置：int 毫秒 -> TimeSpan
                Timeout = TimeSpan.FromMilliseconds(timeOut),

                // UserAgent 设置
                UserAgent = userAgent ?? HttpHelperPluginVersion,  // 提供默认值

                // 其他常用配置
                ThrowOnAnyError = false,
                FollowRedirects = true,
            };

            var client = new RestClient(options);

            //  var client = new RestClient(domain) { Proxy = null, Timeout = timeOut, UserAgent = userAgent };


            return client;
        }

        private const string UserAgentKey = ConstantKeys.UserAgentKey;
        private const string PluginInfoKey = ConstantKeys.HttpPluginKey;

        public static void AppendHttpHeader(this RestRequest request, Dictionary<string, string> headerDict = null)
        {
            headerDict = GeneratorHeaderDict(headerDict);

            foreach (var header in headerDict)
            {
                if (header.Key.Equals(UserAgentKey, StringComparison.OrdinalIgnoreCase))
                {
                    request.AddOrUpdateHeader(UserAgentKey, header.Value);
                }
                else
                {
                    request.AddHeader(header.Key, header.Value);
                }
            }

            request.AddOrUpdateHeader(PluginInfoKey, HttpHelperPluginVersion);
        }

#if !NET45

        //private static RestRequest AddOrUpdateHeader(RestRequest request, string key, string value)
        //{
        //    request.AddOrUpdateHeader(key, value);

        //    return request;
        //}
#endif


#if NET45
        static IRestRequest AddOrUpdateHeader(this IRestRequest request, string key, string value)
        {
            try
            {
                request.AddHeader(key, value);
            }
            catch (Exception ex)
            {

            }
            return request;

        }
#endif


        private static RestRequest GenGetRequest(string url,
            Dictionary<string, string> headerDict = null,
            Dictionary<string, object> queries = null)
        {
            var request = new RestRequest(url, Method.Get);

            request.AppendHttpHeader(headerDict);


            if (queries != null)
            {
                foreach (var query in queries)
                {
                    //  request.AddParameter(query.Key, query.Value);
                    request.AddQueryParameter(query.Key, query.Value?.ToString());
                }
            }


            return request;
        }

        private static RestRequest GenPostRequest(string url,
            Dictionary<string, string> headerDict = null,
            object bodyObjects = null,
            bool isJsonBody = true)
        {
            var request = new RestRequest(url, Method.Post);
            request.AppendHttpHeader(headerDict);

            if (bodyObjects != null)
            {
                if (isJsonBody)
                {
                    request.AddJsonBody(bodyObjects);
                }
                else
                {
                    request.AddObject(bodyObjects);
                }
            }

            return request;
        }

        private static RestRequest GenPutRequest(string url,
            Dictionary<string, string> headerDict = null,
            object bodyObjects = null,
            bool isJsonBody = true)
        {
            var request = new RestRequest(url, Method.Put);
            request.AppendHttpHeader(headerDict);

            if (bodyObjects != null)
            {
                if (isJsonBody)
                {
                    request.AddJsonBody(bodyObjects);
                }
                else
                {
                    request.AddObject(bodyObjects);
                }
            }

            return request;
        }

        private static RestRequest GenHeadRequest(string url, Dictionary<string, string> headerDict = null)
        {
            var request = new RestRequest(url, Method.Head);

            request.AddOrUpdateHeader("Accept", "*/*");


            request.AppendHttpHeader(headerDict);

            return request;
        }


        private static RestRequest GenDownloadRequest(string url, Dictionary<string, string> headerDict = null)
        {
            var request = new RestRequest(url, Method.Get);
            request.AddOrUpdateHeader("Accept", "*/*");
            request.AppendHttpHeader(headerDict);

            request.AlwaysMultipartFormData = true;
            return request;
        }


        private static RestResponse ExecuteHttpClient(string domain, RestRequest request,
            Dictionary<string, string> headerDict = null, int timeOut = 1000 * 60 * 20)
        {
            var client = GenRestClient(domain, timeOut, headerDict);

            var response = DoExecute(client, request);
            return response;
        }

        private static T ExecuteHttpClient<T>(string domain, RestRequest request,
            Dictionary<string, string> headerDict = null, int timeOut = 1000 * 60 * 20,
            Func<string, T> convertFunc = null, Func<RestResponse, bool> errStatusFunc = null)
        {
            var response = ExecuteHttpClient(domain, request, headerDict, timeOut);
            return response.DeserializeResponse<T>(convertFunc, domain, errStatusFunc: errStatusFunc);
        }

        private static async Task<RestResponse> ExecuteHttpClientAsync(string domain, RestRequest request,
            Dictionary<string, string> headerDict = null, CancellationToken token = default,
            int timeOut = 1000 * 60 * 20)
        {
            var client = GenRestClient(domain, timeOut, headerDict);

            var response = await DoExecuteAsync(client, request, token);

            return response;
        }


        private static async Task<T> ExecuteHttpClientAsync<T>(string domain, RestRequest request,
            Dictionary<string, string> headerDict = null, Func<string, T> convertFunc = null,
            CancellationToken token = default, int timeOut = 1000 * 60 * 20,
            Func<RestResponse, bool> errStatusFunc = null)
        {
            var response = await ExecuteHttpClientAsync(domain, request, headerDict, token, timeOut);

            return response.DeserializeResponse<T>(convertFunc, domain, errStatusFunc: errStatusFunc);
        }


        private static RestResponse DoExecute(RestClient client, RestRequest request)
        {
            var response = client.Execute(request);
            return response;
        }

#if !NET45
        private static async Task<RestResponse> DoExecuteAsync(RestClient client, RestRequest request,
            CancellationToken token = default)
        {
            var response = await client.ExecuteAsync(request, token);
            return response;
        }
#endif


#if NET45
        static async Task<IRestResponse> DoExecuteAsync(RestClient client, IRestRequest request, CancellationToken token
 = default(CancellationToken))
        {
            var response = await client.ExecuteTaskAsync(request, token);
            return response;

        }
#endif


        private static IPagedList<T> ExecuteHttpClient2<T>(string domain, RestRequest request,
            Dictionary<string, string> headerDict = null, int timeOut = 1000 * 60 * 20,
            Func<RestResponse, bool> errStatusFunc = null)
        {
            var response = ExecuteHttpClient(domain, request, headerDict, timeOut);

            return DeserializePageListResponse<T>(response, domain, errStatusFunc);
        }

        private static async Task<IPagedList<T>> ExecuteHttpClientAsync2<T>(string domain, RestRequest request,
            Dictionary<string, string> headerDict = null, CancellationToken token = default,
            int timeOut = 1000 * 60 * 20, Func<RestResponse, bool> errStatusFunc = null)
        {
            var response = await ExecuteHttpClientAsync(domain, request, headerDict, token, timeOut);

            return response.DeserializePageListResponse<T>(domain, errStatusFunc);
        }


        private static T ExecuteHttpClient3<T>(string domain, RestRequest request,
            Dictionary<string, string> headerDict = null, int timeOut = 1000 * 60 * 20,
            Func<RestResponse, bool> errStatusFunc = null)
        {
            var response = ExecuteHttpClient(domain, request, headerDict, timeOut);

            return response.DeserializeResResponse<T>(domain, errStatusFunc: errStatusFunc);
        }

        private static async Task<T> ExecuteHttpClientAsync3<T>(string domain, RestRequest request,
            Dictionary<string, string> headerDict = null, CancellationToken token = default,
            int timeOut = 1000 * 60 * 20, Func<RestResponse, bool> errStatusFunc = null)
        {
            var response = await ExecuteHttpClientAsync(domain, request, headerDict, token, timeOut);

            return response.DeserializeResResponse<T>(domain, errStatusFunc: errStatusFunc);
        }

        private static IEnumerable<T> ExecuteHttpClient4<T>(string domain,
            RestRequest request,
            Dictionary<string, string> headerDict = null,
            int timeOut = 1000 * 60 * 20,
            Func<RestResponse, bool> errStatusFunc = null)
        {
            var response = ExecuteHttpClient(domain, request, headerDict, timeOut);

            return response.DeserializeListResponse<T>(domain, errStatusFunc);
        }

        private static async Task<IEnumerable<T>> ExecuteHttpClientAsync4<T>(string domain, RestRequest request,
            Dictionary<string, string> headerDict = null, CancellationToken token = default,
            int timeOut = 1000 * 60 * 20, Func<RestResponse, bool> errStatusFunc = null)
        {
            var response = await ExecuteHttpClientAsync(domain, request, headerDict, token, timeOut);

            return response.DeserializeListResponse<T>(domain, errStatusFunc);
        }
    }
}
