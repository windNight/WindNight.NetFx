using System.Text;
using System.Text.RegularExpressions;
using WindNight.Core;
using WindNight.Core.SQL.Abstractions;

namespace WindNight.Extension.Db.Extensions
{
    /// <summary>
    ///     用于非Dapper时使用自动根据sql语句生成对应的SqlParameter数组
    ///     其中ParameterName 默认不区分大小写 统一转大写
    /// </summary>
    public static class SqlParameterExtension
    {
        /// <summary>
        ///     过滤参数的规则
        /// </summary>
        private static readonly Regex Reg = new(@"@\S{1,}?(,|\s|;|--|\)|$)");

        private static readonly char[] FilterChars = { ' ', ',', ';', '-', ')' };

        /// <summary>
        ///     不区分大小写  自动转大写
        /// </summary>
        /// <param name="originSqlString"></param>
        /// <param name="isIgnoreCase">参数key 是否区分大小写 默认为 true.</param>
        /// <returns></returns>
        private static List<string> GetMatchedKeys(this string originSqlString, bool isIgnoreCase = true)
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
        ///     入参转 可执行的 sql 参数
        /// </summary>
        /// <param name="instance"></param>
        public static string ToParamString(this IEntity instance)
        {
            try
            {


                var sb = new StringBuilder($" -- {instance.GenDefaultTableName()} {Environment.NewLine}");
                sb.AppendLine(" SELECT ");
                if (instance != null)
                {
                    var properties = instance.GetType().GetProperties();
                    foreach (var property in properties)
                    {
                        var value = property.GetValue(instance, null);
                        var stringType = new[] { "String", "DateTime", };
                        //if (property.PropertyType.Name == "String" || property.PropertyType.Name == "DateTime")
                        if (stringType.Contains(property.PropertyType.Name))
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
            catch
            {
                return "";
            }
        }
    }
}
