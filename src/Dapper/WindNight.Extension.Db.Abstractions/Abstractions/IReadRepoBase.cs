using System;
using System.Collections.Generic;
using System.Text;
using WindNight.Core.SQL.Abstractions;

namespace WindNight.Extension.Dapper.Abstractions
{

    public interface IDefaultReaderBaseRepositoryService<TEntity>
        where TEntity : IEntity
    {
        /// <summary>
        ///     同步 获取所有列表
        /// </summary>
        /// <returns></returns>
        IEnumerable<TEntity> QueryAllList(long warnMs = -1L, Action<Exception, string> execErrorHandler = null);

        /// <summary>
        ///     异步 获取所有列表
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<TEntity>> QueryAllListAsync(long warnMs = -1L, Action<Exception, string> execErrorHandler = null);
    }


    public interface ITreeReaderBaseRepositoryService<TEntity> : ITreeReaderBaseRepositoryService<TEntity, int>
        where TEntity : ITreeEntity<int>, new()
    {

    }

    public interface IStatusReaderRepositoryService<TEntity> : IStatusReaderRepositoryService<TEntity, int>
        where TEntity : IEntity, IStatusEntity
    {

    }
    public interface IReaderBaseRepositoryService<TEntity> : IReaderBaseRepositoryService<TEntity, int>
        where TEntity : IEntity
    {

    }


    public interface ICUSReaderBaseRepositoryService<TEntity> : ICUSReaderBaseRepositoryService<TEntity, int>
        where TEntity : ICUSEntityBase<int>, new()
    {

    }

    public interface ICUReaderBaseRepositoryService<TEntity> : ICUReaderBaseRepositoryService<TEntity, int>
        where TEntity : ICUEntityBase<int>, new()
    {

    }






    public interface ICUReaderBaseRepositoryService<TEntity, TId> :
        IReaderBaseRepositoryService<TEntity, TId>
        where TEntity : ICUEntityBase<TId>, new()
        where TId : IEquatable<TId>, IComparable<TId>
    {

    }

    public interface ICUSReaderBaseRepositoryService<TEntity, TId> :
        ICUReaderBaseRepositoryService<TEntity, TId>,
        IStatusReaderRepositoryService<TEntity, TId>
        where TEntity : ICUSEntityBase<TId>, new()
        where TId : IEquatable<TId>, IComparable<TId>
    {

    }

    public interface ITreeReaderBaseRepositoryService<TEntity, TId> : IReaderBaseRepositoryService<TEntity, TId>
        where TEntity : ITreeEntity<TId>, new()
        where TId : IEquatable<TId>, IComparable<TId>
    {

        /// <summary>
        ///     同步 根据父节点获取树形列表
        /// </summary>
        /// <param name="parentId">父节点Id</param>
        /// <param name="rootCondition"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        IEnumerable<TEntity> QueryChildrenByParentId(TId parentId, string rootCondition = "", Dictionary<string, object> param = null, long warnMs = -1);

        /// <summary>
        ///     异步 根据父节点获取树形列表
        /// </summary>
        /// <param name="parentId">父节点Id</param>
        /// <param name="rootCondition"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        Task<IEnumerable<TEntity>> QueryChildrenByParentIdAsync(TId parentId, string rootCondition = "", Dictionary<string, object> param = null, long warnMs = -1L);

        /// <summary>
        ///     同步 根据子节点获取树形列表
        /// </summary>
        /// <param name="childId">子节点Id</param>
        /// <param name="rootCondition"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        IEnumerable<TEntity> QueryParentsByChildId(TId childId, string rootCondition = "", Dictionary<string, object> param = null, long warnMs = -1L);

        /// <summary>
        ///     异步 根据子节点获取树形列表
        /// </summary>
        /// <param name="childId">子节点Id</param>
        /// <param name="rootCondition"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        Task<IEnumerable<TEntity>> QueryParentsByChildIdAsync(TId childId, string rootCondition = "", Dictionary<string, object> param = null, long warnMs = -1L);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="parentId"></param>
        /// <param name="rootCondition"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        IEnumerable<T> QueryChildrenByParentId<T>(TId parentId, string rootCondition = "", Dictionary<string, object> param = null, long warnMs = -1L)
            where T : class, ITreeEntity<TId>, new();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="parentId"></param>
        /// <param name="rootCondition"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        IEnumerable<T> QueryChildrenByParentId<T>(string tableName, TId parentId, string rootCondition = "", Dictionary<string, object> param = null, long warnMs = -1L)
             where T : class, ITreeEntity<TId>, new();


        /// <summary>
        /// 
        /// </summary>
        /// <param name="parentId"></param>
        /// <param name="rootCondition"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        IEnumerable<T> QueryParentsByChildId<T>(TId parentId, string rootCondition = "", Dictionary<string, object> param = null, long warnMs = -1L)
            where T : class, ITreeEntity<TId>, new();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="parentId"></param>
        /// <param name="rootCondition"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        IEnumerable<T> QueryParentsByChildId<T>(string tableName, TId parentId, string rootCondition = "", Dictionary<string, object> param = null, long warnMs = -1L)
            where T : class, ITreeEntity<TId>, new();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parentId"></param>
        /// <param name="rootCondition"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        Task<IEnumerable<T>> QueryParentsByChildIdAsync<T>(TId parentId, string rootCondition = "", Dictionary<string, object> param = null, long warnMs = -1L)
                  where T : class, ITreeEntity<TId>, new();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="parentId"></param>
        /// <param name="rootCondition"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        Task<IEnumerable<T>> QueryParentsByChildIdAsync<T>(string tableName, TId parentId, string rootCondition = "", Dictionary<string, object> param = null, long warnMs = -1L)
                  where T : class, ITreeEntity<TId>, new();


    }


    public interface IStatusReaderRepositoryService<TEntity, TId> : IReaderBaseRepositoryService<TEntity, TId>
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
        IEnumerable<TEntity> QueryListByStatus(int status, long warnMs = -1L, Action<Exception, string> execErrorHandler = null);

        /// <summary>
        ///     异步获取指定状态的数据列表
        /// </summary>
        /// <param name="status">
        ///     <see cref="DataStatusEnums" />
        /// </param>
        /// <returns></returns>
        Task<IEnumerable<TEntity>> QueryListByStatusAsync(int status, long warnMs = -1L, Action<Exception, string> execErrorHandler = null);

    }


    public interface IReaderBaseRepositoryService<TEntity, TId> :
        IDefaultReaderBaseRepositoryService<TEntity>,
        IBaseRepositoryService<TEntity, TId>
        where TEntity : IEntity
        where TId : IEquatable<TId>, IComparable<TId>
    {

        /// <summary>
        ///     同步 根据Id获取数据
        /// </summary>
        /// <param name="id">主键Id </param>
        /// <returns></returns>
        TEntity QueryById(TId id, long warnMs = -1L, Action<Exception, string> execErrorHandler = null);

        /// <summary>
        ///     异步 根据Id获取数据
        /// </summary>
        /// <param name="id">主键Id</param>
        /// <returns></returns>
        Task<TEntity> QueryByIdAsync(TId id, long warnMs = -1L, Action<Exception, string> execErrorHandler = null);

    }







}
