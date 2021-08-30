using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;
using static ConsoleExamples_NetCore5.TestCode;

namespace ConsoleExamples_NetCore5
{

    /***
Question 1. Programming
Background
  The live chat system records the online status of the agent, there are 2 statuses:
        o Online
        o Offline
  The system does not directly record the agent status but records the operation log. There are
        two types of logs:
            o Agent Login (login)
            o Agent logout (logout) 
    The average number of logs per day is less than 10, and the query time range will not exceed one year  
    Log records are not 100% reliable, and omissions or duplications need to be considered. 
        o There may be two consecutive login actions, and the middle of the logout action is lost. Since there is no way to know when you logged out in the middle, it is considered that you have not logged out. 
        o There may be two logout actions in a row, and the login action in the middle is lost. Since it is impossible to know when the login was in the middle, it is deemed as no login
   
     * *
     */

    /**

   问题 1. 编程
背景
 实时聊天系统记录座席的在线状态，有两种状态：
         o 在线
         o 离线
 系统不直接记录座席状态，而是记录操作日志。 有
         两种类型的日志：
             o 代理登录（登录）
             o 代理登出 (logout) 
     日均日志数小于10条，查询时间范围不超过一年 
     日志记录并非100%可靠，需要考虑遗漏或重复。
         o 可能有两次连续的登录动作，中间的注销动作丢失。 由于中途没有办法知道你什么时候退出，就认为你没有退出。
         o 可能连续出现两次注销动作，中间的登录动作丢失。 由于无法知道中途登录的时间，因此视为未登录
     * *
     */


    public class TestCode
    {
        ///<summary>
        /// Log means a log
        /// @property time - time of occurrence,unix timestamp, in seconds. 发生时间，unix 时间戳，以秒为单位。
        /// @property action - log type, possible value is "login" or "logout"
        ///</summary>
        public class Log
        {
            public long time;
            public String action;
        }

        ///<summary>
        /// 
        /// Calculate online time within a period of time
        ///    计算一段时间内的在线时间
        /// @param {long} start - Indicates the start time to be calculated, unix timestamp, in seconds.
        ///                        表示要计算的开始时间，unix时间戳，以秒为单位。
        /// @param {long} end - Indicates the end time to be calculated, unix timestamp, in seconds
        ///                        表示要计算的结束时间，unix时间戳，以秒为单位
        /// @param {Log[]} logs - logs in the time range, sorted in ascending order of time
        ///                        时间范围内的日志，按时间升序排序
        /// @return {long} online time
        /// @example 
        ///
        /// return 20  20-0
        /// getOnlineTime(0, 100, [
        ///     {time: 0, action: "login" }, begin
        ///     {time: 20, action: "logout" },end
        /// ]); 
        ///
        /// return 80 100-20
        ///getOnlineTime(0, 100, [
        ///     {time: 10, action: "logout" },
        ///     {time: 20, action: "login" }, begin
        /// ]);
        ///
        /// return 10 10-0
        ///getOnlineTime(0, 100, [
        ///     {time: 0, action: "login" }, begin
        ///     {time: 10, action: "logout" }, end
        ///     {time: 20, action: "logout" },
        /// ]);
        ///
        /// return 50   50-0 
        /// getOnlineTime(0, 100, [
        ///     {time: 00, action: "login" }, begin
        ///     {time: 30, action: "login" },
        ///     {time: 50, action: "logout" }, end
        /// ]);
        ///
        ///</summary>
        /// <param name="start">
        /// Indicates the start time to be calculated, unix timestamp, in seconds.
        /// need more than 0L
        /// </param>
        /// <param name="end">
        /// Indicates the end time to be calculated, unix timestamp, in seconds
        /// need more than <see cref="start"/>
        /// </param>
        /// <param name="logs">
        ///  logs in the time range, sorted in ascending order of time
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        ///  need start >= 0L, end >= start.
        /// </exception>
        /// <remarks>
        /// <code lang="c#">
        ///   // logs 处理
        ///   <![CDATA[   
        ///     logs =logs.Where(m=>m.time>= start&&m.time< end)
        ///  ]]>
        /// 
        ///  若需要兼容需要处理 time的临界值
        ///   max(start,logs[0].time)
        ///   min(end,logs[logs.length-1].time)
        /// 
        /// </code>
        /// </remarks>
        public long getOnlineTime(long start, long end, Log[] logs)
        {
            if (start < 0L || end < start)
            {
                throw new ArgumentOutOfRangeException($"args {nameof(start)}({start}) or {nameof(end)}({end}) is  invalid .Please check and try again!");
            }
            var ts = DateTimeOffset.FromUnixTimeSeconds(end) - DateTimeOffset.FromUnixTimeSeconds(start);
            if (ts.TotalDays > 365)
            {
                return 0L;
            }

            if (logs.Length == 0) return 0L;

            long totalOnlineTime = 0L, loginIn = 0L, logOut = 0L;
            string preAction = "";

            foreach (var log in logs)
            {
                // 不在范围内的怎么处理？
                // 或者 修正上限和下限 max(start,logs[0].time) min(end,logs[logs.length-1].time) 
                if (log.time < start || log.time > end) continue;

                if (log.action == "login" && (string.IsNullOrEmpty(preAction) || preAction == "logout"))
                {
                    preAction = "login";
                    loginIn = log.time;
                    continue;
                }

                if (log.action == "logout" && preAction == "login")
                {
                    preAction = "logout";
                    logOut = log.time;
                    totalOnlineTime += logOut - loginIn;
                    logOut = 0L;
                    loginIn = 0L;
                    continue;
                }

            }
            // 最后一条是login 没有找到对应的 logout 应该用  end - loginIn ? 或者就处理首次的情况?

            //if (totalOnlineTime == 0 && loginIn > 0 && logOut == 0)
            if (loginIn > 0 && logOut == 0)
            {
                totalOnlineTime += end - loginIn;
                // Trace.Assert(totalOnlineTime == 0 && loginIn > 0 && logOut == 0, $"记录中只有login({loginIn}) 没有logOut=>onlinetime=end({end})-login({loginIn})={totalOnlineTime}");
                Debug.Assert(loginIn > 0 && logOut == 0, $"记录中只有login({loginIn}) 没有logOut=>onlinetime=end({end})-login({loginIn})={totalOnlineTime}");
            }

            return totalOnlineTime;
        }


