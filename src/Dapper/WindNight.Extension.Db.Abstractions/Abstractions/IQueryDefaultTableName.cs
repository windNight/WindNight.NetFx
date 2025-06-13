using WindNight.Core.SQL;
using WindNight.Core.SQL.Abstractions;
using WindNight.Extension.Db.Abstractions;

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
        bool InsertOrUpdateData(TEntity entity, long warnMs = -1, Action<Exception, string> execErrorHandler = null);
        Task<bool> InsertOrUpdateDataAsync(TEntity entity, long warnMs = -1, Action<Exception, string> execErrorHandler = null);


        int BatchInsertOrUpdateData(IEnumerable<TEntity> entities, long warnMs = -1, Action<Exception, string> execErrorHandler = null);
        Task<int> BatchInsertOrUpdateDataAsync(IEnumerable<TEntity> entities, long warnMs = -1, Action<Exception, string> execErrorHandler = null);
    }


}
