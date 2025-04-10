using System.Data;
using System.Data.Common;
using Dapper;
using MySql.Data.MySqlClient;
using WindNight.Core.SQL.Abstractions;
using WindNight.Core.Tools;

namespace WindNight.Extension.Dapper.Mysql
{
    /// <inheritdoc cref="IBaseDbExecute" />
    public partial class MySqlBase : IBaseDbExecute
    {
        public virtual IDbConnection GetConnection(string connStr)
        {
            IDbConnection connection = new MySqlConnection(connStr);
            connection.Open();
            return connection;
        }

        #region Sync

        public virtual T ExecuteScalar<T>(string connStr, string sql, object param = null,
            Action<Exception, string> execErrorHandler = null)
        {
            using (var connection = GetConnection(connStr))
            {
                try
                {
                    return connection.ExecuteScalar<T>(sql, param);
                }
                catch (Exception ex)
                {
                    if (execErrorHandler != null)
                    {
                        ExecErrorHandler(execErrorHandler, ex, sql);
                    }

                    return default;
                }
                finally
                {
                    connection.Close();
                }
            }
        }

        public virtual int Execute(string connStr, string sql, object param = null,
            Action<Exception, string> execErrorHandler = null)
        {
            using (var connection = GetConnection(connStr))
            {
                try
                {
                    return connection.Execute(sql, param);
                }
                catch (Exception ex)
                {
                    if (execErrorHandler != null)
                    {
                        ExecErrorHandler(execErrorHandler, ex, sql);
                    }

                    return default;
                }
                finally
                {
                    connection.Close();
                }
            }
        }

        public virtual IEnumerable<T> QueryList<T>(string connStr, string sql, object param = null,
            Action<Exception, string> execErrorHandler = null)
        {
            using (var connection = GetConnection(connStr))
            {
                try
                {
                    return connection.Query<T>(sql, param).ToList();
                }
                catch (Exception ex)
                {
                    if (execErrorHandler != null)
                    {
                        ExecErrorHandler(execErrorHandler, ex, sql);
                    }

                    return default;
                }
                finally
                {
                    connection.Close();
                }
            }
        }


        public virtual T Query<T>(string connStr, string sql, object param = null,
            Action<Exception, string> execErrorHandler = null)
        {
            using (var connection = GetConnection(connStr))
            {
                try
                {
                    return connection.Query<T>(sql, param).FirstOrDefault();
                }
                catch (Exception ex)
                {
                    if (execErrorHandler != null)
                    {
                        ExecErrorHandler(execErrorHandler, ex, sql);
                    }

                    return default;
                }
                finally
                {
                    connection.Close();
                }
            }
        }

        #endregion //end Sync


        #region Async

        public virtual async Task<T> ExecuteScalarAsync<T>(string connStr, string sql, object param = null,
            Action<Exception, string> execErrorHandler = null)
        {
            using (var connection = GetConnection(connStr))
            {
                try
                {
                    return await connection.ExecuteScalarAsync<T>(sql, param);
                }
                catch (Exception ex)
                {
                    if (execErrorHandler != null)
                    {
                        ExecErrorHandler(execErrorHandler, ex, sql);
                    }

                    return default;
                }
                finally
                {
                    connection.Close();
                }
            }
        }


        public virtual async Task<int> ExecuteAsync(string connStr, string sql, object param = null,
            Action<Exception, string> execErrorHandler = null)
        {
            using (var connection = GetConnection(connStr))
            {
                try
                {
                    return await connection.ExecuteAsync(sql, param);
                }
                catch (Exception ex)
                {
                    if (execErrorHandler != null)
                    {
                        ExecErrorHandler(execErrorHandler, ex, sql);
                    }

                    return default;
                }
                finally
                {
                    connection.Close();
                }
            }
        }


        public virtual async Task<IEnumerable<T>> QueryListAsync<T>(string connStr, string sql, object param = null,
            Action<Exception, string> execErrorHandler = null)
        {
            using (var connection = GetConnection(connStr))
            {
                try
                {
                    return (await connection.QueryAsync<T>(sql, param)).ToList();
                }
                catch (Exception ex)
                {
                    if (execErrorHandler != null)
                    {
                        ExecErrorHandler(execErrorHandler, ex, sql);
                    }

                    return default;
                }
                finally
                {
                    connection.Close();
                }
            }
        }


        public virtual async Task<T> QueryAsync<T>(string connStr, string sql, object param = null,
            Action<Exception, string> execErrorHandler = null)
        {
            using (var connection = GetConnection(connStr))
            {
                try
                {
                    return (await connection.QueryAsync<T>(sql, param)).FirstOrDefault();
                }
                catch (Exception ex)
                {
                    if (execErrorHandler != null)
                    {
                        ExecErrorHandler(execErrorHandler, ex, sql);
                    }

                    return default;
                }
                finally
                {
                    connection.Close();
                }
            }
        }

        #endregion //end Async


    }

    public partial class MySqlBase
    {

        protected virtual T DapperExec<T>(string connStr, string sql, Func<IDbConnection, T> func,
            Action<Exception, string> execErrorHandler = null)
        {
            using (var connection = GetConnection(connStr))
            {
                try
                {
                    return func.Invoke(connection);
                }
                catch (Exception ex)
                {
                    if (execErrorHandler != null)
                    {
                        ExecErrorHandler(execErrorHandler, ex, sql);
                    }

                    return default;
                }
                finally
                {
                    connection.Close();
                }
            }
        }

        public virtual async Task<T> DapperExecAsync<T>(string connStr, string sql, Func<IDbConnection, Task<T>> func, Action<Exception, string> execErrorHandler = null)
        {
            using (var connection = GetConnection(connStr))
            {
                try
                {
                    return await func.Invoke(connection);
                }
                catch (Exception ex)
                {
                    if (execErrorHandler != null)
                    {
                        ExecErrorHandler(execErrorHandler, ex, sql);
                    }

                    return default;
                }
                finally
                {
                    connection.Close();
                }
            }
        }




    }



    public partial class MySqlBase
    {
        public virtual void ExecErrorHandler(Action<Exception, string> execErrorHandler, Exception ex, string execSql)
        {
            execErrorHandler.KeepSafeAction(ex, execSql);
        }
    }
}
