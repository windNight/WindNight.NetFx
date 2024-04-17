using Dapper;
using MySql.Data.MySqlClient;
using System.Data;
using System.Net.Sockets;
using System.Text;
using Newtonsoft.Json.Extension;
using WindNight.Core.SQL;
using WindNight.Core.SQL.Abstractions;
using WindNight.Extension.Dapper.Internals;
using WindNight.Extension.SqlClient.Extensions;


namespace WindNight.Extension.Dapper.Mysql
{
    /// <inheritdoc />
    public partial class MySqlBase : IBaseDbExecute
    {
        public virtual IDbConnection GetConnection(string connStr)
        {
            IDbConnection connection = new MySqlConnection(connStr);
            connection.Open();
            return connection;
        }

        #region Sync

        public virtual T ExecuteScalar<T>(string connStr, string sql, object param = null)
        {
            using (var connection = GetConnection(connStr))
            {
                try
                {
                    return connection.ExecuteScalar<T>(sql, param);
                }
                finally
                {
                    connection.Close();
                }
            }
        }

        public virtual int Execute(string connStr, string sql, object param = null)
        {
            using (var connection = GetConnection(connStr))
            {
                try
                {
                    return connection.Execute(sql, param);
                }
                finally
                {
                    connection.Close();
                }
            }
        }

        public virtual IEnumerable<T> QueryList<T>(string connStr, string sql, object param = null)
        {
            using (var connection = GetConnection(connStr))
            {
                try
                {
                    return connection.Query<T>(sql, param).ToList();
                }
                finally
                {
                    connection.Close();
                }
            }
        }


        public virtual T Query<T>(string connStr, string sql, object param = null)
        {
            using (var connection = GetConnection(connStr))
            {
                try
                {
                    return connection.Query<T>(sql, param).FirstOrDefault();
                }
                finally
                {
                    connection.Close();
                }
            }
        }

        #endregion //end Sync

        #region PagedList

        public virtual async Task<IPagedList<T>> PagedListAsync<T>(string connStr, IQueryPageInfo sqlPageInfo, IDictionary<string, object> parameters)
            where T : class, new()
        {
            var dbData = await PagedListInternalAsync<T>(connStr, sqlPageInfo, parameters);
            return GeneratorPagedList(dbData.list, m => m, sqlPageInfo, dbData.recordCount);
        }

        public virtual IPagedList<T> PagedList<T>(string connStr, IQueryPageInfo sqlPageInfo, IDictionary<string, object> parameters)
            where T : class, new()
        {
            var list = PagedListInternal<T>(connStr, sqlPageInfo, out var recordCount, parameters);
            return GeneratorPagedList(list, m => m, sqlPageInfo, recordCount);
        }


        private DynamicParameters GetDynamicParameters(IDictionary<string, object> parameters)
        {
            var dynamicParameters = new DynamicParameters();
            if (parameters == null) return null;

            foreach (var item in parameters) dynamicParameters.Add(item.Key, item.Value);

            return dynamicParameters;
        }

        protected virtual IEnumerable<T> PagedListInternal<T>(string connStr, IQueryPageInfo sqlPageInfo, out int recordCount, IDictionary<string, object> parameters)
            where T : class, new()
        {
            if (sqlPageInfo.PageIndex <= 0 || sqlPageInfo.PageSize <= 0 ||
                sqlPageInfo.TableName.IsNullOrEmpty())
            {
                recordCount = 0;
                return null;
            }

            var countSql = GeneratorQueryCountSql(sqlPageInfo);
            var param = GetDynamicParameters(parameters);
            //using (var connection = GetConnection(connStr))
            //{
            recordCount = SqlTimer(ExecuteScalar<int>, connStr, countSql,
                param, "QueryCount");
            //  recordCount = connection.Query<int>(countSql, GetDynamicParameters(parameters)).SingleOrDefault();
            // }

            if (recordCount == 0)
                return null;

            var querySql = GeneratorQueryPageListSql(sqlPageInfo);

            // using (var connection = GetConnection(connStr))
            // {
            var dbList = SqlTimer(QueryList<T>, connStr, querySql,
                param, "QueryList");

            // return connection.Query<T>(querySql, GetDynamicParameters(parameters));
            return dbList;
            // }


        }


