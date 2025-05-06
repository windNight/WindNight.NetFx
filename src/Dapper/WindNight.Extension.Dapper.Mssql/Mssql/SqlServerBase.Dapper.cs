using System.Collections.ObjectModel;
using System.Data;
using Dapper;
using FastMember;
using Microsoft.Data.SqlClient;
using WindNight.Core.SQL.Abstractions;
using WindNight.Extension.Dapper.Mssql.@internal;

namespace WindNight.Extension.Dapper.Mssql
{
    /// <inheritdoc />
    public partial class SqlServerBase : IBaseDbExecute
    {
        public IDbConnection GetConnection(string connStr)
        {
            IDbConnection connection = new SqlConnection(connStr);
            connection.Open();
            return connection;
        }


        public T ExecuteScalar<T>(string connStr, string sql, object param = null, Action<Exception, string> execErrorHandler = null)
        {
            using (var connection = GetConnection(connStr))
            {
                return connection.ExecuteScalar<T>(sql, param);
            }
        }


        public async Task<T> ExecuteScalarAsync<T>(string connStr, string sql, object param = null, Action<Exception, string> execErrorHandler = null)
        {
            using (var connection = GetConnection(connStr))
            {
                return await connection.ExecuteScalarAsync<T>(sql, param);
            }
        }


        public int Execute(string connStr, string sql, object param = null, Action<Exception, string> execErrorHandler = null)
        {
            using (var connection = GetConnection(connStr))
            {
                return connection.Execute(sql, param);
            }
        }

        public async Task<int> ExecuteAsync(string connStr, string sql, object param = null, Action<Exception, string> execErrorHandler = null)
        {
            using (var connection = GetConnection(connStr))
            {
                return await connection.ExecuteAsync(sql, param);
            }
        }


        public IEnumerable<T> QueryList<T>(string connStr, string sql, object param = null, Action<Exception, string> execErrorHandler = null)
        {
            using (var connection = GetConnection(connStr))
            {
                return connection.Query<T>(sql, param);
            }
        }


        public async Task<IEnumerable<T>> QueryListAsync<T>(string connStr, string sql, object param = null, Action<Exception, string> execErrorHandler = null)
        {
            using (var connection = GetConnection(connStr))
            {
                return await connection.QueryAsync<T>(sql, param);
            }
        }


        public T Query<T>(string connStr, string sql, object param = null, Action<Exception, string> execErrorHandler = null)
        {
            using (var connection = GetConnection(connStr))
            {
                return connection.Query<T>(sql, param).FirstOrDefault();
            }
        }


        public async Task<T> QueryAsync<T>(string connStr, string sql, object param = null, Action<Exception, string> execErrorHandler = null)
        {
            using (var connection = GetConnection(connStr))
            {
                return (await connection.QueryAsync<T>(sql, param)).FirstOrDefault();
            }
        }

        /// <summary>
        ///     批量写入
        /// </summary>
        /// <param name="connStr"></param>
        /// <param name="tableName"></param>
        /// <param name="list"></param>
        /// <param name="members">指定字段名</param>
        /// <returns></returns>
        protected void BatchInsert<T>(string connStr, string tableName, IList<T> list, params string[] members)
        {
            BatchInsert((SqlConnection)GetConnection(connStr), tableName, list, members);
        }

        /// <summary>
        ///     批量写入
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="list"></param>
        /// <param name="members">指定字段名</param>
        /// <returns></returns>
        protected void BatchInsert<T>(SqlConnection connection, string tableName, IList<T> list, params string[] members)
        {
            if (list == null || list.Count <= 0) return;
            using (var bulkCopy = new SqlBulkCopy(connection))
            {
                try
                {
                    Type GetDataType(Type type)
                    {
                        //枚举默认转换成对应的值类型
                        if (type.IsEnum)
                            return type.GetEnumUnderlyingType();
                        //可空类型
                        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
                            return GetDataType(type.GetGenericArguments().First());
                        return type;
                    }

                    var propertyList = typeof(T).GetProperties().Where(w => w.CanRead).ToArray();
                    var Columns = new ReadOnlyCollection<DataColumn>(propertyList
                        .Select(pr => new DataColumn(pr.Name, GetDataType(pr.PropertyType))).ToArray());

                    foreach (var column in Columns)
                    {
                        //创建字段映射
                        bulkCopy.ColumnMappings.Add(column.ColumnName, column.ColumnName);
                    }


                    using (var reader = ObjectReader.Create(list, members))
                    {
                        bulkCopy.DestinationTableName = tableName;
                        bulkCopy.WriteToServer(reader);
                    }
                }
                catch (Exception ex)
                {
                    LogHelper.Error($"BatchInsert<{typeof(T).Name}> count={list.Count} Handler Error {ex.Message}", ex);
                }
            }
        }


        #region PageResut

