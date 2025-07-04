using Newtonsoft.Json.Extension;
using WindNight.Core.SQL.Abstractions;
using WindNight.Extension.Dapper.Abstractions;
using WindNight.Extension.Dapper.Mysql.@internal;
using WindNight.Extension.Db.Abstractions;
using WindNight.Extension.Db.Extensions;

namespace WindNight.Extension.Dapper.Mysql
{
    /// <inheritdoc cref="NoIdMysqlBase" />
    public abstract partial class MySqlBase<TEntity, TId> : NoIdMysqlBase<TEntity>,
    //IBaseRepositoryServiceWithId<TEntity, TId> 
    IWriterBaseRepositoryService<TEntity, TId>

        where TEntity : class, IEntity, IEntity<TId>, new()
        where TId : IEquatable<TId>, IComparable<TId>
    {
        /// <summary>
        ///  获取整表数据 慎用
        /// </summary>
        /// <returns></returns>
        public virtual IEnumerable<TEntity> QueryAllList(long warnMs = -1L, Action<Exception, string> execErrorHandler = null)
        {
            return DbQueryList(QueryAllSqlStr, warnMs: warnMs, execErrorHandler: execErrorHandler);
        }

        /// <summary>
        /// 异步获取整表数据 慎用
        /// </summary>
        /// <returns></returns>
        public virtual async Task<IEnumerable<TEntity>> QueryAllListAsync(long warnMs = -1L, Action<Exception, string> execErrorHandler = null)
        {
            return await DbQueryListAsync(QueryAllSqlStr, warnMs: warnMs, execErrorHandler: execErrorHandler);
        }


        #region Id opt

        public virtual TEntity QueryById(TId id, long warnMs = -1L, Action<Exception, string> execErrorHandler = null)
        {
            return DbQuery(QueryDataByIdSql, new { Id = id }, warnMs: warnMs, execErrorHandler: execErrorHandler);
        }

        public virtual async Task<TEntity> QueryByIdAsync(TId id, long warnMs = -1L, Action<Exception, string> execErrorHandler = null)
        {
            return await DbQueryAsync(QueryDataByIdSql, new { Id = id }, warnMs: warnMs, execErrorHandler: execErrorHandler);
        }

        public virtual TId InsertOne(TEntity entity, long warnMs = -1L, Action<Exception, string> execErrorHandler = null)
        {
            if (entity == null)
            {
                return default;
            }

            if (execErrorHandler == null)
            {
                execErrorHandler = (ex, sql) =>
                {
                    LogHelper.Error($" sql:{sql} exec error param is {entity.ToParamString()}  {ex.Message} ", ex);
                };
            }

            var id = DbExecuteScalar<TId>(InsertSql, entity, warnMs: warnMs, execErrorHandler: execErrorHandler);

            entity.Id = id;
            // if (id.CompareTo(default) <= 0)
            if (!entity.IdIsValid())
            {
                LogHelper.Warn($"Insert Into {BaseTableName} handler error! param is {entity.ToParamString()}   {InsertSql} ", appendMessage: false);
            }
            entity.Id = id;
            return id;
        }

        public virtual async Task<TId> InsertOneAsync(TEntity entity, long warnMs = -1L, Action<Exception, string> execErrorHandler = null)
        {
            if (entity == null) return default;
            if (execErrorHandler == null)
            {
                execErrorHandler = (ex, sql) =>
                {
                    LogHelper.Error($" sql:{sql} exec error param is {entity.ToParamString()}  {ex.Message} ", ex);
                };
            }
            var id = await DbExecuteScalarAsync<TId>(InsertSql, entity, warnMs: warnMs, execErrorHandler: execErrorHandler);
            entity.Id = id;
            // if (id.CompareTo(default) <= 0)
            if (!entity.IdIsValid())
            {
                LogHelper.Warn($"Insert Into {BaseTableName} handler error! param is {entity.ToParamString()} {InsertSql}", appendMessage: false);
            }
            entity.Id = id;
            return id;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual bool DeleteById(TId id, long warnMs = -1L, Action<Exception, string> execErrorHandler = null)
        {
            var flag = DbExecute(DeleteByIdSql, new { Id = id }, execErrorHandler: execErrorHandler);
            return flag > 0;
        }

        public virtual async Task<bool> DeleteByIdAsync(TId id, long warnMs = -1L, Action<Exception, string> execErrorHandler = null)
        {
            var flag = await DbExecuteAsync(DeleteByIdSql, new { Id = id }, warnMs: warnMs, execErrorHandler: execErrorHandler);
            return flag > 0;
        }

        #region IStatusRepositoryService

        /// <summary>
        /// 
        /// impl<see cref="IStatusRepositoryService{TEntity}"/>
        /// </summary>
        /// <param name="status"><see cref="DataStatusEnums"/></param>
        /// <returns></returns>
        public virtual IEnumerable<TEntity> QueryListByStatus(int status, long warnMs = -1L, Action<Exception, string> execErrorHandler = null)
        {
            return DbQueryList(QueryListByStatusSql, new { QueryStatus = status }, warnMs: warnMs, execErrorHandler: execErrorHandler);
        }


        /// <summary>
        /// 
        /// impl<see cref="IStatusRepositoryService{TEntity, TId}"/>
        /// </summary>
        /// <param name="status"><see cref="DataStatusEnums"/></param>
        /// <returns></returns>
        public virtual async Task<IEnumerable<TEntity>> QueryListByStatusAsync(int status, long warnMs = -1L, Action<Exception, string> execErrorHandler = null)
        {
            return await DbQueryListAsync(QueryListByStatusSql, new { QueryStatus = status }, warnMs: warnMs, execErrorHandler: execErrorHandler);
        }

        #endregion // end IStatusRepositoryService




        #endregion // end Id opt


    }



}
