using System;
using System.Collections.Generic;
using System.Text;

namespace WindNight.Extension.Dapper.Mysql
{
    public partial class MySqlBase
    {

        protected virtual string BaseCColumns => "CreateUserId,CreateDate,CreateUnixTime,IsDeleted";
        protected virtual string BaseCColumnValues => "@CreateUserId,@CreateDate,@CreateUnixTime,@IsDeleted";

        protected virtual string BaseCUColumns => $"{BaseCColumns},UpdateUserId,UpdateDate,UpdateUnixTime";

        protected virtual string BaseCUColumnValues => $"{BaseCColumnValues},@UpdateUserId,@UpdateDate,@UpdateUnixTime";


        protected virtual string BaseStatusColumns => $"Status,{BaseCUColumns}";
        protected virtual string BaseStatusColumnValues => $"@Status,{BaseCUColumnValues}";


        protected virtual string BaseTreeColumns => $"ParentId,{BaseStatusColumns}";
        protected virtual string BaseTreeColumnValues => $"@ParentId,{BaseStatusColumnValues}";


        protected virtual string ToBeUpdateFiled => "";

        protected virtual string QueryAllSqlCondition => " ";





    }
}
