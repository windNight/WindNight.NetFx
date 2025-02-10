using Newtonsoft.Json.Extension;
using WindNight.Core.SQL.Abstractions;
using WindNight.Extension.Db.Abstractions;
using WindNight.Extension.Dapper.Mysql.@internal;

namespace WindNight.Extension.Dapper.Mysql
{
    ///<inheritdoc />
    public abstract partial class MySqlBase<TEntity, TId> : NoIdMysqlBase<TEntity>,
          IBaseRepositoryServiceWithId<TEntity, TId>
        where TEntity : class, IEntity<TId>, new()
        where TId : IEquatable<TId>, IComparable<TId>
    {
        protected virtual string InsertWithIdSql =>
            $"INSERT INTO {BaseTableName} (Id,{InsertTableColumns})  VALUES ( @Id,{InsertTableColumnValues}); ";


        protected virtual string QueryDataByIdSql => $"SELECT * FROM {BaseTableName} WHERE Id=@Id; ";

        protected virtual string QueryListByStatusSql => $"SELECT * FROM {BaseTableName} WHERE Status=@QueryStatus ";


        protected virtual string DeleteByIdSql =>
            $@"UPDATE {BaseTableName} SET IsDeleted=1 WHERE Id=@Id; ";

        protected override string QueryAllSqlCondition => $" IsDeleted=0 ";


        /// <summary>
        ///  获取整表数据 慎用
        /// </summary>
        /// <returns></returns>
        public virtual IEnumerable<TEntity> QueryAllList(long warnMs = -1)
        {
            return DbQueryList(QueryAllSqlStr, warnMs: warnMs);
        }

        /// <summary>
        /// 异步获取整表数据 慎用
        /// </summary>
        /// <returns></returns>
        public virtual async Task<IEnumerable<TEntity>> QueryAllListAsync(long warnMs = -1)
        {
            return await DbQueryListAsync(QueryAllSqlStr, warnMs: warnMs);
        }


        #region Id opt

        public virtual TEntity QueryById(TId id, long warnMs = -1)
        {
            return DbQuery(QueryDataByIdSql, new { Id = id }, warnMs: warnMs);
        }

        public virtual async Task<TEntity> QueryByIdAsync(TId id, long warnMs = -1)
        {
            return await DbQueryAsync(QueryDataByIdSql, new { Id = id }, warnMs: warnMs);
        }

        public virtual TId InsertOne(TEntity entity, long warnMs = -1)
        {
            if (entity == null) return default;


            var id = DbExecuteScalar<TId>(InsertSql, entity, warnMs: warnMs);
            if (id.CompareTo(default) <= 0)
                LogHelper.Warn($"Insert Into {BaseTableName} handler error ,entities is {entity.ToJsonStr()} . ",
                    appendMessage: false);
            entity.Id = id;
            return id;
        }

        public virtual async Task<TId> InsertOneAsync(TEntity entity, long warnMs = -1)
        {
            if (entity == null) return default;

            var id = await DbExecuteScalarAsync<TId>(InsertSql, entity, warnMs: warnMs);
            if (id.CompareTo(default) <= 0)
                LogHelper.Warn($"Insert Into {BaseTableName} handler error ,entities is {entity.ToJsonStr()} . ",
                    appendMessage: false);
            entity.Id = id;
            return id;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual bool DeleteById(TId id, long warnMs = -1)
        {
            var flag = DbExecute(DeleteByIdSql, new { Id = id });
            return flag > 0;
        }

        public virtual async Task<bool> DeleteByIdAsync(TId id, long warnMs = -1)
        {
            var flag = await DbExecuteAsync(DeleteByIdSql, new { Id = id }, warnMs: warnMs);
            return flag > 0;
        }

        #region IStatusRepositoryService

        /// <summary>
        /// 
        /// impl<see cref="IStatusRepositoryService{TEntity}"/>
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        public virtual IEnumerable<TEntity> QueryListByStatus(DataStatusEnums status, long warnMs = -1)
        {
            return DbQueryList(QueryListByStatusSql, new { QueryStatus = (int)status }, warnMs: warnMs);
        }


        /// <summary>
        /// 
        /// impl<see cref="IStatusRepositoryService{TEntity, TId}"/>
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        public virtual async Task<IEnumerable<TEntity>> QueryListByStatusAsync(DataStatusEnums status, long warnMs = -1)
        {
            return await DbQueryListAsync(QueryListByStatusSql, new { QueryStatus = (int)status }, warnMs: warnMs);
        }

        #endregion // end IStatusRepositoryService




        #endregion // end Id opt


    }



}
