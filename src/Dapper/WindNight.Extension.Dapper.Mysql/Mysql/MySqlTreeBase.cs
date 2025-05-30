using WindNight.Core.SQL.Abstractions;
using WindNight.Extension.Dapper.Mysql.@internal;
using WindNight.Extension.Db.Abstractions;

namespace WindNight.Extension.Dapper.Mysql
{


    public abstract partial class MySqlTreeBase<TEntity> : MySqlTreeBase<TEntity, int>
        where TEntity : class, ITreeEntity<int>, IEntity<int>, new()
    {


    }


    ///<inheritdoc />
    public abstract partial class MySqlTreeBase<TEntity, TId> : MySqlBase<TEntity, TId>, ITreeRepositoryService<TEntity, TId>
        where TEntity : class, ITreeEntity<TId>, IEntity<TId>, new()
        where TId : IEquatable<TId>, IComparable<TId>
    {
        //private string QueryChildrenByParentIdSql => $"SELECT * FROM {BaseTableName} WHERE ParentId=@QueryParentId";
        //        protected virtual string QueryChildrenByParentIdSql => @"WITH RECURSIVE td AS (
        //   SELECT * FROM {0} WHERE  Id =@QueryParentId {1}
        //    UNION ALL
        //    SELECT c.* FROM {0} c ,td WHERE c.ParentId = td.Id
        //) SELECT * FROM td WHERE td.IsDeleted=0 ORDER BY td.Id; ";

        protected virtual string QueryChildrenByParentIdSql =>
@"WITH RECURSIVE td AS (
   SELECT * FROM {0} WHERE 1=1 {1}
    UNION ALL
    SELECT c.* FROM {0} c ,td WHERE c.ParentId = td.Id
) SELECT * FROM td WHERE td.IsDeleted=0 ORDER BY td.Id; ";

