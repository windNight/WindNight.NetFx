//using MySql.Data.MySqlClient; 
using MongoDB.Driver;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading;
using System.Windows.Forms;
using WindNight.DataSourceTestTool.RabbitMQ;
using WindNight.DataSourceTestTool.Redis;

namespace WindNight.DataSourceTestTool.Forms
{

    public partial class DataSourceTestForm : Form
    {
        public DataSourceTestForm()
        {
            InitializeComponent();

            InitControl();
        }

        void InitControl()
        {
            cb_DbType.Items.AddRange(ConstConfig.DataSourceType);
            tb_Output.ReadOnly = true;
            btn_Stop.Visible = false;
            btn_Stop.Enabled = false;
            btn_Test.Enabled = !string.IsNullOrEmpty(DbType.ToLower()) && !string.IsNullOrEmpty(ConnectString);
            btn_Start.Visible = false;
            btn_Start.Enabled = false;
        }

        private string DbType => cb_DbType.Text.Trim();
        private string ConnectString => tb_ConnStr.Text.Trim();
        private string RoutingKey => tb_RoutingKey.Text.Trim();
        private string SendMsg => tb_SendMsg.Text.Trim();

        private void btn_Test_Click(object sender, EventArgs e)
        {
            btn_Test.Enabled = false;
            AppendLine(tb_Output, $"Current dbType is {DbType} connectStr is {ConnectString}.");
            switch (DbType.ToLower())
            {
                case "redis":
                    TestRedis();
                    break;
                case "mssql":
                    TestMssql();
                    break;
                case "mysql":
                    TestMysql();
                    break;
                case "mongodb":
                    TestMongodb();
                    break;
                case "consumer":
                    TestConsumer();
                    break;
                case "producer":
                    TestProducer();
                    break;
                default:
                    break;
            }

            btn_Test.Enabled = true;
        }

        private void cb_DbType_Changed(object sender, EventArgs e)
        {
            if (cb_ClearOutput.Checked)
            {
                tb_Output.Text = "";
            }
            AppendLine(tb_Output, $"Changed to {DbType}");
            if (string.IsNullOrEmpty(ConnectString) || cb_ClearOutput.Checked)
            {
                var conn = ConstConfig.DbConnTemplateDict.SafeGetValue(DbType, "");
                tb_ConnStr.Text = conn;
            }

            btn_Stop.Visible = DbTypeIsConsumer;
            btn_Start.Visible = DbTypeIsConsumer;
            cb_IsAck.Visible = DbTypeIsConsumer;
            tb_RoutingKey.Visible = DbTypeIsProducer;
            tb_SendMsg.Visible = DbTypeIsProducer;
            btn_Test.Enabled = !string.IsNullOrEmpty(DbType.ToLower()) && !string.IsNullOrEmpty(ConnectString);


        }

