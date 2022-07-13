using Newtonsoft.Json.Extension;
using WindNight.Core.SQL.Abstractions;
using WindNight.Extension.Dapper.Internals;

namespace WindNight.Extension.Dapper.Mysql
{
    ///<inheritdoc />
    public abstract partial class MySqlBase<TEntity, TId> : NoIdMysqlBase<TEntity>
    where TEntity : class, IEntity<TId>, new()
    where TId : IEquatable<TId>, IComparable<TId>
    {
        protected string QueryDataByIdSql => $"SELECT * FROM {BaseTableName} WHERE Id=@Id;";

        private string QueryListByStatusSql => $"SELECT * FROM {BaseTableName} WHERE Status=@QueryStatus";
        private string QueryChildrenByParentIdSql => $"SELECT * FROM {BaseTableName} WHERE ParentId=@QueryParentId";

        private string DeleteByIdSql =>
$@"UPDATE {BaseTableName} SET IsDeleted={1} WHERE Id=@Id;";

        protected virtual string QueryAllSqlStr => $"SELECT * FROM {BaseTableName}";

        /// <summary>
        ///  获取整表数据 慎用
        /// </summary>
        /// <returns></returns>
        public virtual IEnumerable<TEntity> QueryAllList()
        {
            return DbQueryList(QueryAllSqlStr);
        }

        /// <summary>
        /// 异步获取整表数据 慎用
        /// </summary>
        /// <returns></returns>
        public virtual async Task<IEnumerable<TEntity>> QueryAllListAsync()
        {
            return await DbQueryListAsync(QueryAllSqlStr);
        }


        #region Id opt

        public virtual TEntity QueryById(TId id)
        {
            return DbQuery(QueryDataByIdSql, new { Id = id });
        }

        public virtual async Task<TEntity> QueryByIdAsync(TId id)
        {
            return await DbQueryAsync(QueryDataByIdSql, new { Id = id });
        }

        public virtual TId InsertOne(TEntity entity)
        {
            if (entity == null) return default;


            var id = DbExecuteScalar<TId>(InsertSql, entity);
            if (id.CompareTo(default) < 0)
                LogHelper.Warn($"Insert Into {BaseTableName} handler error ,entities is {entity.ToJsonStr()} . ",
                    appendMessage: false);
            entity.Id = id;
            return id;
        }

        public virtual async Task<TId> InsertOneAsync(TEntity entity)
        {
            if (entity == null) return default;

            var id = await DbExecuteScalarAsync<TId>(InsertSql, entity);
            if (id.CompareTo(default) < 0)
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
        public virtual bool DeleteById(TId id)
        {
            var flag = DbExecute(DeleteByIdSql, new { Id = id });
            return flag > 0;
        }

        public virtual async Task<bool> DeleteByIdAsync(TId id)
        {
            var flag = await DbExecuteAsync(DeleteByIdSql, new { Id = id });
            return flag > 0;
        }

        #region IStatusRepositoryService

        /// <summary>
        /// 
        /// impl<see cref="IStatusRepositoryService{TEntity}"/>
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        public IEnumerable<TEntity> QueryListByStatus(DataStatusEnums status)
        {
            return DbQueryList(QueryListByStatusSql, new { QueryStatus = (int)status });
        }


        /// <summary>
        /// 
        /// impl<see cref="IStatusRepositoryService{TEntity, TId}"/>
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        public async Task<IEnumerable<TEntity>> QueryListByStatusAsync(DataStatusEnums status)
        {
            return await DbQueryListAsync(QueryListByStatusSql, new { QueryStatus = (int)status });
        }

        #endregion // end IStatusRepositoryService



        #region ITreeRepositoryService

        /// <summary>
        /// 
        /// impl<see cref="ITreeRepositoryService{TEntity, TId}"/>
        /// </summary>
        /// <param name="parentId"></param>
        /// <returns></returns>
        public IEnumerable<TEntity> QueryChildrenByParentId(TId parentId)
        {
            return DbQueryList(QueryChildrenByParentIdSql, new { QueryParentId = parentId });
        }


        /// <summary>
        /// 
        /// impl<see cref="ITreeRepositoryService{TEntity, TId}"/>
        /// </summary>
        /// <param name="parentId"></param>
        /// <returns></returns>
        public async Task<IEnumerable<TEntity>> QueryChildrenByParentIdAsync(TId parentId)
        {
            return await DbQueryListAsync(QueryChildrenByParentIdSql, new { QueryParentId = parentId });
        }

        #endregion //end ITreeRepositoryService

        #endregion// end Id opt

        /// <summary>
        /// 分页
        /// impl<see cref="IPagedListRepositoryService{TEntity, TId}"/>
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="condition"></param>
        /// <param name="orderBy"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public IPagedList<TEntity> QueryPagedList(int pageIndex, int pageSize, string condition, string orderBy,
            IDictionary<string, object> parameters = null)
        {
            return DbPagedList(pageIndex, pageSize, condition, orderBy, parameters);
        }

        /// <summary>
        /// 异步分页
        /// impl<see cref="IPagedListRepositoryService{TEntity, TId}"/>
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="condition"></param>
        /// <param name="orderBy"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public async Task<IPagedList<TEntity>> QueryPagedListAsync(int pageIndex, int pageSize, string condition,
            string orderBy, IDictionary<string, object> parameters = null)
        {
            return await DbPagedListAsync(pageIndex, pageSize, condition, orderBy, parameters);
        }



    }
}