using WindNight.Core.SQL.Abstractions;

namespace WindNight.Extension.Db.Abstractions
{
    /// <summary>
    ///     含Id的分页仓库基类 默认主键是 int
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <inheritdoc />
    public interface IPagedListRepositoryService<TEntity> : IPagedListRepositoryService<TEntity, int>
        where TEntity : IEntity, ICanPageEntity
    {
    }


    /// <summary>
    ///     含Id的分页仓库基类
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TId"></typeparam>
    public interface IPagedListRepositoryService<TEntity, TId>
        where TEntity : IEntity, ICanPageEntity
        where TId : IEquatable<TId>, IComparable<TId>
    {

        #region IEntity



        /// <summary>
        ///   
        /// </summary>
        /// <param name="pagedInfo"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        IPagedList<TEntity> QueryPagedList(IQueryPageInfo pagedInfo, IDictionary<string, object> parameters = null, long warnMs = -1L, Action<Exception, string> execErrorHandler = null);


        /// <summary>
        ///  异步分页
        /// </summary>
        /// <param name="pagedInfo"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        Task<IPagedList<TEntity>> QueryPagedListAsync(IQueryPageInfo pagedInfo, IDictionary<string, object> parameters = null, long warnMs = -1L, Action<Exception, string> execErrorHandler = null);




        IPagedList<TEntity> QueryPagedList(IQueryPageBase pageQueryBase, string whereSql, IDictionary<string, object> paramDict = null, string orderby = "", bool tableNameToLower = true, bool tableNameAppendPlural = true, long warnMs = -1L, Action<Exception, string> execErrorHandler = null);

        Task<IPagedList<TEntity>> QueryPagedListAsync(IQueryPageBase pageQueryBase, string whereSql, IDictionary<string, object> paramDict = null, string orderby = "", bool tableNameToLower = true, bool tableNameAppendPlural = true, long warnMs = -1L, Action<Exception, string> execErrorHandler = null);

        #endregion //end IEntity



        /// <summary>
        ///   
        /// </summary>
        /// <param name="pagedInfo"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        IPagedList<T> QueryPagedEList<T>(IQueryPageInfo pagedInfo, IDictionary<string, object> parameters = null, long warnMs = -1L, Action<Exception, string> execErrorHandler = null)
            where T : class, new();


        /// <summary>
        ///  异步分页
        /// </summary>
        /// <param name="pagedInfo"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        Task<IPagedList<T>> QueryPagedEListAsync<T>(IQueryPageInfo pagedInfo, IDictionary<string, object> parameters = null, long warnMs = -1L, Action<Exception, string> execErrorHandler = null)
            where T : class, new();

        IPagedList<T> QueryPagedEList<T>(IQueryPageBase pageQueryBase, string whereSql, IDictionary<string, object> paramDict = null, string orderby = "", string tableName = "", long warnMs = -1L, Action<Exception, string> execErrorHandler = null)
            where T : class, new();

        Task<IPagedList<T>> QueryPagedEListAsync<T>(IQueryPageBase pageQueryBase, string whereSql, IDictionary<string, object> paramDict = null, string orderby = "", string tableName = "", long warnMs = -1L, Action<Exception, string> execErrorHandler = null)
            where T : class, new();





        #region Obsolete

        /// <summary>
        ///     常规分页 同步
        /// </summary>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">页大小</param>
        /// <param name="condition"> 条件语句(不用加where) </param>
        /// <param name="orderBy"> 排序字段(必须需要!支持多字段，不用加order by) </param>
        /// <param name="parameters"></param>
        /// <param name="queryTableName"></param>
        /// <returns></returns>
        [Obsolete("Please Use IQueryPageBase or IQueryPageInfo   ")]
        IPagedList<TEntity> QueryPagedList(int pageIndex, int pageSize, string condition, string orderBy, IDictionary<string, object> parameters = null, string queryTableName = "", long warnMs = -1L, Action<Exception, string> execErrorHandler = null);

        /// <summary>
        ///     常规分页 异步
        /// </summary>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">页大小</param>
        /// <param name="condition"> 条件语句(不用加where) </param>
        /// <param name="orderBy"> 排序字段(必须需要!支持多字段，不用加order by) </param>
        /// <param name="parameters"></param>
        /// <param name="queryTableName"></param>
        /// <returns></returns>
        [Obsolete("Please Use IQueryPageBase or IQueryPageInfo ")]
        Task<IPagedList<TEntity>> QueryPagedListAsync(int pageIndex, int pageSize, string condition, string orderBy, IDictionary<string, object> parameters = null, string queryTableName = "", long warnMs = -1L, Action<Exception, string> execErrorHandler = null);

        [Obsolete("Please Use IQueryPageBase or IQueryPageInfo ")]
        IPagedList<T> QueryPagedEList<T>(int pageIndex, int pageSize, string condition, string orderBy, IDictionary<string, object> parameters = null, string queryTableName = "", long warnMs = -1L, Action<Exception, string> execErrorHandler = null)
            where T : class, new();

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
        Task<IPagedList<T>> QueryPagedEListAsync<T>(int pageIndex, int pageSize, string condition, string orderBy, IDictionary<string, object> parameters = null, string queryTableName = "", long warnMs = -1L, Action<Exception, string> execErrorHandler = null)
            where T : class, new();


        /// <summary>
        ///   
        /// </summary>
        /// <param name="pagedInfo"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        [Obsolete("Please Use rewrite func  ", true)]
        IPagedList<T> QueryPagedEList<T>(IQueryPageBase pagedInfo, string condition, string orderBy, IDictionary<string, object> parameters = null, string queryTableName = "", long warnMs = -1L, Action<Exception, string> execErrorHandler = null)
            where T : class, new();



        /// <summary>
        ///  异步分页
        /// </summary>
        /// <param name="pagedInfo"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        [Obsolete("Please Use rewrite func  ", true)]
        Task<IPagedList<T>> QueryPagedEListAsync<T>(IQueryPageBase pagedInfo, string condition, string orderBy, IDictionary<string, object> parameters = null, string queryTableName = "", long warnMs = -1L, Action<Exception, string> execErrorHandler = null)
            where T : class, new();

        [Obsolete("Please Use rewrite func  ", true)]
        Task<IPagedList<TEntity>> QueryPagedListAsync(IQueryPageBase pagedInfo, string condition, string orderBy, IDictionary<string, object> parameters = null, string queryTableName = "", long warnMs = -1L, Action<Exception, string> execErrorHandler = null);

        [Obsolete("Please Use rewrite func  ", true)]
        IPagedList<TEntity> QueryPagedList(IQueryPageBase pagedInfo, string condition, string orderBy, IDictionary<string, object> parameters = null, string queryTableName = "", long warnMs = -1L, Action<Exception, string> execErrorHandler = null);


        #endregion


    }
}
