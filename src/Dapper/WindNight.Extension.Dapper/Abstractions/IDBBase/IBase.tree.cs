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
        ///     同步 根据节点获取树形列表
        /// </summary>
        /// <param name="parentId">节点Id</param>
        /// <returns></returns>
        IEnumerable<TEntity> QueryChildrenByParentId(TId parentId);

        /// <summary>
        ///     异步 根据节点获取树形列表
        /// </summary>
        /// <param name="parentId">节点Id</param>
        /// <returns></returns>
        Task<IEnumerable<TEntity>> QueryChildrenByParentIdAsync(TId parentId);
    }
}