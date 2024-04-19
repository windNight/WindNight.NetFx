using WindNight.Core.SQL.Abstractions;
using WindNight.Extension.Db.Abstractions;

namespace WindNight.Extension.Dapper.Mysql
{
    ///<inheritdoc /> 
    public abstract partial class MySqlBase<TEntity, TId>
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
            IDictionary<string, object> parameters = null, string queryTableName = "")
        {
            return DbPagedList(pageIndex, pageSize, condition, orderBy, parameters, queryTableName);
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
            string orderBy, IDictionary<string, object> parameters = null, string queryTableName = "")
        {
            return await DbPagedListAsync(pageIndex, pageSize, condition, orderBy, parameters, queryTableName);
        }


        /// <summary>
        ///   
        /// </summary>
        /// <param name="pagedInfo"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public virtual IPagedList<TEntity> QueryPagedList(IQueryPageInfo pagedInfo,
            IDictionary<string, object> parameters)
        {
            return DbPagedList(pagedInfo, parameters);

        }


        /// <summary>
        ///  异步分页
        /// </summary>
        /// <param name="pagedInfo"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public virtual async Task<IPagedList<TEntity>> QueryPagedListAsync(IQueryPageInfo pagedInfo,
            IDictionary<string, object> parameters)
        {
            return await DbPagedListAsync(pagedInfo, parameters);
        }

        #endregion //end  TEntity

        public virtual IPagedList<T> QueryPagedEList<T>(int pageIndex, int pageSize, string condition,
            string orderBy, IDictionary<string, object> parameters = null, string queryTableName = "")
            where T : class, new()
        {
            return DbPagedList<T>(pageIndex, pageSize, condition, orderBy, parameters, queryTableName);

        }

        /// <summary>
        ///   
        /// </summary>
        /// <param name="pagedInfo"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public virtual IPagedList<T> QueryPagedEList<T>(IQueryPageInfo pagedInfo, IDictionary<string, object> parameters)
            where T : class, new()
        {
            return DbPagedList<T>(pagedInfo, parameters);

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
            IDictionary<string, object> parameters = null, string queryTableName = "")
            where T : class, new()
        {
            return await DbPagedListAsync<T>(pageIndex, pageSize, condition, orderBy, parameters, queryTableName);

        }

        /// <summary>
        ///  异步分页
        /// </summary>
        /// <param name="pagedInfo"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public virtual async Task<IPagedList<T>> QueryPagedEListAsync<T>(IQueryPageInfo pagedInfo,
            IDictionary<string, object> parameters)
            where T : class, new()
        {
            return await DbPagedListAsync<T>(pagedInfo, parameters);

        }











    }
}