        void TestRedis()
        {
            var redisOp = RedisOption.BuildRedisOption(ConnectString);
            AppendLine(tb_Output, $"Current RedisOption is {redisOp}");
            var client = new RedisClient(redisOp);
            bool flag = client.IsConnected;
            AppendLine(tb_Output, $"Current RedisClient is IsConnected {flag},If False While Try Auth With Password({redisOp.Password})");

            if (!flag && !string.IsNullOrEmpty(redisOp.Password))
            {
                try
                {
                    var authRet = client.Auth(redisOp.Password);
                    AppendLine(tb_Output, $"client.Auth({redisOp.Password})->{authRet} [Success]");
                    flag = true;
                }
                catch (Exception ex)
                {
                    AppendLine(tb_Output, $"client.Auth({redisOp.Password})-> [Failed] Handler Error {ex.Message}\r\n{ex}");
                    flag = false;
                }
            }

            if (!flag) return;

            if (redisOp.DefaultDb > 0)
            {
                try
                {
                    var selectRet = client.Select(redisOp.DefaultDb);
                    AppendLine(tb_Output, $"client.Select(db:{redisOp.DefaultDb})->{selectRet} [Success]");
                    flag = true;
                }
                catch (Exception ex)
                {
                    AppendLine(tb_Output, $"client.Select(db:{redisOp.DefaultDb}) [Failed] Handler Error {ex.Message}\r\n{ex}");
                    flag = false;
                }
            }
            if (!flag) return;
            try
            {
                var clientListRet = client.ClientList().Replace("\n", "\r\n");
                AppendLine(tb_Output, $"client.ClientList()->\r\n{clientListRet}  ");
            }
            catch (Exception ex)
            {
                AppendLine(tb_Output, $"client.ClientList() [Failed] Handler Error {ex.Message}\r\n{ex}");
            }
            try
            {
                var clientInfoRet = client.Info();
                AppendLine(tb_Output, $"client.Info()->\r\n{clientInfoRet}");
            }
            catch (Exception ex)
            {
                AppendLine(tb_Output, $"client.Info() [Failed] Handler Error {ex.Message}\r\n{ex}");
            }

            try
            {
                var quitRet = client.Quit();
                AppendLine(tb_Output, $"{ConnectString} Close Success ->{quitRet}");
                flag = true;

            }
            catch (Exception ex)
            {
                AppendLine(tb_Output, $"{ConnectString} Close Handler Error {ex.Message}\r\n{ex}");
                flag = false;
            }

        }

        void TestMysql()
        {
            bool flag = false;
            IDbConnection? connection = null;
            try
            {
                connection = new MySqlConnection(ConnectString);
                flag = true;
            }
            catch (Exception ex)
            {
                AppendLine(tb_Output, $"{ConnectString} New MySqlConnection Handler Error {ex.Message}\r\n{ex}");

            }
            if (!flag || connection == null) return;

            try
            {
                connection.Open();
                flag = true;

            }
            catch (Exception ex)
            {
                // tb_Output.Text += $"{ConnectString} OpenHandler Error {ex.Message}\r\n{ex}\r\n";
                AppendLine(tb_Output, $"{ConnectString} Open Handler Error {ex.Message}\r\n{ex}");
                flag = false;
            }
            AppendLine(tb_Output, $"{ConnectString} Open {(flag ? "Success" : "Failed")}");
            //tb_Output.Text += $"{ConnectString} Open {(flag ? "Success" : "Failed")}\r\n";
            if (!flag) return;
            try
            {
                connection.Close();
                flag = true;

            }
            catch (Exception ex)
            {
                AppendLine(tb_Output, $"{ConnectString} Close Handler Error {ex.Message}\r\n{ex}");
                flag = false;
            }
            //tb_Output.Text += $"{ConnectString} Close {(flag ? "Success" : "Failed")}\r\n";
            AppendLine(tb_Output, $"{ConnectString} Close {(flag ? "Success" : "Failed")}");
        }

        void TestMssql()
        {

            bool flag = false;
            IDbConnection? connection = null;
            try
            {
                connection = new SqlConnection(ConnectString);
                flag = connection != null;
            }
            catch (Exception ex)
            {
                AppendLine(tb_Output, $"{ConnectString} New SqlConnection Handler Error {ex.Message}\r\n{ex}");
                flag = false;
            }

            if (!flag) return;
            try
            {
                connection.Open();
                flag = true;

            }
            catch (Exception ex)
            {
                AppendLine(tb_Output, $"{ConnectString} Open Handler Error {ex.Message}\r\n{ex}");
                flag = false;
            }
            AppendLine(tb_Output, $"{ConnectString} Open {(flag ? "Success" : "Failed")}");

            if (!flag) return;
            try
            {
                connection?.Close();
                flag = true;

            }
            catch (Exception ex)
            {
                AppendLine(tb_Output, $"{ConnectString} Close Handler Error {ex.Message}\r\n{ex}");
                flag = false;
            }

            AppendLine(tb_Output, $"{ConnectString} Close {(flag ? "Success" : "Failed")}");

        }

