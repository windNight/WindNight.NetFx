using System.Data.Common;
using System.Runtime.Serialization;
using System.Security.Cryptography.Extensions;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.WnExtensions.Controllers;
using Microsoft.Extensions.DependencyInjection.WnExtension;
using Newtonsoft.Json.Extension;
using Newtonsoft.Json.Linq;
using WindNight.Core.Abstractions;
using WindNight.Core.Attributes.Abstractions;
using WindNight.Extension;
using WindNight.Extension.Logger.DcLog.Extensions;
using WindNight.LogExtension;

namespace Net8ApiDemo.Controllers
{
    [ApiController]
    [Route("api/test")]
    public partial class WeatherForecastController : DefaultApiControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching",
        };

        private readonly ILogger<WeatherForecastController> _logger;


        private readonly Dictionary<string, string> SignDict = new()
        {
            { "Authorization", "格式 Bearer xx" },
            { "AppId", "AppId" },
            { "AppCode", "AppCode" },
            { "AppToken", "Sign" },
            { "Ts", "当前时间戳" },
        };

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }


        private string GetHeaderData(HttpRequest httpRequest, string headerName)
        {
            if (httpRequest.Headers.TryGetValue(headerName, out var requestHeader))
            {
                var header = requestHeader.FirstOrDefault();
                if (!header.IsNullOrEmpty())
                {
                    return header.Trim();
                }
            }

            return string.Empty;
        }

        private Dictionary<string, string> GetAllHeaderData(HttpRequest httpRequest)
        {
            var dict = new Dictionary<string, string>();
            foreach (var item in httpRequest.Headers)
            {
                dict.Add(item.Key, item.Value);
            }

            return dict;
        }

        [HttpGet("dingtalk")]
        [NonAuth]
        [DebugApi]
        public async Task<object> TestDingTalkNoticeAsync([FromQuery] TIn req = null)
        {
            var content = GetNoticeContent("测试内容测试内容测试内容");

            var title = "调度任务通知";
            var obj = new { msgtype = "markdown", markdown = new { title, text = content }, at = new { } };
            var token = "";
            var signKey = "";
            var ts = HardInfo.NowUnixTime;
            var sign = EncryptHelper.CalcDingTalkSign(ts, signKey);
            //var sign1 = EncryptHelper.CalcDingTalkSign1(ts, signKey);
            var requestUri = $"https://oapi.dingtalk.com/robot/send?access_token={token}&timestamp={ts}&sign={sign}";
            //var rlt = await HttpPostAsync(requestUri, obj);

            var rlt = await HttpHelper.PostAsync<string>(requestUri, obj, convertFunc: _ => _.ToString());


            return rlt;
        }

        private async Task<string> HttpPostAsync(string url, object bodyObj)
        {
            // var requestUri = $"https://oapi.dingtalk.com/robot/send?access_token={token}";
            var request = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = new StringContent(bodyObj.ToJsonStr(), Encoding.UTF8, "application/json"),
            };
            var httpClient = new HttpClient { Timeout = TimeSpan.FromSeconds(60) };
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("content-type", "application/json;charset=UTF-8");

            var res = await httpClient.SendAsync(request);
            var rlt = await res.Content.ReadAsStringAsync();
            return rlt;
        }


        private string GetNoticeContent(string message)
        {
            var env = Ioc.GetService<IHostEnvironment>();
            var environmentName = env?.EnvironmentName;
            var applicationName = env?.ApplicationName;
            var content = "###  Test 任务通知\n" +
                          " #### 任务基础属性\n\n" +
                          "> #### JobCode:(任务代号)\n\n" +
                          "> ##### TestJobCode\n\n" +
                          "> #### JobId:(本次运行的jobId)\n\n" +
                          "> ##### TestJobId\n\n" +
                          "#### 执行情况:\n\n" +
                          $"> ##### {message}\n\n" +
                          // "> ![screenshot](https://gw.alipayobjects.com/zos/skylark-tools/public/files/84111bbeba74743d2771ed4f062d1f25.png)\n" +
                          $"  ####  {applicationName}_{environmentName} \n";
            return content;
        }

        [HttpGet("debugapi")]
        [NonAuth]
        [DebugApi]
        public object DebugApi([FromQuery] TIn req = null)
        {
            LogHelper.Info("DebugApi");
            return Get(req);
        }

        [HttpPost("debugapi/post")]
        [NonAuth]
        [DebugApi]
        public object DebugApiPost([FromBody] TIn req = null)
        {
            LogHelper.Info("DebugApiPost");
            return Get(req);
        }


        [HttpGet("sysapi/v0")]
        [NonAuth]
        [SysApi]
        public object SysApi([FromQuery] TIn req = null)
        {
            return Get(req);
        }

        [HttpGet("sysapi/v10")]
        [NonAuth]
        [SysApi(10)]
        public object SysApiV10([FromQuery] TIn req = null)
        {
            return Get(req);
        }

        [HttpGet("tt")]
        [NonAuth]
        public object Get([FromQuery] TIn req = null)
        {
            var signData = new Dictionary<string, string>();

            foreach (var item in SignDict)
            {
                var data = GetHeaderData(Request, item.Key);
                signData.Add(item.Key, data);
            }

            var rangeData = Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)],
            })
                .ToArray();
            var allHeaderData = GetAllHeaderData(Request);
            return new { allHeaderData, signData, rangeData };
        }

        [HttpGet("loghelper")]
        public object TestLogHelper()
        {
            var testMsg =
                "sql执行报错   MySql.Data.MySqlClient.MySqlException (0x80004005): Unknown column 'CommunityId' in 'field list'\n   at MySql.Data.MySqlClient.MySqlStream.ReadPacketAsync(Boolean execAsync)\n   at MySql.Data.MySqlClient.NativeDriver.GetResultAsync(Int32 affectedRow, Int64 insertedId, Boolean execAsync)\n   at MySql.Data.MySqlClient.Driver.GetResultAsync(Int32 statementId, Int32 affectedRows, Int64 insertedId, Boolean execAsync)\n   at MySql.Data.MySqlClient.Driver.NextResultAsync(Int32 statementId, Boolean force, Boolean execAsync)\n   at MySql.Data.MySqlClient.MySqlDataReader.NextResultAsync(Boolean execAsync, CancellationToken cancellationToken)\n   at MySql.Data.MySqlClient.MySqlDataReader.NextResultAsync(Boolean execAsync, CancellationToken cancellationToken)\n   at MySql.Data.MySqlClient.MySqlCommand.ExecuteReaderAsync(CommandBehavior behavior, Boolean execAsync, CancellationToken cancellationToken)\n   at MySql.Data.MySqlClient.MySqlCommand.ExecuteReaderAsync(CommandBehavior behavior, Boolean execAsync, CancellationToken cancellationToken)\n   at MySql.Data.MySqlClient.MySqlCommand.ExecuteReaderAsync(CommandBehavior behavior, Boolean execAsync, CancellationToken cancellationToken)\n   at MySql.Data.MySqlClient.MySqlCommand.ExecuteNonQueryAsync(Boolean execAsync, CancellationToken cancellationToken)\n   at MySql.Data.MySqlClient.MySqlCommand.ExecuteNonQuery()\n   at Dapper.SqlMapper.ExecuteCommand(IDbConnection cnn, CommandDefinition& command, Action`2 paramReader) in /_/Dapper/SqlMapper.cs:line 2994\n   at Dapper.SqlMapper.ExecuteImpl(IDbConnection cnn, CommandDefinition& command) in /_/Dapper/SqlMapper.cs:line 685\n   at Dapper.SqlMapper.Execute(IDbConnection cnn, String sql, Object param, IDbTransaction transaction, Nullable`1 commandTimeout, Nullable`1 commandType) in /_/Dapper/SqlMapper.cs:line 556\n   at WindNight.Extension.Dapper.Mysql.MySqlBase.Execute(String connStr, String sql, Object param, Action`2 execErrorHandler)\n   at WindNight.Extension.Dapper.Mysql.MySqlBase.SqlTimer[T](Func`5 sqlFunc, String connectString, String sql, Object param, String actionName, Int64 warnMs, Action`2 execErrorHandler, Boolean isDebug)\n\nMySql.Data.MySqlClient.MySqlException (0x80004005): Unknown column 'CommunityId' in 'field list'\n   at MySql.Data.MySqlClient.MySqlStream.ReadPacketAsync(Boolean execAsync)\n   at MySql.Data.MySqlClient.NativeDriver.GetResultAsync(Int32 affectedRow, Int64 insertedId, Boolean execAsync)\n   at MySql.Data.MySqlClient.Driver.GetResultAsync(Int32 statementId, Int32 affectedRows, Int64 insertedId, Boolean execAsync)\n   at MySql.Data.MySqlClient.Driver.NextResultAsync(Int32 statementId, Boolean force, Boolean execAsync)\n   at MySql.Data.MySqlClient.MySqlDataReader.NextResultAsync(Boolean execAsync, CancellationToken cancellationToken)\n   at MySql.Data.MySqlClient.MySqlDataReader.NextResultAsync(Boolean execAsync, CancellationToken cancellationToken)\n   at MySql.Data.MySqlClient.MySqlCommand.ExecuteReaderAsync(CommandBehavior behavior, Boolean execAsync, CancellationToken cancellationToken)\n   at MySql.Data.MySqlClient.MySqlCommand.ExecuteReaderAsync(CommandBehavior behavior, Boolean execAsync, CancellationToken cancellationToken)\n   at MySql.Data.MySqlClient.MySqlCommand.ExecuteReaderAsync(CommandBehavior behavior, Boolean execAsync, CancellationToken cancellationToken)\n   at MySql.Data.MySqlClient.MySqlCommand.ExecuteNonQueryAsync(Boolean execAsync, CancellationToken cancellationToken)\n   at MySql.Data.MySqlClient.MySqlCommand.ExecuteNonQuery()\n   at Dapper.SqlMapper.ExecuteCommand(IDbConnection cnn, CommandDefinition& command, Action`2 paramReader) in /_/Dapper/SqlMapper.cs:line 2994\n   at Dapper.SqlMapper.ExecuteImpl(IDbConnection cnn, CommandDefinition& command) in /_/Dapper/SqlMapper.cs:line 685\n   at Dapper.SqlMapper.Execute(IDbConnection cnn, String sql, Object param, IDbTransaction transaction, Nullable`1 commandTimeout, Nullable`1 commandType) in /_/Dapper/SqlMapper.cs:line 556\n   at WindNight.Extension.Dapper.Mysql.MySqlBase.Execute(String connStr, String sql, Object param, Action`2 execErrorHandler)\n   at WindNight.Extension.Dapper.Mysql.MySqlBase.SqlTimer[T](Func`5 sqlFunc, String connectString, String sql, Object param, String actionName, Int64 warnMs, Action`2 execErrorHandler, Boolean isDebug)";

            var ex = new MySqlException(" (0x80004005): Unknown column 'CommunityId' in 'field list'");
            DcLogHelper.Error(testMsg, ex);

            return true;
        }

        [HttpGet("t")]
        public object Get111()
        {
            var signData = new Dictionary<string, string>();

            foreach (var item in SignDict)
            {
                var data = GetHeaderData(Request, item.Key);
                signData.Add(item.Key, data);
            }

            var rangeData = Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)],
            })
                .ToArray();
            var allHeaderData = GetAllHeaderData(Request);
            return new { allHeaderData, signData, rangeData };
        }

        [HttpPost("log")]
        public bool ReportLog([FromBody] SelfLogReport log)
        {
            DcLogHelper.Report(log.LogData, log.TraceId);

            return true;
        }

        [HttpPost("log/1")]
        public bool ReportLog([FromBody] JObject log)
        {
            DcLogHelper.Report(log);

            return true;
        }


        [HttpGet]
        public IEnumerable<WeatherForecast> GetTTT()
        {
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)],
            })
                .ToArray();
        }

        public sealed class MySqlException : DbException
        {
            internal MySqlException()
            {
            }

            internal MySqlException(string msg)
                : base(msg)
            {
            }

            internal MySqlException(string msg, Exception ex)
                : base(msg, ex)
            {
            }

            internal MySqlException(string msg, bool isFatal, Exception inner)
                : base(msg, inner)
            {
                IsFatal = isFatal;
            }

            internal MySqlException(string msg, int errno, Exception inner)
                : this(msg, inner)
            {
                Number = errno;
                Data.Add("Server Error Code", errno);
            }

            internal MySqlException(string msg, int errno, bool isFatal)
                : this(msg)
            {
                Number = errno;
                IsFatal = isFatal;
                Data.Add("Server Error Code", errno);
            }

            internal MySqlException(string msg, int errno)
                : this(msg, errno, null)
            {
            }

            internal MySqlException(uint code, string sqlState, string msg)
                : base(msg)
            {
                Code = code;
                SqlState = sqlState;
            }

            private MySqlException(SerializationInfo info, StreamingContext context)
                : base(info, context)
            {
            }

            /// <summary>Gets a number that identifies the type of error.</summary>
            public int Number { get; }

            /// <summary>
            ///     True if this exception was fatal and cause the closing of the connection, false otherwise.
            /// </summary>
            internal bool IsFatal { get; }

            internal bool IsQueryAborted => Number == 1317 || Number == 1028;

            /// <summary>Gets the SQL state.</summary>
            public new string SqlState { get; private set; }

            /// <summary>
            ///     Gets an integer that representes the MySQL error code.
            /// </summary>
            public uint Code { get; private set; }
        }
    }

    public partial class WeatherForecastController
    {
        [HttpGet("/api/monitor/baseinfo")]
        [NonAuth, SysApi]
        public virtual ISvrMonitorInfo GetBaseInfo()
        {
            if (!IsAuthType1())
            {
                return null;
            }
            var svrInfo = DefaultSvrMonitorInfo.GenSvrMonitorInfo(SvrMonitorTypeEnum.Query);
            svrInfo.ClientIp = Request.HttpContext.GetClientIp();
            svrInfo.ServerIp = GetHttpServerIp();

            return svrInfo;

        }
    }

    public class SelfLogReport
    {
        public JObject LogData { get; set; }
        public string TraceId { get; set; } = "";
        public string ReqTraceId { get; set; } = "";
    }

    /// <summary> </summary>
    public class TIn
    {
        /// <summary>  KL1 </summary>
        public string ReqTraceId { get; set; }
        public string KL1 { get; set; }

        /// <summary> kl1 </summary>
        public int kl1 { get; set; }

        /// <summary> Kl1 </summary>
        public decimal Kl1 { get; set; }

        /// <summary> kL1 </summary>
        public string kL1 { get; set; }
    }
}
