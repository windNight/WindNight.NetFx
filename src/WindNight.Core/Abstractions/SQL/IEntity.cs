using System;

namespace WindNight.Core.SQL.Abstractions
{
    public interface ICanPageEntity
    {

    }

    public interface IEntity : ICanPageEntity
    {
    }

    public interface IEntity<TPrimaryKey> : IEntity
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




    /// <summary>
    ///     树状
    /// </summary>
    /// <typeparam name="TPrimaryKey"></typeparam>
    public interface ITreeEntity<TPrimaryKey> : IEntity
    {
        /// <summary>
        ///     父级 Id
        /// </summary>
        TPrimaryKey ParentId { get; set; }
    }

    /// <summary>
    ///     运行数据或者非配置数据
    /// </summary>
    public interface ICreateEntityBase : IEntity
    {
        /// <summary> 创建人编号 </summary>
        int CreateUserId { get; set; }

        /// <summary> 创建时间, 单位毫秒 </summary>
        long CreateUnixTime { get; set; }

        /// <summary> 创建日期, 格式 yyyyMMdd, 举例: 20171231 </summary>
        int CreateDate { get; set; }
    }


    /// <summary>
    ///     运行数据或者非配置数据
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