        void TestMongodb()
        {
            var mongoUrl = new MongoUrl(ConnectString);//  MongoUrl.Create(ConnectString);// new MongoUrl(ConnectString);
            var mongoClientSettings = MongoClientSettings.FromUrl(mongoUrl);
            var defaultTs = TimeSpan.FromMinutes(2);
            mongoClientSettings.MaxConnectionIdleTime = defaultTs;
            mongoClientSettings.ConnectTimeout = defaultTs;
            mongoClientSettings.HeartbeatInterval = defaultTs;
            mongoClientSettings.HeartbeatTimeout = defaultTs;

            //mongoClientSettings.ClusterConfigurator = cb =>
            //    cb.ConfigureTcp(tcp => tcp.With(socketConfigurator: (Action<Socket>)SocketConfigurator));

            MongoClient mongoClient = new MongoClient(mongoClientSettings);
            var databaseName = mongoUrl.DatabaseName;
            try
            {
                var dbList = mongoClient.ListDatabases();
            }
            catch (Exception ex)
            {
                AppendLine(tb_Output, $"ListDatabases Handler Error {ex.Message}\r\n{ex}");
            }
            IMongoDatabase Database = mongoClient.GetDatabase(databaseName);

            try
            {

                var alldbs = Database.ListCollectionNames();
                var collectNames = new List<string>();
                while (alldbs.MoveNext())
                {
                    collectNames.AddRange(alldbs.Current);
                }
                AppendLine(tb_Output, $"collectNames is {string.Join(",", collectNames)}");
            }
            catch (Exception ex)
            {
                AppendLine(tb_Output, $"ListCollectionNames Handler Error {ex.Message}\r\n{ex}");

            }
            Database.Client.Cluster.Dispose();

        }

        void TestConsumer()
        {
            (string amqpUrl, string queueName) GetConsumerInfo(string config)
            {
                var array = config.Split(",");
                if (array.Length != 2)
                {
                    return (config, "");
                }
                return (array[0], array[1]);

            }

            var configInfo = GetConsumerInfo(ConnectString);
            AppendLine(tb_Output, $"Consumer config  amqpUrl is {configInfo.amqpUrl} .\r\nqueueName is {configInfo.queueName}");

            if (string.IsNullOrEmpty(configInfo.queueName))
            {
                AppendLine(tb_Output, $"queueName Can Not be NullOrEmpty");
                return;
            }

            var amqpUrl = configInfo.amqpUrl;
            var queueName = configInfo.queueName;
            var consumer = new Consumer(amqpUrl, new ConsumerConfigInfo { QueueName = queueName, });
            // using (var consumer = new Consumer(amqpUrl, new ConsumerConfigInfo { QueueName = queueName }))
            // {
            AppendLine(tb_Output, $"IsChannelOpen: {consumer.IsChannelOpen}");

            int loop = 0;
            bool isAck = false;
            var deliveryTag = 0UL;
            //TODO 使用异步
            while (loop < 10)
            {
                try
                {
                    if (consumer.ReceiveNeedAck(out var message, out deliveryTag, out var routingKey))
                    {
                        AppendLine(tb_Output, $"ReceiveNeedAck(loop:{loop}) message is {message} deliveryTag is {deliveryTag}\r\nroutingKey is {routingKey}");
                        isAck = cb_IsAck.Checked;// false; // maybe true
                    }

                }
                catch (Exception ex)
                {
                    AppendLine(tb_Output, $"ReceiveNeedAck(loop:{loop}) Handler Error {ex.Message}\r\n{ex}");
                    isAck = false;

                }

                if (!isAck)
                {
                    try
                    {
                        consumer.NoAck(deliveryTag, false, true);
                        AppendLine(tb_Output, $"message with deliveryTag {deliveryTag} NoAck Success!");

                    }
                    catch (Exception ex)
                    {
                        AppendLine(tb_Output, $"message with deliveryTag {deliveryTag} NoAck Failed With  Error {ex.Message}\r\n{ex}");
                    }
                }
                loop++;
                Thread.Sleep(1000 * 5);

            }

            try
            {
                consumer.Dispose();
                AppendLine(tb_Output, $"consumer.Dispose() Success!");

            }
            catch (Exception ex)
            {
                AppendLine(tb_Output, $"consumer.Dispose() Failed With  Error {ex.Message}\r\n{ex}");

            }

            //  }



        }

