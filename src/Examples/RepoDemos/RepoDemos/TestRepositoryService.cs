using System;
using System.Collections.Generic;
using System.Text;
using WindNight.Core.SQL;
using WindNight.Extension.Db.Abstractions;
using WindNight.Extension.Dapper.Mysql;

namespace RepoDemos
{
    public class TestStatusE : CreateAndUpdateWithStatusBase<int>
    {

    }

    public interface ITestRepositoryService :
        IStatusRepositoryService<TestStatusE>,
        IBaseRepositoryServiceWithId<TestStatusE>
    {

    }


    public class TestRepositoryService : DbBase<TestStatusE>, ITestRepositoryService
    {
        private readonly string BusinessColumns =
            "F1,F2,F3 ";

        private readonly string BusinessColumnValues =
            "@F1,@F2,@F3 ";
        protected override string InsertTableColumns => $"{BusinessColumns},{BaseStatusColumns}";
        protected override string InsertTableColumnValues => $"{BusinessColumnValues},{BaseStatusColumnValues}";
        protected override string EqualEntityCondition => "";


    }



    public class TestTreeE : CommonTreeEntityBase<int>
    {

    }

    public interface ITestTreeERepositoryService :
        IStatusRepositoryService<TestTreeE>,
        IBaseRepositoryServiceWithId<TestTreeE>,
        ITreeRepositoryService<TestTreeE>
    {

    }


    public class TestTreeERepositoryService : DbTreeBase<TestTreeE>, ITestTreeERepositoryService
    {
        private readonly string BusinessColumns =
            "F1,F2,F3 ";

        private readonly string BusinessColumnValues =
            "@F1,@F2,@F3 ";
        protected override string InsertTableColumns => $"{BusinessColumns},{BaseStatusColumns}";
        protected override string InsertTableColumnValues => $"{BusinessColumnValues},{BaseStatusColumnValues}";
        protected override string EqualEntityCondition => "";


    }



}
