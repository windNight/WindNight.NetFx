using System;
using System.Collections.Generic;
using System.Text;
using WindNight.Core.SQL.Abstractions;
using WindNight.Extension.Db.Abstractions;

namespace WindNight.Extension.Dapper.Abstractions
{
    public interface IWriterBaseRepositoryService<TEntity> : IWriterBaseRepositoryService<TEntity, int>, IReaderBaseRepositoryService<TEntity>
        where TEntity : IEntity
    {

    }
    public interface ITreeWriterBaseRepositoryService<TEntity> : ITreeWriterBaseRepositoryService<TEntity, int>
        where TEntity : ITreeEntity<int>, new()
    { }

    public interface ICUWriterBaseRepositoryService<TEntity> : ICUWriterBaseRepositoryService<TEntity, int>
        where TEntity : ICUEntityBase<int>, new()
    { }

    public interface ICUSWriterBaseRepositoryService<TEntity> : ICUSWriterBaseRepositoryService<TEntity, int>
        where TEntity : ICUSEntityBase<int>, new()
    { }



    public interface ITreeWriterBaseRepositoryService<TEntity, TId> : IWriterBaseRepositoryService<TEntity, TId>
        where TEntity : ITreeEntity<TId>, new()
        where TId : IEquatable<TId>, IComparable<TId>
    {

    }


    public interface ICUWriterBaseRepositoryService<TEntity, TId> : IWriterBaseRepositoryService<TEntity, TId>, ICUReaderBaseRepositoryService<TEntity, TId>
        where TEntity : ICUEntityBase<TId>, new()
        where TId : IEquatable<TId>, IComparable<TId>
    {

    }
    public interface ICUSWriterBaseRepositoryService<TEntity, TId> : IWriterBaseRepositoryService<TEntity, TId>, ICUSReaderBaseRepositoryService<TEntity, TId>
        where TEntity : ICUSEntityBase<TId>, new()
        where TId : IEquatable<TId>, IComparable<TId>
    {

    }

    public interface IWriterBaseRepositoryService<TEntity, TId> : IReaderBaseRepositoryService<TEntity, TId>,
        IInsertRepositoryService<TEntity, TId>
        where TEntity : IEntity
        where TId : IEquatable<TId>, IComparable<TId>
    {

        /// <summary>
        ///     同步 逻辑删除数据
        /// </summary>
        /// <param name="id"> 主键Id </param>
        /// <returns></returns>
        bool DeleteById(TId id, long warnMs = -1L, Action<Exception, string> execErrorHandler = null);

        /// <summary>
        ///     异步 逻辑删除数据
        /// </summary>
        /// <param name="id"> 主键Id </param>
        /// <returns></returns>
        Task<bool> DeleteByIdAsync(TId id, long warnMs = -1L, Action<Exception, string> execErrorHandler = null);




    }




}
