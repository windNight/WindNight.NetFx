﻿using WindNight.Core.SQL;
using WindNight.Core.SQL.Abstractions;

namespace WindNight.Extension.Dapper.Mysql
{
    /// <summary>
    ///     自定义基于Dapper的Mysql基类
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <inheritdoc />
    public abstract partial class NoIdMysqlBase<TEntity> : MySqlBase
        where TEntity : class, IEntity, new()
    {

        #region Sync

        /// <summary>
        /// 查询自定义对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        protected virtual T DbQueryE<T>(string sql, object param = null) => DbQueryE<T>(DbConnectString, sql, param);

        //    {
        //        return SqlTimer((_sql, _param) => Query<T>(DbConnectString, _sql, _param),
        //            sql, param, nameof(DbQueryE));
        //}

        protected virtual T DbQueryE<T>(string conn, string sql, object param = null)
        {
            return SqlTimer(Query<T>, conn, sql, param, nameof(DbQueryE));
        }

        /// <summary>
        /// 查询自定义对象列表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        protected virtual IEnumerable<T> DbQueryEList<T>(string sql, object param = null)
        {
            return SqlTimer((_sql, _param) => QueryList<T>(DbConnectString, _sql, _param),
                sql, param, nameof(DbQueryEList));
        }

        protected virtual IEnumerable<T> DbQueryEList<T>(string conn, string sql, object param = null)
        {
            return SqlTimer(QueryList<T>, conn, sql, param, nameof(DbQueryEList));
        }

        /// <summary>
        /// 查询实体对象
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        protected virtual TEntity DbQuery(string sql, object param = null)
        {
            return SqlTimer((_sql, _param) => Query<TEntity>(DbConnectString, _sql, _param),
                sql, param, nameof(DbQuery));
        }

        protected virtual TEntity DbQuery(string conn, string sql, object param = null)
        {
            return SqlTimer(Query<TEntity>, conn, sql, param, nameof(DbQuery));
        }

        /// <summary>
        /// 查询实体对象列表
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        protected virtual IEnumerable<TEntity> DbQueryList(string sql, object param = null)
        {
            return SqlTimer((_sql, _param) => QueryList<TEntity>(DbConnectString, _sql, _param),
                sql, param, nameof(DbQueryList));
        }

        protected virtual IEnumerable<TEntity> DbQueryList(string conn, string sql, object param = null)
        {
            return SqlTimer(QueryList<TEntity>, conn, sql, param, nameof(DbQueryList));
        }

        /// <summary>
        /// 执行受影响行数
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        protected virtual int DbExecute(string sql, object param = null)
        {
            return SqlTimer((_sql, _param) => Execute(DbConnectString, _sql, _param),
                sql, param, nameof(DbExecute));
        }
        protected virtual int DbExecute(string conn, string sql, object param = null)
        {
            return SqlTimer(Execute, conn, sql, param, nameof(DbExecute));
        }

        /// <summary>
        /// 首行首列数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        protected virtual T DbExecuteScalar<T>(string sql, object param = null)
        {
            return SqlTimer((_sql, _param) => ExecuteScalar<T>(DbConnectString, _sql, _param),
                sql, param, nameof(ExecuteScalar));
        }

        protected virtual T DbExecuteScalar<T>(string conn, string sql, object param = null)
        {
            return SqlTimer(ExecuteScalar<T>, conn, sql, param, nameof(ExecuteScalar));
        }

        /// <summary>
        /// 分页
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="condition"></param>
        /// <param name="orderBy"></param>
        /// <param name="queryTableName">查询的表或者临时表 ,NullOrEmpty=><see cref="BaseTableName"/></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        protected virtual IPagedList<TEntity> DbPagedList(int pageIndex, int pageSize, string condition,
            string orderBy, IDictionary<string, object> parameters = null, string queryTableName = "")
        {
            var pagedInfo = new QueryPageInfo
            {
                TableName = queryTableName.IsNullOrEmpty() ? BaseTableName : queryTableName,
                Fields = "*",
                SqlWhere = condition,
                OrderField = orderBy,
                PageIndex = pageIndex,
                PageSize = pageSize
            };

            return PagedList<TEntity>(DbConnectString, pagedInfo, parameters);
        }

