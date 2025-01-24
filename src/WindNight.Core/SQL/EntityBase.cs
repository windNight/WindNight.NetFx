using System;
using WindNight.Core.SQL.Abstractions;

namespace WindNight.Core.SQL
{

    /// <inheritdoc cref="IEntity" />
    public class EntityBase<TPrimaryKey> : IEntity<TPrimaryKey>, ICanPageEntity //暂时选定所有带Id的单表都可分页
    {
        public virtual TPrimaryKey Id { get; set; }
    }

    /// <inheritdoc cref="ICreateEntityBase" />
    public class CreateBase<TPrimaryKey> : EntityBase<TPrimaryKey>, IDeletedEntity, ICreateEntityBase
    {
        public CreateBase()
        {
            var now = HardInfo.Now;
            CreateDate = now.ToString("yyyyMMdd").ToInt();
            CreateUnixTime = now.ConvertToUnixTime();
        }

        public virtual void InitData()
        {
            var now = HardInfo.Now;
            CreateDate = now.ToString("yyyyMMdd").ToInt();
            CreateUnixTime = now.ConvertToUnixTime();
        }

        public virtual int CreateUserId { get; set; }
        public virtual long CreateUnixTime { get; set; }

        public virtual int CreateDate { get; set; }

        public virtual int IsDeleted { get; set; } = 0;
    }


    /// <inheritdoc cref="IUpdateEntityBase" />
    public class CreateAndUpdateBase<TPrimaryKey> : CreateBase<TPrimaryKey>, IUpdateEntityBase
    {
        public virtual int UpdateUserId { get; set; }
        public virtual long UpdateUnixTime { get; set; }

        public virtual int UpdateDate { get; set; }
    }


    /// <inheritdoc cref="IUpdateEntityBase" />
    public class CreateAndUpdateWithStatusBase<TPrimaryKey> : CreateAndUpdateBase<TPrimaryKey>, IStatusEntity
    {
        public CreateAndUpdateWithStatusBase() : base()
        {
            Status = (int)DataStatusEnums.Enable;
        }
        public virtual int Status { get; set; }
    }


    /// <inheritdoc cref="ITreeEntity{TPrimaryKey}" />
    public class CommonTreeEntityBase<TPrimaryKey> : CreateAndUpdateWithStatusBase<TPrimaryKey>,
        ITreeEntity<TPrimaryKey>
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