        void TestProducer()
        {

            (string amqpUrl, string topicName) GetProducerInfo(string config)
            {
                var array = config.Split(",");
                if (array.Length != 2)
                {
                    return (config, "");
                }
                return (array[0], array[1]);

            }

            var configInfo = GetProducerInfo(ConnectString);
            var amqpUrl = configInfo.amqpUrl;
            var topicName = configInfo.topicName;
            var routingKey = RoutingKey;
            if (string.IsNullOrEmpty(configInfo.topicName))
            {
                AppendLine(tb_Output, $"topicName Can Not be NullOrEmpty");
                return;
            }

            if (string.IsNullOrEmpty(routingKey))
            {
                AppendLine(tb_Output, $"routingKey Can Not be NullOrEmpty");
                return;
            }
            if (string.IsNullOrEmpty(SendMsg))
            {
                AppendLine(tb_Output, $"SendMsg Can Not be NullOrEmpty");
                return;
            }


            var producer = new Producer(amqpUrl, new ProducerConfigInfo
            {
                ExchangeName = topicName,
                ExchangeDurable = true,
                ExchangeTypeCode = "topic"
            });

            AppendLine(tb_Output, $"IsChannelOpen: {producer.IsChannelOpen}");
            try
            {
                producer.Send(SendMsg, RoutingKey, true);
                AppendLine(tb_Output, $"sendMsg Success! \r\n{SendMsg}");

            }
            catch (Exception ex)
            {
                AppendLine(tb_Output, $"sendMsg  Failed With  Error {ex.Message}\r\n{ex}");
            }

            try
            {

                producer.Dispose();
                AppendLine(tb_Output, $"producer.Dispose() Success!");

            }
            catch (Exception ex)
            {
                AppendLine(tb_Output, $"producer.Dispose() Failed With  Error {ex.Message}\r\n{ex}");

            }


        }

        void AppendLine(TextBoxBase c, string message)
        {
            c.Text += $"{message}\r\n";
        }

        bool DbTypeIsConsumer => DbType.ToLower() == "consumer";
        bool DbTypeIsProducer => DbType.ToLower() == "producer";
        bool DbTypeIsRedis => DbType.ToLower() == "redis";
        bool DbTypeIsMssql => DbType.ToLower() == "mssql";
        bool DbTypeIsMysql => DbType.ToLower() == "mysql";
        bool DbTypeIsMongodb => DbType.ToLower() == "mongodb";

        private void btn_Stop_Click(object sender, EventArgs e)
        {
            btn_Stop.Enabled = false;
            btn_Test.Enabled = false;



            btn_Stop.Enabled = true;
            btn_Test.Enabled = true;

        }

        private void btn_Start_Click(object sender, EventArgs e)
        {
            btn_Start.Enabled = false;



            btn_Start.Enabled = true;

        }


    }

    internal static class ConstConfig
    {
        // Redis #HOST#:#PORT#,password=#PASSWORD#,defaultDataBase=#DATABASE#,timeout=20000,connectRetry=1,connectTimeout=1000,Prefix=xxx
        // Mssql data source=#HOST#,#PORT#;database=#DATABASE#;user id=#USER#;password=#PASSWORD#;Connection Timeout=15;
        // Mysql Data Source=#HOST#;Port=#PORT#;Database=#DATABASE#;User ID=#USER#;Password=#PASSWORD#;Charset=utf8mb4;SslMode=None;
        // Mongodb mongodb://#USER#:#PASSWORD#@#HOST1#:#PORT1#,#HOST2#:#PORT2#/#DATABASE#?replicaSet=YourReplicaSet
        // Consumer  amqp://#USER#:#PASSWORD#@#HOST#:#PORT#,#QUEUENAME#
        // Producer  amqp://#USER#:#PASSWORD#@#HOST#:#PORT#,#TOPICNAME#

