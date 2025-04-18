using WindNight.Core.SQL.Abstractions;
using WindNight.Extension.Db.Abstractions;

namespace WindNight.Extension.Dapper.Mysql
{
    ///<inheritdoc /> 
    public abstract partial class MySqlBase<TEntity, TId> : IPagedListRepositoryService<TEntity, TId>
    {

        #region TEntity 


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
        public virtual IPagedList<TEntity> QueryPagedList(int pageIndex, int pageSize, string condition, string orderBy,
            IDictionary<string, object> parameters = null, string queryTableName = "", long warnMs = -1)
        {
            return DbPagedList(pageIndex, pageSize, condition, orderBy, parameters, queryTableName, warnMs: warnMs);
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
        public virtual async Task<IPagedList<TEntity>> QueryPagedListAsync(int pageIndex, int pageSize, string condition,
            string orderBy, IDictionary<string, object> parameters = null, string queryTableName = "", long warnMs = -1)
        {
            return await DbPagedListAsync(pageIndex, pageSize, condition, orderBy, parameters, queryTableName, warnMs: warnMs);
        }
        /// <summary>
        ///  异步分页
        /// </summary>
        /// <param name="pagedInfo"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public virtual async Task<IPagedList<TEntity>> QueryPagedListAsync(IQueryPageBase pagedInfo,
            string condition, string orderBy,
            IDictionary<string, object> parameters = null, string queryTableName = "", long warnMs = -1)
        {
            return await DbPagedListAsync(pagedInfo.PageIndex, pagedInfo.PageSize, condition, orderBy, parameters, queryTableName, warnMs: warnMs);
        }
        /// <summary>
        ///  异步分页
        /// </summary>
        /// <param name="pagedInfo"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public virtual IPagedList<TEntity> QueryPagedList(IQueryPageBase pagedInfo,
            string condition, string orderBy,
            IDictionary<string, object> parameters = null, string queryTableName = "", long warnMs = -1)
        {
            return DbPagedList(pagedInfo.PageIndex, pagedInfo.PageSize, condition, orderBy, parameters, queryTableName, warnMs: warnMs);
        }

        /// <summary>
        ///   
        /// </summary>
        /// <param name="pagedInfo"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public virtual IPagedList<TEntity> QueryPagedList(IQueryPageInfo pagedInfo,
            IDictionary<string, object> parameters, long warnMs = -1)
        {
            return DbPagedList(pagedInfo, parameters, warnMs: warnMs);

        }


        /// <summary>
        ///  异步分页
        /// </summary>
        /// <param name="pagedInfo"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public virtual async Task<IPagedList<TEntity>> QueryPagedListAsync(IQueryPageInfo pagedInfo,
            IDictionary<string, object> parameters, long warnMs = -1)
        {
            return await DbPagedListAsync(pagedInfo, parameters, warnMs: warnMs);
        }

        #endregion //end  TEntity

        public virtual IPagedList<T> QueryPagedEList<T>(int pageIndex, int pageSize, string condition,
            string orderBy, IDictionary<string, object> parameters = null, string queryTableName = "", long warnMs = -1)
            where T : class, new()
        {
            return DbPagedList<T>(pageIndex, pageSize, condition, orderBy, parameters, queryTableName, warnMs: warnMs);

        }

        /// <summary>
        ///   
        /// </summary>
        /// <param name="pagedInfo"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public virtual IPagedList<T> QueryPagedEList<T>(IQueryPageInfo pagedInfo, IDictionary<string, object> parameters, long warnMs = -1)
            where T : class, new()
        {
            return DbPagedList<T>(pagedInfo, parameters, warnMs: warnMs);

        }

        /// <summary>
        ///  异步分页
        /// </summary>
        /// <param name="pagedInfo"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public virtual IPagedList<T> QueryPagedEList<T>(IQueryPageBase pagedInfo,
            string condition, string orderBy,
            IDictionary<string, object> parameters = null, string queryTableName = "", long warnMs = -1)
            where T : class, new()
        {
            return DbPagedList<T>(pagedInfo.PageIndex, pagedInfo.PageSize, condition, orderBy, parameters, queryTableName, warnMs: warnMs);
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
        public virtual async Task<IPagedList<T>> QueryPagedEListAsync<T>(
            int pageIndex, int pageSize,
            string condition, string orderBy,
            IDictionary<string, object> parameters = null, string queryTableName = "", long warnMs = -1)
            where T : class, new()
        {
            return await DbPagedListAsync<T>(pageIndex, pageSize, condition, orderBy, parameters, queryTableName, warnMs: warnMs);

        }

        /// <summary>
        ///  异步分页
        /// </summary>
        /// <param name="pagedInfo"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public virtual async Task<IPagedList<T>> QueryPagedEListAsync<T>(IQueryPageInfo pagedInfo,
            IDictionary<string, object> parameters, long warnMs = -1)
            where T : class, new()
        {
            return await DbPagedListAsync<T>(pagedInfo, parameters, warnMs: warnMs);

        }

        /// <summary>
        ///  异步分页
        /// </summary>
        /// <param name="pagedInfo"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public virtual async Task<IPagedList<T>> QueryPagedEListAsync<T>(IQueryPageBase pagedInfo,
            string condition, string orderBy,
            IDictionary<string, object> parameters = null, string queryTableName = "", long warnMs = -1)
            where T : class, new()
        {
            return await DbPagedListAsync<T>(pagedInfo.PageIndex, pagedInfo.PageSize, condition, orderBy, parameters, queryTableName, warnMs: warnMs);
        }











    }
}
