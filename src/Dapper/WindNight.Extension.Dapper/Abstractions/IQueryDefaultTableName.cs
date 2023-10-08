using System;
using System.Collections.Generic;
using System.Text;
using WindNight.Core.SQL;
using WindNight.Core.SQL.Abstractions;
using WindNight.Extension.Dapper.Abstractions.DB;

namespace WindNight.Extension.Dapper.Abstractions
{
    public interface IQueryDefaultTableName
    {
        string DefaultTableName { get; }
    }

    public interface IDefaultEnBaseRepositoryService<TEntity> : IBaseRepositoryServiceWithId<TEntity>,
        IQueryDefaultTableName
        where TEntity : IEntity
    {
    }

    public interface IDefaultEnBaseRepositoryService<TEntity, TId> : IBaseRepositoryServiceWithId<TEntity, TId>,
        IQueryDefaultTableName
        where TEntity : IEntity
        where TId : IEquatable<TId>, IComparable<TId>
    {
    }
    public interface IInsertOrUpdateRepositoryService<TEntity> : IInsertOrUpdateRepositoryService<TEntity, int>
        where TEntity : CreateAndUpdateWithStatusBase<int>, new()
    {
    }

    public interface IInsertOrUpdateRepositoryService<TEntity, TId>
        where TEntity : CreateAndUpdateWithStatusBase<TId>, new()
        where TId : IEquatable<TId>, IComparable<TId>
    {
        bool InsertOrUpdateData(TEntity entity);
        Task<bool> InsertOrUpdateDataAsync(TEntity entity);


        int BatchInsertOrUpdateData(IEnumerable<TEntity> entities);
        Task<int> BatchInsertOrUpdateDataAsync(IEnumerable<TEntity> entities);
    }


}
