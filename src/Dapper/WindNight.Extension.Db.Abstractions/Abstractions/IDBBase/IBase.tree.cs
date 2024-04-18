using WindNight.Core.SQL.Abstractions;

namespace WindNight.Extension.Db.Abstractions
{
    /// <summary>
    ///     含Id的树形仓库基类 默认主键是 int
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <inheritdoc />
    public interface ITreeRepositoryService<TEntity> : ITreeRepositoryService<TEntity, int>
        where TEntity : IEntity, ITreeEntity<int>
    {
    }

    /// <summary>
    ///     含Id的树形仓库基类
    /// </summary>
    /// <typeparam name="TEntity"> inherit from <see cref="IEntity" /> , <see cref="ITreeEntity{TId}" /></typeparam>
    /// <typeparam name="TId"></typeparam>
    public interface ITreeRepositoryService<TEntity, TId>
        where TEntity : IEntity, ITreeEntity<TId>
        where TId : IEquatable<TId>, IComparable<TId>
    {
        /// <summary>
        ///     同步 根据父节点获取树形列表
        /// </summary>
        /// <param name="parentId">父节点Id</param>
        /// <param name="rootCondition"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        IEnumerable<TEntity> QueryChildrenByParentId(TId parentId, string rootCondition = "", Dictionary<string, object> param = null);

        /// <summary>
        ///     异步 根据父节点获取树形列表
        /// </summary>
        /// <param name="parentId">父节点Id</param>
        /// <param name="rootCondition"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        Task<IEnumerable<TEntity>> QueryChildrenByParentIdAsync(TId parentId, string rootCondition = "", Dictionary<string, object> param = null);

        /// <summary>
        ///     同步 根据子节点获取树形列表
        /// </summary>
        /// <param name="childId">子节点Id</param>
        /// <param name="rootCondition"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        IEnumerable<TEntity> QueryParentsByChildId(TId childId, string rootCondition = "", Dictionary<string, object> param = null);

        /// <summary>
        ///     异步 根据子节点获取树形列表
        /// </summary>
        /// <param name="childId">子节点Id</param>
        /// <param name="rootCondition"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        Task<IEnumerable<TEntity>> QueryParentsByChildIdAsync(TId childId, string rootCondition = "", Dictionary<string, object> param = null);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="parentId"></param>
        /// <param name="rootCondition"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        IEnumerable<T> QueryChildrenByParentId<T>(TId parentId, string rootCondition = "", Dictionary<string, object> param = null) where T : class, ITreeEntity<TId>, new();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="parentId"></param>
        /// <param name="rootCondition"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        IEnumerable<T> QueryChildrenByParentId<T>(string tableName, TId parentId, string rootCondition = "", Dictionary<string, object> param = null)
             where T : class, ITreeEntity<TId>, new();


        /// <summary>
        /// 
        /// </summary>
        /// <param name="parentId"></param>
        /// <param name="rootCondition"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        IEnumerable<T> QueryParentsByChildId<T>(TId parentId, string rootCondition = "", Dictionary<string, object> param = null)
            where T : class, ITreeEntity<TId>, new();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="parentId"></param>
        /// <param name="rootCondition"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        IEnumerable<T> QueryParentsByChildId<T>(string tableName, TId parentId, string rootCondition = "", Dictionary<string, object> param = null)
            where T : class, ITreeEntity<TId>, new();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parentId"></param>
        /// <param name="rootCondition"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        Task<IEnumerable<T>> QueryParentsByChildIdAsync<T>(TId parentId, string rootCondition = "", Dictionary<string, object> param = null)
                  where T : class, ITreeEntity<TId>, new();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="parentId"></param>
        /// <param name="rootCondition"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        Task<IEnumerable<T>> QueryParentsByChildIdAsync<T>(string tableName, TId parentId, string rootCondition = "", Dictionary<string, object> param = null)
                  where T : class, ITreeEntity<TId>, new();



    }
}