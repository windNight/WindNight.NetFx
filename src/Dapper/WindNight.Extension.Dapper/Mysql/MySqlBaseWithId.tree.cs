using System;
using System.Collections.Generic;
using System.Text;
using WindNight.Core.SQL.Abstractions;
using WindNight.Extension.Dapper.Abstractions.DB;

namespace WindNight.Extension.Dapper.Mysql
{
    ///<inheritdoc />
    public abstract partial class MySqlTreeBase<TEntity, TId> : MySqlBase<TEntity, TId>, ITreeRepositoryService<TEntity, TId>
        where TEntity : class, ITreeEntity<TId>, IEntity<TId>, new()
        where TId : IEquatable<TId>, IComparable<TId>
    {
        //private string QueryChildrenByParentIdSql => $"SELECT * FROM {BaseTableName} WHERE ParentId=@QueryParentId";
        private string QueryChildrenByParentIdSql = @"WITH RECURSIVE td AS (
   SELECT * FROM {0} WHERE Id =@QueryParentId
    UNION ALL
    SELECT c.* FROM {0} c ,td WHERE c.ParentId = td.Id
) SELECT * FROM td ORDER BY td.Id; ";

        private string QueryParentsByChildIdSql = @"WITH RECURSIVE td AS (
   SELECT * FROM {0} WHERE Id = @QueryChildId
    UNION ALL
    SELECT c.* FROM {0} c ,td WHERE c.Id = td.ParentId
) SELECT * FROM td ORDER BY td.Id; ";


        #region ITreeRepositoryService

        public virtual IEnumerable<TEntity> QueryChildrenByParentId(TId parentId)
        {
            return QueryChildrenByParentId<TEntity>(BaseTableName, parentId);
        }

        public virtual async Task<IEnumerable<TEntity>> QueryChildrenByParentIdAsync(TId parentId)
        {
            return await QueryChildrenByParentIdAsync<TEntity>(BaseTableName, parentId);
        }

        public virtual IEnumerable<TEntity> QueryParentsByChildId(TId childId)
        {
            return QueryParentsByChildId<TEntity>(BaseTableName, childId);
        }

        public virtual async Task<IEnumerable<TEntity>> QueryParentsByChildIdAsync(TId childId)
        {
            return await QueryParentsByChildIdAsync<TEntity>(BaseTableName, childId);
        }

        /// <summary>
        /// 
        /// impl<see cref="ITreeRepositoryService{TEntity, TId}"/>
        /// </summary>
        /// <param name="parentId"></param>
        /// <returns></returns>
        public virtual IEnumerable<T> QueryChildrenByParentId<T>(TId parentId) where T : class, ITreeEntity<TId>, new()
        {
            return QueryChildrenByParentId<T>(BaseTableName, parentId);
        }

        /// <summary>
        /// 
        /// impl<see cref="ITreeRepositoryService{TEntity, TId}"/>
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="parentId"></param>
        /// <returns></returns>
        public virtual IEnumerable<T> QueryChildrenByParentId<T>(string tableName, TId parentId) where T : class, ITreeEntity<TId>, new()
        {
            if (tableName.IsNullOrEmpty())
            {
                tableName = BaseTableName;
            }
            var sql = string.Format(QueryChildrenByParentIdSql, tableName);
            return DbQueryEList<T>(sql, new { QueryParentId = parentId });
        }


        /// <summary>
        /// 
        /// impl<see cref="ITreeRepositoryService{TEntity, TId}"/>
        /// </summary>
        /// <param name="parentId"></param>
        /// <returns></returns>
        public virtual async Task<IEnumerable<T>> QueryChildrenByParentIdAsync<T>(TId parentId) where T : class, ITreeEntity<TId>, new()
        {
            return await QueryChildrenByParentIdAsync<T>(BaseTableName, parentId);

        }

        /// <summary>
        /// 
        /// impl<see cref="ITreeRepositoryService{TEntity, TId}"/>
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="parentId"></param>
        /// <returns></returns>
        public virtual async Task<IEnumerable<T>> QueryChildrenByParentIdAsync<T>(string tableName, TId parentId) where T : class, ITreeEntity<TId>, new()
        {
            if (tableName.IsNullOrEmpty())
            {
                tableName = BaseTableName;
            }
            var sql = string.Format(QueryChildrenByParentIdSql, tableName);
            return await DbQueryEListAsync<T>(sql, new { QueryParentId = parentId });
        }


        /// <summary>
        /// 
        /// impl<see cref="ITreeRepositoryService{TEntity, TId}"/>
        /// </summary>
        /// <param name="parentId"></param>
        /// <returns></returns>
        public virtual IEnumerable<T> QueryParentsByChildId<T>(TId parentId) where T : class, ITreeEntity<TId>, new()
        {
            return QueryParentsByChildId<T>(BaseTableName, parentId);

        }

        /// <summary>
        /// 
        /// impl<see cref="ITreeRepositoryService{TEntity, TId}"/>
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="childId"></param>
        /// <returns></returns>
        public virtual IEnumerable<T> QueryParentsByChildId<T>(string tableName, TId childId) where T : class, ITreeEntity<TId>, new()
        {
            if (tableName.IsNullOrEmpty())
            {
                tableName = BaseTableName;
            }
            var sql = string.Format(QueryParentsByChildIdSql, tableName);
            return DbQueryEList<T>(sql, new { QueryChildId = childId });
        }


        /// <summary>
        /// 
        /// impl<see cref="ITreeRepositoryService{TEntity, TId}"/>
        /// </summary>
        /// <param name="parentId"></param>
        /// <returns></returns>
        public virtual async Task<IEnumerable<T>> QueryParentsByChildIdAsync<T>(TId parentId) where T : class, ITreeEntity<TId>, new()
        {
            return await QueryParentsByChildIdAsync<T>(BaseTableName, parentId);

        }

        /// <summary>
        /// 
        /// impl<see cref="ITreeRepositoryService{TEntity, TId}"/>
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="childId"></param>
        /// <returns></returns>
        public virtual async Task<IEnumerable<T>> QueryParentsByChildIdAsync<T>(string tableName, TId childId) where T : class, ITreeEntity<TId>, new()
        {
            if (tableName.IsNullOrEmpty())
            {
                tableName = BaseTableName;
            }
            var sql = string.Format(QueryParentsByChildIdSql, tableName);
            return await DbQueryEListAsync<T>(sql, new { QueryChildId = childId });
        }




        #endregion //end ITreeRepositoryService

    }

}
