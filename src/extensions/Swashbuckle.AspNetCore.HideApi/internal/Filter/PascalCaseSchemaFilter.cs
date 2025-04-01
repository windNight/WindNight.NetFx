using System;
using System.Collections.Generic;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Extensions.@internal;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Swashbuckle.AspNetCore.HideApi.@internal
{
    internal class PascalCaseSchemaFilter : ISchemaFilter
    {
        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            if (schema?.Properties != null)
            {
                var newProperties = new Dictionary<string, OpenApiSchema>();
                foreach (var property in schema.Properties)
                {
                    try
                    {

                        // 转换为大写驼峰命名法 
                        var pascalCaseName = ToPascalCase(property.Key);
                        if (newProperties.ContainsKey(pascalCaseName))
                        {
                            Console.WriteLine($"[{context.Type.Name}] duplicate key {property.Key}->{pascalCaseName}");
                            continue;
                        }

                        newProperties.Add(pascalCaseName, property.Value);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($" {ex.Message} ");
                    }
                }

                schema.Properties = newProperties;
            }
        }

        private string ToPascalCase(string s)
        {
            if (s.IsNullOrEmpty()) return s;
            return char.ToUpper(s[0]) + s.Substring(1);
        }
    }
}
