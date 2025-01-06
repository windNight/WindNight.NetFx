using WindNight.Core.SQL.Abstractions;

namespace WindNight.Core.SQL
{


    public class QueryPageBase : IQueryPageBase
    {
        /// <summary>
        ///     指定当前为第几页
        /// </summary>
        public int PageIndex { get; set; }

        /// <summary>
        ///     每页多少条记录
        /// </summary>
        public int PageSize { get; set; }

        /// <summary> 从第几页开始 默认 1 大部分都是1   </summary>
        [Newtonsoft.Json.JsonIgnore]
        [System.Text.Json.Serialization.JsonIgnore]
        public int IndexFrom { get; set; } = 1;

    }

    public class QueryPageInfo : QueryPageBase, IQueryPageInfo
    {
        /// <summary>
        ///     分页信息实例
        /// </summary>
        public QueryPageInfo()
        {
        }


        /// <summary>
        ///     分页信息实例
        /// </summary>
        /// <param name="tableName">表名(多表连接表名实例："test1 as a left join test2 as b on a.cid=b.cid")</param>
        /// <param name="fields">字段名(全部字段为*)</param>
        /// <param name="sqlWhere">条件语句(不用加where)</param>
        /// <param name="orderField">排序字段(必须需要!支持多字段，不用加order by)</param>
        /// <param name="pageInfo"> <see cref="IQueryPageBase"/> </param>
        public QueryPageInfo(string tableName, string fields, string sqlWhere, string orderField, IQueryPageBase pageInfo) : this(pageInfo)
        {
            TableName = tableName;
            Fields = fields;
            SqlWhere = sqlWhere;
            OrderField = orderField;
        }

        /// <summary>
        ///     分页信息实例
        /// </summary> 
        /// <param name="pageInfo"> <see cref="IQueryPageBase"/> </param>
        public QueryPageInfo(IQueryPageBase pageInfo)
        {
            PageIndex = pageInfo.PageIndex;
            PageSize = pageInfo.PageSize;
        }

        /// <summary>
        ///     分页信息实例
        /// </summary> 
        /// <param name="pageInfo"> <see cref="IQueryPageBase"/> </param>
        public static QueryPageInfo GenQueryPageInfoForCreateEntity<TEntity>(IQueryPageBase pageInfo)
            where TEntity : class, ICreateEntityBase, IEntity, new()

        {
            return pageInfo.GenQueryPageInfoForCreateEntity<TEntity>();
            //return new QueryPageInfo(pageInfo)
            //{
            //    TableName = pageInfo.GenDefaultTableName<TEntity>(),
            //    Fields = "*",
            //    OrderField = "CreateUnixTime DESC"
            //};


        }

        /// <summary>
        ///     分页信息实例
        /// </summary> 
        /// <param name="pageInfo"> <see cref="IQueryPageBase"/> </param>
        public static QueryPageInfo GenQueryPageInfo<TEntity>(IQueryPageBase pageInfo)
            where TEntity : class, ICanPageEntity, IEntity, new()

        {
            return pageInfo.GenQueryPageInfo<TEntity>();
            //return new QueryPageInfo(pageInfo)
            //{
            //    TableName = pageInfo.GenDefaultTableName<TEntity>(),
            //    Fields = "*",
            //    OrderField = "Id DESC"
            //};
        }


        /// <summary>
        ///     分页信息实例
        /// </summary>
        /// <param name="tableName">表名(多表连接表名实例："test1 as a left join test2 as b on a.cid=b.cid")</param>
        /// <param name="fields">字段名(全部字段为*)</param>
        /// <param name="sqlWhere">条件语句(不用加where)</param>
        /// <param name="orderField">排序字段(必须需要!支持多字段，不用加order by)</param>
        /// <param name="pageIndex">每页多少条记录</param>
        /// <param name="pageSize">指定当前为第几页</param>
        public QueryPageInfo(string tableName, string fields, string sqlWhere, string orderField, int pageIndex,
            int pageSize)
        {
            TableName = tableName;
            Fields = fields;
            SqlWhere = sqlWhere;
            OrderField = orderField;
            PageIndex = pageIndex;
            PageSize = pageSize;
        }


        /// <summary>
        ///     表名(多表连接表名实例："test1 as a left join test2 as b on a.cid=b.cid")
        /// </summary>
        public string TableName { get; set; } = string.Empty;

        /// <summary>
        ///     字段名(全部字段为*)
        /// </summary>
        public string Fields { get; set; } = "*";

        /// <summary>
        ///     条件语句(不用加where)
        /// </summary>
        public string SqlWhere { get; set; } = string.Empty;

        /// <summary>
        ///     排序字段(必须需要!支持多字段，不用加order by)
        /// </summary>
        public string OrderField { get; set; } = string.Empty;

    }
}