        protected virtual IPagedList<T> DbPagedList<T>(int pageIndex, int pageSize, string condition,
            string orderBy, IDictionary<string, object> parameters = null, string queryTableName = "")
        where T : class, new()
        {
            var pagedInfo = new QueryPageInfo
            {
                TableName = queryTableName.IsNullOrEmpty() ? BaseTableName : queryTableName,
                Fields = "*",
                SqlWhere = condition,
                OrderField = orderBy,
                PageIndex = pageIndex,
                PageSize = pageSize
            };

            return PagedList<T>(DbConnectString, pagedInfo, parameters);
        }

        /// <summary>
        ///   
        /// </summary>
        /// <param name="pagedInfo"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        protected virtual IPagedList<T> DbPagedList<T>(IQueryPageInfo pagedInfo, IDictionary<string, object> parameters)
            where T : class, new()
        {

            return PagedList<T>(DbConnectString, pagedInfo, parameters);
        }

        /// <summary>
        ///   
        /// </summary>
        /// <param name="pagedInfo"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        protected virtual IPagedList<TEntity> DbPagedList(IQueryPageInfo pagedInfo, IDictionary<string, object> parameters)
        {

            return PagedList<TEntity>(DbConnectString, pagedInfo, parameters);
        }

        #endregion //end Sync

        #region Async

        /// <summary>
        /// 查询自定义对象 Async
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        protected virtual async Task<T> DbQueryEAsync<T>(string sql, object param = null)
        {
            return await SqlTimerAsync((_1, _2) => QueryAsync<T>(DbConnectString, _1, _2),
                sql, param, nameof(DbQueryE));
        }

        protected virtual async Task<T> DbQueryEAsync<T>(string conn, string sql, object param = null)
        {
            return await SqlTimerAsync(async (_1, _sql, _param) => await QueryAsync<T>(_1, _sql, _param), conn, sql, param, nameof(DbQueryE));
        }

        /// <summary>
        /// 查询自定义对象列表 Async
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        protected virtual async Task<IEnumerable<T>> DbQueryEListAsync<T>(string sql, object param = null)
        {
            return await SqlTimerAsync(async (_sql, _param) => await QueryListAsync<T>(DbConnectString, _sql, _param),
                sql, param, nameof(DbQueryEList));
        }

        protected virtual async Task<IEnumerable<T>> DbQueryEListAsync<T>(string conn, string sql, object param = null)
        {
            return await SqlTimerAsync(async (_1, _sql, _param) => await QueryListAsync<T>(_1, _sql, _param), conn,
                sql, param, nameof(DbQueryEList));
        }

        /// <summary>
        /// 查询实体对象 Async
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        protected virtual async Task<TEntity> DbQueryAsync(string sql, object param = null)
        {
            return await SqlTimerAsync(async (_sql, _param) => await QueryAsync<TEntity>(DbConnectString, _sql, _param),
                sql, param, nameof(DbQuery));
        }

        protected virtual async Task<TEntity> DbQueryAsync(string conn, string sql, object param = null)
        {
            return await SqlTimerAsync(async (_1, _sql, _param) => await QueryAsync<TEntity>(_1, _sql, _param),
               conn, sql, param, nameof(DbQuery));
        }

        /// <summary>
        /// 查询实体对象列表 Async
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        protected virtual async Task<IEnumerable<TEntity>> DbQueryListAsync(string sql, object param = null)
        {
            return await SqlTimerAsync(
                async (_sql, _param) => await QueryListAsync<TEntity>(DbConnectString, _sql, _param),
                sql, param, nameof(DbQueryList));
        }

        protected virtual async Task<IEnumerable<TEntity>> DbQueryListAsync(string conn, string sql, object param = null)
        {
            return await SqlTimerAsync(
                async (_1, _sql, _param) => await QueryListAsync<TEntity>(_1, _sql, _param),
                conn, sql, param, nameof(DbQueryList));
        }

