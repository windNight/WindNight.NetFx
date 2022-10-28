using Microsoft.Extensions.DependencyInjection.WnExtension;
using Microsoft.Extensions.Options;
using WindNight.Core.SQL;
using WindNight.Core.SQL.Abstractions;
using WindNight.Extension.Dapper.Mysql;
using WindNight.Extension.Logger.DbLog.Abstractions;

namespace WindNight.Extension.Logger.Mysql.DbLog
{
    internal class MysqlLogsProcess : MysqlDbBase<SysLogs>, ISystemLogsProcess
    {
        private readonly string BusinessColumns =
            "LevelType,`Level`,SerialNumber,RequestUrl,ServerIp,ClientIp,LogTs,Content,NodeCode,LogAppCode,LogAppName,Exceptions";

        private readonly string BusinessColumnValues =
            "@LevelType,@Level,@SerialNumber,@RequestUrl,@ServerIp,@ClientIp,@LogTs,@Content,@NodeCode,@LogAppCode,@LogAppName,@Exceptions";


        protected override string InsertTableColumns => $"{BusinessColumns},{BaseCreateColumns}";
        protected override string InsertTableColumnValues => $"{BusinessColumnValues},{BaseCreateColumnValues}";

        protected override string EqualEntityCondition { get; }

        public bool Insert(SysLogs entity)
        {
            var rtl = InsertOne(entity);
            return rtl > 0;
        }

        public async Task<bool> InsertAsync(SysLogs entity)
        {
            var rtl = await InsertOneAsync(entity);
            return rtl > 0;
        }

        public bool BatchInsert(List<SysLogs> entities)
        {
            var rtl = BatchInsertUseValues(entities);
            return rtl;
        }

        public async Task<bool> BatchInsertAsync(List<SysLogs> entities)
        {
            var rtl = await BatchInsertUseValuesAsync(entities);
            return rtl; ;
        }


    }


    internal abstract class MysqlDbBase<TEntity> : MysqlDbBase<TEntity, long>
        where TEntity : CreateBase<long>, new()
    {

        protected override string Db { get; }

        /// <summary> </summary>
        protected string BaseCreateColumns =
            "CreateUserId,CreateDate,CreateUnixTime,IsDeleted";

        /// <summary> </summary>
        protected string BaseCreateColumnValues =
            "@CreateUserId,@CreateDate,@CreateUnixTime,@IsDeleted";


    }


    internal abstract class MysqlDbBase<TEntity, TId> : MySqlBase<TEntity, TId>
        where TEntity : class, IEntity<TId>, new()
        where TId : IEquatable<TId>, IComparable<TId>
    {

        protected override string GetConnStr()
        {
            var dbLogOptions = Ioc.GetService<IOptionsMonitor<DbLogOptions>>().CurrentValue;
            return dbLogOptions.DbConnectString;
        }

    }
}