        ///<summary>
        /// 
        /// Count the daily online time of an agent for a period of time  左闭右开 [start,end)
        ///   统计一段时间内座席每天的在线时间 秒级
        /// @param {long} start - start and end, it has been aligned to 0:00:00 of the day, no need to round up.
        ///                         开始和结束，已经对齐到当天的 0:00:00，不需要四舍五入。
        /// @param {long} end - the end time, which has been aligned to 0:00:00 of the day, and does not need to be rounded up.
        ///                         结束时间，与当天的 0:00:00 对齐，不需要四舍五入。
        /// @param {Log[]} logs - logs in the time range, sorted in ascending order of time
        ///                           时间范围内的日志，按时间升序排序
        /// @return {long[]} Daily online time list, excluding the day where the end time is located.
        ///                          每日在线时间列表，不包括结束时间所在的日期 
        /// @example
        ///
        /// return [20, 30]
        ///getOnlineTimePerDay(0, 86400000 * 2, [
        /// {time: 0, action: "login" },
        /// {time: 20, action: "logout" },
        /// {time: 86400000, action: "login" },
        /// {time: 86400030, action: "logout" },
        /// ]);
        ///
        /// return [86400000, 30]
        ///getOnlineTimePerDay(0, 86400000 * 2, [
        /// {time: 0, action: "login" },
        /// {time: 86400000, action: "login" },
        /// {time: 86400030, action: "logout" },
        /// ]);
        ///
        ///</summary>
        long[] getOnlineTimePerDay1(long start, long end, Log[] logs)
        {
            // 假设logs里是当天的数据 只求当天活跃时间段 

            if (start < 0L || end < start)
            {
                throw new ArgumentOutOfRangeException($"args {nameof(start)}({start}) or {nameof(end)}({end}) is  invalid .Please check and try again!");
            }

            var ts = DateTimeOffset.FromUnixTimeSeconds(end) - DateTimeOffset.FromUnixTimeSeconds(start);
            if (ts.TotalDays > 365)
            {
                return new long[0] { };
            }

            if (logs.Length == 0) return new long[0] { };

            long totalOnlineTime = 0L, loginIn = 0L, logOut = 0L;
            string preAction = "";
            List<long> onlineTimeList = new List<long>();
            foreach (var log in logs)
            {
                // 不在范围内的怎么处理？
                // 或者 修正上限和下限 max(start,logs[0].time) min(end,logs[logs.length-1].time) 
                if (log.time < start || log.time > end) continue;

                if (log.action == "login" && (string.IsNullOrEmpty(preAction) || preAction == "logout"))
                {
                    preAction = "login";
                    loginIn = log.time;
                    continue;
                }

                if (log.action == "logout" && preAction == "login")
                {
                    preAction = "logout";
                    logOut = log.time;
                    onlineTimeList.Add(logOut - loginIn);
                    logOut = 0L;
                    loginIn = 0L;
                    continue;
                }

            }
            // 最后一条是login 没有找到对应的 logout 应该用  end - loginIn ? 或者就处理首次的情况?

            //if (totalOnlineTime == 0 && loginIn > 0 && logOut == 0)
            if (loginIn > 0 && logOut == 0)
            {
                onlineTimeList.Add(end - loginIn);
                // Trace.Assert(totalOnlineTime == 0 && loginIn > 0 && logOut == 0, $"记录中只有login({loginIn}) 没有logOut=>onlinetime=end({end})-login({loginIn})={totalOnlineTime}");
                Debug.Assert(loginIn > 0 && logOut == 0, $"记录中只有login({loginIn}) 没有logOut=>onlinetime=end({end})-login({loginIn})={totalOnlineTime}");
            }

            return onlineTimeList.ToArray();
        }

