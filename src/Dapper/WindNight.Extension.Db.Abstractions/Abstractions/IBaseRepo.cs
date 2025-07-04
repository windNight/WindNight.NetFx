using System;
using System.Collections.Generic;
using System.Text;
using WindNight.Core.SQL.Abstractions;

namespace WindNight.Extension.Dapper.Abstractions
{
    public interface INoIdBaseRepositoryService<TEntity>
        where TEntity : IEntity
    {

    }

    public interface IBaseRepositoryService<TEntity> : IBaseRepositoryService<TEntity, int>
        where TEntity : IEntity
    {

    }

    public interface IBaseRepositoryService<TEntity, TId> : IQueryDefaultTableName
        where TEntity : IEntity
        where TId : IEquatable<TId>, IComparable<TId>
    {

    }



}
