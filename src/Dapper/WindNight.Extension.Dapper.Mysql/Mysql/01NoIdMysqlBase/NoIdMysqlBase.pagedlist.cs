using WindNight.Core;
using WindNight.Core.SQL;
using WindNight.Core.SQL.Abstractions;

namespace WindNight.Extension.Dapper.Mysql
{
    public abstract partial class NoIdMysqlBase<TEntity>
    {

        #region T



        protected virtual IPagedList<T> DbPagedEList<T>(IQueryPageBase pageQueryBase, string whereSql, IDictionary<string, object> paramDict, string orderby = "", string tableName = "", long warnMs = -1L, Action<Exception, string> execErrorHandler = null)
            where T : class, new()
        {

            return DbPagedEList<T>(DbConnectString, pageQueryBase, whereSql, paramDict, orderby, tableName, warnMs, execErrorHandler);
        }


        protected virtual async Task<IPagedList<T>> DbPagedEListAsync<T>(IQueryPageBase pageQueryBase, string whereSql, IDictionary<string, object> paramDict, string orderby = "", string tableName = "", long warnMs = -1L, Action<Exception, string> execErrorHandler = null)
            where T : class, new()
        {
            return await DbPagedEListAsync<T>(DbConnectString, pageQueryBase, whereSql, paramDict, orderby, tableName, warnMs, execErrorHandler);

        }





        protected virtual IPagedList<T> DbPagedEList<T>(string connStr, IQueryPageBase pageQueryBase, string whereSql, IDictionary<string, object> paramDict, string orderby = "", string tableName = "", long warnMs = -1L, Action<Exception, string> execErrorHandler = null)
            where T : class, new()
        {
            if (tableName.IsNullOrEmpty())
            {
                tableName = BaseTableName;
            }
            var pageInfo = pageQueryBase.GenQueryPageInfoForDto<T>(tableName);

            pageInfo.SqlWhere = whereSql;
            if (!orderby.IsNullOrEmpty())
            {
                pageInfo.OrderField = orderby;

            }

            return DbPagedEList<T>(connStr, pageInfo, paramDict, warnMs, execErrorHandler);
        }


        protected virtual async Task<IPagedList<T>> DbPagedEListAsync<T>(string connStr, IQueryPageBase pageQueryBase, string whereSql, IDictionary<string, object> paramDict, string orderby = "", string tableName = "", long warnMs = -1L, Action<Exception, string> execErrorHandler = null)
            where T : class, new()
        {
            if (tableName.IsNullOrEmpty())
            {
                tableName = BaseTableName;
            }
            var pageInfo = pageQueryBase.GenQueryPageInfoForDto<T>(tableName);

            pageInfo.SqlWhere = whereSql;
            if (!orderby.IsNullOrEmpty())
            {
                pageInfo.OrderField = orderby;

            }

            return await DbPagedEListAsync<T>(connStr, pageInfo, paramDict, warnMs, execErrorHandler);

        }



        /// <summary>
        /// </summary>
        /// <param name="pagedInfo"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        protected virtual IPagedList<T> DbPagedEList<T>(IQueryPageInfo pagedInfo, IDictionary<string, object> parameters, long warnMs = -1L, Action<Exception, string> execErrorHandler = null)
            where T : class, new()
        {
            return DbPagedEList<T>(DbConnectString, pagedInfo, parameters, warnMs, execErrorHandler);
        }


        /// <summary>
        ///     异步分页
        /// </summary>
        /// <param name="pagedInfo"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        protected virtual async Task<IPagedList<T>> DbPagedEListAsync<T>(IQueryPageInfo pagedInfo, IDictionary<string, object> parameters, long warnMs = -1L, Action<Exception, string> execErrorHandler = null)
            where T : class, new()
        {
            return await DbPagedEListAsync<T>(DbConnectString, pagedInfo, parameters, warnMs, execErrorHandler);
        }

        /// <summary>
        /// </summary>
        /// <param name="pagedInfo"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        protected virtual IPagedList<T> DbPagedEList<T>(string connStr, IQueryPageInfo pagedInfo, IDictionary<string, object> parameters, long warnMs = -1L, Action<Exception, string> execErrorHandler = null)
            where T : class, new()
        {
            return QueryPagedList<T>(connStr, pagedInfo, parameters, warnMs, execErrorHandler);
        }


