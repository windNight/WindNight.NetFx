﻿using System;
using System.Collections.Generic;
using System.Text;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WindNight.Core.SQL.Abstractions;

namespace WindNight.Extension.Dapper.Abstractions.DB
{

    /// <summary>
    ///     表带Id的仓储基类 默认主键Id
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <inheritdoc />
    public interface IBaseRepositoryServiceWithId<TEntity> : IBaseRepositoryServiceWithId<TEntity, int>
        where TEntity : IEntity
    {
    }

    /// <summary>
    ///     表带Id的仓储基类
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TId"></typeparam>
    /// <inheritdoc cref="IQueryAllRepositoryService{TEntity}" />
    public interface IBaseRepositoryServiceWithId<TEntity, TId> : IQueryAllRepositoryService<TEntity>,
        IInsertRepositoryService<TEntity, TId>
        where TEntity : IEntity
        where TId : IEquatable<TId>, IComparable<TId>
    {
        /// <summary>
        ///     同步 根据Id获取数据
        /// </summary>
        /// <param name="id">主键Id </param>
        /// <returns></returns>
        TEntity QueryById(TId id);

        /// <summary>
        ///     异步 根据Id获取数据
        /// </summary>
        /// <param name="id">主键Id</param>
        /// <returns></returns>
        Task<TEntity> QueryByIdAsync(TId id);
    }

    /// <summary>
    ///     通用获取数据仓储基类
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public interface IQueryAllRepositoryService<TEntity> where TEntity : IEntity
    {
        /// <summary>
        ///     同步 获取所有列表
        /// </summary>
        /// <returns></returns>
        IEnumerable<TEntity> QueryAllList();

        /// <summary>
        ///     异步 获取所有列表
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<TEntity>> QueryAllListAsync();
    }
}