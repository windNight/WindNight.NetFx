using WindNight.Core.SQL;
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

        #region T

        /// <summary>
        /// 查询自定义对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <param name="warnMs"></param>
        /// <param name="execErrorHandler"></param>
        /// <returns></returns>
        protected virtual T DbQueryE<T>(string sql, object param = null, long warnMs = -1, Action<Exception, string> execErrorHandler = null, bool isDebug = false) =>
            DbQueryE<T>(DbConnectString, sql, param, warnMs, execErrorHandler, isDebug);

        /// <summary>
        /// 查询自定义对象 自定义连接
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="conn"></param>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <param name="warnMs"></param>
        /// <param name="execErrorHandler"></param>
        /// <returns></returns>
        protected virtual T DbQueryE<T>(string conn, string sql, object param = null, long warnMs = -1, Action<Exception, string> execErrorHandler = null, bool isDebug = false)
        {
            return SqlTimer(Query<T>, conn, sql, param, nameof(DbQueryE), warnMs, execErrorHandler, isDebug);
        }

        /// <summary>
        /// 查询自定义对象列表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <param name="warnMs"></param>
        /// <param name="execErrorHandler"></param>
        /// <returns></returns>
        protected virtual IEnumerable<T> DbQueryEList<T>(string sql, object param = null, long warnMs = -1, Action<Exception, string> execErrorHandler = null, bool isDebug = false)
        {
            return DbQueryEList<T>(DbConnectString, sql, param, warnMs, execErrorHandler, isDebug);
            //return SqlTimer((_sql, _param, _3) => QueryList<T>(DbConnectString, _sql, _param, _3),
            //    sql, param, nameof(DbQueryEList), warnMs, execErrorHandler);
        }

        /// <summary>
        ///  查询自定义对象列表  自定义连接
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="conn"></param>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <param name="warnMs"></param>
        /// <param name="execErrorHandler"></param>
        /// <returns></returns>
        protected virtual IEnumerable<T> DbQueryEList<T>(string conn, string sql, object param = null, long warnMs = -1, Action<Exception, string> execErrorHandler = null, bool isDebug = false)
        {
            return SqlTimer(QueryList<T>, conn, sql, param, nameof(DbQueryEList), warnMs, execErrorHandler, isDebug);
        }

        #endregion

        #region TEntity


        /// <summary>
        ///  查询实体对象 <see cref="TEntity"/>
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <param name="warnMs"></param>
        /// <param name="execErrorHandler"></param>
        /// <returns></returns>
        protected virtual TEntity DbQuery(string sql, object param = null, long warnMs = -1, Action<Exception, string> execErrorHandler = null, bool isDebug = false)
        {

            return DbQuery(DbConnectString, sql, param, warnMs, execErrorHandler, isDebug);
            //return SqlTimer((_sql, _param, _3) => Query<TEntity>(DbConnectString, _sql, _param, _3),
            //    sql, param, nameof(DbQuery), warnMs, execErrorHandler);
        }


        /// <summary>
        ///  查询实体对象 <see cref="TEntity"/> 自定义连接
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <param name="warnMs"></param>
        /// <param name="execErrorHandler"></param>
        /// <returns></returns>
        protected virtual TEntity DbQuery(string conn, string sql, object param = null, long warnMs = -1, Action<Exception, string> execErrorHandler = null, bool isDebug = false)
        {
            return SqlTimer(Query<TEntity>, conn, sql, param, nameof(DbQuery), warnMs, execErrorHandler, isDebug);
        }

        /// <summary>
        ///  查询实体对象列表 <see cref="TEntity"/>
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <param name="warnMs"></param>
        /// <param name="execErrorHandler"></param>
        /// <returns></returns>
        protected virtual IEnumerable<TEntity> DbQueryList(string sql, object param = null, long warnMs = -1, Action<Exception, string> execErrorHandler = null, bool isDebug = false)
        {
            return DbQueryList(DbConnectString, sql, param, warnMs, execErrorHandler, isDebug);
            //return SqlTimer((_sql, _param, _3) => QueryList<TEntity>(DbConnectString, _sql, _param, _3),
            //    sql, param, nameof(DbQueryList), warnMs, execErrorHandler);
        }

        /// <summary>
        ///  查询实体对象列表 <see cref="TEntity"/> 自定义连接
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <param name="warnMs"></param>
        /// <param name="execErrorHandler"></param>
        /// <returns></returns>
        protected virtual IEnumerable<TEntity> DbQueryList(string conn, string sql, object param = null, long warnMs = -1, Action<Exception, string> execErrorHandler = null, bool isDebug = false)
        {
            return SqlTimer(QueryList<TEntity>, conn, sql, param, nameof(DbQueryList), warnMs, execErrorHandler, isDebug);
        }

        #endregion



        /// <summary>
        /// 执行受影响行数 
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <param name="warnMs"></param>
        /// <param name="execErrorHandler"></param>
        /// <returns></returns>
        protected virtual int DbExecute(string sql, object param = null, long warnMs = -1, Action<Exception, string> execErrorHandler = null, bool isDebug = false)
        {
            return DbExecute(DbConnectString, sql, param, warnMs, execErrorHandler, isDebug);
            //return SqlTimer((_sql, _param, _3) => Execute(DbConnectString, _sql, _param, _3),
            //    sql, param, nameof(DbExecute), warnMs, execErrorHandler);
        }

        /// <summary>
        ///  执行受影响行数 自定义连接
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <param name="warnMs"></param>
        /// <param name="execErrorHandler"></param>
        /// <returns></returns>
        protected virtual int DbExecute(string conn, string sql, object param = null, long warnMs = -1, Action<Exception, string> execErrorHandler = null, bool isDebug = false)
        {
            return SqlTimer(Execute, conn, sql, param, nameof(DbExecute), warnMs, execErrorHandler, isDebug);
        }

        /// <summary>
        /// 首行首列数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <param name="warnMs"></param>
        /// <param name="execErrorHandler"></param>
        /// <returns></returns>
        protected virtual T DbExecuteScalar<T>(string sql, object param = null, long warnMs = -1, Action<Exception, string> execErrorHandler = null, bool isDebug = false)
        {

            return DbExecuteScalar<T>(DbConnectString, sql, param, warnMs, execErrorHandler, isDebug);
            //return SqlTimer((_sql, _param, _3) =>
            //        ExecuteScalar<T>(DbConnectString, _sql, _param, _3),
            //    sql, param, nameof(ExecuteScalar), warnMs, execErrorHandler);
        }

        /// <summary>
        /// 首行首列数据  自定义连接
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="conn"></param>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <param name="warnMs"></param>
        /// <param name="execErrorHandler"></param>
        /// <returns></returns>
        protected virtual T DbExecuteScalar<T>(string conn, string sql, object param = null, long warnMs = -1, Action<Exception, string> execErrorHandler = null, bool isDebug = false)
        {
            return SqlTimer(ExecuteScalar<T>, conn, sql, param, nameof(ExecuteScalar), warnMs, execErrorHandler, isDebug);
        }

        #endregion //end Sync

    }

}
