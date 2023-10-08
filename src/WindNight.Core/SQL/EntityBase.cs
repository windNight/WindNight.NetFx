using System;
using WindNight.Core.SQL.Abstractions;

namespace WindNight.Core.SQL
{

    /// <inheritdoc cref="IEntity" />
    public class EntityBase<TPrimaryKey> : IEntity<TPrimaryKey>, ICanPageEntity //暂时选定所有带Id的单表都可分页
    {
        public TPrimaryKey Id { get; set; }
    }

    /// <inheritdoc cref="ICreateEntityBase" />
    public class CreateBase<TPrimaryKey> : EntityBase<TPrimaryKey>, IDeletedEntity, ICreateEntityBase
    {
        public CreateBase()
        {
            var now = DateTime.Now;
            CreateDate = now.ToString("yyyyMMdd").ToInt();
            CreateUnixTime = now.ConvertToUnixTime();
        }

        public int CreateUserId { get; set; }
        public long CreateUnixTime { get; set; }

        public int CreateDate { get; set; }

        public int IsDeleted { get; set; } = 0;
    }


    /// <inheritdoc cref="IUpdateEntityBase" />
    public class CreateAndUpdateBase<TPrimaryKey> : CreateBase<TPrimaryKey>, IUpdateEntityBase
    {
        public int UpdateUserId { get; set; }
        public long UpdateUnixTime { get; set; }

        public int UpdateDate { get; set; }
    }


    /// <inheritdoc cref="IUpdateEntityBase" />
    public class CreateAndUpdateWithStatusBase<TPrimaryKey> : CreateAndUpdateBase<TPrimaryKey>, IStatusEntity
    {
        public CreateAndUpdateWithStatusBase() : base()
        {
            Status = (int)DataStatusEnums.Enable;
        }
        public int Status { get; set; }
    }


    /// <inheritdoc cref="ITreeEntity{TPrimaryKey}" />
    public class CommonTreeEntityBase<TPrimaryKey> : CreateAndUpdateWithStatusBase<TPrimaryKey>,
        ITreeEntity<TPrimaryKey>
    {
        public TPrimaryKey ParentId { get; set; }
    }


}