        protected virtual string QueryParentsByChildIdSql =>
@"WITH RECURSIVE td AS (
   SELECT * FROM {0} WHERE Id = @QueryChildId {1}
    UNION ALL
    SELECT c.* FROM {0} c ,td WHERE c.Id = td.ParentId
) SELECT * FROM td WHERE td.IsDeleted=0  ORDER BY td.Id; ";


        #region ITreeRepositoryService

        public virtual IEnumerable<TEntity> QueryChildrenByParentId(TId parentId, string rootCondition = "", Dictionary<string, object> param = null, long warnMs = -1)
        {
            return QueryChildrenByParentId<TEntity>(BaseTableName, parentId, warnMs: warnMs);
        }

        public virtual async Task<IEnumerable<TEntity>> QueryChildrenByParentIdAsync(TId parentId, string rootCondition = "", Dictionary<string, object> param = null, long warnMs = -1)
        {
            return await QueryChildrenByParentIdAsync<TEntity>(BaseTableName, parentId, warnMs: warnMs);
        }

        public virtual IEnumerable<TEntity> QueryParentsByChildId(TId childId, string rootCondition = "", Dictionary<string, object> param = null, long warnMs = -1)
        {
            return QueryParentsByChildId<TEntity>(BaseTableName, childId, warnMs: warnMs);
        }

        public virtual async Task<IEnumerable<TEntity>> QueryParentsByChildIdAsync(TId childId, string rootCondition = "", Dictionary<string, object> param = null, long warnMs = -1)
        {
            return await QueryParentsByChildIdAsync<TEntity>(BaseTableName, childId, warnMs: warnMs);
        }

        /// <summary>
        /// 
        /// impl<see cref="ITreeRepositoryService{TEntity, TId}"/>
        /// </summary>
        /// <param name="parentId"></param>
        /// <param name="rootCondition"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public virtual IEnumerable<T> QueryChildrenByParentId<T>(TId parentId, string rootCondition = "", Dictionary<string, object> param = null, long warnMs = -1) where T : class, ITreeEntity<TId>, new()
        {
            return QueryChildrenByParentId<T>(BaseTableName, parentId, rootCondition, param, warnMs: warnMs);
        }

        bool IdIsValid(TId id)
        {
            return id != null && id.CompareTo(default(TId)) > 0;
        }




        string GeneratorRealSqlForQueryChildren(string tableName, TId parentId, string rootCondition = "")
        {

            bool idValid = IdIsValid(parentId);
            if (rootCondition.IsNullOrEmpty())
            {
                if (idValid) // parentId>0
                {
                    rootCondition = $" AND Id= @QueryParentId ";
                }
                else
                {
                    rootCondition = $" AND ParentId=0 "; //参数id=0 默认查 所有根节点的tree

                }
            }
            else
            {
                if (idValid)
                {
                    rootCondition = $" AND Id= @QueryParentId AND {rootCondition} ";
                }

            }

            var sql = string.Format(QueryChildrenByParentIdSql, tableName, rootCondition);
            if (ConfigItems.OpenDapperLog)
            {
                LogHelper.Debug($"QueryChildrenByParentId({tableName}) sql is :{sql}");
            }

            return sql;
        }


        /// <summary>
        /// 
        /// impl<see cref="ITreeRepositoryService{TEntity, TId}"/>
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="parentId"></param>
        /// <param name="rootCondition"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public virtual IEnumerable<T> QueryChildrenByParentId<T>(string tableName, TId parentId, string rootCondition = "", Dictionary<string, object> param = null, long warnMs = -1)
            where T : class, ITreeEntity<TId>, new()
        {
            if (tableName.IsNullOrEmpty())
            {
                tableName = BaseTableName;
            }

            var sql = GeneratorRealSqlForQueryChildren(tableName, parentId, rootCondition);

            param ??= new Dictionary<string, object>();
            if (IdIsValid(parentId))
            {
                param.Add("QueryParentId", parentId);
            }

            return DbQueryEList<T>(sql, param, warnMs: warnMs);//new { QueryParentId = parentId });
        }


        /// <summary>
        /// 
        /// impl<see cref="ITreeRepositoryService{TEntity, TId}"/>
        /// </summary>
        /// <param name="parentId"></param>
        /// <param name="rootCondition"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public virtual async Task<IEnumerable<T>> QueryChildrenByParentIdAsync<T>(TId parentId, string rootCondition = "", Dictionary<string, object> param = null, long warnMs = -1)
            where T : class, ITreeEntity<TId>, new()
        {
            return await QueryChildrenByParentIdAsync<T>(BaseTableName, parentId, warnMs: warnMs);

        }

        /// <summary>
        /// 
        /// impl<see cref="ITreeRepositoryService{TEntity, TId}"/>
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="parentId"></param>
        /// <param name="rootCondition"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public virtual async Task<IEnumerable<T>> QueryChildrenByParentIdAsync<T>(string tableName, TId parentId, string rootCondition = "", Dictionary<string, object> param = null, long warnMs = -1)
            where T : class, ITreeEntity<TId>, new()
        {
            if (tableName.IsNullOrEmpty())
            {
                tableName = BaseTableName;
            }

            var sql = GeneratorRealSqlForQueryChildren(tableName, parentId, rootCondition);


            param ??= new Dictionary<string, object>();
            if (IdIsValid(parentId))
            {
                param.Add("QueryParentId", parentId);
            }

            return await DbQueryEListAsync<T>(sql, param, warnMs: warnMs);// new { QueryParentId = parentId });
        }


        /// <summary>
        /// 
        /// impl<see cref="ITreeRepositoryService{TEntity, TId}"/>
        /// </summary>
        /// <param name="childId"></param>
        /// <param name="rootCondition"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public virtual IEnumerable<T> QueryParentsByChildId<T>(TId childId, string rootCondition = "", Dictionary<string, object> param = null, long warnMs = -1)
            where T : class, ITreeEntity<TId>, new()
        {
            return QueryParentsByChildId<T>(BaseTableName, childId, rootCondition, param, warnMs: warnMs);

        }

        /// <summary>
        /// 
        /// impl<see cref="ITreeRepositoryService{TEntity, TId}"/>
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="childId"></param>
        /// <param name="rootCondition"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public virtual IEnumerable<T> QueryParentsByChildId<T>(string tableName, TId childId, string rootCondition = "", Dictionary<string, object> param = null, long warnMs = -1)
            where T : class, ITreeEntity<TId>, new()
        {
            if (tableName.IsNullOrEmpty())
            {
                tableName = BaseTableName;
            }
            var sql = string.Format(QueryParentsByChildIdSql, tableName, rootCondition);
            if (ConfigItems.OpenDapperLog)
            {
                LogHelper.Debug($"{nameof(QueryParentsByChildId)}({tableName}) sql is :{sql}");
            }
            param ??= new Dictionary<string, object>();
            param.Add("QueryChildId", childId);
            return DbQueryEList<T>(sql, param, warnMs: warnMs);
        }


        /// <summary>
        /// 
        /// impl<see cref="ITreeRepositoryService{TEntity, TId}"/>
        /// </summary>
        /// <param name="childId"></param>
        /// <param name="rootCondition"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public virtual async Task<IEnumerable<T>> QueryParentsByChildIdAsync<T>(TId childId, string rootCondition = "", Dictionary<string, object> param = null, long warnMs = -1)
            where T : class, ITreeEntity<TId>, new()
        {
            return await QueryParentsByChildIdAsync<T>(BaseTableName, childId, rootCondition, param, warnMs: warnMs);

        }

        /// <summary>
        /// 
        /// impl<see cref="ITreeRepositoryService{TEntity, TId}"/>
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="childId"></param>
        /// <param name="rootCondition"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public virtual async Task<IEnumerable<T>> QueryParentsByChildIdAsync<T>(string tableName, TId childId, string rootCondition = "", Dictionary<string, object> param = null, long warnMs = -1)
            where T : class, ITreeEntity<TId>, new()
        {
            if (tableName.IsNullOrEmpty())
            {
                tableName = BaseTableName;
            }
            var sql = string.Format(QueryParentsByChildIdSql, tableName, rootCondition);
            if (ConfigItems.OpenDapperLog)
            {
                LogHelper.Debug($"{nameof(QueryParentsByChildIdAsync)}({tableName}) sql is :{sql}");
            }

            param ??= new Dictionary<string, object>();
            param.Add("QueryChildId", childId);
            return await DbQueryEListAsync<T>(sql, param, warnMs: warnMs);
        }




        #endregion //end ITreeRepositoryService

    }

}
