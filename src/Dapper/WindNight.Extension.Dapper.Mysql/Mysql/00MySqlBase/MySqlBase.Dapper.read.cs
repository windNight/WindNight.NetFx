using System.Data;
using System.Data.Common;
using Dapper;
using MySql.Data.MySqlClient;
using WindNight.Core.SQL.Abstractions;
using WindNight.Core.Tools;

namespace WindNight.Extension.Dapper.Mysql
{
    /// <inheritdoc cref="IDbReadExecute" />
    public partial class MySqlBase : IDbReadExecute
    {



        #region Sync

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


}
