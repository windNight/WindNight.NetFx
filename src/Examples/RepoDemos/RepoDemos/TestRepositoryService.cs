using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WindNight.Core.SQL;
using WindNight.Extension.Dapper.Abstractions;
using WindNight.Extension.Dapper.Mysql;
using WindNight.Extension.Db.Abstractions;

namespace RepoDemos
{
    public class TestStatusE : CreateAndUpdateWithStatusBase
    {

    }

    public interface ITestRepositoryService : ICUSWriterBaseRepositoryService<TestStatusE>
    {

    }


    public class TestRepositoryService : DbBase<TestStatusE>, ITestRepositoryService
    {
        protected override string BusinessColumns =>
            "F1,F2,F3 ";

        protected override string BusinessColumnValues =>
            "@F1,@F2,@F3 ";
        protected override string InsertTableColumns => $"{BusinessColumns},{BaseStatusColumns}";
        protected override string InsertTableColumnValues => $"{BusinessColumnValues},{BaseStatusColumnValues}";
        protected override string EqualEntityCondition => "";


        public int InsertOne(TestStatusE entity, long warnMs = -1, Action<Exception, string> execErrorHandler = null)
        {
            return base.InsertOneInternal(entity, warnMs, execErrorHandler);
        }

        public async Task<int> InsertOneAsync(TestStatusE entity, long warnMs = -1, Action<Exception, string> execErrorHandler = null)
        {
            return await base.InsertOneInternalAsync(entity, warnMs, execErrorHandler);

        }

        public bool BatchInsertUseValues(IList<TestStatusE> insertList, long warnMs = -1, Action<Exception, string> execErrorHandler = null)
        { 
            return base.BatchInsertUseValuesInternal(insertList, warnMs, execErrorHandler);
        }

        public async Task<bool> BatchInsertUseValuesAsync(IList<TestStatusE> insertList, long warnMs = -1, Action<Exception, string> execErrorHandler = null)
        {
            return await base.BatchInsertUseValuesInternalAsync(insertList, warnMs, execErrorHandler);

        }

        public bool DeleteById(int id, long warnMs = -1, Action<Exception, string> execErrorHandler = null)
        {
            return base.DeleteByIdInternal(id, warnMs, execErrorHandler);
        }

        public async Task<bool> DeleteByIdAsync(int id, long warnMs = -1, Action<Exception, string> execErrorHandler = null)
        {
            return await base.DeleteByIdInternalAsync(id, warnMs, execErrorHandler);

        }
    }



    public class TestTreeE : TreeEntityBase
    {

    }

    public interface ITestTreeERepositoryService :
        ITreeWriterBaseRepositoryService<TestTreeE>,
        ICUSWriterBaseRepositoryService<TestTreeE>

    {

    }


    public partial class TestTreeERepositoryService : DbTreeBase<TestTreeE>, ITestTreeERepositoryService
    {
        protected override string BusinessColumns =>
            "F1,F2,F3 ";

        protected override string BusinessColumnValues =>
            "@F1,@F2,@F3 ";
        protected override string InsertTableColumns => $"{BusinessColumns},{BaseStatusColumns}";
        protected override string InsertTableColumnValues => $"{BusinessColumnValues},{BaseStatusColumnValues}";
        protected override string EqualEntityCondition => "";


        public int InsertOne(TestTreeE entity, long warnMs = -1, Action<Exception, string> execErrorHandler = null)
        {
            throw new NotImplementedException();
        }

        public Task<int> InsertOneAsync(TestTreeE entity, long warnMs = -1, Action<Exception, string> execErrorHandler = null)
        {
            throw new NotImplementedException();
        }

        public bool BatchInsertUseValues(IList<TestTreeE> insertList, long warnMs = -1, Action<Exception, string> execErrorHandler = null)
        {
            throw new NotImplementedException();
        }

        public Task<bool> BatchInsertUseValuesAsync(IList<TestTreeE> insertList, long warnMs = -1, Action<Exception, string> execErrorHandler = null)
        {
            throw new NotImplementedException();
        }

        public bool DeleteById(int id, long warnMs = -1, Action<Exception, string> execErrorHandler = null)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteByIdAsync(int id, long warnMs = -1, Action<Exception, string> execErrorHandler = null)
        {
            throw new NotImplementedException();
        }
    }

    public partial class TestTreeERepositoryService
    {

    }


}