        /*
        Data Source=mysql9.tcy365.org;Port=3306;Database=cpsopendatadb;User ID=cpsopendata;Password=m7hvgrdf;Charset=utf8mb4;SslMode=None;

        redis9.tcy365.org:10013,password=msc67b3u,defaultDataBase=15,Prefix=xxxx

        data source=192.168.1.5,1436;database=ctdeveloperdb;user id=ctdeveloper;password=wq4jchag;Connection Timeout=15;


         */
#if DEBUG
        public static Dictionary<string, string> DbConnTemplateDict = new Dictionary<string, string>
        {
            { "Redis","redis9.tcy365.org:10013,password=msc67b3u,defaultDataBase=15,Prefix=xxxx"},
            { "Mssql","data source=192.168.1.5,1436;database=ctdeveloperdb;user id=ctdeveloper;password=wq4jchag;Connection Timeout=15;"},
            { "Mysql","Data Source=mysql9.tcy365.org;Port=3306;Database=cpsopendatadb;User ID=cpsopendata;Password=m7hvgrdf;Charset=utf8mb4;SslMode=None;"},
            { "Mongodb","mongodb://lycpsopen:avgqhp5b@mongodb1.tcy365.org:60002,mongodb2.tcy365.org:60002/lycpsopendb?replicaSet=mongodb34"},
            { "Consumer","amqp://#USER#:#PASSWORD#@#HOST#:#PORT#,#QUEUENAME#"},
            { "Producer","amqp://#USER#:#PASSWORD#@#HOST#:#PORT#,#TOPICNAME#"},
        };
#else
        public static Dictionary<string, string> DbConnTemplateDict = new Dictionary<string, string>
        {
            { "Redis","#HOST#:#PORT#,password=#PASSWORD#,defaultDataBase=#DATABASE#,Prefix=xx"},
            { "Mssql","data source=#HOST#,#PORT#;database=#DATABASE#;user id=#USER#;password=#PASSWORD#;Connection Timeout=15;"},
            { "Mysql","Source=#HOST#;Port=#PORT#;Database=#DATABASE#;User ID=#USER#;Password=#PASSWORD#;Charset=utf8mb4;SslMode=None;"},
            { "Mongodb","mongodb://#USER#:#PASSWORD#@#HOST1#:#PORT1#,#HOST2#:#PORT2#/#DATABASE#?replicaSet=YourReplicaSet"},
            { "Consumer","amqp://#USER#:#PASSWORD#@#HOST#:#PORT#,#QUEUENAME#"},
            { "Producer","amqp://#USER#:#PASSWORD#@#HOST#:#PORT#,#TOPICNAME#"},
        };
#endif

        public static object[] DataSourceType = {
            "Redis" ,
            "Mssql",
            "Mysql",
            "Mongodb",
            "Consumer",
            "Producer",
        };
    }

    internal static class DictionaryExtensions
    {
        /// <summary>
        /// </summary>
        /// <param name="dict"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static int SafeGetValue(this IReadOnlyDictionary<string, int> dict, string key)
        {
            return dict.SafeGetValue(key, 0);
        }

        /// <summary>
        /// </summary>
        /// <param name="dict"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static int SafeGetValue(this IReadOnlyDictionary<long, int> dict, long key)
        {
            return dict.SafeGetValue(key, 0);
        }

        public static T SafeGetValue<TKey, T>(this IReadOnlyDictionary<TKey, T> dict, TKey key,
            T defaultValue = default)
        {
            if (key == null) return defaultValue;
            return dict.ContainsKey(key) ? dict[key] : defaultValue;
        }
    }


}
