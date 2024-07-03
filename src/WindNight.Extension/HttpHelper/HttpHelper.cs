using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Extension;
using WindNight.Core.Tools;

namespace WindNight.Extension
{
    public static partial class HttpHelper
    {


        public static bool CheckRemoteFile(string url, Dictionary<string, string> headerDict = null, int warnMiSeconds = 200, int timeOut = 1000 * 60 * 20)
        {
            return TimeWatcherHelper.TimeWatcher(() =>
                {
                    var request = GenHeadRequest(url, headerDict);

                    var client = GenRestClient(url, timeOut);
                    //var client = new RestClient(url)
                    //{
                    //    Proxy = null,
                    //    Timeout = timeOut,// 1000 * 60 * 20
                    //};

                    //var response = client.Execute(request);
                    var response = DoExecute(client, request);
                    var isOk = response.StatusCode == HttpStatusCode.OK;
                    return isOk;
                },
                $"HttpHead({url})   , header={headerDict?.ToJsonStr()}",
                warnMiSeconds: warnMiSeconds);
        }


        public static async Task<bool> CheckRemoteFileAsync(string url, Dictionary<string, string> headerDict = null,
            int warnMiSeconds = 200, int timeOut = 1000 * 60 * 20, CancellationToken token = default)
        {
            return await TimeWatcherHelper.TimeWatcher(async () =>
                {
                    var request = GenHeadRequest(url, headerDict);

                    var client = GenRestClient(url, timeOut);
                    //var client = new RestClient(url)
                    //{
                    //    Proxy = null,
                    //    Timeout = timeOut,// 1000 * 60 * 20
                    //};

                    //#if NET45
                    //            var response = await client.ExecuteTaskAsync(request);
                    //#else
                    //                    var response = await client.ExecuteAsync(request, default(CancellationToken));
                    //#endif
                    var response = await DoExecuteAsync(client, request, token);

                    var isOk = response.StatusCode == HttpStatusCode.OK;
                    return isOk;
                },
                $"HttpHead({url})   , header={headerDict?.ToJsonStr()}",
                warnMiSeconds: warnMiSeconds);
        }

        public static byte[] Download(string url, Dictionary<string, string> headerDict = null, int warnMiSeconds = 200, int timeOut = 1000 * 60 * 20, bool checkExist = true)
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

                    var client = GenRestClient(url, timeOut);
                    //var client = new RestClient(url)
                    //{
                    //    Proxy = null,
                    //    Timeout = timeOut,// 1000 * 60 * 20

                    //};
                    var bytes = client.DownloadData(request);
                    return bytes;
                },
                $"Download({url})   , header={headerDict?.ToJsonStr()}, checkExist={checkExist} ",
                warnMiSeconds: warnMiSeconds);

        }




    }

}
