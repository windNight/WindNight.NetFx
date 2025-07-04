using WindNight.Core.SQL.Abstractions;
using WindNight.Extension.Db.Abstractions;

namespace WindNight.Extension.Dapper.Mysql
{
    /// <inheritdoc cref="WindNight.Extension.Db.Abstractions.IPagedListRepositoryService" /> 
    public abstract partial class MySqlBase<TEntity, TId> : IPagedListRepositoryService<TEntity, TId>
    {
        /* =========================TEntity===========================================*/

        public virtual IPagedList<TEntity> QueryPagedList(IQueryPageBase pageQueryBase, string whereSql, IDictionary<string, object> paramDict = null, string orderby = "", bool tableNameToLower = true, bool tableNameAppendPlural = true, long warnMs = -1L, Action<Exception, string> execErrorHandler = null)
        {
            return DbPagedList(pageQueryBase, whereSql, paramDict, orderby, tableNameToLower, tableNameAppendPlural, warnMs: warnMs, execErrorHandler: execErrorHandler);
        }

        public virtual async Task<IPagedList<TEntity>> QueryPagedListAsync(IQueryPageBase pageQueryBase, string whereSql, IDictionary<string, object> paramDict = null, string orderby = "", bool tableNameToLower = true, bool tableNameAppendPlural = true, long warnMs = -1L, Action<Exception, string> execErrorHandler = null)
        {
            return await DbPagedListAsync(pageQueryBase, whereSql, paramDict, orderby, tableNameToLower, tableNameAppendPlural, warnMs: warnMs, execErrorHandler: execErrorHandler);
        }


        /// <summary>
        ///   
        /// </summary>
        /// <param name="pagedInfo"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public virtual IPagedList<TEntity> QueryPagedList(IQueryPageInfo pagedInfo, IDictionary<string, object> parameters, long warnMs = -1L, Action<Exception, string> execErrorHandler = null)
        {
            return DbPagedList(pagedInfo, parameters, warnMs: warnMs, execErrorHandler: execErrorHandler);

        }


        /// <summary>
        ///  异步分页
        /// </summary>
        /// <param name="pagedInfo"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public virtual async Task<IPagedList<TEntity>> QueryPagedListAsync(IQueryPageInfo pagedInfo, IDictionary<string, object> parameters, long warnMs = -1L, Action<Exception, string> execErrorHandler = null)
        {
            return await DbPagedListAsync(pagedInfo, parameters, warnMs: warnMs, execErrorHandler: execErrorHandler);
        }


        /* =========================TEntity======End=====================================*/


        /* =========================TEntity====ConnStr=======================================*/

        public virtual IPagedList<TEntity> QueryPagedList(string connStr, IQueryPageBase pageQueryBase, string whereSql, IDictionary<string, object> paramDict = null, string orderby = "", bool tableNameToLower = true, bool tableNameAppendPlural = true, long warnMs = -1L, Action<Exception, string> execErrorHandler = null)
        {
            return DbPagedList(connStr, pageQueryBase, whereSql, paramDict, orderby, tableNameToLower, tableNameAppendPlural, warnMs: warnMs, execErrorHandler: execErrorHandler);
        }

        public virtual async Task<IPagedList<TEntity>> QueryPagedListAsync(string connStr, IQueryPageBase pageQueryBase, string whereSql, IDictionary<string, object> paramDict = null, string orderby = "", bool tableNameToLower = true, bool tableNameAppendPlural = true, long warnMs = -1L, Action<Exception, string> execErrorHandler = null)
        {
            return await DbPagedListAsync(connStr, pageQueryBase, whereSql, paramDict, orderby, tableNameToLower, tableNameAppendPlural, warnMs: warnMs, execErrorHandler: execErrorHandler);
        }


        /// <summary>
        ///   
        /// </summary>
        /// <param name="pagedInfo"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public virtual IPagedList<TEntity> QueryPagedList(string connStr, IQueryPageInfo pagedInfo, IDictionary<string, object> parameters, long warnMs = -1L, Action<Exception, string> execErrorHandler = null)
        {
            return DbPagedList(connStr, pagedInfo, parameters, warnMs: warnMs, execErrorHandler: execErrorHandler);

        }


