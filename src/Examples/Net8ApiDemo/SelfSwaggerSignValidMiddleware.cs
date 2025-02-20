using WindNight.AspNetCore.Hosting.Middleware;
using WindNight.Core.Extension;

namespace Net8ApiDemo
{
    public class SelfSwaggerSignValidMiddleware : WindSwaggerSignValidMiddleware
    {
        public SelfSwaggerSignValidMiddleware(RequestDelegate next, Dictionary<string, string> signKeyDict) : base(next, signKeyDict)
        {

        }


        protected override bool CheckValidData(HttpContext context, Dictionary<string, string> dict)
        {

            var appToken = dict.SafeGetValue("AppToken");
            return !appToken.IsNullOrEmpty();

        }


    }
}
