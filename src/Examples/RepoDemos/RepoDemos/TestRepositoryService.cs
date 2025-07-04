using System;
using System.Collections.Generic;
using System.Text;
using WindNight.Core.SQL;
using WindNight.Extension.Dapper.Abstractions;
using WindNight.Extension.Dapper.Mysql;
using WindNight.Extension.Db.Abstractions;

namespace RepoDemos
{
    public class TestStatusE : CreateAndUpdateWithStatusBase
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



    public class TestTreeE : TreeEntityBase
    {

    }

    public interface ITestTreeERepositoryService :
        ITreeWriterBaseRepositoryService<TestTreeE>,
        ICUSWriterBaseRepositoryService<TestTreeE>
    // IStatusRepositoryService<TestTreeE>,
    // IBaseRepositoryServiceWithId<TestTreeE>,
    // ITreeRepositoryService<TestTreeE>
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


    }

    public partial class TestTreeERepositoryService
    {

    }


}