        protected virtual async Task<(IEnumerable<T> list, int recordCount)>
            PagedListInternalAsync<T>(string connStr, IQueryPageInfo sqlPageInfo, IDictionary<string, object> parameters)
            where T : class, new()
        {
            var recordCount = 0;
            if (sqlPageInfo.PageIndex <= 0 || sqlPageInfo.PageSize <= 0 ||
                sqlPageInfo.TableName.IsNullOrEmpty())
                return await Task.FromResult<(IEnumerable<T> list, int recordCount)>((null, 0));
            var queryParams = GetDynamicParameters(parameters);

            var countSql = GeneratorQueryCountSql(sqlPageInfo);
            //using (var connection = GetConnection(connStr))
            //{
            //    recordCount = await connection.ExecuteScalarAsync<int>(countSql, queryParams);
            //}

            recordCount = await SqlTimerAsync<int>(ExecuteScalarAsync<int>, connStr,
                countSql, queryParams,
                "QueryCountAsync");

            if (recordCount == 0)
                return await Task.FromResult<(IEnumerable<T> list, int recordCount)>((null, 0));

            var querySql = GeneratorQueryPageListSql(sqlPageInfo);

            //using (var connection = GetConnection(connStr))
            //{
            //    var list = await connection.QueryAsync<T>(querySql, queryParams);
            //    return (list, recordCount);
            //}

            var list = await SqlTimerAsync(QueryListAsync<T>,
                connStr, querySql,
                queryParams, "QueryListAsync");

            return (list, recordCount);


        }

        protected virtual IPagedList<TResult> GeneratorPagedList<TSource, TResult>(IEnumerable<TSource> sList,
            Func<IEnumerable<TSource>, IEnumerable<TResult>> converter, IQueryPageBase pageInfo, int recordCount)
        {
            var pageIndex = pageInfo.PageIndex;
            var pageSize = pageInfo.PageSize;
            if (sList == null)
                return PagedListExtension.GeneratorPagedList(pageIndex, pageSize, 1, recordCount, 0,
                    new List<TResult>());
            var list = (IList<TResult>)new List<TResult>(converter(sList));
            var pageCount = (int)Math.Ceiling(recordCount / (double)pageSize);
            return PagedListExtension.GeneratorPagedList(pageIndex, pageSize, 1, recordCount, pageCount, list);
        }


        protected virtual string GeneratorQueryCountSql(IQueryPageInfo sqlPageInfo)
        {
            var countSql = new StringBuilder($"SELECT COUNT(1) FROM {sqlPageInfo.TableName}");
            if (!sqlPageInfo.SqlWhere.IsNullOrEmpty())
                countSql.Append($" WHERE {sqlPageInfo.SqlWhere}");
            return countSql.ToString();
        }

        protected virtual string GeneratorQueryPageListSql(IQueryPageInfo sqlPageInfo)
        {
            var querySql = new StringBuilder($"SELECT {sqlPageInfo.Fields} FROM {sqlPageInfo.TableName}");
            if (!sqlPageInfo.SqlWhere.IsNullOrEmpty())
                querySql.Append($" WHERE {sqlPageInfo.SqlWhere}");

            if (!sqlPageInfo.OrderField.IsNullOrEmpty())
                querySql.Append($" ORDER BY {sqlPageInfo.OrderField}");

            var pageIndex = sqlPageInfo.PageIndex;
            var pageSize = sqlPageInfo.PageSize;
            var begIndex = (pageIndex - 1) * pageSize;
            if (begIndex < 0) begIndex = 0;

            querySql.Append($" LIMIT {begIndex},{pageSize}");

            return querySql.ToString();
        }

        #endregion //end PagedList

        #region Async

        public virtual async Task<T> ExecuteScalarAsync<T>(string connStr, string sql, object param = null)
        {
            using (var connection = GetConnection(connStr))
            {
                try
                {
                    return await connection.ExecuteScalarAsync<T>(sql, param);
                }
                finally
                {
                    connection.Close();
                }
            }
        }


        public virtual async Task<int> ExecuteAsync(string connStr, string sql, object param = null)
        {
            using (var connection = GetConnection(connStr))
            {
                try
                {
                    return await connection.ExecuteAsync(sql, param);
                }
                finally
                {
                    connection.Close();
                }
            }
        }


        public virtual async Task<IEnumerable<T>> QueryListAsync<T>(string connStr, string sql, object param = null)
        {
            using (var connection = GetConnection(connStr))
            {
                try
                {
                    return (await connection.QueryAsync<T>(sql, param)).ToList();
                }
                finally
                {
                    connection.Close();
                }
            }
        }


        public virtual async Task<T> QueryAsync<T>(string connStr, string sql, object param = null)
        {
            using (var connection = GetConnection(connStr))
            {
                try
                {
                    return (await connection.QueryAsync<T>(sql, param)).FirstOrDefault();
                }
                finally
                {
                    connection.Close();
                }
            }
        }

        #endregion //end Async