        /// <summary>
        ///     异步分页
        /// </summary>
        /// <param name="pagedInfo"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        protected virtual async Task<IPagedList<T>> DbPagedEListAsync<T>(string connStr, IQueryPageInfo pagedInfo, IDictionary<string, object> parameters, long warnMs = -1L, Action<Exception, string> execErrorHandler = null)
            where T : class, new()
        {
            return await QueryPagedListAsync<T>(connStr, pagedInfo, parameters, warnMs, execErrorHandler);
        }

        #endregion


        #region TEntity


        protected IPagedList<TEntity> DbPagedList(IQueryPageBase pageQueryBase, string whereSql, IDictionary<string, object> paramDict = null, string orderby = "", bool tableNameToLower = true, bool tableNameAppendPlural = true, long warnMs = -1L, Action<Exception, string> execErrorHandler = null)
        {

            return DbPagedList(DbConnectString, pageQueryBase, whereSql, paramDict, orderby, tableNameToLower, tableNameAppendPlural, warnMs, execErrorHandler);

        }

        protected virtual async Task<IPagedList<TEntity>> DbPagedListAsync(IQueryPageBase pageQueryBase, string whereSql, IDictionary<string, object> paramDict = null, string orderby = "", bool tableNameToLower = true, bool tableNameAppendPlural = true, long warnMs = -1L, Action<Exception, string> execErrorHandler = null)
        {

            return await DbPagedListAsync(DbConnectString, pageQueryBase, whereSql, paramDict, orderby,
                tableNameToLower, tableNameAppendPlural, warnMs, execErrorHandler);

        }

        protected IPagedList<TEntity> DbPagedList(string connStr, IQueryPageBase pageQueryBase, string whereSql, IDictionary<string, object> paramDict = null, string orderby = "", bool tableNameToLower = true, bool tableNameAppendPlural = true, long warnMs = -1L, Action<Exception, string> execErrorHandler = null)
        {
            var pageInfo = pageQueryBase.GenQueryPageInfo<TEntity>(tableNameToLower, tableNameAppendPlural);

            pageInfo.SqlWhere = whereSql;
            if (!orderby.IsNullOrEmpty())
            {
                pageInfo.OrderField = orderby;

            }

            return DbPagedList(connStr, pageInfo, paramDict, warnMs, execErrorHandler);

        }

        protected virtual async Task<IPagedList<TEntity>> DbPagedListAsync(string connStr, IQueryPageBase pageQueryBase, string whereSql, IDictionary<string, object> paramDict = null, string orderby = "", bool tableNameToLower = true, bool tableNameAppendPlural = true, long warnMs = -1L, Action<Exception, string> execErrorHandler = null)
        {
            var pageInfo = pageQueryBase.GenQueryPageInfo<TEntity>(tableNameToLower, tableNameAppendPlural);

            pageInfo.SqlWhere = whereSql;
            if (!orderby.IsNullOrEmpty())
            {
                pageInfo.OrderField = orderby;

            }

            return await DbPagedListAsync(connStr, pageInfo, paramDict, warnMs, execErrorHandler);

        }



        /// <summary>
        ///     异步分页
        /// </summary>
        /// <param name="pagedInfo"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        protected virtual async Task<IPagedList<TEntity>> DbPagedListAsync(IQueryPageInfo pagedInfo, IDictionary<string, object> parameters = null, long warnMs = -1L, Action<Exception, string> execErrorHandler = null)
        {
            return await DbPagedListAsync(DbConnectString, pagedInfo, parameters, warnMs, execErrorHandler);
        }

        /// <summary>
        /// </summary>
        /// <param name="pagedInfo"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        protected virtual IPagedList<TEntity> DbPagedList(IQueryPageInfo pagedInfo, IDictionary<string, object> parameters, long warnMs = -1L, Action<Exception, string> execErrorHandler = null)
        {
            return DbPagedList(DbConnectString, pagedInfo, parameters, warnMs, execErrorHandler);
        }


        /// <summary>
        ///     异步分页
        /// </summary>
        /// <param name="pagedInfo"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        protected virtual async Task<IPagedList<TEntity>> DbPagedListAsync(string connStr, IQueryPageInfo pagedInfo, IDictionary<string, object> parameters = null, long warnMs = -1L, Action<Exception, string> execErrorHandler = null)
        {
            return await QueryPagedListAsync<TEntity>(connStr, pagedInfo, parameters, warnMs, execErrorHandler);
        }

