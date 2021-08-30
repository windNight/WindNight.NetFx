using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Xunit;
using Xunit.Abstractions;
using static comm100.Question1;

namespace comm100
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


    public class Question1
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
            var server = new Question1();
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






}
