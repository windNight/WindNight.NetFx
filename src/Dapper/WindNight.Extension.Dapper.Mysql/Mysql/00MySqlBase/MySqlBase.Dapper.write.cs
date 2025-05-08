using System.Data;
using System.Data.Common;
using Dapper;
using MySql.Data.MySqlClient;
using WindNight.Core.SQL.Abstractions;
using WindNight.Core.Tools;

namespace WindNight.Extension.Dapper.Mysql
{
    /// <inheritdoc cref="IDbWriteExecute" />
    public partial class MySqlBase : IDbWriteExecute
    {


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
                        return default;
                    }

                    throw;
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
                        return default;
                    }

                    throw;
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
                        return default;
                    }

                    throw;
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
                        return default;
                    }

                    throw;
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