        /// <summary>
        /// </summary>
        /// <param name="pagedInfo"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        protected virtual IPagedList<TEntity> DbPagedList(string connStr, IQueryPageInfo pagedInfo, IDictionary<string, object> parameters, long warnMs = -1L, Action<Exception, string> execErrorHandler = null)
        {
            return QueryPagedList<TEntity>(connStr, pagedInfo, parameters, warnMs, execErrorHandler);
        }


        #endregion



    }


    /// <summary>
    ///   Obsolete
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public abstract partial class NoIdMysqlBase<TEntity>
    {

        #region Obsolete

        [Obsolete("Please Use IQueryPageBase or IQueryPageInfo ")]
        protected virtual IPagedList<T> DbPagedEList<T>(int pageIndex, int pageSize, string condition, string orderBy, IDictionary<string, object> parameters = null, string queryTableName = "", long warnMs = -1L, Action<Exception, string> execErrorHandler = null)
            where T : class, new()
        {
            var pagedInfo = new QueryPageInfo
            {
                TableName = queryTableName.IsNullOrEmpty() ? BaseTableName : queryTableName,
                Fields = "*",
                SqlWhere = condition,
                OrderField = orderBy,
                PageIndex = pageIndex,
                PageSize = pageSize,
            };

            return DbPagedEList<T>(pagedInfo, parameters, warnMs, execErrorHandler);
        }


        /// <summary>
        ///     异步分页
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="condition"></param>
        /// <param name="orderBy"></param>
        /// <param name="parameters"></param>
        /// <param name="queryTableName">查询的表或者临时表 ,NullOrEmpty=><see cref="BaseTableName" /></param>
        /// <returns></returns>
        [Obsolete("Please Use IQueryPageBase or IQueryPageInfo ")]
        protected virtual async Task<IPagedList<T>> DbPagedEListAsync<T>(int pageIndex, int pageSize, string condition, string orderBy, IDictionary<string, object> parameters = null, string queryTableName = "", long warnMs = -1L, Action<Exception, string> execErrorHandler = null)
            where T : class, new()
        {
            var pagedInfo = new QueryPageInfo
            {
                TableName = queryTableName.IsNullOrEmpty() ? BaseTableName : queryTableName,
                Fields = "*",
                SqlWhere = condition,
                OrderField = orderBy,
                PageIndex = pageIndex,
                PageSize = pageSize,
            };

            return await DbPagedEListAsync<T>(pagedInfo, parameters, warnMs, execErrorHandler);
        }

        /// <summary>
        ///     分页
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="condition"></param>
        /// <param name="orderBy"></param>
        /// <param name="queryTableName">查询的表或者临时表 ,NullOrEmpty=><see cref="BaseTableName" /></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        [Obsolete("Please Use IQueryPageBase or IQueryPageInfo ")]
        protected virtual IPagedList<TEntity> DbPagedList(int pageIndex, int pageSize, string condition, string orderBy, IDictionary<string, object> parameters = null, string queryTableName = "", long warnMs = -1L, Action<Exception, string> execErrorHandler = null)
        {
            var pagedInfo = new QueryPageInfo
            {
                TableName = queryTableName.IsNullOrEmpty() ? BaseTableName : queryTableName,
                Fields = "*",
                SqlWhere = condition,
                OrderField = orderBy,
                PageIndex = pageIndex,
                PageSize = pageSize,
            };

            return DbPagedList(pagedInfo, parameters, warnMs, execErrorHandler);
        }

        /// <summary>
        ///     异步分页
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="condition"></param>
        /// <param name="orderBy"></param>
        /// <param name="parameters"></param>
        /// <param name="queryTableName">查询的表或者临时表 ,NullOrEmpty=><see cref="BaseTableName" /></param>
        /// <returns></returns>
        [Obsolete("Please Use IQueryPageBase or IQueryPageInfo ")]
        protected virtual async Task<IPagedList<TEntity>> DbPagedListAsync(int pageIndex, int pageSize, string condition, string orderBy, IDictionary<string, object> parameters = null, string queryTableName = "", long warnMs = -1L, Action<Exception, string> execErrorHandler = null)
        {
            var pagedInfo = new QueryPageInfo
            {
                TableName = queryTableName.IsNullOrEmpty() ? BaseTableName : queryTableName,
                Fields = "*",
                SqlWhere = condition,
                OrderField = orderBy,
                PageIndex = pageIndex,
                PageSize = pageSize,
            };

            return await DbPagedListAsync(pagedInfo, parameters, warnMs, execErrorHandler);
        }


        #endregion


    }
}
