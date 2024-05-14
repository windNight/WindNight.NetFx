using System.Data;
using System.Net.Sockets;
using System.Text;
using Dapper;
using MySql.Data.MySqlClient;
using Newtonsoft.Json.Extension;
using WindNight.Core.SQL.Abstractions;
using WindNight.Extension.Dapper.Mysql.@internal;
using WindNight.Extension.Db.Extensions;

namespace WindNight.Extension.Dapper.Mysql
{
    /// <inheritdoc />
    public partial class MySqlBase : IQueryPagedList
    {


        #region PagedList

        public virtual async Task<IPagedList<T>> PagedListAsync<T>(string connStr, IQueryPageInfo sqlPageInfo, IDictionary<string, object> parameters, long warnMs = -1)
            where T : class, new()
        {
            var dbData = await PagedListInternalAsync<T>(connStr, sqlPageInfo, parameters);
            return GeneratorPagedList(dbData.list, m => m, sqlPageInfo, dbData.recordCount);
        }

        public virtual IPagedList<T> PagedList<T>(string connStr, IQueryPageInfo sqlPageInfo, IDictionary<string, object> parameters, long warnMs = -1)
            where T : class, new()
        {
            var list = PagedListInternal<T>(connStr, sqlPageInfo, out var recordCount, parameters);
            return GeneratorPagedList(list, m => m, sqlPageInfo, recordCount);
        }


        private DynamicParameters GetDynamicParameters(IDictionary<string, object> parameters, long warnMs = -1)
        {
            var dynamicParameters = new DynamicParameters();
            if (parameters == null) return null;

            foreach (var item in parameters) dynamicParameters.Add(item.Key, item.Value);

            return dynamicParameters;
        }

        protected virtual IEnumerable<T> PagedListInternal<T>(string connStr, IQueryPageInfo sqlPageInfo,
            out int recordCount, IDictionary<string, object> parameters, long warnMs = -1)
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
                param, "QueryCount", warnMs);
            //  recordCount = connection.Query<int>(countSql, GetDynamicParameters(parameters)).SingleOrDefault();
            // }

            if (recordCount == 0)
                return null;

            var querySql = GeneratorQueryPageListSql(sqlPageInfo);

            // using (var connection = GetConnection(connStr))
            // {
            var dbList = SqlTimer(QueryList<T>, connStr, querySql,
                param, "QueryList", warnMs);

            // return connection.Query<T>(querySql, GetDynamicParameters(parameters));
            return dbList;
            // }


        }


        protected virtual async Task<(IEnumerable<T> list, int recordCount)>
            PagedListInternalAsync<T>(string connStr, IQueryPageInfo sqlPageInfo, IDictionary<string, object> parameters, long warnMs = -1)
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
                "QueryCountAsync", warnMs);

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
                queryParams, "QueryListAsync", warnMs);

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






    }
}