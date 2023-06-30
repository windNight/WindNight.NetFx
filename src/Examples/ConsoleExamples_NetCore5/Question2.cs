namespace comm100
{

    /**
     *
Question 2. Database Design
Background
     A ticketing system handles customer issues from email or website and create a Ticket for each case. 
       A ticket includes 30 pre-defined system fields such as id, title, description, creation time, status, assignee, department etc.
    as well as other custom fields that admin can add/remove. Custom fields can have different types, such as textbox, drop down list, check box, check box list etc. The drop down list and check box list contains multiple options which are also customizable. 
       For example, we have a field about problem category with options like pre-sales, product, payment etc. and an agent could choose one (or more if it is a check box list) category of a ticket. 
       Assumptions
        1. It is a SaaS (Software as a Service) Solution and multiple-tenancy system. 
        2. It should support more than one million tickets. 
        3. We need support search tickets very frequently with conditions of multiple fields of the ticket. For example search tickets that status is open, assignee to myself and category (custom field) is product. 
        4. Tickets those created more than one month will not be viewed/searched or modified frequently,let's say about 100 times less than recent tickets. 
        5. Ticket's fields will rarely change after an admin setup and start using
Problems 
        Please complete the database design for the relational database to describe the definition of custom fields and ticket

    *
     **/


    // db mysql >=5.7  json 
    /*
     *
CREATE TABLE `CustomIssues2`  (
      `Id` int NOT NULL COMMENT '问题编号',
      `IssueCode` varchar(255) NOT NULL DEFAULT '' COMMENT '问题代号',
      `CustomId` int NOT NULL DEFAULT 0 COMMENT '用户编号',
      `Title` varchar(100) NOT NULL DEFAULT '' COMMENT '标题',
      `IssueJson` varchar(255) NOT NULL DEFAULT '' COMMENT '其他属性以json的方式存储在该字段',
      `Status` int NOT NULL DEFAULT 0 COMMENT '问题状态：提交；打开；受理；结束；挂起等状态枚举',
      `AssigneeId` int NOT NULL DEFAULT 0 COMMENT '受理人编号',
      `AssigneeDepartmentId` int NOT NULL DEFAULT 0 COMMENT '受理人所在部门',
      `AssigneeTime` bigint NOT NULL DEFAULT 0 COMMENT '受理时间戳（毫秒）',
      `CreateTimsStamp` bigint NOT NULL DEFAULT 0 COMMENT '用户创建时间戳（毫秒）',
      `CreateDateInt` int NOT NULL DEFAULT 0 COMMENT '用户创建日期（yyyMMdd）',
      `CreateDateMonth` int NOT NULL DEFAULT 0 COMMENT '用户创建月份（yyyMM）',
      `TenantId` int NOT NULL DEFAULT 0 COMMENT '租户编号',
      	--  and other need index fields
    PRIMARY KEY (`Id`),
      INDEX `IX_Issues_AssigneeId`(`AssigneeId`) USING BTREE,
      INDEX `IX_Issues_Status`(`Status`) USING BTREE,
      INDEX `IX_Issues_CreateTimeStamp`(`CreateTimeStamp`) USING BTREE
      INDEX `IX_Issues_CreateDateInt`(`CreateDateInt`) USING BTREE,
      INDEX `IX_Issues_TenantId`(`TenantId`) USING BTREE


    );
     */
    /// <summary> db mysql >=5.7 使用 json 类型存储自定义项  </summary>
    public class CustomIssue2
    {
        /// <summary>问题编号 </summary>
        public long Id { get; set; }
        /// <summary>用户编号  </summary>
        public long CustomId { get; set; }
        /// <summary>标题  </summary>
        public string Title { get; set; }
        /// <summary>问题状态：提交；打开；受理；结束；挂起等状态枚举  </summary>
        public int Status { get; set; }
        /// <summary>其他属性以json的方式存储在该字段  </summary>
        public string IssueJson { get; set; }
        /// <summary> 受理人编号 </summary>
        public int AssigneeId { get; set; }
        /// <summary> 受理人所在部门 </summary>
        public int AssigneeDepartmentId { get; set; }
        /// <summary>  受理时间戳（毫秒）</summary>
        public long AssigneeTime { get; set; }
        /**
         * and so on ……
         **/
        /// <summary> 用户创建时间戳（毫秒） </summary>
        public long CreateTimeStamp { get; set; }
        /// <summary> 用户创建日期（yyyMMdd） </summary>
        public int CreateDateInt { get; set; }
        /// <summary>用户创建月份（yyyMM）  </summary>
        public int CreateDateMonth { get; set; }
        /// <summary> 租户编号 </summary>
        public int TenantId { get; set; }

    }





    /**
CREATE TABLE `Customers`  (
     `Id` int NOT NULL COMMENT '用户编号：自增Id',
     `Name` varchar(50) NOT NULL DEFAULT '' COMMENT '用户名称',
     `Code` varchar(255) NOT NULL DEFAULT '' COMMENT '用户代号',
     `Email` varchar(100) NOT NULL DEFAULT '' COMMENT '用户邮箱',
     `Password` varchar(50) NOT NULL DEFAULT '' COMMENT '用户密码',
     `RegionType` int NOT NULL DEFAULT 0 COMMENT '用户区域类型',
     `CreateTimeStamp` bigint NOT NULL DEFAULT 0 COMMENT '用户创建时间戳（毫秒）',
     `CreateDateInt` int NOT NULL DEFAULT 0 COMMENT '用户创建日期（yyyMMdd）',
     `TenantId` int NOT NULL DEFAULT 0 COMMENT '租户编号',
     `Status` int NOT NULL DEFAULT 0 COMMENT '用户状态',
     PRIMARY KEY (`Id`),
     INDEX `IX_Customers_TenantId`(`TenantId`) USING BTREE,
     INDEX `IX_Customers_Name`(`Name`) USING BTREE,
     INDEX `IX_Customers_Code`(`Code`) USING BTREE,
     INDEX `IX_Customers_RegionType`(`RegionType`) USING BTREE,
     INDEX `IX_Customers_CreateDateInt`(`CreateDateInt`) USING BTREE,
     INDEX `IX_Customers_CreateTimeStamp`(`CreateTimeStamp`) USING BTREE,
     INDEX `IX_Customers_Status`(`Status`) USING BTREE,
     UNIQUE INDEX `IX_Customers_Email`(`Email`) USING BTREE


);
    */

    /// <summary>   </summary>
    public class Customer
    {
        /// <summary> 用户编号 </summary>
        public int Id { get; set; }
        /// <summary> 用户名称 </summary>
        public string Name { get; set; }
        /// <summary> 用户代号 </summary>
        public string Code { get; set; }
        /// <summary> 用户邮箱 </summary>
        public string Email { get; set; }
        /// <summary>用户区域类型  </summary>
        public int RegionType { get; set; }
        /// <summary> 用户密码 </summary>
        public string Password { get; set; }
        /// <summary> 用户创建时间戳（毫秒） </summary>
        public long CreateTimeStamp { get; set; }
        /// <summary> 用户创建日期（yyyMMdd） </summary>
        public int CreateDateInt { get; set; }
        /// <summary> 租户编号 </summary>
        public int TenantId { get; set; }

    }
    /*
     *
CREATE TABLE `CustomIssues`  (
      `Id` int NOT NULL COMMENT '问题编号',
      `IssueCode` varchar(255) NOT NULL DEFAULT '' COMMENT '问题代号',
      `CustomId` int NOT NULL DEFAULT 0 COMMENT '用户编号',
      `Title` varchar(100) NOT NULL DEFAULT '' COMMENT '标题',
      `Description` varchar(255) NOT NULL DEFAULT '' COMMENT '描述',
      `Status` int NOT NULL DEFAULT 0 COMMENT '问题状态：提交；打开；受理；结束；挂起等状态枚举',
      `AssigneeId` int NOT NULL DEFAULT 0 COMMENT '受理人编号',
      `AssigneeDepartmentId` int NOT NULL DEFAULT 0 COMMENT '受理人所在部门',
      `AssigneeTime` bigint NOT NULL DEFAULT 0 COMMENT '受理时间戳（毫秒）',
      `CreateTimsStamp` bigint NOT NULL DEFAULT 0 COMMENT '用户创建时间戳（毫秒）',
      `CreateDateInt` int NOT NULL DEFAULT 0 COMMENT '用户创建日期（yyyMMdd）',
      `CreateDateMonth` int NOT NULL DEFAULT 0 COMMENT '用户创建日期（yyyMM）',
      `TenantId` int NOT NULL DEFAULT 0 COMMENT '租户编号',
      	--  and other need index fields
    PRIMARY KEY (`Id`),
      INDEX `IX_Issues_AssigneeId`(`AssigneeId`) USING BTREE,
      INDEX `IX_Issues_Status`(`Status`) USING BTREE,
      INDEX `IX_Issues_CreateTimeStamp`(`CreateTimeStamp`) USING BTREE
      INDEX `IX_Issues_CreateDateInt`(`CreateDateInt`) USING BTREE,
      INDEX `IX_Issues_TenantId`(`TenantId`) USING BTREE
    );
     */

    // db mysql < 5.7  claims
    public class CustomIssue
    {
        /// <summary> 问题编号 </summary>
        public long Id { get; set; }
        /// <summary> 用户编号 </summary>
        public long CustomId { get; set; }
        /// <summary> 标题 </summary>
        public string Title { get; set; }
        /// <summary> 描述 </summary>
        public string Description { get; set; }
        /// <summary> 问题状态：提交；打开；受理；结束；挂起等状态枚举  </summary>
        public int Status { get; set; }
        /// <summary> 受理人编号 </summary>
        public int AssigneeId { get; set; }
        /// <summary> 受理人所在部门 </summary>
        public int AssigneeDepartmentId { get; set; }
        /// <summary>  受理时间戳（毫秒）</summary>
        public long AssigneeTime { get; set; }
        /**
         * and so on ……
         **/
        /// <summary> 用户创建时间戳（毫秒） </summary>
        public long CreateTimeStamp { get; set; }
        /// <summary> 用户创建日期（yyyMMdd） </summary>
        public int CreateDateInt { get; set; }
        /// <summary>用户创建月份（yyyMM）  </summary>
        public int CreateDateMonth { get; set; }
        /// <summary> 租户编号 </summary>
        public int TenantId { get; set; }

    }


    /*
     *
CREATE TABLE `IssuesClaimsType`  (
      `Id` int NOT NULL COMMENT '主键Id',
      `CustomId` int NOT NULL DEFAULT 0 COMMENT '用户编号',
      `Status` int NOT NULL DEFAULT 0 COMMENT 'ClaimsType状态：',
      `ClaimType` varchar(100) NOT NULL DEFAULT '' COMMENT 'ClaimType 字段名',
      `Version` varchar(100) NOT NULL DEFAULT '' COMMENT 'ClaimType  版本',
      `TenantId` int NOT NULL DEFAULT 0 COMMENT '租户编号',
    PRIMARY KEY (`Id`),
      INDEX `IX_IssuesClaimsType_CustomId`(`CustomId`) USING BTREE,
      INDEX `IX_IssuesClaimsType_TenantId`(`TenantId`) USING BTREE
    );
     */

    // for  duplicate to  IssuesClaims
    public class IssuesClaimsType
    {
        /// <summary>主键Id  </summary>
        public long Id { get; set; }

        /// <summary> 用户编号 </summary>
        public long CustomId { get; set; }
        /// <summary> ClaimType 字段名 </summary>
        public string ClaimType { get; set; }
        /// <summary> ClaimsType状态  </summary>
        public int Status { get; set; }
        /// <summary>ClaimType  版本  </summary>
        public string Version { get; set; }
        /// <summary> 租户编号 </summary>
        public int TenantId { get; set; }

    }
    /*
        CREATE TABLE `IssuesClaims`  (
          `Id` int NOT NULL COMMENT '主键Id',
          `CustomId` int NOT NULL DEFAULT 0 COMMENT '用户编号',
          `IssuesId` int NOT NULL DEFAULT 0 COMMENT '问题编号',
          `ClaimType` varchar(100) NOT NULL DEFAULT '' COMMENT '属性名',
          `ClaimValue` varchar(100) NOT NULL DEFAULT '' COMMENT '属性值',
          PRIMARY KEY(`Id`),
          INDEX `IX_IssuesClaims_CustomId`(`CustomId`) USING BTREE,
          UNIQUE INDEX `IX_IssuesClaims_CustomId_IssuesId`(`CustomId`, `IssuesId`) USING BTREE

        );
         */

    //  其他不在检索条件里的属性放入到 IssuesClaims 原则上 库里不支持检索 所以程序上在内存中做逻辑检索
    public class IssuesClaims
    {
        /// <summary> 主键Id </summary>
        public long Id { get; set; }
        /// <summary> 用户编号 </summary>
        public long C问题编号ustomId { get; set; }
        /// <summary>  </summary>
        public long IssuesId { get; set; }
        /// <summary>属性名  </summary>
        public string ClaimType { get; set; }
        /// <summary> 属性值 </summary>
        public string ClaimValue { get; set; }

    }



}
