using Dapper;
using MySql.Data.MySqlClient;
using System.Data;
using System.Text;
using WindNight.Core.SQL;
using WindNight.Core.SQL.Abstractions;


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

        private IEnumerable<T> PagedListInternal<T>(string connStr, IQueryPageInfo sqlPageInfo, out int recordCount, IDictionary<string, object> parameters)
            where T : class, new()
        {
            if (sqlPageInfo.PageIndex <= 0 || sqlPageInfo.PageSize <= 0 ||
                sqlPageInfo.TableName.IsNullOrEmpty())
            {
                recordCount = 0;
                return null;
            }

            var countSql = GeneratorQueryCountSql(sqlPageInfo);
            using (var connection = GetConnection(connStr))
            {
                recordCount = connection.Query<int>(countSql, GetDynamicParameters(parameters)).SingleOrDefault();
            }

            if (recordCount == 0)
                return null;

            var querySql = GeneratorQueryPageListSql(sqlPageInfo);

            using (var connection = GetConnection(connStr))
            {
                return connection.Query<T>(querySql, GetDynamicParameters(parameters));
            }
        }


        private async Task<(IEnumerable<T> list, int recordCount)>
            PagedListInternalAsync<T>(string connStr, IQueryPageInfo sqlPageInfo, IDictionary<string, object> parameters)
            where T : class, new()
        {
            var recordCount = 0;
            if (sqlPageInfo.PageIndex <= 0 || sqlPageInfo.PageSize <= 0 ||
                sqlPageInfo.TableName.IsNullOrEmpty())
                return await Task.FromResult<(IEnumerable<T> list, int recordCount)>((null, 0));
            var queryParams = GetDynamicParameters(parameters);

            var countSql = GeneratorQueryCountSql(sqlPageInfo);
            using (var connection = GetConnection(connStr))
            {
                recordCount = await connection.ExecuteScalarAsync<int>(countSql, queryParams);
            }

            if (recordCount == 0)
                return await Task.FromResult<(IEnumerable<T> list, int recordCount)>((null, 0));

            var querySql = GeneratorQueryPageListSql(sqlPageInfo);
            using (var connection = GetConnection(connStr))
            {
                var list = await connection.QueryAsync<T>(querySql, queryParams);
                return (list, recordCount);
            }
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


        private string GeneratorQueryCountSql(IQueryPageInfo sqlPageInfo)
        {
            var countSql = new StringBuilder($"SELECT COUNT(*) FROM {sqlPageInfo.TableName}");
            if (!sqlPageInfo.SqlWhere.IsNullOrEmpty())
                countSql.Append($" WHERE {sqlPageInfo.SqlWhere}");
            return countSql.ToString();
        }

        private string GeneratorQueryPageListSql(IQueryPageInfo sqlPageInfo)
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


    }
}