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

        public virtual MySqlConnectionStringBuilder ParseConnectString(string connectionString, bool isAnalyzed = true)
        {
            var sqlConnStringBuilder = new MySqlConnectionStringBuilder(connectionString, isAnalyzed);
            return sqlConnStringBuilder;
        }

        public virtual IDbConnection GetConnection(string connStr)
        {
            IDbConnection connection = new MySqlConnection(connStr);
            connection.Open();
            return connection;
        }


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




    }



    public partial class MySqlBase
    {
        public virtual void ExecErrorHandler(Action<Exception, string> execErrorHandler, Exception ex, string execSql)
        {
            execErrorHandler.KeepSafeAction(ex, execSql);
        }
    }
}
