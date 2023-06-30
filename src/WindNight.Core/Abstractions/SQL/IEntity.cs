namespace WindNight.Core.SQL.Abstractions
{
 
 public interface IEntity
    {
    }

    public interface IEntity<TPrimaryKey> : IEntity
    {
        /// <summary> 主键 </summary>
        public TPrimaryKey Id { get; set; }
    }

    public interface IDeletedEntity
    {
        /// <summary> 软删除标志 </summary>
        bool IsDeleted { get; set; }
    }

    public interface IStatusEntity
    {
        /// <summary> 状态 0=未知， 1 =正常，2=禁用 detail to <see cref="DataStatusEnums" /> </summary>
        int Status { get; set; }
    }


    public interface ICanPageEntity
    {

    }

    /// <summary>
    ///     树状
    /// </summary>
    /// <typeparam name="TPrimaryKey"></typeparam>
    public interface ITreeEntity<TPrimaryKey>
    {
        /// <summary>
        ///     父级 Id
        /// </summary>
        TPrimaryKey ParentId { get; set; }
    }

    /// <summary>
    ///     运行数据或者非配置数据
    /// </summary>
    public interface ICreateEntityBase
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
    public interface IUpdateEntityBase
    {
        /// <summary> 更新人编号 </summary>
        int UpdateUserId { get; set; }

        /// <summary> 更新时间, 单位毫秒 </summary>
        long UpdateUnixTime { get; set; }

        /// <summary> 更新日期, 格式 yyyyMMdd, 举例: 20171231 </summary>
        int UpdateDate { get; set; }
    }
 
}