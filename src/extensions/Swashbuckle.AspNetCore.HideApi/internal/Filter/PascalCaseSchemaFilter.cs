using Microsoft.OpenApi;
//using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Extensions.@internal;
using Swashbuckle.AspNetCore.SwaggerGen;
using WindNight.Linq.Extensions.Expressions;

namespace Swashbuckle.AspNetCore.HideApi.@internal
{
    internal class PascalCaseSchemaFilter : ISchemaFilter
    {
         
        //public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        //{
        //    if (schema?.Properties != null)
        //    {
        //        var newProperties = new Dictionary<string, OpenApiSchema>();
        //        //var newProperties11 = new Dictionary<string, OpenApiSchema>();
        //        foreach (var property in schema.Properties)
        //        {
        //            try
        //            {
        //                // 转换为大写驼峰命名法 
        //                var pascalCaseName = ToPascalCase(property.Key);
        //                if (newProperties.ContainsKey(pascalCaseName))
        //                {
        //                    Console.WriteLine($"[{context.Type.Name}] duplicate key {property.Key}->{pascalCaseName}");
        //                    continue;
        //                }

        //                newProperties.Add(pascalCaseName, property.Value);
        //            }
        //            catch (Exception ex)
        //            {
        //                Console.WriteLine($" {ex.Message} ");
        //            }
        //        }

        //        schema.Properties = newProperties;

        //    }
        //}
     
        public void Apply(IOpenApiSchema schema, SchemaFilterContext context)
        {

            if (schema is not OpenApiSchema concreteSchema)
            {
                return;
            }

            if (concreteSchema.Properties.IsNotNullOrEmpty())
            {
                var newProperties = new Dictionary<string, IOpenApiSchema>();

                foreach (var property in concreteSchema.Properties)
                {
                    try
                    {
                        // 转换为大写驼峰命名法（PascalCase）
                        var pascalCaseName = ToPascalCase(property.Key);

                        if (newProperties.ContainsKey(pascalCaseName))
                        {
                            $"[{context.Type.Name}] duplicate key {property.Key}->{pascalCaseName}".Log2Console();
                            continue;
                        }

                        // 确保 property.Value 是 OpenApiSchema 类型
                        if (property.Value is OpenApiSchema propertySchema)
                        {
                            newProperties.Add(pascalCaseName, propertySchema);
                        }
                        else if (property.Value != null)
                        {
                            // 如果是 IOpenApiSchema 接口类型，尝试转换
                            newProperties.Add(pascalCaseName, property.Value as OpenApiSchema ?? new OpenApiSchema());
                        }
                    }
                    catch (Exception ex)
                    {
                        $"Error processing property {property.Key}: {ex.Message}".Log2Console(ex);
                    }
                }

                // 重新赋值 Properties
                concreteSchema.Properties = newProperties;
            }
        }

        private string ToPascalCase(string s)
        {
            if (s.IsNullOrEmpty())
            {
                return s;
            }
            return char.ToUpper(s[0]) + s.Substring(1);
        }


     
    }
}