        Dictionary<string, long> getOnlineTimePerDay2(long start, long end, Log[] logs)
        {
            //   求logs里在区间[start,end)内每天的活跃时间总数 
            // 实际代码需要考虑时区？
            var startDate = DateTimeOffset.FromUnixTimeSeconds(start).Date;
            var endDate = DateTimeOffset.FromUnixTimeSeconds(end).Date;

            var dict = new Dictionary<string, long>();
            for (var date = startDate; date < endDate; date = date.AddDays(1))
            {
                var s = new DateTimeOffset(startDate).ToUnixTimeSeconds();
                var e = new DateTimeOffset(startDate.AddDays(1)).ToUnixTimeSeconds();
                var dayLogs = logs.Where(m => m.time >= s && m.time < e).ToArray();

                var onlineTimes = getOnlineTime(s, e, dayLogs);
                dict.Add(date.ToString("yyyyMMdd"), onlineTimes);

            }
            return dict;

        }

    }


    public class TestCore
    {
        protected readonly ITestOutputHelper OutputHelper;

        public TestCore(ITestOutputHelper outputHelper)
        {
            OutputHelper = outputHelper;
        }

        protected void Output(string message)
        {
            Console.WriteLine($"Console:{DateTime.Now:yyyy-MM-dd HH:mm:sss}  {message}");
            OutputHelper.WriteLine($"ITestOutputHelper:{DateTime.Now:yyyy-MM-dd HH:mm:sss}  {message}");

        }

        class TestM
        {
            public long start { get; set; }
            public long end { get; set; }
            public Log[] logs { get; set; }
            public long expected { get; set; }

            public override string ToString()
            {
                return $"start{start}:end{end}:expected:{expected}";
            }

        }

        static readonly List<TestM> MockTestData = new List<TestM>()
        {
            new TestM{ start=00L,end=100L,logs=new []
            {
                new  Log {time= 00L, action= "login" },
                new  Log {time= 20L, action= "logout" },
            },expected=20L,},

            new TestM{ start=00L,end=100L,logs=new []
            {
                new  Log {time= 00L, action= "login" },
                new  Log {time= 20L, action= "logout" },
                new  Log {time= 20L, action= "login" },
                new  Log {time= 40L, action= "logout" },
                new  Log {time= 40L, action= "login" },
                new  Log {time= 60L, action= "logout" },
                new  Log {time= 60L, action= "login" },
                new  Log {time= 80L, action= "logout" },

            },expected=80L,},

            new TestM{ start=00L,end=100L,logs=new []
            {
                new  Log {time= 10L, action= "logout" },
                new  Log {time= 20L, action= "login" },
            },expected=80L,},

            new TestM{ start=00L,end=100L,logs=new []
            {
                new  Log {time= 00L, action= "login" },
                new  Log {time= 10L, action= "logout" },
                new  Log {time= 20L, action= "logout" },
            },expected=10L,},

            new TestM{ start=00L,end=100L,logs=new []
            {
                new  Log {time= 00L, action= "login" },
                new  Log {time= 30L, action= "login" },
                new  Log {time= 50L, action= "logout" },
            },expected=50L,},

            new TestM{ start=00L,end=100L,logs=new []
            {
                new  Log {time= 00L, action= "login" },
                new  Log {time= 30L, action= "login" },
                new  Log {time= 50L, action= "logout" },
                new  Log {time= 50L, action= "login" },
            },expected=100L,},

            new TestM{ start=00L,end=100L,logs=new []
            {
                new  Log {time= 50L, action= "logout" },
            },expected=0L,},

            new TestM{ start=00L,end=100L,logs=new []
            {
                new  Log {time= 50L, action= "login" },
            },expected=50L,},


        };

