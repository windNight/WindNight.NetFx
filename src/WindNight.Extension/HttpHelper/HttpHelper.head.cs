using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Extension;
using RestSharp;
using WindNight.Core.Abstractions;
using WindNight.Core.Tools;

namespace WindNight.Extension
{
    public static partial class HttpHelper
    {
        public static RemoteFileInfo HttpHead(string url,
            Dictionary<string, string> headerDict = null,
            int warnMiSeconds = 200,
            int timeOut = 1000 * 60 * 20,
            Func<IRestResponse, RemoteFileInfo> errStatusFunc = null)
        {
            return TimeWatcherHelper.TimeWatcher(() =>
                {
                    var request = GenHeadRequest(url, headerDict);

                    var client = GenRestClient(url, timeOut, headerDict);

                    var fileName = Path.GetFileName(url);
                    var remoteInfo = new RemoteFileInfo { FileName = fileName };

                    //var response = client.Execute(request);
                    var response = DoExecute(client, request);

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

                    if (errStatusFunc != null)
                    {
                        return errStatusFunc.Invoke(response);
                    }

                    return remoteInfo;
                },
                $"HttpHead({url})   , header={headerDict?.ToJsonStr()}",
                warnMiSeconds: warnMiSeconds);
        }

        public static async Task<RemoteFileInfo> HeadAsync(string url,
            Dictionary<string, string> headerDict = null,
            int warnMiSeconds = 200,
            int timeOut = 1000 * 60 * 20,
            CancellationToken token = default,
            Func<IRestResponse, RemoteFileInfo> errStatusFunc = null)
        {
            return await TimeWatcherHelper.TimeWatcher(async () =>
                {
                    var request = GenHeadRequest(url, headerDict);

                    var client = GenRestClient(url, timeOut, headerDict);

                    var fileName = Path.GetFileName(url);
                    var remoteInfo = new RemoteFileInfo { FileName = fileName };


                    var response = await DoExecuteAsync(client, request, token);

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

                    if (errStatusFunc != null)
                    {
                        return errStatusFunc.Invoke(response);
                    }

                    return remoteInfo;
                },
                $"HttpHead({url})   , header={headerDict?.ToJsonStr()}",
                warnMiSeconds: warnMiSeconds);
        }
    }
}
