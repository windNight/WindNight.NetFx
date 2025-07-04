using System;

namespace WindNight.Core.SQL.Abstractions
{
    public interface ICanPageEntity
    {

    }

    public interface IEntity : ICanPageEntity
    {

    }

    public interface IEntity<TPrimaryKey> : IEntityWithId<TPrimaryKey>
        where TPrimaryKey : IEquatable<TPrimaryKey>, IComparable<TPrimaryKey>
    {

    }

    public interface IEntityWithId<TPrimaryKey> : IEntity
        where TPrimaryKey : IEquatable<TPrimaryKey>, IComparable<TPrimaryKey>
    {
        /// <summary> 主键 </summary>
        TPrimaryKey Id { get; set; }
        bool IdIsValid();
    }



    public interface IDeletedEntity : IEntity
    {
        /// <summary> 软删除标志 </summary>
        int IsDeleted { get; set; }
    }

    public interface IStatusEntity : IEntity
    {
        /// <summary> 状态 0=未知， 1 =正常，2=禁用 detail to <see cref="DataStatusEnums" /> </summary>
        int Status { get; set; }
    }

    public interface ITreeEntity : ITreeEntity<int>
    {

    }


    /// <summary>
    ///     树状
    /// </summary>
    /// <typeparam name="TPrimaryKey"></typeparam>
    public interface ITreeEntity<TPrimaryKey> : ICUSEntityBase<TPrimaryKey>
        where TPrimaryKey : IEquatable<TPrimaryKey>, IComparable<TPrimaryKey>
    {
        /// <summary>
        ///     父级 Id
        /// </summary>
        TPrimaryKey ParentId { get; set; }
    }

    public interface ILogCreateEntityBase : ILogCreateEntityBase<int>
    {

    }

    public interface ILogCreateEntityBase<TPrimaryKey> : ICreateEntityBase<TPrimaryKey>
        where TPrimaryKey : IEquatable<TPrimaryKey>, IComparable<TPrimaryKey>
    {
        string CreateUserName { get; set; }
    }

    public interface ICreateEntityBase : ICreateEntityBase<int>
    {

    }
    public interface ICUEntityBase : ICUEntityBase<int>
    {

    }
    public interface ICUEntityBase<TPrimaryKey> :
        ICreateEntityBase<TPrimaryKey>,
        IUpdateEntityBase,
        IDeletedEntity
        where TPrimaryKey : IEquatable<TPrimaryKey>, IComparable<TPrimaryKey>
    {

    }
    public interface ICUSEntityBase : ICUSEntityBase<int>
    {

    }
    public interface ICUSEntityBase<TPrimaryKey> :
        ICUEntityBase<TPrimaryKey>,
        IStatusEntity
        where TPrimaryKey : IEquatable<TPrimaryKey>, IComparable<TPrimaryKey>
    {

    }
    /// <summary>
    ///     
    /// </summary>
    public interface ICreateEntityBase<TPrimaryKey> : IEntity
        where TPrimaryKey : IEquatable<TPrimaryKey>, IComparable<TPrimaryKey>
    {
        /// <summary> 创建人编号 </summary>
        int CreateUserId { get; set; }

        /// <summary> 创建时间, 单位毫秒 </summary>
        long CreateUnixTime { get; set; }

        /// <summary> 创建日期, 格式 yyyyMMdd, 举例: 20171231 </summary>
        int CreateDate { get; set; }
    }


    /// <summary>
    ///     
    /// </summary>
    public interface IUpdateEntityBase : IEntity
    {
        /// <summary> 更新人编号 </summary>
        int UpdateUserId { get; set; }

        /// <summary> 更新时间, 单位毫秒 </summary>
        long UpdateUnixTime { get; set; }

        /// <summary> 更新日期, 格式 yyyyMMdd, 举例: 20171231 </summary>
        int UpdateDate { get; set; }
    }
}
