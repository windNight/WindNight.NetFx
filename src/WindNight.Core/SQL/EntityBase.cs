using System;
using WindNight.Core.SQL.Abstractions;

namespace WindNight.Core.SQL
{

    public class EntityBase : EntityBase<int> { }

    public class CreateBase : CreateBase<int> { }

    public class LogCreateBase : LogCreateBase<int> { }

    public class CreateAndUpdateBase : CreateAndUpdateBase<int> { }

    public class CreateAndUpdateWithStatusBase : CreateAndUpdateWithStatusBase<int> { }

    public class TreeEntityBase : CommonTreeEntityBase<int> { }



    /// <inheritdoc cref="IEntity" />
    public class EntityBase<TPrimaryKey> : IEntity<TPrimaryKey> //暂时选定所有带Id的单表都可分页
        where TPrimaryKey : IEquatable<TPrimaryKey>, IComparable<TPrimaryKey>
    {
        public virtual TPrimaryKey Id { get; set; }

        public virtual bool IdIsValid()
        {
            if (Id == null)
            {
                return false;
            }

            // 针对常见主键类型分别处理
            if (typeof(TPrimaryKey) == typeof(int) || typeof(TPrimaryKey) == typeof(long))
            {
                return Id.CompareTo(default(TPrimaryKey)) > 0;
            }

            if (typeof(TPrimaryKey) == typeof(Guid))
            {
                return !Id.Equals((TPrimaryKey)(object)Guid.Empty);
            }

            if (typeof(TPrimaryKey) == typeof(string))
            {
                return !string.IsNullOrWhiteSpace(Id as string);
            }

            // 其他类型默认用 CompareTo
            return Id.CompareTo(default(TPrimaryKey)) != 0;
        }
    }

    /// <inheritdoc cref="ICreateEntityBase" />
    public class CreateBase<TPrimaryKey> : EntityBase<TPrimaryKey>,
        IDeletedEntity, ICreateEntityBase
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

    public class LogCreateBase<TPrimaryKey> : CreateBase<TPrimaryKey>, ILogCreateEntityBase<TPrimaryKey>
        where TPrimaryKey : IEquatable<TPrimaryKey>, IComparable<TPrimaryKey>
    {
        public LogCreateBase() : base()
        {

        }

        public string CreateUserName { get; set; }

    }

    /// <inheritdoc cref="IUpdateEntityBase" />
    public class CreateAndUpdateBase<TPrimaryKey> : CreateBase<TPrimaryKey>, ICUEntityBase<TPrimaryKey>
        where TPrimaryKey : IEquatable<TPrimaryKey>, IComparable<TPrimaryKey>
    {
        public CreateAndUpdateBase() : base()
        {

        }

        public int UpdateUserId { get; set; }
        public long UpdateUnixTime { get; set; }

        public int UpdateDate { get; set; }

    }


    /// <inheritdoc cref="IUpdateEntityBase" />
    public class CreateAndUpdateWithStatusBase<TPrimaryKey> : CreateAndUpdateBase<TPrimaryKey>, ICUSEntityBase<TPrimaryKey>
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






}
