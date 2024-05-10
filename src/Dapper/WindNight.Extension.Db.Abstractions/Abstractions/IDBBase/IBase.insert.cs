using WindNight.Core.SQL.Abstractions;

namespace WindNight.Extension.Db.Abstractions
{
    /// <summary>
    ///     带Id的 新增仓储基类 默认主键是 int
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <inheritdoc />
    public interface IInsertRepositoryService<TEntity> : IInsertRepositoryService<TEntity, int> where TEntity : IEntity
    {
    }

    /// <summary>
    ///     带Id的 新增仓储基类
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TId"></typeparam>
    public interface IInsertRepositoryService<TEntity, TId> where TEntity : IEntity
        where TId : IEquatable<TId>, IComparable<TId>
    {
        /// <summary>
        ///     同步 单条数据插入
        /// </summary>
        /// <param name="entity">need inherit from <see cref="IEntity" /></param>
        /// <returns></returns>
        TId InsertOne(TEntity entity, long warnMs = -1);


        /// <summary>
        ///     异步 单条数据插入
        /// </summary>
        /// <param name="entity">need inherit from <see cref="IEntity" /></param>
        /// <returns></returns>
        Task<TId> InsertOneAsync(TEntity entity, long warnMs = -1);

        /// <summary>
        ///     同步 批量插入数据
        /// </summary>
        /// <param name="insertList">list of <see cref="IEntity" /></param>
        /// <returns></returns>
        bool BatchInsertUseValues(IList<TEntity> insertList, long warnMs = -1);

        /// <summary>
        ///     异步 批量插入数据
        /// </summary>
        /// <param name="insertList">list of <see cref="IEntity" /></param>
        /// <returns></returns>
        Task<bool> BatchInsertUseValuesAsync(IList<TEntity> insertList, long warnMs = -1);
    }
}