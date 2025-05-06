using Newtonsoft.Json.Extension;
using WindNight.Core;
using WindNight.Core.SQL.Abstractions;
using WindNight.Extension.Dapper.Mysql.@internal;
using WindNight.Extension.Db.Extensions;

namespace WindNight.Extension.Dapper.Mysql
{
    /// <summary>
    ///     自定义基于Dapper的Mysql基类 通用方法
    /// </summary>
    public abstract partial class NoIdMysqlBase<TEntity>
    {


        protected abstract string GetConnStr();

        protected virtual IEnumerable<TT> EmptyListT<TT>() => Array.Empty<TT>();

        protected virtual IPagedList<TT> EmptyPagedListT<TT>() => System.Collections.Generic.PagedList.Empty<TT>();

        protected virtual IEnumerable<TEntity> EmptyList => Array.Empty<TEntity>();

        protected virtual IPagedList<TEntity> EmptyPagedList => System.Collections.Generic.PagedList.Empty<TEntity>();


        /// <summary>
        ///     使用默认连接
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sqlFunc"></param>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <param name="actionName"></param>
        /// <param name="warnMs"></param>
        /// <returns></returns>
        protected virtual T SqlTimer<T>(Func<string, object, Action<Exception, string>, T> sqlFunc, string sql,
            object param = null,
            string actionName = "", long warnMs = -1, Action<Exception, string> execErrorHandler = null)
        {
            var ticks = HardInfo.Now.Ticks;
            try
            {
                return sqlFunc(sql, param, execErrorHandler);
            }
            catch (Exception ex)
            {
                if (param is IEntity entity)
                {
                    LogHelper.Error(
                        $" sql执行报错 {(ConfigItems.IsLogConnectString ? $"【{DbConnectString}】" : "")}  {actionName} Failed，{sql}.param is {entity.ToParamString()}",
                        ex);
                }
                else
                {
                    LogHelper.Error(
                        $" sql执行报错 {(ConfigItems.IsLogConnectString ? $"【{DbConnectString}】" : "")}  {actionName} Failed，{sql}.param is {param.ToJsonStr()}",
                        ex);
                }
            }
            finally
            {
                try
                {
                    var milliseconds = (long)TimeSpan.FromTicks(HardInfo.Now.Ticks - ticks).TotalMilliseconds;
                    warnMs = FixWarnMs(warnMs);
                    if (milliseconds > warnMs)
                    {
                        LogHelper.Warn(
                            $"sql执行耗时：{milliseconds} ms.{(ConfigItems.IsLogConnectString ? $"【{DbConnectString}】" : "")}  sql:{sql}  {(milliseconds >= 100 ? $"param is {param.ToJsonStr()}" : "")}",
                            millisecond: milliseconds);
                    }
                    else if (ConfigItems.OpenDapperLog)
                    {
                        LogHelper.Info(
                            $"sql执行耗时：{milliseconds} ms.{(ConfigItems.IsLogConnectString ? $"【{DbConnectString}】" : "")} sql:{sql}  {(milliseconds >= 100 ? $"param is {param.ToJsonStr()}" : "")}",
                            milliseconds);
                    }
                }
                catch
                {
                }
            }

            return default;
        }

        /// <summary>
        ///     使用默认连接
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sqlFunc"> execSql execParams errorHandler </param>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <param name="actionName"></param>
        /// <param name="warnMs"></param>
        /// <param name="execErrorHandler"></param>
        /// <returns></returns>
        protected virtual async Task<T> SqlTimerAsync<T>(
            Func<string, object, Action<Exception, string>, Task<T>> sqlFunc, string sql, object param = null,
            string actionName = "", long warnMs = -1, Action<Exception, string> execErrorHandler = null)
        {
            var ticks = HardInfo.Now.Ticks;
            try
            {
                return await sqlFunc(sql, param, execErrorHandler);
            }
            catch (Exception ex)
            {
                if (param is IEntity entity)
                {
                    LogHelper.Error(
                        $" sql执行报错 {(ConfigItems.IsLogConnectString ? $"【{DbConnectString}】" : "")}  {actionName} Failed，{sql}.param is {entity.ToParamString()}",
                        ex);
                }
                else
                {
                    LogHelper.Error(
                        $" sql执行报错 {(ConfigItems.IsLogConnectString ? $"【{DbConnectString}】" : "")}  {actionName} Failed，{sql}.param is {param.ToJsonStr()}",
                        ex);
                }
            }
            finally
            {
                try
                {
                    var milliseconds = (long)TimeSpan.FromTicks(HardInfo.Now.Ticks - ticks).TotalMilliseconds;
                    warnMs = FixWarnMs(warnMs);
                    if (milliseconds > warnMs)
                    {
                        LogHelper.Warn(
                            $"sql执行耗时：{milliseconds} ms.{(ConfigItems.IsLogConnectString ? $"【{DbConnectString}】" : "")}  sql:{sql}  {(milliseconds >= 100 ? $"param is {param.ToJsonStr()}" : "")}",
                            millisecond: milliseconds);
                    }
                    else if (ConfigItems.OpenDapperLog)
                    {
                        LogHelper.Info(
                            $"sql执行耗时：{milliseconds} ms.{(ConfigItems.IsLogConnectString ? $"【{DbConnectString}】" : "")} sql:{sql}  {(milliseconds >= 100 ? $"param is {param.ToJsonStr()}" : "")}",
                            milliseconds);
                    }
                }
                catch
                {
                }
            }

            return default;
        }
    }
}
