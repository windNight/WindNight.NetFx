using Newtonsoft.Json.Extension;
using WindNight.Extension.Dapper.Mysql.@internal;
using WindNight.Linq.Extensions.Expressions;

namespace WindNight.Extension.Dapper.Mysql
{
    public abstract partial class MySqlBase<TEntity, TId>
    {

        /// <summary>
        ///  使用<see cref="BatchInsertSql"/> 语句批量插入
        /// </summary>
        /// <param name="insertList"></param>
        /// <param name="warnMs"></param>
        /// <returns></returns>
        protected virtual bool BatchInsertUseValuesInternal(IList<TEntity> insertList, long warnMs = -1L, Action<Exception, string> execErrorHandler = null)
        {
            if (insertList.IsNullOrEmpty())
            {
                return false;
            }

            var flag = DbExecute(BatchInsertSql, insertList.ToArray(), warnMs: warnMs, execErrorHandler: execErrorHandler) > 0;
            if (!flag)
            {
                LogHelper.Warn($"Insert Into {BaseTableName} handler error ,entities is {insertList.ToJsonStr()} . ",
                    appendMessage: false);

            }
            return flag;
        }

        /// <summary>
        /// 使用<see cref="BatchInsertSql"/> 语句批量插入 异步
        /// </summary>
        /// <param name="insertList"></param>
        /// <param name="warnMs"></param>
        /// <returns></returns>
        protected virtual async Task<bool> BatchInsertUseValuesInternalAsync(IList<TEntity> insertList, long warnMs = -1L, Action<Exception, string> execErrorHandler = null)
        {
            if (insertList.IsNullOrEmpty())
            {
                return false;
            }

            var flag = await DbExecuteAsync(BatchInsertSql, insertList.ToArray(), warnMs: warnMs, execErrorHandler: execErrorHandler) > 0;
            if (!flag)
            {
                LogHelper.Warn($"Insert Into {BaseTableName} handler error ,entities is {insertList.ToJsonStr()} . ",
                    appendMessage: false);

            }
            return flag;
        }

        /// <summary>
        /// 使用自定义语句批量插入
        /// </summary>
        /// <param name="insertSql"></param>
        /// <param name="insertList"></param>
        protected virtual void BatchInsertInternal(string insertSql, IList<TEntity> insertList, long warnMs = -1L, Action<Exception, string> execErrorHandler = null)
        {
            if (insertList.IsNullOrEmpty())
            {
                return;
            }

            var flag = DbExecute(insertSql, insertList.ToArray(), warnMs: warnMs, execErrorHandler: execErrorHandler) > 0;
            if (!flag)
            {
                LogHelper.Warn($"Insert Into {BaseTableName} handler error ,entities is {insertList.ToJsonStr()} . ",
                    appendMessage: false);

            }
        }

        /// <summary>
        /// 按条一条一条插入
        /// </summary>
        /// <param name="insertList"></param>
        /// <param name="warnMs"></param>
        protected virtual void ListInsertOneByOneInternal(IList<TEntity> insertList, long warnMs = -1L, Action<Exception, string> execErrorHandler = null)
        {
            var insertSql =
                $@"INSERT INTO {BaseTableName}({InsertTableColumns})
   SELECT {InsertTableColumnValues}
   FROM {BaseTableName}
   WHERE NOT EXISTS(SELECT 1 FROM {BaseTableName} WHERE {EqualEntityCondition})";

            Parallel.ForEach(insertList, item =>
            {
                var flag = DbExecute(insertSql, item, warnMs: warnMs, execErrorHandler: execErrorHandler) > 0;
                if (!flag)
                {
                    LogHelper.Warn($"Insert Into {BaseTableName} handler Failed ,entity is {item.ToJsonStr()} . ",
                        appendMessage: false);

                }
            });
        }

        /// <summary>
        /// 使用并行任务执行update 慎用
        /// </summary>
        /// <param name="updateSql"></param>
        /// <param name="updateList"></param>
        /// <param name="warnMs"></param>
        /// <returns></returns>
        protected virtual bool BatchUpdateInternal(string updateSql, IList<TEntity> updateList, long warnMs = -1L, Action<Exception, string> execErrorHandler = null)
        {
            if (updateList.IsNullOrEmpty())
            {
                return false;
            }
            Parallel.ForEach(updateList, item =>
            {
                var flag = DbExecute(updateSql, item, warnMs: warnMs, execErrorHandler: execErrorHandler) > 0;
                if (!flag)
                {
                    LogHelper.Warn($"Update {BaseTableName} handler Failed ,entity is {item.ToJsonStr()} . ",
                        appendMessage: false);
                }
            });
            return true;
        }