        /// <summary>
        /// 执行受影响行数 Async
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        protected virtual async Task<int> DbExecuteAsync(string sql, object param = null)
        {
            return await SqlTimerAsync(async (_sql, _param) => await ExecuteAsync(DbConnectString, _sql, _param),
                sql, param, nameof(DbExecute));
        }

        protected virtual async Task<int> DbExecuteAsync(string conn, string sql, object param = null)
        {
            return await SqlTimerAsync(async (_1, _sql, _param) => await ExecuteAsync(_1, _sql, _param),
                conn, sql, param, nameof(DbExecute));
        }

        /// <summary>
        /// 首行首列数据 Async
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        protected virtual async Task<T> DbExecuteScalarAsync<T>(string sql, object param = null)
        {
            return await SqlTimerAsync(
                async (_sql, _param) => await ExecuteScalarAsync<T>(DbConnectString, _sql, _param),
                sql, param, nameof(ExecuteScalar));
        }

        protected virtual async Task<T> DbExecuteScalarAsync<T>(string conn, string sql, object param = null)
        {
            return await SqlTimerAsync(
                async (_1, _sql, _param) => await ExecuteScalarAsync<T>(_1, _sql, _param),
                conn, sql, param, nameof(ExecuteScalar));
        }

        /// <summary>
        ///  异步分页
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="condition"></param>
        /// <param name="orderBy"></param>
        /// <param name="parameters"></param>
        /// <param name="queryTableName">查询的表或者临时表 ,NullOrEmpty=><see cref="BaseTableName"/></param>
        /// <returns></returns>
        protected virtual async Task<IPagedList<TEntity>> DbPagedListAsync(int pageIndex,
            int pageSize, string condition, string orderBy, IDictionary<string, object> parameters = null, string queryTableName = "")
        {
            var pagedInfo = new QueryPageInfo
            {
                TableName = queryTableName.IsNullOrEmpty() ? BaseTableName : queryTableName,
                Fields = "*",
                SqlWhere = condition,
                OrderField = orderBy,
                PageIndex = pageIndex,
                PageSize = pageSize
            };

            return await PagedListAsync<TEntity>(DbConnectString, pagedInfo, parameters);
        }

        /// <summary>
        ///  异步分页
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="condition"></param>
        /// <param name="orderBy"></param>
        /// <param name="parameters"></param>
        /// <param name="queryTableName">查询的表或者临时表 ,NullOrEmpty=><see cref="BaseTableName"/></param>
        /// <returns></returns>
        protected virtual async Task<IPagedList<T>> DbPagedListAsync<T>(
            int pageIndex, int pageSize,
            string condition, string orderBy,
            IDictionary<string, object> parameters = null, string queryTableName = "")
        where T : class, new()
        {
            var pagedInfo = new QueryPageInfo
            {
                TableName = queryTableName.IsNullOrEmpty() ? BaseTableName : queryTableName,
                Fields = "*",
                SqlWhere = condition,
                OrderField = orderBy,
                PageIndex = pageIndex,
                PageSize = pageSize
            };

            return await PagedListAsync<T>(DbConnectString, pagedInfo, parameters);
        }

        /// <summary>
        ///  异步分页
        /// </summary>
        /// <param name="pagedInfo"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        protected virtual async Task<IPagedList<T>> DbPagedListAsync<T>(IQueryPageInfo pagedInfo, IDictionary<string, object> parameters)
            where T : class, new()
        {

            return await PagedListAsync<T>(DbConnectString, pagedInfo, parameters);
        }

        /// <summary>
        ///  异步分页
        /// </summary>
        /// <param name="pagedInfo"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        protected virtual async Task<IPagedList<TEntity>> DbPagedListAsync(IQueryPageInfo pagedInfo, IDictionary<string, object> parameters)
        {

            return await PagedListAsync<TEntity>(DbConnectString, pagedInfo, parameters);
        }

        #endregion

    }
}