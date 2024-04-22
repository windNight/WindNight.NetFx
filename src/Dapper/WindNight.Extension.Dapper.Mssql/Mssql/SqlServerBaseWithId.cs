using Newtonsoft.Json.Extension;
using WindNight.Extension.Dapper.Mssql.@internal;

namespace WindNight.Extension.Dapper.Mssql
{
    public abstract partial class SqlServerBase<TEntity, TId>
    {
        protected string InsertSql =>
            $@"INSERT INTO {BaseTableName}({InsertTableColumns})
   VALUES ({InsertTableColumnValues});
   SELECT @@identity;";

        protected string BatchInsertSql =>
            $@"INSERT INTO {BaseTableName}({InsertTableColumns})
   VALUES ({InsertTableColumnValues})";


        /// <summary>
        ///  使用<see cref="BatchInsertSql"/> 语句批量插入
        /// </summary>
        /// <param name="insertList"></param>
        /// <returns></returns>
        public bool BatchInsertUseValues(IList<TEntity> insertList)
        {
            if (insertList == null || !insertList.Any()) return false;

            var flag = DbExecute(BatchInsertSql, insertList.ToArray()) > 0;
            if (!flag)
                LogHelper.Warn($"Insert Into {BaseTableName} handler error ,entities is {insertList.ToJsonStr()} . ",
                    appendMessage: false);
            return flag;
        }

        /// <summary>
        /// 使用<see cref="BatchInsertSql"/> 语句批量插入 异步
        /// </summary>
        /// <param name="insertList"></param>
        /// <returns></returns>
        public async Task<bool> BatchInsertUseValuesAsync(IList<TEntity> insertList)
        {
            if (insertList == null || !insertList.Any()) return false;

            var flag = await DbExecuteAsync(BatchInsertSql, insertList.ToArray()) > 0;
            if (!flag)
                LogHelper.Warn($"Insert Into {BaseTableName} handler error ,entities is {insertList.ToJsonStr()} . ",
                    appendMessage: false);
            return flag;
        }

        /// <summary>
        /// 使用自定义语句批量插入
        /// </summary>
        /// <param name="insertSql"></param>
        /// <param name="insertList"></param>
        public void BatchInsert(string insertSql, IList<TEntity> insertList)
        {
            if (insertList == null || !insertList.Any()) return;

            var flag = DbExecute(insertSql, insertList.ToArray()) > 0;
            if (!flag)
                LogHelper.Warn($"Insert Into {BaseTableName} handler error ,entities is {insertList.ToJsonStr()} . ",
                    appendMessage: false);
        }

        /// <summary>
        /// 按条一条一条插入
        /// </summary>
        /// <param name="insertList"></param>
        public void ListInsertOneByOne(IList<TEntity> insertList)
        {
            var insertSql =
                $@"INSERT INTO {BaseTableName}({InsertTableColumns})
   SELECT {InsertTableColumnValues}
   FROM {BaseTableName}
   WHERE NOT EXISTS(SELECT 1 FROM {BaseTableName} WHERE {EqualEntityCondition})";

            Parallel.ForEach(insertList, item =>
            {
                var flag = DbExecute(insertSql, item) > 0;
                if (!flag)
                    LogHelper.Warn($"Insert Into {BaseTableName} handler Failed ,entity is {item.ToJsonStr()} . ",
                        appendMessage: false);
            });
        }

        /// <summary>
        /// 使用并行任务执行update 慎用
        /// </summary>
        /// <param name="updateSql"></param>
        /// <param name="updateList"></param>
        /// <returns></returns>
        public bool BatchUpdate(string updateSql, IList<TEntity> updateList)
        {
            if (updateList == null || !updateList.Any()) return false;
            Parallel.ForEach(updateList, item =>
            {
                var flag = DbExecute(updateSql, item) > 0;
                if (!flag)
                    LogHelper.Warn($"Update {BaseTableName} handler Failed ,entity is {item.ToJsonStr()} . ",
                        appendMessage: false);
            });
            return true;
        }

        /// <summary>
        ///  带重试机制的批量插入
        /// </summary>
        /// <param name="action"></param>
        /// <param name="retryCount"></param>
        public void BatchInsertWithRetry(Action action, int retryCount = 3)
        {
            DoRetryWhenHandlerSocketException(action, $"BatchInsert_{BaseTableName}", retryCount);
        }
    }

}
