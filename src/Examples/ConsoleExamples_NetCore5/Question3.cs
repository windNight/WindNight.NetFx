namespace comm100
{
    /**
Question 3. Architecture Design
Background
    Here is a Live Chat SaaS system which is open to the customers from different countries to use. One customer could register one account and add multiple agents in this account to chat with his web site visitors.
    To meet the data security policy, we must store the agent data in the data center located in the customer’s region.
        o For example, we setup the US platform (Application platform) to serve the US customers and store their data.
        o We deployed a series of live chat applications and its databases for one platform.  
    Now we have US platform, EU platform, CA platform and APAC platform to serve the customers from different regions. 
        o When a US customer registers an account, system will route this registration to US platform and add the account in this platform.
        o At the same time, the first agent will be created automatically along with the registration.
        o Similarly, the customer will add/update/delete an agent, or modify the password for agent on US platform.
    There is a unified login entry for all the customers to login. So there should be a global platform with applications and database which stored all the agent information to enable this.
Requirement
    1. The Fields for Agent: Name, Email, Password. The Email must be unique.
    2. When adding/updating/deleting an agent at application platform, the agent could login within 5 seconds at global platform.
    3. Database can only be connected within the platform and is not accessible from the other platform.
Problems
    Please design the diagram to show relationship between the application platforms and global platform.
    Please design the table in the application database and global database.
    Please design the system to meet the requirement.
     
     *
     * 
     **/


    /**
     *  
     *    register email->user 
     *    login
     *    proxy add del update
     *    chat
     *    ?
     *    用户注册后是否可以变更自己的区域？ 如果变更区域后数据存储的方式：变更后数据在新库|同步历史数据到新库
     **/

    // global db
    /**
CREATE TABLE `UserAccessTokens`  (
 `Id` int NOT NULL COMMENT '自增Id',
 `UserId` int NOT NULL DEFAULT 0 COMMENT '用户编号',
 `AccessToken` varchar(50) NOT NULL DEFAULT '' COMMENT '用户访问令牌 SSO凭证',
 `RefreshAccessToken` varchar(255) NOT NULL DEFAULT '' COMMENT '用户刷新访问令牌',
 `ExpiredAt`  bigint NOT NULL DEFAULT 0 COMMENT '用户访问令牌过期时间戳（毫秒）',
 `CreateTimeStamp` bigint NOT NULL DEFAULT 0 COMMENT '用户创建时间戳（毫秒）',
 `CreateDateInt` int NOT NULL DEFAULT 0 COMMENT '用户创建日期（yyyMMdd）',
 `TenantId` int NOT NULL DEFAULT 0 COMMENT '租户编号',
 PRIMARY KEY (`Id`),
 INDEX `IX_Customers_TenantId`(`TenantId`) USING BTREE,
 INDEX `IX_Customers_UserId`(`UserId`) USING BTREE,
 INDEX `IX_Customers_AccessToken`(`AccessToken`) USING BTREE,
 INDEX `IX_Customers_CreateDateInt`(`CreateDateInt`) USING BTREE,
 INDEX `IX_Customers_CreateTimeStamp`(`CreateTimeStamp`) USING BTREE
);
*/
    public class UserAccessToken
    {
        /// <summary> 自增Id </summary>
        public int Id { get; set; }
        /// <summary> 用户编号 </summary>
        public int UserId { get; set; }
        /// <summary> 用户访问令牌 SSO凭证  </summary>
        public string AccessToken { get; set; }
        /// <summary>用户刷新访问令牌  </summary>
        public string RefreshAccessToken { get; set; }
        /// <summary> 用户访问令牌过期时间戳（毫秒）  </summary>
        public long ExpiredAt { get; set; }
        /// <summary>  租户编号 </summary>
        public int TenantId { get; set; }
        /// <summary>用户创建时间戳（毫秒）  </summary>
        public long CreateTimeStamp { get; set; }
        /// <summary> 用户创建日期（yyyMMdd）  </summary>
        public int CreateDateInt { get; set; }
    }

    /**
CREATE TABLE `Users`  (
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




    public class User
    {
        /// <summary> 用户编号</summary>
        public int Id { get; set; }
        /// <summary> 用户名称</summary>
        public string Name { get; set; }
        /// <summary> 用户代号</summary>
        public string Code { get; set; }
        /// <summary> 用户邮箱:唯一</summary>
        public string Email { get; set; }
        /// <summary> 用户密码</summary>
        public string Password { get; set; }
        /// <summary> 用户区域类型</summary>
        public int RegionType { get; set; }
        /// <summary> 用户创建时间戳（毫秒） </summary>
        public long CreateTimeStamp { get; set; }
        /// <summary> 用户创建日期（yyyMMdd）  </summary>
        public int CreateDateInt { get; set; }
        /// <summary> 租户编号</summary>
        public int TenantId { get; set; }
        /// <summary> 用户状态</summary>
        public int Status { get; set; }
    }


    // agent db
    /**
CREATE TABLE `AgentUsers`  (
     `Id` int NOT NULL COMMENT '用户编号：自增Id',
     `Name` varchar(50) NOT NULL DEFAULT '' COMMENT '代理用户名称',
     `Code` varchar(255) NOT NULL DEFAULT '' COMMENT '代理用户代号',
     `Email` varchar(100) NOT NULL DEFAULT '' COMMENT '用户邮箱',
     `Password` varchar(50) NOT NULL DEFAULT '' COMMENT '用户密码',
     `RegionType` int NOT NULL DEFAULT 0 COMMENT '代理用户区域类型',
     `CreateUserId` int NOT NULL DEFAULT 0 COMMENT '创建人',
     `CreateTimeStamp` bigint NOT NULL DEFAULT 0 COMMENT '代理用户创建时间戳（毫秒）',
     `CreateDateInt` int NOT NULL DEFAULT 0 COMMENT '代理用户创建日期（yyyMMdd）',
     `TenantId` int NOT NULL DEFAULT 0 COMMENT '租户编号',
     `Status` int NOT NULL DEFAULT 0 COMMENT '代理用户状态',
     `UpdateUserId` int NOT NULL DEFAULT 0 COMMENT '最后编辑人',
     `UpdateTimeStamp` bigint NOT NULL DEFAULT 0 COMMENT '最后编辑时间戳（毫秒）',
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
    public class AgentUser
    {
        /// <summary> 用户编号 </summary>
        public int Id { get; set; }
        /// <summary> 代理用户名称 </summary>
        public string Name { get; set; }
        /// <summary> 代理用户代号 </summary>
        public string Code { get; set; }
        /// <summary>  代理用户邮箱 </summary>
        public string Email { get; set; }
        /// <summary> 用户密码 </summary>
        public string Password { get; set; }
        /// <summary>  代理用户区域类型 </summary>
        public int RegionType { get; set; }
        /// <summary> 创建人  </summary>
        public int CreateUserId { get; set; }
        /// <summary> 代理用户创建时间戳（毫秒） </summary>
        public long CreateTimeStamp { get; set; }
        /// <summary>  代理用户创建日期（yyyMMdd） </summary>
        public int CreateDateInt { get; set; }
        /// <summary> 租户编号  </summary>
        public int TenantId { get; set; }
        /// <summary>  代理用户状态 </summary>
        public int Status { get; set; }
        /// <summary> 最后编辑人 </summary>
        public int UpdateUserId { get; set; }
        /// <summary> 最后编辑时间戳（毫秒） </summary>
        public long UpdateTimeStamp { get; set; }
    }

    /**
CREATE TABLE `AgentUserOperatorLog`  (
 `Id` int NOT NULL COMMENT '自增Id', 
 `AgentUserId` int NOT NULL DEFAULT 0 COMMENT '代理用户编号',
 `ContentBefore` varchar(100) NOT NULL DEFAULT '' COMMENT '变更前内容',
 `ContentAfter` varchar(100) NOT NULL DEFAULT '' COMMENT '变更后内容',
 `CreateUserId` int NOT NULL DEFAULT 0 COMMENT '创建人',
 `CreateTimeStamp` bigint NOT NULL DEFAULT 0 COMMENT '代理用户创建时间戳（毫秒）',
 `CreateDateInt` int NOT NULL DEFAULT 0 COMMENT '代理用户创建日期（yyyMMdd）',
 `TenantId` int NOT NULL DEFAULT 0 COMMENT '租户编号',
 PRIMARY KEY (`Id`),
 INDEX `IX_Customers_TenantId`(`TenantId`) USING BTREE,
 INDEX `IX_Customers_AgentUserId`(`AgentUserId`) USING BTREE 

);
*/
    public class AgentUserOperatorLog
    {
        /// <summary> 自增Id </summary>
        public int Id { get; set; }
        /// <summary> 代理用户编号 </summary>
        public int AgentUserId { get; set; }
        /// <summary> 变更前内容 </summary>
        public string ContentBefore { get; set; }
        /// <summary>  变更后内容 </summary>
        public string ContentAfter { get; set; }
        /// <summary>  创建人 </summary>
        public int CreateUserId { get; set; }
        /// <summary>  代理用户创建时间戳（毫秒） </summary>
        public long CreateTimeStamp { get; set; }
        /// <summary>  代理用户创建日期（yyyMMdd） </summary>
        public int CreateDateInt { get; set; }
        /// <summary>  租户编号 </summary>
        public int TenantId { get; set; }
    }


}
