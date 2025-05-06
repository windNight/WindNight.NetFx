using System;
using System.Collections.Generic;
using System.Text;
using WindNight.Core.SQL;
using WindNight.Extension.Dapper.Mysql;
using WindNight.Extension.Db.Abstractions;

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
        protected override string BusinessColumns =>
            "F1,F2,F3 ";

        protected override string BusinessColumnValues =>
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
        protected override string BusinessColumns =>
            "F1,F2,F3 ";

        protected override string BusinessColumnValues =>
            "@F1,@F2,@F3 ";
        protected override string InsertTableColumns => $"{BusinessColumns},{BaseStatusColumns}";
        protected override string InsertTableColumnValues => $"{BusinessColumnValues},{BaseStatusColumnValues}";
        protected override string EqualEntityCondition => "";


    }



}
