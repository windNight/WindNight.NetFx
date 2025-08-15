using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Extension;
using RestSharp;
using WindNight.Core.Tools;

namespace WindNight.Extension
{
    public static partial class HttpHelper
    {

        public static string CurrentVersion => BuildInfo.BuildVersion;

        public static string CurrentCompileTime => BuildInfo.BuildTime;

        public static string HttpHelperPluginVersion => $"{nameof(HttpHelper)}/{CurrentVersion} {CurrentCompileTime}";


        public static bool CheckRemoteFile(string url,
            Dictionary<string, string> headerDict = null,
            int warnMiSeconds = 200,
            int timeOut = 1000 * 60 * 20,
            Func<IRestResponse, bool> errStatusFunc = null)
        {
            return TimeWatcherHelper.TimeWatcher(() =>
                {
                    var request = GenHeadRequest(url, headerDict);

                    var client = GenRestClient(url, timeOut, headerDict);

                    var response = DoExecute(client, request);
                    var isOk = response.StatusCode == HttpStatusCode.OK;

                    if (!isOk && errStatusFunc != null)
                    {
                        return errStatusFunc.Invoke(response);
                    }

                    return isOk;
                },
                $"HttpHead({url})   , header={headerDict?.ToJsonStr()}",
                warnMiSeconds: warnMiSeconds);
        }


        public static async Task<bool> CheckRemoteFileAsync(string url,
            Dictionary<string, string> headerDict = null,
            int warnMiSeconds = 200,
            int timeOut = 1000 * 60 * 20,
            CancellationToken token = default,
            Func<IRestResponse, bool> errStatusFunc = null)
        {
            return await TimeWatcherHelper.TimeWatcher(async () =>
                {
                    var request = GenHeadRequest(url, headerDict);

                    var client = GenRestClient(url, timeOut, headerDict);

                    var response = await DoExecuteAsync(client, request, token);

                    var isOk = response.StatusCode == HttpStatusCode.OK;

                    if (!isOk && errStatusFunc != null)
                    {
                        return errStatusFunc.Invoke(response);
                    }

                    return isOk;
                },
                $"HttpHead({url})   , header={headerDict?.ToJsonStr()}",
                warnMiSeconds: warnMiSeconds);
        }

        public static byte[] Download(string url, Dictionary<string, string> headerDict = null, int warnMiSeconds = 200,
            int timeOut = 1000 * 60 * 20, bool checkExist = true)
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

                    var client = GenRestClient(url, timeOut, headerDict);

                    var bytes = client.DownloadData(request);
                    return bytes;
                },
                $"Download({url})   , header={headerDict?.ToJsonStr()}, checkExist={checkExist} ",
                warnMiSeconds: warnMiSeconds);
        }
    }
}