        /// <summary>
        /// 使用自定义连接
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sqlFunc"></param>
        /// <param name="connectString"></param>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <param name="actionName"></param>
        /// <returns></returns>
        protected virtual T SqlTimer<T>(Func<string, string, object, T> sqlFunc, string connectString, string sql, object param = null,
            string actionName = "")
        {
            var ticks = DateTime.Now.Ticks;
            try
            {
                return sqlFunc(connectString, sql, param);
            }
            catch (Exception ex)
            {
                if (param is IEntity entity)
                {
                    LogHelper.Error($" sql执行报错 {(ConfigItems.IsLogConnectString ? $"【{connectString}】" : "")}  {actionName} Failed，{sql}.param is {entity.ToParamString()}", ex);

                }
                else
                {
                    LogHelper.Error($" sql执行报错 {(ConfigItems.IsLogConnectString ? $"【{connectString}】" : "")}  {actionName} Failed，{sql}.param is {param.ToJsonStr()}", ex);
                }
            }
            finally
            {
                try
                {
                    var milliseconds = (long)TimeSpan.FromTicks(DateTime.Now.Ticks - ticks).TotalMilliseconds;
                    if (milliseconds > ConfigItems.DapperWarnMs)
                    {
                        LogHelper.Warn($"sql执行耗时：{milliseconds} ms.{(ConfigItems.IsLogConnectString ? $"【{connectString}】" : "")}  sql:{sql}  {(milliseconds >= 100 ? $"param is {param.ToJsonStr()}" : "")}", millisecond: milliseconds);

                    }
                    else if (ConfigItems.OpenDapperLog)
                    {
                        LogHelper.Info($"sql执行耗时：{milliseconds} ms.{(ConfigItems.IsLogConnectString ? $"【{connectString}】" : "")} sql:{sql}  {(milliseconds >= 100 ? $"param is {param.ToJsonStr()}" : "")}", millisecond: milliseconds);
                    }
                }
                catch
                {

                }
            }

            return default;
        }


        protected virtual async Task<T> SqlTimerAsync<T>(Func<string, string, object, Task<T>> sqlFunc, string connectString, string sql, object param = null,
            string actionName = "")
        {
            var ticks = DateTime.Now.Ticks;
            try
            {
                return await sqlFunc(connectString, sql, param);
            }
            catch (Exception ex)
            {
                if (param is IEntity entity)
                {
                    LogHelper.Error($"sql执行报错 {(ConfigItems.IsLogConnectString ? $"【{connectString}】" : "")}  {actionName} Failed，{sql}.param is {entity.ToParamString()}", ex);
                }
                else
                {
                    LogHelper.Error($"sql执行报错 {(ConfigItems.IsLogConnectString ? $"【{connectString}】" : "")}  {actionName} Failed，{sql}.param is {param.ToJsonStr()}", ex);

                }
            }
            finally
            {
                try
                {
                    var milliseconds = (long)TimeSpan.FromTicks(DateTime.Now.Ticks - ticks).TotalMilliseconds;
                    if (milliseconds > ConfigItems.DapperWarnMs)
                    {
                        LogHelper.Warn($"sql执行耗时：{milliseconds} ms.{(ConfigItems.IsLogConnectString ? $"【{connectString}】" : "")}  sql:{sql}  {(milliseconds >= 100 ? $"param is {param.ToJsonStr()}" : "")}", millisecond: milliseconds);

                    }
                    else if (ConfigItems.OpenDapperLog)
                    {
                        LogHelper.Info(
                            $"sql执行耗时：{milliseconds} ms.{(ConfigItems.IsLogConnectString ? $"【{connectString}】" : "")} sql:{sql}  {(milliseconds >= 100 ? $"param is {param.ToJsonStr()}" : "")}", millisecond: milliseconds);
                    }
                }
                catch
                {

                }
            }

            return default;
        }


        protected virtual void DoRetryWhenHandlerSocketException(Action action, string actionName, int retryCount = 3)
        {
            var runCount = 0;
            var isRun = true;
            while (isRun)
                try
                {
                    runCount++;
                    action.Invoke();
                    if (runCount > 1) LogHelper.Warn($"[{actionName}] 经过[{runCount}]次重试后成功！");
                    break;
                }
                catch (Exception ex)
                {
                    if (ex is SocketException)
                    {
                        LogHelper.Warn($"[{actionName}] 第{runCount}次重试后失败并捕获异常SocketException！{ex.Message}", ex);
                        if (runCount >= retryCount)
                        {
                            LogHelper.Error(
                                $"[{actionName}] 第[{runCount}]次重试后达到重试次数上限[{retryCount}]次，将不再重试！{ex.Message}", ex);
                            isRun = false;
                        }

                        Thread.Sleep(1000 * 1);
                        continue;
                    }

                    isRun = false;
                    LogHelper.Error($"[{actionName}] 捕获未知异常，将不再重试！{ex.Message}", ex);
                }
        }





    }
}