using WindNight.Core.SQL;
using WindNight.Core.SQL.Abstractions;

namespace WindNight.Extension.Dapper.Mysql
{
    /// <summary>
    ///     自定义基于Dapper的Mysql基类
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <inheritdoc />
    public abstract partial class NoIdMysqlBase<TEntity>
    {

        #region Async

        #region T

        /// <summary>
        ///  查询自定义对象 Async
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <param name="warnMs"></param>
        /// <param name="execErrorHandler"></param>
        /// <returns></returns>
        protected virtual async Task<T> DbQueryEAsync<T>(string sql, object param = null, long warnMs = -1L, Action<Exception, string> execErrorHandler = null)
        {

            return await DbQueryEAsync<T>(DbConnectString, sql, param, warnMs, execErrorHandler);

            //return await SqlTimerAsync((_1, _2, _3) => QueryAsync<T>(DbConnectString, _1, _2, _3),
            //    sql, param, nameof(DbQueryE), warnMs, execErrorHandler);


        }

        /// <summary>
        ///  查询自定义对象 Async 自定义连接
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="conn"></param>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <param name="warnMs"></param>
        /// <param name="execErrorHandler"></param>
        /// <returns></returns>
        protected virtual async Task<T> DbQueryEAsync<T>(string conn, string sql, object param = null, long warnMs = -1L, Action<Exception, string> execErrorHandler = null)
        {
            return await SqlTimerAsync(async (_1, _sql, _param, _4) => await QueryAsync<T>(_1, _sql, _param, _4), conn,
                sql,
                param,
                nameof(DbQueryE), warnMs, execErrorHandler);
        }

        /// <summary>
        ///  查询自定义对象列表 Async
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <param name="warnMs"></param>
        /// <param name="execErrorHandler"></param>
        /// <returns></returns>
        protected virtual async Task<IEnumerable<T>> DbQueryEListAsync<T>(string sql, object param = null, long warnMs = -1L, Action<Exception, string> execErrorHandler = null)
        {

            return await DbQueryEListAsync<T>(DbConnectString, sql, param, warnMs, execErrorHandler);
            //return await SqlTimerAsync(
            //    async (_sql, _param, _3) => await QueryListAsync<T>(DbConnectString, _sql, _param, _3),
            //    sql, param, nameof(DbQueryEList), warnMs, execErrorHandler);
        }

        /// <summary>
        ///  查询自定义对象列表 Async 自定义连接
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="conn"></param>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <param name="warnMs"></param>
        /// <param name="execErrorHandler"></param>
        /// <returns></returns>
        protected virtual async Task<IEnumerable<T>> DbQueryEListAsync<T>(string conn, string sql, object param = null, long warnMs = -1L, Action<Exception, string> execErrorHandler = null)
        {
            return await SqlTimerAsync(async (_1, _sql, _param, _4) => await QueryListAsync<T>(_1, _sql, _param, _4),
                conn,
                sql, param, nameof(DbQueryEList), warnMs, execErrorHandler);
        }

        #endregion


        #region TEntity

        /// <summary>
        ///  查询实体对象 Async <see cref="IEntity"/>
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <param name="warnMs"></param>
        /// <param name="execErrorHandler"></param>
        /// <returns></returns>
        protected virtual async Task<TEntity> DbQueryAsync(string sql, object param = null, long warnMs = -1L, Action<Exception, string> execErrorHandler = null)
        {
            return await DbQueryAsync(DbConnectString, sql, param, warnMs, execErrorHandler);
            //return await SqlTimerAsync(
            //    async (_sql, _param, _3) => await QueryAsync<TEntity>(DbConnectString, _sql, _param, _3),
            //    sql, param, nameof(DbQuery), warnMs, execErrorHandler);
        }


        /// <summary>
        ///  查询实体对象 Async <see cref="IEntity"/> 自定义连接
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <param name="warnMs"></param>
        /// <param name="execErrorHandler"></param>
        /// <returns></returns>
        protected virtual async Task<TEntity> DbQueryAsync(string conn, string sql, object param = null, long warnMs = -1L, Action<Exception, string> execErrorHandler = null)
        {
            return await SqlTimerAsync(async (_1, _sql, _param, _4) => await QueryAsync<TEntity>(_1, _sql, _param, _4),
                conn, sql, param, nameof(DbQueryAsync), warnMs, execErrorHandler);
        }

