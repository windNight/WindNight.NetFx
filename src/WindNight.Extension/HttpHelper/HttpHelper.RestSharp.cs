using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using RestSharp;

namespace WindNight.Extension
{
    public static partial class HttpHelper
    {

        static RestClient GenRestClient(string domain, int timeOut)
        {
            var client = new RestClient(domain)
            {
                Proxy = null,
                Timeout = timeOut,
            };

            return client;
        }


        static RestRequest GenGetRequest(string url,
            Dictionary<string, string> headerDict = null,
            Dictionary<string, object> queries = null)
        {
            var request = new RestRequest(url, Method.GET);
            headerDict = GeneratorHeaderDict(headerDict);

            foreach (var header in headerDict) request.AddHeader(header.Key, header.Value);
            if (queries != null)
            {
                foreach (var query in queries)
                    request.AddParameter(query.Key, query.Value);

            }


            return request;

        }

        static RestRequest GenPostRequest(string url,
            Dictionary<string, string> headerDict = null,
            object bodyObjects = null
        )
        {
            var request = new RestRequest(url, Method.POST);
            headerDict = GeneratorHeaderDict(headerDict);

            foreach (var header in headerDict) request.AddHeader(header.Key, header.Value);

            if (bodyObjects != null)
            {
                request.AddJsonBody(bodyObjects);
            }

            return request;

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


        private static IRestResponse ExecuteHttpClient(string domain, IRestRequest request, int timeOut = 1000 * 60 * 20)
        {
            var client = GenRestClient(domain, timeOut);
            //var client = new RestClient(domain)
            //{
            //    Proxy = null,
            //    Timeout = timeOut,
            //};

            //var response = client.Execute(request);
            var response = DoExecute(client, request);
            return response;

        }

        private static T ExecuteHttpClient<T>(string domain, IRestRequest request, int timeOut = 1000 * 60 * 20
            , Func<string, T> convertFunc = null)
        {
            //var client = GenRestClient(domain, timeOut);
            //var client = new RestClient(domain)
            //{
            //    Proxy = null,
            //    Timeout = timeOut,
            //};

            //var response = client.Execute(request);
            //var response = DoExecute(client, request);
            var response = ExecuteHttpClient(domain, request, timeOut);
            return DeserializeResponse<T>(response, convertFunc);
        }

        private static async Task<IRestResponse> ExecuteHttpClientAsync(string domain, IRestRequest request, CancellationToken token = default(CancellationToken), int timeOut = 1000 * 60 * 20)
        {
            var client = GenRestClient(domain, timeOut);
            //var client = new RestClient(domain)
            //{
            //    Proxy = null,
            //    Timeout = timeOut,// 1000 * 60 * 20
            //};

            //#if  NET45
            //            var response = await client.ExecuteTaskAsync(request, token);
            //#else
            //            var response = await client.ExecuteAsync(request, token);
            //#endif
            var response = await DoExecuteAsync(client, request, token);

            return response;
            // return DeserializeResponse<T>(response);
        }


        private static async Task<T> ExecuteHttpClientAsync<T>(string domain, IRestRequest request, Func<string, T> convertFunc = null,
            CancellationToken token = default(CancellationToken), int timeOut = 1000 * 60 * 20)
        {
            //            var client = GenRestClient(domain, timeOut);
            //            //var client = new RestClient(domain)
            //            //{
            //            //    Proxy = null,
            //            //    Timeout = timeOut,// 1000 * 60 * 20
            //            //};

            //#if  NET45
            //            var response = await client.ExecuteTaskAsync(request, token);
            //#else
            //            var response = await client.ExecuteAsync(request, token);
            //#endif

            var response = await ExecuteHttpClientAsync(domain, request, token);

            return DeserializeResponse<T>(response, convertFunc);


            // return DeserializeResponse<T>(response);
        }








        static IRestResponse DoExecute(RestClient client, IRestRequest request)
        {
            var response = client.Execute(request);
            return response;

        }

#if !NET45
        static async Task<IRestResponse> DoExecuteAsync(RestClient client, IRestRequest request, CancellationToken token = default(CancellationToken))
        {
            var response = await client.ExecuteAsync(request, token);
            return response;

        }
#endif


#if NET45

        static async Task<IRestResponse> DoExecuteAsync(RestClient client, IRestRequest request, CancellationToken token = default(CancellationToken))
        {
            var response = await client.ExecuteTaskAsync(request, token);
            return response;

        }
#endif


        private static IPagedList<T> ExecuteHttpClient2<T>(string domain, IRestRequest request, int timeOut = 1000 * 60 * 20)
        {
            //var client = GenRestClient(domain, timeOut);
            ////var client = new RestClient(domain)
            ////{
            ////    Proxy = null,
            ////    Timeout = timeOut,
            ////};
            ////var response = client.Execute(request);
            //var response = DoExecute(client, request);


            var response = ExecuteHttpClient(domain, request, timeOut);

            return DeserializePageListResponse<T>(response);
        }

        private static async Task<IPagedList<T>> ExecuteHttpClientAsync2<T>(string domain, IRestRequest request, CancellationToken token = default(CancellationToken), int timeOut = 1000 * 60 * 20)
        {
            //var client = GenRestClient(domain, timeOut);
            ////var client = new RestClient(domain)
            ////{
            ////    Proxy = null,
            ////    Timeout = timeOut,// 1000 * 60 * 20
            ////};

            ////#if  NET45
            ////            var response = await client.ExecuteTaskAsync(request);
            ////#else
            ////            var response = await client.ExecuteAsync(request, token);
            ////#endif
            //var response = await DoExecuteAsync(client, request, token);

            var response = await ExecuteHttpClientAsync(domain, request, token);

            return response.DeserializePageListResponse<T>();


        }


        private static T ExecuteHttpClient3<T>(string domain, IRestRequest request, int timeOut = 1000 * 60 * 20)
        {
            var response = ExecuteHttpClient(domain, request, timeOut);

            return response.DeserializeResResponse<T>();
        }

        private static async Task<T> ExecuteHttpClientAsync3<T>(string domain, IRestRequest request, CancellationToken token = default(CancellationToken), int timeOut = 1000 * 60 * 20)
        {

            var response = await ExecuteHttpClientAsync(domain, request, token);

            return response.DeserializeResResponse<T>();

        }

        private static IEnumerable<T> ExecuteHttpClient4<T>(string domain, IRestRequest request, int timeOut = 1000 * 60 * 20)
        {
            var response = ExecuteHttpClient(domain, request, timeOut);

            return response.DeserializeListResponse<T>();
        }

        private static async Task<IEnumerable<T>> ExecuteHttpClientAsync4<T>(string domain, IRestRequest request, CancellationToken token = default(CancellationToken), int timeOut = 1000 * 60 * 20)
        {

            var response = await ExecuteHttpClientAsync(domain, request, token);

            return response.DeserializeListResponse<T>();

        }

    }

}
