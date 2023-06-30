namespace Microsoft.AspNetCore.Mvc.WnExtensions
{
    /// <summary> 管理线程临时参数 </summary>
    public class WebConst
    {
        /// <summary> 业务序列号  </summary>
        public const string SERIZLNUMBER = "serialnumber";

        public const string HEARDER = "___header_______";
        public const string REQUESTPATH = "___request_path_";
        public const string REQUESTPARAMS = "___params_______";

        /// <summary>  IP  </summary>
        public const string CLIENTIP = "___clientip_____";

        public const string SERVERIP = "___serverip_____";

        public const string LOCK_KEY = "___lockkey______";
        public const string ACCESSTOKEN = "___accesstoken__";

        public const string RESPONSE = "response";

        public const string APPCODE = "appcode";

        public const string TIMESTAMPS = "timestamps";

        public const string VERSION = "version";

        public const string HTTPSIGN = "sign";

        public const string GAMECODE = "gamecode";

        public const string ADMININFO = "admininfo"; // 正常在后台

        public const string USERID = "userid"; //一般用于前台
        public const string THIRDORDERNO = "thirdorderno"; //一般用于前台


        public const string MS_HttpRequestMessage = "MS_HttpRequestMessage";


        #region 时间相关

        public const string YEAR = "YEAR";
        public const string HOUR = "HOUR";
        public const string HOST = "host";
        public const string MONTH = "MONTH";
        public const string DAY = "DAY";
        public const string TODAY = "TODAY";
        public const string YESTERDAY = "YESTERDAY";
        public const string MINUTE = "MINUTE";

        /// <summary>
        ///     yyyy-MM-dd HH:mm:ss
        /// </summary>
        public const string NOW = "NOW";

        /// <summary>
        ///     unittime ms
        /// </summary>
        public const string NOW1 = "NOW1";

        /// <summary>
        ///     HHmm
        /// </summary>
        public const string TIME = "TIME";

        /// <summary>
        ///     HH:mm
        /// </summary>
        public const string TIME1 = "TIME1";

        public const string BEGINTIME = "___begintime____";
        public const string ENDTIME = "___endtime______";

        #endregion 时间相关
    }
}