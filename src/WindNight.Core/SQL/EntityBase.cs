using System;
using WindNight.Core.SQL.Abstractions;

namespace WindNight.Core.SQL
{
    /// <inheritdoc cref="IEntity" />
    public class EntityBase<TPrimaryKey> : IEntity<TPrimaryKey>, ICanPageEntity //暂时选定所有带Id的单表都可分页
        where TPrimaryKey : IEquatable<TPrimaryKey>, IComparable<TPrimaryKey>
    {
        public virtual TPrimaryKey Id { get; set; }

        public bool IdIsValid()
        {
            return Id.CompareTo(default) > 0;
        }
    }

    /// <inheritdoc cref="ICreateEntityBase" />
    public class CreateBase<TPrimaryKey> : EntityBase<TPrimaryKey>, IDeletedEntity, ICreateEntityBase
        where TPrimaryKey : IEquatable<TPrimaryKey>, IComparable<TPrimaryKey>
    {
        public CreateBase()
        {
            var now = HardInfo.Now;
            CreateDate = now.ToString("yyyyMMdd").ToInt();
            CreateUnixTime = now.ConvertToUnixTime();
        }

        public int CreateUserId { get; set; }
        public long CreateUnixTime { get; set; }

        public int CreateDate { get; set; }

        public virtual int IsDeleted { get; set; } = 0;

        public virtual void InitData()
        {
            var now = HardInfo.Now;
            CreateDate = now.ToString("yyyyMMdd").ToInt();
            CreateUnixTime = now.ConvertToUnixTime();
        }
    }


    /// <inheritdoc cref="IUpdateEntityBase" />
    public class CreateAndUpdateBase<TPrimaryKey> : CreateBase<TPrimaryKey>, IUpdateEntityBase
        where TPrimaryKey : IEquatable<TPrimaryKey>, IComparable<TPrimaryKey>
    {
        public int UpdateUserId { get; set; }
        public long UpdateUnixTime { get; set; }

        public int UpdateDate { get; set; }
    }


    /// <inheritdoc cref="IUpdateEntityBase" />
    public class CreateAndUpdateWithStatusBase<TPrimaryKey> : CreateAndUpdateBase<TPrimaryKey>, IStatusEntity
        where TPrimaryKey : IEquatable<TPrimaryKey>, IComparable<TPrimaryKey>
    {
        public virtual int Status { get; set; } = (int)DataStatusEnums.Enable;
    }


    /// <inheritdoc cref="ITreeEntity{TPrimaryKey}" />
    public class CommonTreeEntityBase<TPrimaryKey> : CreateAndUpdateWithStatusBase<TPrimaryKey>,
        ITreeEntity<TPrimaryKey>
        where TPrimaryKey : IEquatable<TPrimaryKey>, IComparable<TPrimaryKey>
    {
        public virtual TPrimaryKey ParentId { get; set; }
    }

    public static class SqlEx
    {
        //public static string GenDefaultTableName<TEntity>(this object t, bool toLower = true, bool appendPlural = false)
        //where TEntity : class, IEntity, new()
        //{
        //    var tableName = typeof(TEntity).Name;
        //    if (toLower) tableName = tableName.ToLower();
        //    if (appendPlural && !tableName.EndsWith("s"))
        //    {
        //        tableName = $"{tableName}s";
        //    }
        //    return tableName;
        //}
    }
}