        protected IEnumerable<T> PagedListInternal<T>(string connStr, IQueryPageInfo pageInfo, out int recordCount, IDictionary<string, object> parameters)
            where T : class, new()
        {
            if (pageInfo.PageIndex <= 0 || pageInfo.PageSize <= 0 || pageInfo.TableName.IsNullOrEmpty())
            {
                recordCount = 0;
                return null;
            }

            var sql = $"SELECT COUNT(*) FROM {pageInfo.TableName}";
            if (!pageInfo.SqlWhere.IsNullOrEmpty())
                sql = $"{sql} WHERE {pageInfo.SqlWhere}";
            var param = GetDynamicParameters(parameters);
            using (var connection = GetConnection(connStr))
            {
                recordCount = connection.ExecuteScalar<int>(sql, param);
            }

            if (recordCount == 0)
                return null;

            var skipCount = (pageInfo.PageIndex - 1) * pageInfo.PageSize;

            sql =
                $"SELECT TOP {pageInfo.PageSize} * FROM (SELECT ROW_NUMBER() OVER(ORDER BY {pageInfo.OrderField}) AS RowNum,* FROM {pageInfo.TableName} AS QueryTable";
            if (!pageInfo.SqlWhere.IsNullOrEmpty())
                sql += $" WHERE {pageInfo.SqlWhere}) AS A WHERE RowNum>{skipCount} ORDER BY {pageInfo.OrderField}";


            using (var connection = GetConnection(connStr))
            {
                return connection.Query<T>(sql, param);
            }
        }

        private (IEnumerable<T> list, int recordCount) GetEmpty<T>() => (null, 0)!;

        protected async Task<(IEnumerable<T> list, int recordCount)> PagedListInternalAsync<T>(string connStr, IQueryPageInfo pageInfo, IDictionary<string, object> parameters)
            where T : class, new()
        {
            var recordCount = 0;
            if (pageInfo.PageIndex <= 0 || pageInfo.PageSize <= 0 || pageInfo.TableName.IsNullOrEmpty())
            {
                recordCount = 0;
                return await Task.FromResult(GetEmpty<T>());
            }

            var sql = $"SELECT COUNT(*) FROM {pageInfo.TableName}";
            if (!pageInfo.SqlWhere.IsNullOrEmpty())
                sql = $"{sql} WHERE {pageInfo.SqlWhere}";
            var param = GetDynamicParameters(parameters);
            using (var connection = GetConnection(connStr))
            {
                recordCount = await connection.ExecuteScalarAsync<int>(sql, param);
            }

            if (recordCount == 0)
                return await Task.FromResult(GetEmpty<T>());

            var skipCount = (pageInfo.PageIndex - 1) * pageInfo.PageSize;

            sql =
                $"SELECT TOP {pageInfo.PageSize} * FROM (SELECT ROW_NUMBER() OVER(ORDER BY {pageInfo.OrderField}) AS RowNum,* FROM {pageInfo.TableName} AS QueryTable";
            if (!pageInfo.SqlWhere.IsNullOrEmpty())
                sql += $" WHERE {pageInfo.SqlWhere}) AS A WHERE RowNum>{skipCount} ORDER BY {pageInfo.OrderField}";


            using (var connection = GetConnection(connStr))
            {
                var list = await connection.QueryAsync<T>(sql, param);
                return (list, recordCount);
            }
        }

        public async Task<IPagedList<T>> QueryPagedListAsync<T>(string connStr, IQueryPageInfo sqlPageInfo, IDictionary<string, object> parameters, long warnMs = -1, Action<Exception, string> execErrorHandler = null)
            where T : class, new()
        {
            var dbData = await PagedListInternalAsync<T>(connStr, sqlPageInfo, parameters);
            return GeneratorPagedList(dbData.list, m => m, sqlPageInfo, dbData.recordCount);
        }

        public IPagedList<T> QueryPagedList<T>(string connStr, IQueryPageInfo sqlPageInfo, IDictionary<string, object> parameters, long warnMs = -1, Action<Exception, string> execErrorHandler = null)
            where T : class, new()
        {
            var list = PagedListInternal<T>(connStr, sqlPageInfo, out var recordCount, parameters);
            return GeneratorPagedList(list, m => m, sqlPageInfo, recordCount);
        }

        protected virtual IPagedList<TResult> GeneratorPagedList<TSource, TResult>(IEnumerable<TSource> sList, Func<IEnumerable<TSource>, IEnumerable<TResult>> converter, IQueryPageBase pageInfo, int recordCount)
        {
            var pageIndex = pageInfo.PageIndex;
            var pageSize = pageInfo.PageSize;
            if (sList == null)
                return PagedListExtension.GeneratorPagedList(pageIndex, pageSize, 1, recordCount, 0, new List<TResult>());
            var list = (IList<TResult>)new List<TResult>(converter(sList));
            var pageCount = (int)Math.Ceiling(recordCount / (double)pageSize);
            return PagedListExtension.GeneratorPagedList(pageIndex, pageSize, 1, recordCount, pageCount, list);
        }

        /// <summary>
        ///     获取动态参数信息
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        private DynamicParameters GetDynamicParameters(IDictionary<string, object> parameters)
        {
            var dynamicParameters = new DynamicParameters();
            if (parameters == null) return null;

            foreach (var item in parameters) dynamicParameters.Add(item.Key, item.Value);

            return dynamicParameters;
        }

        #endregion
    }


    public partial class SqlServerBase
    {
        public virtual void ExecErrorHandler(Action<Exception, string> execErrorHandler, Exception ex, string execSql)
        {
        }
    }
}
