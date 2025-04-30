using WindNight.Core;
using WindNight.Core.SQL;
using WindNight.Core.SQL.Abstractions;

namespace WindNight.Extension.Dapper.Mysql
{
    public abstract partial class NoIdMysqlBase<TEntity>
    {

        #region T


        protected virtual IPagedList<T> DbPagedEList<T>(int pageIndex, int pageSize, string condition, string orderBy, IDictionary<string, object> parameters = null, string queryTableName = "", long warnMs = -1, Action<Exception, string> execErrorHandler = null)
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
        protected virtual async Task<IPagedList<T>> DbPagedEListAsync<T>(int pageIndex, int pageSize, string condition, string orderBy, IDictionary<string, object> parameters = null, string queryTableName = "", long warnMs = -1, Action<Exception, string> execErrorHandler = null)
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
        /// </summary>
        /// <param name="pagedInfo"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        protected virtual IPagedList<T> DbPagedEList<T>(IQueryPageInfo pagedInfo, IDictionary<string, object> parameters, long warnMs = -1, Action<Exception, string> execErrorHandler = null)
            where T : class, new()
        {
            return PagedList<T>(DbConnectString, pagedInfo, parameters, warnMs, execErrorHandler);
        }


        /// <summary>
        ///     异步分页
        /// </summary>
        /// <param name="pagedInfo"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        protected virtual async Task<IPagedList<T>> DbPagedEListAsync<T>(IQueryPageInfo pagedInfo, IDictionary<string, object> parameters, long warnMs = -1, Action<Exception, string> execErrorHandler = null)
            where T : class, new()
        {
            return await PagedListAsync<T>(DbConnectString, pagedInfo, parameters, warnMs, execErrorHandler);
        }



        protected virtual IPagedList<T> DbPagedEList<T>(IQueryPageBase pageQueryBase, string whereSql, IDictionary<string, object> paramDict, string orderby = "", bool tableNameToLower = true, bool tableNameAppendPlural = true, long warnMs = -1, Action<Exception, string> execErrorHandler = null)
            where T : class, ICreateEntityBase, new()
        {
            var pageInfo = pageQueryBase.GenQueryPageInfoForCreateEntity<T>(tableNameToLower, tableNameAppendPlural);

            pageInfo.SqlWhere = whereSql;
            if (!orderby.IsNullOrEmpty())
            {
                pageInfo.OrderField = orderby;

            }
            return DbPagedEList<T>(pageInfo, paramDict, warnMs, execErrorHandler);
        }

        protected virtual async Task<IPagedList<T>> DbPagedEListAsync<T>(IQueryPageBase pageQueryBase, string whereSql, IDictionary<string, object> paramDict, string orderby = "", bool tableNameToLower = true, bool tableNameAppendPlural = true, long warnMs = -1, Action<Exception, string> execErrorHandler = null)
            where T : class, ICreateEntityBase, new()
        {
            var pageInfo = pageQueryBase.GenQueryPageInfoForCreateEntity<T>(tableNameToLower, tableNameAppendPlural);

            pageInfo.SqlWhere = whereSql;
            if (!orderby.IsNullOrEmpty())
            {
                pageInfo.OrderField = orderby;

            }

            return await DbPagedEListAsync<T>(pageInfo, paramDict, warnMs, execErrorHandler);

        }

        #endregion



        #region TEntity

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
        protected virtual IPagedList<TEntity> DbPagedList(int pageIndex, int pageSize, string condition, string orderBy, IDictionary<string, object> parameters = null, string queryTableName = "", long warnMs = -1, Action<Exception, string> execErrorHandler = null)
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

            return DbPagedList<TEntity>(pagedInfo, parameters, warnMs, execErrorHandler);
        }


        /// <summary>
        /// </summary>
        /// <param name="pagedInfo"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        protected virtual IPagedList<TEntity> DbPagedList(IQueryPageInfo pagedInfo, IDictionary<string, object> parameters, long warnMs = -1, Action<Exception, string> execErrorHandler = null)
        {
            return PagedList<TEntity>(DbConnectString, pagedInfo, parameters, warnMs, execErrorHandler);
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
        protected virtual async Task<IPagedList<TEntity>> DbPagedListAsync(int pageIndex, int pageSize, string condition, string orderBy, IDictionary<string, object> parameters = null, string queryTableName = "", long warnMs = -1, Action<Exception, string> execErrorHandler = null)
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

            return await DbPagedListAsync<TEntity>(pagedInfo, parameters, warnMs, execErrorHandler);
        }

        /// <summary>
        ///     异步分页
        /// </summary>
        /// <param name="pagedInfo"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        protected virtual async Task<IPagedList<TEntity>> DbPagedListAsync(IQueryPageInfo pagedInfo, IDictionary<string, object> parameters, long warnMs = -1, Action<Exception, string> execErrorHandler = null)
        {
            return await PagedListAsync<TEntity>(DbConnectString, pagedInfo, parameters, warnMs, execErrorHandler);
        }

        protected IPagedList<TEntity> DbPagedList(IQueryPageBase pageQueryBase, string whereSql, IDictionary<string, object> paramDict, string orderby = "", bool tableNameToLower = true, bool tableNameAppendPlural = true, long warnMs = -1, Action<Exception, string> execErrorHandler = null)
        {
            var pageInfo = pageQueryBase.GenQueryPageInfo<TEntity>(tableNameToLower, tableNameAppendPlural);

            pageInfo.SqlWhere = whereSql;
            if (!orderby.IsNullOrEmpty())
            {
                pageInfo.OrderField = orderby;

            }

            return DbPagedList(pageInfo, paramDict, warnMs, execErrorHandler);

        }

        protected virtual async Task<IPagedList<TEntity>> DbPagedListAsync(IQueryPageBase pageQueryBase, string whereSql, IDictionary<string, object> paramDict, string orderby = "", bool tableNameToLower = true, bool tableNameAppendPlural = true, long warnMs = -1, Action<Exception, string> execErrorHandler = null)
        {
            var pageInfo = pageQueryBase.GenQueryPageInfo<TEntity>(tableNameToLower, tableNameAppendPlural);

            pageInfo.SqlWhere = whereSql;
            if (!orderby.IsNullOrEmpty())
            {
                pageInfo.OrderField = orderby;

            }

            return await DbPagedListAsync(pageInfo, paramDict, warnMs, execErrorHandler);

        }
        #endregion


    }
}
