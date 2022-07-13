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
    ///     含Id的状态仓储基类 默认主键是 int
    /// </summary>
    /// <typeparam name="TEntity">  inherit from <see cref="IEntity" /> , <see cref="IStatusEntity" /></typeparam>
    /// <inheritdoc />
    public interface IStatusRepositoryService<TEntity> : IStatusRepositoryService<TEntity, int>
        where TEntity : IEntity, IStatusEntity
    {
    }

    /// <summary>
    ///     含Id的状态仓储基类
    /// </summary>
    /// <typeparam name="TEntity">  inherit from <see cref="IEntity" /> , <see cref="IStatusEntity" /></typeparam>
    /// <typeparam name="TId"></typeparam>
    public interface IStatusRepositoryService<TEntity, TId>
        where TEntity : IEntity, IStatusEntity
        where TId : IEquatable<TId>, IComparable<TId>
    {
        /// <summary>
        ///     同步获取指定状态的数据列表
        /// </summary>
        /// <param name="status">
        ///     <see cref="DataStatusEnums" />
        /// </param>
        /// <returns></returns>
        IEnumerable<TEntity> QueryListByStatus(DataStatusEnums status);

        /// <summary>
        ///     异步获取指定状态的数据列表
        /// </summary>
        /// <param name="status">
        ///     <see cref="DataStatusEnums" />
        /// </param>
        /// <returns></returns>
        Task<IEnumerable<TEntity>> QueryListByStatusAsync(DataStatusEnums status);


        /// <summary>
        ///     同步 逻辑删除数据
        /// </summary>
        /// <param name="id"> 主键Id </param>
        /// <returns></returns>
        bool DeleteById(TId id);

        /// <summary>
        ///     异步 逻辑删除数据
        /// </summary>
        /// <param name="id"> 主键Id </param>
        /// <returns></returns>
        Task<bool> DeleteByIdAsync(TId id);
    }
}