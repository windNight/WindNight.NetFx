using System;
using System.Collections.Generic;
using System.ComponentModel;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Extensions.@internal;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Swashbuckle.AspNetCore.HideApi
{
    public class CustomResponseOperationFilter : IOperationFilter
    {
        private readonly Dictionary<string, string> _resDict;

        public CustomResponseOperationFilter(Dictionary<string, string> resDict)
        {
            _resDict = resDict;
        }

        protected virtual Dictionary<string, string> DefaultResDict { get; } = new();

        protected virtual Dictionary<string, string> CurrentResKeyDict
        {
            get
            {
                try
                {
                    if (!DefaultResDict.IsNullOrEmpty())
                    {
                        return DefaultResDict;
                    }

                    if (!_resDict.IsNullOrEmpty())
                    {
                        return _resDict;
                    }

                    var signKeyDict = ConfigItems.SwaggerConfigs.ResConfigs;
                    return signKeyDict;
                }
                catch (Exception)
                {
                    return new Dictionary<string, string>();
                }
            }
        }

        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if (CurrentResKeyDict.IsNullOrEmpty())
            {
                return;
            }
            var schemaId = "SwaggerCustomResponse";
            var flag = context.SchemaRepository.TryLookupByType(typeof(SwaggerCustomResponse), out var schema);
            if (!flag)
            {
                context.SchemaRepository.RegisterType(typeof(SwaggerCustomResponse), schemaId);
            }
            context.SchemaRepository.TryLookupByType(typeof(SwaggerCustomResponse), out var resSchema);

            foreach (var item in CurrentResKeyDict)
            {
                var key = item.Key;
                try
                {
                    if (key == "0" || key == "100200")
                    {
                        var m = operation.Responses["200"];
                        operation.Responses.Add(key, m);
                        operation.Responses.Remove("200");
                    }
                    else
                    {
                        if (!operation.Responses.ContainsKey(key))
                        {


                            operation.Responses.Add(key,
                                new OpenApiResponse
                                {
                                    Description = item.Value,
                                    Content = new Dictionary<string, OpenApiMediaType>
                                    {
                                        { "application/json", new OpenApiMediaType { Schema = resSchema } },
                                    },
                                });
                            //operation.Responses.Add(key, new OpenApiResponse { Description = item.Value });
                        }
                    }
                }
                catch
                {
                }
            }
        }
    }

    public enum SwaggerCustomStatusCodeEnum
    {
        [Description("Ok")] Ok = 0,

        [Description("BadRequest")] BadRequest = 100400,

        [Description("Unauthorized")] Unauthorized = 100401,

        [Description("NotFound")] NotFound = 100404,

        [Description("SystemError")] SystemError = 100500,
    }

    public class SwaggerCustomResponse
    {
        // [JsonProperty("code")]
        public int Code { get; set; }

        //[JsonProperty("message")]
        public string Message { get; set; }

        //// [JsonProperty("data")]
        //public object Data { get; set; }
    }
}