        /// <summary>
        ///   查询实体对象列表 Async <see cref="IEntity"/>
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <param name="warnMs"></param>
        /// <param name="execErrorHandler"></param>
        /// <returns></returns>
        protected virtual async Task<IEnumerable<TEntity>> DbQueryListAsync(string sql, object param = null, long warnMs = -1L, Action<Exception, string> execErrorHandler = null)
        {

            return await DbQueryListAsync(DbConnectString, sql, param, warnMs, execErrorHandler);
            //return await SqlTimerAsync(
            //    async (_sql, _param, _3) => await QueryListAsync<TEntity>(DbConnectString, _sql, _param, _3),
            //    sql, param, nameof(DbQueryList), warnMs, execErrorHandler);
        }

        /// <summary>
        ///   查询实体对象列表 Async <see cref="IEntity"/> 自定义连接
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <param name="warnMs"></param>
        /// <param name="execErrorHandler"></param>
        /// <returns></returns>
        protected virtual async Task<IEnumerable<TEntity>> DbQueryListAsync(string conn, string sql, object param = null, long warnMs = -1L, Action<Exception, string> execErrorHandler = null)
        {
            return await SqlTimerAsync(
                async (_1, _sql, _param, _4) => await QueryListAsync<TEntity>(_1, _sql, _param, _4),
                conn, sql, param, nameof(DbQueryList), warnMs, execErrorHandler);
        }


        #endregion





        /// <summary>
        ///   执行受影响行数 Async
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <param name="warnMs"></param>
        /// <param name="execErrorHandler"></param>
        /// <returns></returns>
        protected virtual async Task<int> DbExecuteAsync(string sql, object param = null, long warnMs = -1L, Action<Exception, string> execErrorHandler = null)
        {

            return await DbExecuteAsync(DbConnectString, sql, param, warnMs, execErrorHandler);
            //return await SqlTimerAsync(
            //    async (_sql, _param, _3) => await ExecuteAsync(DbConnectString, _sql, _param, _3),
            //    sql, param, nameof(DbExecute), warnMs, execErrorHandler);
        }

        /// <summary>
        ///  执行受影响行数 Async  自定义连接
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <param name="warnMs"></param>
        /// <param name="execErrorHandler"></param>
        /// <returns></returns>
        protected virtual async Task<int> DbExecuteAsync(string conn, string sql, object param = null, long warnMs = -1L, Action<Exception, string> execErrorHandler = null)
        {
            return await SqlTimerAsync(async (_1, _sql, _param, _4) => await ExecuteAsync(_1, _sql, _param, _4),
                conn, sql, param, nameof(DbExecute), warnMs, execErrorHandler);
        }

        /// <summary>
        ///  首行首列数据 Async 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <param name="warnMs"></param>
        /// <param name="execErrorHandler"></param>
        /// <returns></returns>
        protected virtual async Task<T> DbExecuteScalarAsync<T>(string sql, object param = null, long warnMs = -1L, Action<Exception, string> execErrorHandler = null)
        {

            return await DbExecuteScalarAsync<T>(DbConnectString, sql, param, warnMs, execErrorHandler);

            //return await SqlTimerAsync(
            //    async (_sql, _param, _3) => await ExecuteScalarAsync<T>(DbConnectString, _sql, _param, _3),
            //    sql, param, nameof(ExecuteScalar), warnMs, execErrorHandler);
        }


        /// <summary>
        ///  首行首列数据 Async  自定义连接
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="conn"></param>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <param name="warnMs"></param>
        /// <param name="execErrorHandler"></param>
        /// <returns></returns>
        protected virtual async Task<T> DbExecuteScalarAsync<T>(string conn, string sql, object param = null, long warnMs = -1L, Action<Exception, string> execErrorHandler = null)
        {
            return await SqlTimerAsync(
                async (_1, _sql, _param, _4) => await ExecuteScalarAsync<T>(_1, _sql, _param, _4),
                conn, sql, param, nameof(ExecuteScalar), warnMs, execErrorHandler);
        }


        #endregion


    }

}
