using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json.Extension;
using WindNight.Core.SQL.Abstractions;
using WindNight.Linq.Extensions.Expressions;

namespace WindNight.Extension.SqlClient.Extensions
{
    /// <summary>
    /// 用于非Dapper时使用自动根据sql语句生成对应的SqlParameter数组
    /// 其中ParameterName 默认不区分大小写 统一转大写
    /// </summary>
    public static class SqlParameterExtension
    {
        /// <summary>
        /// 过滤参数的规则
        /// </summary>
        private static readonly Regex Reg = new(@"@\S{1,}?(,|\s|;|--|\)|$)");

        private static readonly char[] FilterChars = { ' ', ',', ';', '-', ')' };

        /// <summary>
        /// 不区分大小写  自动转大写
        /// </summary>
        /// <param name="originSqlString"></param>
        /// <param name="isIgnoreCase">参数key 是否区分大小写 默认为 true.</param>
        /// <returns></returns>
        static List<string> GetMatchedKeys(this string originSqlString, bool isIgnoreCase = true)
        {
            // TODO 可以优化
            var listStr = new List<string>();
            var myMatch = Reg.Match(originSqlString);
            while (myMatch.Success)
            {
                var key = myMatch.Value.TrimEnd(FilterChars).TrimStart('@').Trim('\r').TrimEnd('\n');
                if (isIgnoreCase)
                {
                    key = key.ToUpper();
                }
                listStr.Add(key);
                myMatch = myMatch.NextMatch();
            }

            return listStr.Distinct().ToList();
        }

        /// <summary>
        /// 根据sql语句和实体对象自动生成参数化查询SqlParameter列表
        /// <see cref="T"/> 的属性不区分大小写
        /// </summary>
        /// <typeparam name="T">实体对象类型</typeparam>
        /// <param name="sqlStr">sql语句</param>
        /// <param name="obj">实体对象</param>
        /// <param name="isIgnoreCase">参数key(<see cref="SqlParameter.ParameterName"/> of <see cref="SqlParameter"/>)是否区分大小写 默认为 true.</param>
        /// <returns> List of <see cref="SqlParameter"/></returns>
        public static List<SqlParameter> GeneratorParams<T>(this string sqlStr, T obj, bool isIgnoreCase = true)
        where T : class
        {
            var parameters = new List<SqlParameter>();

            var listStr = sqlStr.GetMatchedKeys(isIgnoreCase);
            if (listStr.IsNullOrEmpty())
            {
                return parameters;
            }

            var t = typeof(T);
            var propInfo = t.GetProperties();
            foreach (var item in listStr)
            {
                var hitProp = propInfo.FirstOrDefault(m => m.Name.Equals(item, StringComparison.OrdinalIgnoreCase));
                if (hitProp != null)
                {
                    parameters.Add(new SqlParameter { ParameterName = $"@{item}", Value = hitProp.GetValue(obj, null) });
                }
                else
                {
                    throw new Exception($"查询参数[@{item}]在类型[{t.Name}({obj.ToJsonStr()})]中未找到赋值属性");
                }


            }

            return parameters;

        }

        /// <summary>
        /// 根据sql语句和实体对象自动生成参数化查询SqlParameter列表
        /// obj的属性不区分大小写
        /// </summary>
        /// <param name="sqlStr">sql语句</param>
        /// <param name="obj">实体对象</param>
        /// <param name="isIgnoreCase">参数key(<see cref="SqlParameter.ParameterName"/> of <see cref="SqlParameter"/>)是否区分大小写 默认为 true.</param>
        /// <returns> List of <see cref="SqlParameter"/></returns>
        public static List<SqlParameter> GeneratorParams(this string sqlStr, object obj, bool isIgnoreCase = true)
        {
            var parameters = new List<SqlParameter>();
            var listStr = sqlStr.GetMatchedKeys(isIgnoreCase);
            if (listStr.IsNullOrEmpty())
            {
                return parameters;
            }

            var t = obj.GetType();

            var propInfo = t.GetProperties();

            foreach (var item in listStr)
            {
                var hitProp = propInfo.FirstOrDefault(m => m.Name.Equals(item, StringComparison.OrdinalIgnoreCase));
                if (hitProp != null)
                {
                    parameters.Add(new SqlParameter { ParameterName = $"@{item}", Value = hitProp.GetValue(obj, null) });
                }
                else
                {
                    throw new Exception($"查询参数[@{item}]在类型[{t.Name}({obj.ToJsonStr()})]中未找到赋值属性");
                }


            }

            return parameters;
        }

        /// <summary>
        /// 根据sql语句和ExpandoObject对象自动生成参数化查询SqlParameter列表
        /// obj的key 不区分大小写
        /// </summary>
        /// <param name="sqlStr">sql语句</param>
        /// <param name="obj">ExpandoObject对象</param>
        /// <param name="isIgnoreCase">参数key(<see cref="SqlParameter.ParameterName"/> of <see cref="SqlParameter"/>)是否区分大小写 默认为 true.</param>
        /// <returns> List of <see cref="SqlParameter"/></returns>
        public static List<SqlParameter> GeneratorParams(this string sqlStr, ExpandoObject obj, bool isIgnoreCase = true)
        {
            var parameters = new List<SqlParameter>();
            var listStr = sqlStr.GetMatchedKeys(isIgnoreCase);
            if (listStr.IsNullOrEmpty())
            {
                return parameters;
            }
            foreach (var item in listStr)
            {
                var reachCount = 0;
                // GetMatchedKeys 已经被转换成大写 所以这里只能遍历字典key 
                foreach (var property in obj)
                {
                    if (item.Equals(property.Key, StringComparison.OrdinalIgnoreCase))
                    {
                        var key = property.Key;
                        parameters.Add(new SqlParameter { ParameterName = $"@{key}", Value = property.Value });
                        break;
                    }
                    else
                    {
                        if (reachCount == obj.Count() - 1)
                        {
                            throw new Exception($"查询参数[@{item}]在类型[ExpandoObject({obj.ToJsonStr()})]中未找到赋值属性");
                        }
                    }
                    reachCount++;
                }
            }

            return parameters;

        }

        /// <summary>
        ///  入参转 可执行的 sql 参数
        /// </summary>
        /// <param name="instance"></param>
        public static string ToParamString(this IEntity instance)
        {
            var sb = new StringBuilder("SELECT ");
            if (instance != null)
            {
                var properties = instance.GetType().GetProperties();
                foreach (var property in properties)
                {
                    var value = property.GetValue(instance, null);
                    if (property.PropertyType.Name == "String")
                    {
                        sb.Append($"@{property.Name}:='{value}',");
                    }
                    else
                    {
                        sb.Append($"@{property.Name}:={value},");

                    }
                }
            }

            return sb.ToString().TrimEnd(',');
        }

    }
}