        /// <summary>
        ///  带重试机制的批量插入
        /// </summary>
        /// <param name="action"></param>
        /// <param name="retryCount"></param>
        /// <param name="warnMs"></param>
        protected virtual void BatchInsertWithRetryInternal(Action action, int retryCount = 3)
        {
            DoRetryWhenHandlerSocketException(action, $"BatchInsert_{BaseTableName}", retryCount);
        }


        protected virtual int BatchInsertOrUpdateDataInternal(IEnumerable<TEntity> entities, long warnMs = -1L, Action<Exception, string> execErrorHandler = null)
        {
            var count = 0;
            var error = new List<TEntity>();
            foreach (var entity in entities)
            {

                try
                {
                    var flag = InsertOrUpdateDataInternal(entity, warnMs, execErrorHandler);
                    if (flag)
                    {
                        count++;
                    }
                    else
                    {
                        error.Add(entity);
                    }
                }
                catch (Exception ex)
                {
                    LogHelper.Error($"BatchInsertOrUpdateData({entity.ToJsonStr()}) Handler Error {ex.Message}", ex);
                }
            }

            if (entities.Count() != count)
            {
                LogHelper.Warn($" 批量插入部分失败，预期【{entities.Count()}】条 实际成功【{count}】条 ，失败记录：{error.ToJsonStr()}");
            }
            return count;

        }

        protected virtual async Task<int> BatchInsertOrUpdateDataInternalAsync(IEnumerable<TEntity> entities, long warnMs = -1L, Action<Exception, string> execErrorHandler = null)
        {
            var count = 0;
            var error = new List<TEntity>();
            foreach (var entity in entities)
            {

                try
                {
                    var flag = await InsertOrUpdateDataInternalAsync(entity, warnMs, execErrorHandler);
                    if (flag)
                    {
                        count++;
                    }
                    else
                    {
                        error.Add(entity);
                    }
                }
                catch (Exception ex)
                {
                    LogHelper.Error($"InsertOrUpdateDataAsync({entity.ToJsonStr()}) Handler Error {ex.Message}", ex);
                }
            }

            if (entities.Count() != count)
            {
                LogHelper.Warn($" 异步批量插入部分失败，预期【{entities.Count()}】条 实际成功【{count}】条 ，失败记录：{error.ToJsonStr()}");
            }
            return count;

        }


        /// <summary>
        ///   AppendCreateInfo 和 AppendUpdateInfo 需要业务方自行执行
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="warnMs"></param>
        /// <returns></returns>
        protected virtual bool InsertOrUpdateDataInternal(TEntity entity, long warnMs = -1L, Action<Exception, string> execErrorHandler = null)
        {
            var dbData = DbQuery(QueryByUniqueKeySql, entity, warnMs, execErrorHandler);

            if (dbData is { Id: > 0 })
            {
                var exec = DbExecute(UpdateByUniqueKeySql, entity, warnMs, execErrorHandler);
                return exec > 0;
            }
            else
            {
                var id = InsertOneInternal(entity, warnMs, execErrorHandler);
                return id.CompareTo(default) > 0;
            }

        }

        /// <summary>
        ///   AppendCreateInfo 和 AppendUpdateInfo 需要业务方自行执行
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="warnMs"></param>
        /// <returns></returns>
        protected virtual async Task<bool> InsertOrUpdateDataInternalAsync(TEntity entity, long warnMs = -1L, Action<Exception, string> execErrorHandler = null)
        {
            var dbData = await DbQueryAsync(QueryByUniqueKeySql, entity, warnMs, execErrorHandler);
            if (dbData is { Id: > 0 })
            {
                var exec = await DbExecuteAsync(UpdateByUniqueKeySql, entity, warnMs, execErrorHandler);
                return exec > 0;
            }
            else
            {
                var id = await InsertOneInternalAsync(entity, warnMs, execErrorHandler);
                return id.CompareTo(default) > 0;
            }

        }


    }
}
