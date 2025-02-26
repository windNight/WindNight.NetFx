using WindNight.Core.SQL;
using WindNight.Core.SQL.Abstractions;

namespace WindNight.Core
{
    public static class PageInfoEx
    {
        /// <summary>
        ///     分页信息实例
        /// </summary>
        /// <param name="pageInfo">
        ///     <see cref="IQueryPageBase" />
        /// </param>
        /// <param name="tableNameToLower">   </param>
        /// <param name="tableNameAppendPlural">   </param>
        public static QueryPageInfo GenQueryPageInfoForCreateEntity<TEntity>(this IQueryPageBase pageInfo,
            bool tableNameToLower = true, bool tableNameAppendPlural = false)
            where TEntity : class, ICreateEntityBase, IEntity, new()

        {
            return new QueryPageInfo(pageInfo)
            {
                TableName = pageInfo.GenDefaultTableName<TEntity>(tableNameToLower, tableNameAppendPlural),
                Fields = "*",
                OrderField = "CreateUnixTime DESC",
            };
        }

        /// <summary>
        ///     分页信息实例
        /// </summary>
        /// <param name="pageInfo">
        ///     <see cref="IQueryPageBase" />
        /// </param>
        /// <param name="tableNameToLower">   </param>
        /// <param name="tableNameAppendPlural">   </param>
        public static QueryPageInfo GenQueryPageInfo<TEntity>(this IQueryPageBase pageInfo,
            bool tableNameToLower = true, bool tableNameAppendPlural = false)
            where TEntity : class, ICanPageEntity, IEntity, new()

        {
            return new QueryPageInfo(pageInfo)
            {
                TableName = pageInfo.GenDefaultTableName<TEntity>(tableNameToLower, tableNameAppendPlural),
                Fields = "*",
                OrderField = "Id DESC",
            };
        }

        public static string GenDefaultTableName<TEntity>(this object t, bool toLower = true, bool appendPlural = false)
            where TEntity : class, IEntity, new()
        {
            var tableName = typeof(TEntity).Name;
            if (toLower)
            {
                tableName = tableName.ToLower();
            }

            if (appendPlural && !tableName.EndsWith("s"))
            {
                tableName = $"{tableName}s";
            }

            return tableName;
        }
    }
}