        /// <summary>
        ///  异步分页
        /// </summary>
        /// <param name="pagedInfo"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public virtual async Task<IPagedList<TEntity>> QueryPagedListAsync(string connStr, IQueryPageInfo pagedInfo, IDictionary<string, object> parameters, long warnMs = -1L, Action<Exception, string> execErrorHandler = null)
        {
            return await DbPagedListAsync(connStr, pagedInfo, parameters, warnMs: warnMs, execErrorHandler: execErrorHandler);
        }


        /* =========================TEntity====ConnStr==End=====================================*/





        /* =========================T_T===========================================*/

        /// <summary>
        ///  
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="pageQueryBase"></param>
        /// <param name="whereSql"></param>
        /// <param name="paramDict"></param>
        /// <param name="orderby"></param>
        /// <param name="tableNameToLower"></param>
        /// <param name="tableNameAppendPlural"></param>
        /// <param name="warnMs"></param>
        /// <param name="execErrorHandler"></param>
        /// <returns></returns>
        public virtual IPagedList<T> QueryPagedEList<T>(IQueryPageBase pageQueryBase, string whereSql, IDictionary<string, object> paramDict, string orderby = "", string tableName = "", long warnMs = -1L, Action<Exception, string> execErrorHandler = null)
            where T : class, new()
        {
            return DbPagedEList<T>(pageQueryBase, whereSql, paramDict, orderby, tableName, warnMs: warnMs, execErrorHandler);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="pageQueryBase"></param>
        /// <param name="whereSql"></param>
        /// <param name="paramDict"></param>
        /// <param name="orderby"></param>
        /// <param name="tableNameToLower"></param>
        /// <param name="tableNameAppendPlural"></param>
        /// <param name="warnMs"></param>
        /// <param name="execErrorHandler"></param>
        /// <returns></returns>
        public virtual async Task<IPagedList<T>> QueryPagedEListAsync<T>(IQueryPageBase pageQueryBase, string whereSql, IDictionary<string, object> paramDict, string orderby = "", string tableName = "", long warnMs = -1L, Action<Exception, string> execErrorHandler = null)
            where T : class, new()
        {
            return await DbPagedEListAsync<T>(pageQueryBase, whereSql, paramDict, orderby, tableName, warnMs: warnMs, execErrorHandler: execErrorHandler);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="pagedInfo"></param>
        /// <param name="parameters"></param>
        /// <param name="warnMs"></param>
        /// <param name="execErrorHandler"></param>
        /// <returns></returns>
        public virtual IPagedList<T> QueryPagedEList<T>(IQueryPageInfo pagedInfo, IDictionary<string, object> parameters, long warnMs = -1L, Action<Exception, string> execErrorHandler = null)
            where T : class, new()
        {
            return DbPagedEList<T>(pagedInfo, parameters, warnMs: warnMs, execErrorHandler: execErrorHandler);

        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="pagedInfo"></param>
        /// <param name="parameters"></param>
        /// <param name="warnMs"></param>
        /// <param name="execErrorHandler"></param>
        /// <returns></returns>
        public virtual async Task<IPagedList<T>> QueryPagedEListAsync<T>(IQueryPageInfo pagedInfo, IDictionary<string, object> parameters, long warnMs = -1L, Action<Exception, string> execErrorHandler = null)
            where T : class, new()
        {
            return await DbPagedEListAsync<T>(pagedInfo, parameters, warnMs: warnMs, execErrorHandler: execErrorHandler);

        }


        /* =========================T_T===========End================================*/

        /* =========================T_T======ConnStr=====================================*/

        /// <summary>
        ///  
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="pageQueryBase"></param>
        /// <param name="whereSql"></param>
        /// <param name="paramDict"></param>
        /// <param name="orderby"></param>
        /// <param name="tableNameToLower"></param>
        /// <param name="tableNameAppendPlural"></param>
        /// <param name="warnMs"></param>
        /// <param name="execErrorHandler"></param>
        /// <returns></returns>
        public virtual IPagedList<T> QueryPagedEList<T>(string connStr, IQueryPageBase pageQueryBase, string whereSql, IDictionary<string, object> paramDict, string orderby = "", string tableName = "", long warnMs = -1L, Action<Exception, string> execErrorHandler = null)
            where T : class, new()
        {
            return DbPagedEList<T>(connStr, pageQueryBase, whereSql, paramDict, orderby, tableName, warnMs: warnMs, execErrorHandler);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="pageQueryBase"></param>
        /// <param name="whereSql"></param>
        /// <param name="paramDict"></param>
        /// <param name="orderby"></param>
        /// <param name="tableNameToLower"></param>
        /// <param name="tableNameAppendPlural"></param>
        /// <param name="warnMs"></param>
        /// <param name="execErrorHandler"></param>
        /// <returns></returns>
        public virtual async Task<IPagedList<T>> QueryPagedEListAsync<T>(string connStr, IQueryPageBase pageQueryBase, string whereSql, IDictionary<string, object> paramDict, string orderby = "", string tableName = "", long warnMs = -1L, Action<Exception, string> execErrorHandler = null)
            where T : class, new()
        {
            return await DbPagedEListAsync<T>(connStr, pageQueryBase, whereSql, paramDict, orderby, tableName, warnMs: warnMs, execErrorHandler: execErrorHandler);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="pagedInfo"></param>
        /// <param name="parameters"></param>
        /// <param name="warnMs"></param>
        /// <param name="execErrorHandler"></param>
        /// <returns></returns>
        public virtual IPagedList<T> QueryPagedEList<T>(string connStr, IQueryPageInfo pagedInfo, IDictionary<string, object> parameters, long warnMs = -1L, Action<Exception, string> execErrorHandler = null)
            where T : class, new()
        {
            return DbPagedEList<T>(connStr, pagedInfo, parameters, warnMs: warnMs, execErrorHandler: execErrorHandler);

        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="pagedInfo"></param>
        /// <param name="parameters"></param>
        /// <param name="warnMs"></param>
        /// <param name="execErrorHandler"></param>
        /// <returns></returns>
        public virtual async Task<IPagedList<T>> QueryPagedEListAsync<T>(string connStr, IQueryPageInfo pagedInfo, IDictionary<string, object> parameters, long warnMs = -1L, Action<Exception, string> execErrorHandler = null)
            where T : class, new()
        {
            return await DbPagedEListAsync<T>(connStr, pagedInfo, parameters, warnMs: warnMs, execErrorHandler: execErrorHandler);

        }


        /* =========================T_T=====ConnStr======End================================*/


        #region Obsolete

        /// <summary>
        /// 分页
        /// impl<see cref="IPagedListRepositoryService{TEntity, TId}"/>
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="condition"></param>
        /// <param name="orderBy"></param>
        /// <param name="parameters"></param>
        /// <param name="queryTableName"></param>
        /// <returns></returns>
        [Obsolete("Please Use IQueryPageBase or IQueryPageInfo ")]
        public virtual IPagedList<TEntity> QueryPagedList(int pageIndex, int pageSize, string condition, string orderBy, IDictionary<string, object> parameters = null, string queryTableName = "", long warnMs = -1L, Action<Exception, string> execErrorHandler = null)
        {
            return DbPagedList(pageIndex, pageSize, condition, orderBy, parameters, queryTableName, warnMs: warnMs, execErrorHandler: execErrorHandler);
        }

        /// <summary>
        /// 异步分页
        /// impl<see cref="IPagedListRepositoryService{TEntity, TId}"/>
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="condition"></param>
        /// <param name="orderBy"></param>
        /// <param name="parameters"></param>
        /// <param name="queryTableName"></param>
        /// <returns></returns>
        [Obsolete("Please Use IQueryPageBase or IQueryPageInfo ")]
        public virtual async Task<IPagedList<TEntity>> QueryPagedListAsync(int pageIndex, int pageSize, string condition, string orderBy, IDictionary<string, object> parameters = null, string queryTableName = "", long warnMs = -1L, Action<Exception, string> execErrorHandler = null)
        {
            return await DbPagedListAsync(pageIndex, pageSize, condition, orderBy, parameters, queryTableName, warnMs: warnMs, execErrorHandler: execErrorHandler);
        }


        [Obsolete("Please Use IQueryPageBase or IQueryPageInfo ")]
        public virtual IPagedList<T> QueryPagedEList<T>(int pageIndex, int pageSize, string condition, string orderBy, IDictionary<string, object> parameters = null, string queryTableName = "", long warnMs = -1L, Action<Exception, string> execErrorHandler = null)
            where T : class, new()
        {
            return DbPagedEList<T>(pageIndex, pageSize, condition, orderBy, parameters, queryTableName, warnMs: warnMs, execErrorHandler: execErrorHandler);

        }


        /// <summary>
        ///  异步分页
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="condition"></param>
        /// <param name="orderBy"></param>
        /// <param name="parameters"></param>
        /// <param name="queryTableName">查询的表或者临时表 ,NullOrEmpty=><see cref="BaseTableName"/></param>
        /// <returns></returns>
        [Obsolete("Please Use IQueryPageBase or IQueryPageInfo ")]
        public virtual async Task<IPagedList<T>> QueryPagedEListAsync<T>(int pageIndex, int pageSize, string condition, string orderBy, IDictionary<string, object> parameters = null, string queryTableName = "", long warnMs = -1L, Action<Exception, string> execErrorHandler = null)
            where T : class, new()
        {
            return await DbPagedEListAsync<T>(pageIndex, pageSize, condition, orderBy, parameters, queryTableName, warnMs: warnMs, execErrorHandler: execErrorHandler);

        }



        /// <summary>
        ///  异步分页
        /// </summary>
        /// <param name="pagedInfo"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        [Obsolete("Please Use rewrite func  ", true)]
        public virtual IPagedList<T> QueryPagedEList<T>(IQueryPageBase pagedInfo, string condition, string orderBy, IDictionary<string, object> parameters = null, string queryTableName = "", long warnMs = -1L, Action<Exception, string> execErrorHandler = null)
            where T : class, new()
        {
            return DbPagedEList<T>(pagedInfo.PageIndex, pagedInfo.PageSize, condition, orderBy, parameters, queryTableName, warnMs: warnMs, execErrorHandler: execErrorHandler);
        }

        /// <summary>
        ///  异步分页
        /// </summary>
        /// <param name="pagedInfo"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        [Obsolete("Please Use rewrite func  ", true)]
        public virtual async Task<IPagedList<T>> QueryPagedEListAsync<T>(IQueryPageBase pagedInfo, string condition, string orderBy, IDictionary<string, object> parameters = null, string queryTableName = "", long warnMs = -1L, Action<Exception, string> execErrorHandler = null)
            where T : class, new()
        {
            return await DbPagedEListAsync<T>(pagedInfo.PageIndex, pagedInfo.PageSize, condition, orderBy, parameters, queryTableName, warnMs: warnMs, execErrorHandler: execErrorHandler);
        }

        /// <summary>
        ///  异步分页
        /// </summary>
        /// <param name="pagedInfo"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        [Obsolete("Please Use rewrite func  ", true)]
        public virtual async Task<IPagedList<TEntity>> QueryPagedListAsync(IQueryPageBase pagedInfo, string condition, string orderBy, IDictionary<string, object> parameters = null, string queryTableName = "", long warnMs = -1L, Action<Exception, string> execErrorHandler = null)
        {
            return await DbPagedListAsync(pagedInfo.PageIndex, pagedInfo.PageSize, condition, orderBy, parameters, queryTableName, warnMs: warnMs, execErrorHandler: execErrorHandler);
        }

        /// <summary>
        ///  异步分页
        /// </summary>
        /// <param name="pagedInfo"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        [Obsolete("Please Use rewrite func  ", true)]
        public virtual IPagedList<TEntity> QueryPagedList(IQueryPageBase pagedInfo, string condition, string orderBy, IDictionary<string, object> parameters = null, string queryTableName = "", long warnMs = -1L, Action<Exception, string> execErrorHandler = null)
        {
            return DbPagedList(pagedInfo.PageIndex, pagedInfo.PageSize, condition, orderBy, parameters, queryTableName, warnMs: warnMs, execErrorHandler: execErrorHandler);
        }

        #endregion



    }
}