        ///<summary>
        /// return 20                         // 20-0
        /// getOnlineTime(0, 100, [
        ///     {time: 0, action: "login" },  // begin
        ///     {time: 20, action: "logout" },// end
        /// ]); 
        ///
        /// return 80                           //100-20
        ///getOnlineTime(0, 100, [
        ///     {time: 10, action: "logout" },
        ///     {time: 20, action: "login" }, // begin
        /// ]);
        ///
        /// return 10                          // 10-0
        ///getOnlineTime(0, 100, [
        ///     {time: 0, action: "login" },   // begin
        ///     {time: 10, action: "logout" }, // end
        ///     {time: 20, action: "logout" },
        /// ]);
        ///
        /// return 50                          // 50-0 
        /// getOnlineTime(0, 100, [
        ///     {time: 00, action: "login" },  // begin
        ///     {time: 30, action: "login" },
        ///     {time: 50, action: "logout" }, //  end
        /// ]);
        ///
        ///</summary>
        [Fact(DisplayName = "GetOnlineTimeTest")]
        public void GetOnlineTimeTest()
        {
            var list = MockTestData;

            bool flag = true;
            var server = new TestCode();
            foreach (var item in list)
            {
                var testValue = server.getOnlineTime(item.start, item.end, item.logs);
                long expected = item.expected;
                Output($"getOnlineTime({item})={testValue},expected is {expected}");
                Assert.True(expected == testValue, $"getOnlineTime({item})={testValue} 和预期值{expected} 不相等");

                flag &= expected == testValue;
            }

            Assert.True(flag, $"getOnlineTime有用例没有通过");
        }

    }


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

    /**
问题 2. 数据库设计
背景
  票务系统处理来自电子邮件或网站的客户问题，并为每个客户创建一个票证案件。
    一张工单包括 30 个预定义的系统字段，例如 id、标题、描述、创建时间、状态、受理人、部门等，以及管理员可以添加/删除的其他自定义字段。自定义字段可以有不同的类型，例如文本框、下拉列表、复选框、复选框列表等下拉列表和复选框列表包含多个选项，这些选项也是可定制的。
    例如，我们有一个关于问题类别的字段，其中包含售前、产品、付款等选项，代理可以选择一个（或多个，如果它是复选框列表）工单类别。
    假设
        1. 它是一个 SaaS（软件即服务）解决方案和多租户系统。 
        2. 支持100万张以上的门票。 
        3. 我们需要非常频繁的支持搜索票，支持票的多个字段的条件。例如搜索状态为打开的工单，受托人为我自己和类别（自定义字段）为产品。 
        4. 创建超过一个月的工单不会被频繁查看​​/搜索或修改，让我们说比最近的票少大约 100 倍。 
        5. 管理员设置并开始使用后，工单的字段很少会发生变化

问题请完成关系型数据库的数据库设计描述定义
自定义字段和票证存储。
     * *
     */




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
问题 3. 架构设计
背景
    这是一个实时聊天SaaS系统，对来自不同国家的客户开放使用。一位客户可以注册一个帐户并在此帐户中添加多个代理与他的网站访问者聊天。
    为了满足数据安全策略，我们必须将代理数据存储在位于客户所在区域的数据中心。
        o 例如，我们设置美国平台（应用平台）来服务美国客户并存储他们的数据。
        o 我们为一个平台部署了一系列实时聊天应用程序及其数据库。

    现在我们拥有美国平台、欧盟平台、CA平台和APAC平台，为来自不同地区的客户提供服务。
        o 美国客户注册账户时，系统会将此次注册路由至美国平台，并在该平台添加账户。
        o 同时，第一个代理将随着注册自动创建。
        o 同样，客户将在美国平台上添加/更新/删除代理，或修改代理密码。

    有统一的登录入口供所有客户登录。因此，应该有一个包含应用程序和数据库的全球平台，用于存储所有代理信息以实现这一点。

要求
    1. 代理字段：姓名、电子邮件、密码。电子邮件必须是唯一的。
    2、在应用平台添加/更新/删除代理时，代理可在5秒内登录全球平台。
    3. 数据库只能在平台内连接，不能从其他平台访问。

问题
    请设计图表以显示应用程序平台和全球平台之间的关系。
    请设计应用数据库和全局数据库中的表。
    请设计系统以满足要求。  
     *
     **/

    /**
     *  
     *    register email->user 
     *    login
     *    proxy add del update
     *
     *    chat
     *    
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
