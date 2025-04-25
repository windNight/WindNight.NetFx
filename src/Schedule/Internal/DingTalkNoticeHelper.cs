using System;
using System.Net.Http;
using System.Security.Cryptography;
using System.Security.Cryptography.Extensions;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Extensions.DependencyInjection.WnExtension;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json.Extension;
using Schedule.Abstractions;
using WindNight.Extension;

namespace Schedule.@internal
{
    internal static class DingTalkNoticeHelper
    {
        internal static async Task DoNoticeAsync(JobBaseInfo jobBaseInfo, string message)
        {
            var noticeDingConfig = ConfigItems.JobsConfig?.NoticeDingConfig;
            if (noticeDingConfig == null)
            {
                noticeDingConfig = new NoticeDingConfig
                {
                    NoticeDingToken = ConfigItems.DingtalkToken,
                    NoticeDingSignKey = ConfigItems.NoticeDingSignKey,
                    NoticeDingPhones = ConfigItems.DingtalkPhones,
                    NoticeDingAtAll = ConfigItems.DingtalkAtAll,
                    NoticeDingIsOpen = !ConfigItems.DingtalkToken.IsNullOrEmpty(),
                };
            }

            var token = noticeDingConfig?.NoticeDingToken ?? ConfigItems.DingtalkToken;
            if (token.IsNullOrEmpty())
            {
                return;
            }

            if (!noticeDingConfig.NoticeDingIsOpen)
            {
                return;
            }

            var postData = GetDingTalkPostData(jobBaseInfo, message, noticeDingConfig);
            if (postData == null)
            {
                return;
            }

            var requestUri = $"https://oapi.dingtalk.com/robot/send?access_token={token}";
            var signKey = noticeDingConfig?.NoticeDingSignKey ?? "";
            if (!signKey.IsNullOrEmpty())
            {
                var ts = HardInfo.NowUnixTime;
                var sign = EncryptHelper.CalcDingTalkSign(ts, signKey);
                requestUri = $"{requestUri}&timestamp={ts}&sign={sign}";
            }

            // var rlt = await HttpPostAsync(requestUri, postData);
            var rlt = await HttpHelper.PostAsync<string>(requestUri, postData, convertFunc: _ => _.ToString());

            JobLogHelper.Debug(
                $"DoNoticeAsync response is {rlt}, \r\n message is {postData.ToJsonStr()} ,\r\n token is {token}",
                nameof(DoNoticeAsync));

        }

        static string CalcTalkSign(long ts, string signKey)
        {
            try
            {

                var stringToSign = $"{ts}\n{signKey}"; //ts + "\n" + signKey;
                using (HMACSHA256 hmac = new HMACSHA256(Encoding.UTF8.GetBytes(signKey)))
                {
                    var signData = hmac.ComputeHash(Encoding.UTF8.GetBytes(stringToSign));
                    var sign = HttpUtility.UrlEncode(Convert.ToBase64String(signData));
                    return sign;
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error($"", ex);
                return "";
            }

        }

        private static string GetNoticeContent(JobBaseInfo jobBaseInfo, string message)
        {
            var env = Ioc.GetService<IHostEnvironment>();
            var environmentName = env?.EnvironmentName;
            var applicationName = env?.ApplicationName;
            var content = $"### {jobBaseInfo.JobName} 任务通知\n" +
                          " #### 任务基础属性\n\n" +
                          "> #### JobCode:(任务代号)\n\n" +
                          $"> ##### {jobBaseInfo.JobCode}\n\n" +
                          "> #### JobId:(本次运行的jobId)\n\n" +
                          $"> ##### {jobBaseInfo.JobId}\n\n" +
                          "#### 执行情况:\n\n" +
                          $"> ##### {message}\n\n" +
                          // "> ![screenshot](https://gw.alipayobjects.com/zos/skylark-tools/public/files/84111bbeba74743d2771ed4f062d1f25.png)\n" +
                          $"  ####  {applicationName}_{environmentName} \n";
            return content;
        }

        private static object GetDingTalkPostData(JobBaseInfo jobBaseInfo, string message,
            NoticeDingConfig noticeDingConfig)
        {
            var atMobiles = string.Empty;
            var isAtAll = false;
            if (noticeDingConfig != null)
            {
                if (!noticeDingConfig.NoticeDingIsOpen)
                {
                    return null;
                }
                atMobiles = noticeDingConfig.NoticeDingPhones;
                isAtAll = noticeDingConfig.NoticeDingAtAll;
            }
            else
            {
                atMobiles = ConfigItems.DingtalkPhones;
                isAtAll = ConfigItems.DingtalkAtAll;
            }

            // <font color=#228B22>Failed</font>
            var content = GetNoticeContent(jobBaseInfo, message);

            var title = "调度任务通知";
            var obj = new
            {
                msgtype = "markdown",
                markdown = new
                {
                    title,
                    text = content,
                },
                at = new
                {
                    atMobiles,
                    isAtAll,
                },
            };
            return obj;
        }

        private static async Task<string> HttpPostAsync(string url, object bodyObj)
        {
            var rlt = await HttpHelper.PostAsync<string>(url, bodyObj, convertFunc: _ => _.ToString());

            //// var requestUri = $"https://oapi.dingtalk.com/robot/send?access_token={token}";
            //var request = new HttpRequestMessage(HttpMethod.Post, url)
            //{
            //    Content = new StringContent(bodyObj.ToJsonStr(), Encoding.UTF8, "application/json"),
            //};
            //var httpClient = new HttpClient { Timeout = TimeSpan.FromSeconds(60) };
            //httpClient.DefaultRequestHeaders.TryAddWithoutValidation("content-type", "application/json;charset=UTF-8");

            //var res = await httpClient.SendAsync(request);
            //var rlt = await res.Content.ReadAsStringAsync();
            return rlt;
        }
    }
}
