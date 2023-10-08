using System;
using System.Collections.Generic;
using System.Text;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WindNight.Core.SQL.Abstractions;

namespace WindNight.Extension.Dapper.Abstractions.DB
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
        ///     常规分页 同步
        /// </summary>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">页大小</param>
        /// <param name="condition"> 条件语句(不用加where) </param>
        /// <param name="orderBy"> 排序字段(必须需要!支持多字段，不用加order by) </param>
        /// <param name="parameters"></param>
        /// <param name="queryTableName"></param>
        /// <returns></returns>
        IPagedList<TEntity> QueryPagedList(int pageIndex, int pageSize, string condition, string orderBy,
            IDictionary<string, object> parameters = null, string queryTableName = "");

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
        Task<IPagedList<TEntity>> QueryPagedListAsync(int pageIndex, int pageSize, string condition,
            string orderBy, IDictionary<string, object> parameters = null, string queryTableName = "");

        /// <summary>
        ///   
        /// </summary>
        /// <param name="pagedInfo"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        IPagedList<TEntity> QueryPagedList(IQueryPageInfo pagedInfo, IDictionary<string, object> parameters);


        /// <summary>
        ///  异步分页
        /// </summary>
        /// <param name="pagedInfo"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        Task<IPagedList<TEntity>> QueryPagedListAsync(IQueryPageInfo pagedInfo, IDictionary<string, object> parameters);

        #endregion //end IEntity




        IPagedList<T> QueryPagedEList<T>(int pageIndex, int pageSize, string condition,
            string orderBy, IDictionary<string, object> parameters = null, string queryTableName = "")
            where T : class, new();

        /// <summary>
        ///   
        /// </summary>
        /// <param name="pagedInfo"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        IPagedList<T> QueryPagedEList<T>(IQueryPageInfo pagedInfo, IDictionary<string, object> parameters)
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
        Task<IPagedList<T>> QueryPagedEListAsync<T>(
                 int pageIndex, int pageSize,
                 string condition, string orderBy,
                 IDictionary<string, object> parameters = null, string queryTableName = "")
                 where T : class, new();

        /// <summary>
        ///  异步分页
        /// </summary>
        /// <param name="pagedInfo"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        Task<IPagedList<T>> QueryPagedEListAsync<T>(IQueryPageInfo pagedInfo, IDictionary<string, object> parameters)
                 where T : class, new();





    }
}