using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection.WnExtension;
using Newtonsoft.Json.Extension;
using RestSharp;
using RestSharp.Extensions;
using WindNight.Core;
using WindNight.Core.Abstractions;
using WindNight.Core.Tools;
using WindNight.Extension.Internals;
//using WindNight.NetCore.Extension;

namespace WindNight.Extension
{
    public static partial class HttpHelper
    {

        /// <summary>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url"></param>
        /// <param name="headerDict"></param>
        /// <param name="warnMiSeconds"></param>
        /// <param name="timeOut">Timeout in milliseconds to be used for the request </param>
        /// <returns></returns>
        public static IEnumerable<T> GetList<T>(string url, Dictionary<string, string> headerDict = null, int warnMiSeconds = 200,
            int timeOut = 1000 * 60 * 20, bool isThrow = false)
        {
            var res = Get<ResponseResult<IEnumerable<T>>>(url, headerDict, warnMiSeconds, timeOut);
            if (res.Code == 0)
            {
                return res.Data;
            }

            if (isThrow)
            {
                throw new Exception($"GetListAsync Handler Error  {res.Message}");
            }

            return default;
        }


        public static async Task<IEnumerable<T>> GetListAsync<T>(string domain, string path,
            Dictionary<string, object> queries,
            Dictionary<string, string> headerDict = null,
            int warnMiSeconds = 200, int timeOut = 1000 * 60 * 20,
            bool isThrow = false) //where T : new()
        {
            var res = await GetAsync<ResponseResult<IEnumerable<T>>>(domain, path, queries, headerDict, warnMiSeconds, timeOut);
            if (res.Code == 0)
            {
                return res.Data;
            }

            if (isThrow)
            {
                throw new Exception($"GetListAsync Handler Error  {res.Message}");
            }

            return default;
        }

        public static async Task<IEnumerable<T>> GetListAsync<T>(string domain, string path,
            object queries,
            Dictionary<string, string> headerDict = null,
            int warnMiSeconds = 200, int timeOut = 1000 * 60 * 20,
            bool isThrow = false) //where T : new()
        {

            var queryDict = queries.GenQueryDict();
            return await GetListAsync<T>(domain, path, queryDict, headerDict, warnMiSeconds, timeOut, isThrow);


        }

        public static IEnumerable<T> GetList<T>(string domain, string path,
          object queries,
            Dictionary<string, string> headerDict = null,
            int warnMiSeconds = 200, int timeOut = 1000 * 60 * 20,
            bool isThrow = false) //where T : new()
        {

            var queryDict = queries.GenQueryDict();
            return GetList<T>(domain, path, queryDict, headerDict, warnMiSeconds, timeOut, isThrow);


        }


        public static IEnumerable<T> GetList<T>(string domain, string path,
            Dictionary<string, object> queries,
            Dictionary<string, string> headerDict = null,
            int warnMiSeconds = 200, int timeOut = 1000 * 60 * 20,
            bool isThrow = false) //where T : new()
        {
            var res = Get<ResponseResult<IEnumerable<T>>>(domain, path, queries, headerDict, warnMiSeconds, timeOut);
            if (res.Code == 0)
            {
                return res.Data;
            }

            if (isThrow)
            {
                throw new Exception($"GetList Handler Error  {res.Message}");
            }

            return default;

        }



        public static async Task<IEnumerable<T>> PostListAsync<T>(string domain, string path, object bodyObjects,
            Dictionary<string, string> headerDict = null, int warnMiSeconds = 200,
            int timeOut = 1000 * 60 * 20, bool isThrow = false) //where T : new()
        {
            var res = await PostAsync<ResponseResult<IEnumerable<T>>>(domain, path, bodyObjects, headerDict, warnMiSeconds, timeOut);
            if (res.Code == 0)
            {
                return res.Data;
            }

            if (isThrow)
            {
                throw new Exception($"PostListAsync Handler Error  {res.Message}");
            }

            return default;


        }

        public static IEnumerable<T> PostList<T>(string domain, string path, object bodyObjects,
            Dictionary<string, string> headerDict = null, int warnMiSeconds = 200,
            int timeOut = 1000 * 60 * 20, bool isThrow = false) //where T : new()
        {

            var res = Post<ResponseResult<IEnumerable<T>>>(domain, path, bodyObjects, headerDict, warnMiSeconds, timeOut);
            if (res.Code == 0)
            {
                return res.Data;
            }

            if (isThrow)
            {
                throw new Exception($"PostList Handler Error  {res.Message}");
            }

            return default;

        }



    }


}
