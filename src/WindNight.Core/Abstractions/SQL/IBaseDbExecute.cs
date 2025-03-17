using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace WindNight.Core.SQL.Abstractions
{
    public interface IQueryPagedList
    {
        #region PagedList
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connStr"></param>
        /// <param name="sqlPageInfo"></param>
        /// <param name="parameters"></param>
        /// <param name="warnMs"></param>
        /// <param name="execErrorHandler"></param>
        /// <returns></returns>
        Task<IPagedList<T>> PagedListAsync<T>(string connStr, IQueryPageInfo sqlPageInfo,
            IDictionary<string, object> parameters, long warnMs = -1, Action<Exception, string> execErrorHandler = null) where T : class, new();

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connStr"></param>
        /// <param name="sqlPageInfo"></param>
        /// <param name="parameters"></param>
        /// <param name="warnMs"></param>
        /// <param name="execErrorHandler"></param>
        /// <returns></returns>
        IPagedList<T> PagedList<T>(string connStr, IQueryPageInfo sqlPageInfo,
            IDictionary<string, object> parameters, long warnMs = -1, Action<Exception, string> execErrorHandler = null) where T : class, new();

        #endregion
    }

    /// <summary>
    ///     数据库访问基础接口
    /// </summary>
    public interface IBaseDbExecute : IQueryPagedList
    {
        /// <summary>
        ///     create IDbConnection
        /// </summary>
        /// <param name="connStr"></param>
        /// <returns></returns>
        IDbConnection GetConnection(string connStr);

        /// <summary>
        ///     ExecuteScalar 首行首列数据
        /// </summary>
        /// <param name="connStr"></param>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <param name="execErrorHandler"></param>
        /// <returns>The first cell returned, as <typeparamref name="T" />.</returns>
        T ExecuteScalar<T>(string connStr, string sql, object param = null, Action<Exception, string> execErrorHandler = null);


        /// <summary>
        ///     Execute 执行受影响行数
        /// </summary>
        /// <remarks>
        ///     需要注意项：
        ///     1、mysql update 数据没有变更的情况下 返回为 0
        /// </remarks>
        /// <param name="connStr"></param>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <param name="execErrorHandler"></param>
        /// <returns></returns>
        int Execute(string connStr, string sql, object param = null, Action<Exception, string> execErrorHandler = null);

        /// <summary>
        ///     QueryList 执行结果 列表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connStr"></param>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <param name="execErrorHandler"></param>
        /// <returns></returns>
        IEnumerable<T> QueryList<T>(string connStr, string sql, object param = null, Action<Exception, string> execErrorHandler = null);

        /// <summary>
        ///     Query 执行结果 第一行
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connStr"></param>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <param name="execErrorHandler"></param>
        /// <returns></returns>
        T Query<T>(string connStr, string sql, object param = null, Action<Exception, string> execErrorHandler = null);

        #region Async

        /// <summary>
        ///     ExecuteScalarAsync 首行首列数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connStr"></param>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <param name="execErrorHandler"></param>
        /// <returns></returns>
        Task<T> ExecuteScalarAsync<T>(string connStr, string sql, object param = null, Action<Exception, string> execErrorHandler = null);

        /// <summary>
        ///     ExecuteAsync 执行受影响行数
        /// </summary>
        /// <remarks>
        ///     需要注意项：
        ///     1、mysql update 数据没有变更的情况下 返回为 0
        /// </remarks>
        /// <param name="connStr"></param>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <param name="execErrorHandler"></param>
        /// <returns></returns>
        Task<int> ExecuteAsync(string connStr, string sql, object param = null, Action<Exception, string> execErrorHandler = null);

        /// <summary>
        ///     QueryList 执行结果 列表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connStr"></param>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <param name="execErrorHandler"></param>
        /// <returns></returns>
        Task<IEnumerable<T>> QueryListAsync<T>(string connStr, string sql, object param = null, Action<Exception, string> execErrorHandler = null);

        /// <summary>
        ///     Query 执行结果 第一行
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connStr"></param>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <param name="execErrorHandler"></param>
        /// <returns></returns>
        Task<T> QueryAsync<T>(string connStr, string sql, object param = null, Action<Exception, string> execErrorHandler = null);

        #endregion

        void ExecErrorHandler(Action<Exception, string> execErrorHandler, Exception ex, string execSql);


    }
}
