
using System.Configuration;
using System.Numerics;
using System.Text;
using Microsoft.AspNetCore.Hosting.WnExtensions;
using Microsoft.Extensions.DependencyInjection.WnExtension;
using Newtonsoft.Json.Extension;
using WindNight.AspNetCore.Hosting;
using WindNight.Config.Abstractions;
using WindNight.ConfigCenter.Extension;
using WindNight.Core.Abstractions;
using WindNight.Extension.Logger.DcLog;
using WindNight.Extension.Logger.DcLog.Abstractions;
using WindNight.Extension.Logger.DcLog.Extensions;
using WindNight.LogExtension;

namespace WebApiDemo
{
    public class Program : DefaultProgramBase
    {
        public static void Main(string[] args)
        {
            var buildType = "";
#if DEBUG
            buildType = "Debug";
#else
            buildType = "Release";
#endif
            Init(CreateHostBuilder, buildType, () =>
            {


            }, args);
        }

        private static IHostBuilder CreateHostBuilder(string buildType, string[] args)
        {
            return CreateHostBuilderDefaults(buildType, args,
                         (hostingContext, configBuilder) =>
                         {
                             configBuilder.SetBasePath(AppContext.BaseDirectory)
                                 .AddJsonFile("Config/AppSettings.json", false, true)
                                 .AddJsonFile("Config/ConnectionStrings.json", false, true)

                                 ;
                         },
                webHostConfigure: webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                },
                configureServicesDelegate: (context, services) =>
                {
                    ConfigItems.Init(configuration: context.Configuration, sleepTimeInMs: 10000000);
                })

                ;
        }




    }

    public class ConfigItems : ConfigItemsBase
    {
        public static void Init(int sleepTimeInMs = 5000,
            ILogService? logService = null,
            IConfiguration? configuration = null)
        {
            StartConfigCenter(sleepTimeInMs, logService, configuration);
        }
    }


    public class Program2
    {



        public static void Main11(string[] args)
        {
            TestDemo122();
            var buildType = "";
#if DEBUG
            buildType = "Debug";
#else
            buildType = "Release";
#endif


            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            IServiceCollection services = builder.Services;
            IConfiguration configuration = builder.Configuration;

            builder.Configuration.SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("Config/AppSettings.json", false, true)

                ;

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            services.AddTransient<ILogService, DefaultLogService>();

            LogHelper.RegisterProcessEvent(LogHelper.Log4NetSubscribe);

            services.AddDefaultEgDcLogService(configuration);

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseAuthorization();


            app.MapControllers();

            Ioc.Instance.InitServiceProvider(app.Services);

            LogHelper.LogRegisterInfo(buildType);
            app.Run();
            LogHelper.LogOfflineInfo(buildType);


        }



        public static object TestDemo122()
        {
            // n: nonce，一次性数字，用于确保每个消息的加密结果不同。
            // k: key，密钥，用于加密和解密数据。
            // a: additional data，附加数据，用于提供额外的信息给加密算法，但不包含在密文中。
            // m: plaintext message，明文消息，即原始未加密的消息。
            // c: ciphertext，密文，即经过加密后的消息。
            // s: authenticated data，认证数据，用于验证消息的完整性。
            string n = "0000000000000000";
            string k = "0000000000000000";
            string a = "ASCON"; // 附加数据
            string m = "ascon"; // 明文消息
            m = "000102030405060708"; // 明文消息

            k = "1234567898765432";  // key 
            a = "31373131303131383633";
            n = "31363738393b3c3d3e3f414243444647";


            // var kBytes = Encoding.ASCII.GetBytes(k);

            // byte[] nBytes = Encoding.ASCII.GetBytes(n);
            //  byte[] aBytes = Encoding.ASCII.GetBytes(a);
            // byte[] mBytes = Encoding.ASCII.GetBytes(m);

            var mBytes = new byte[] { 0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08 };
            var aBytes = new byte[] { 0x31, 0x37, 0x31, 0x31, 0x30, 0x31, 0x31, 0x38, 0x36, 0x33 };
            var nBytes = new byte[] { 0x31, 0x36, 0x37, 0x38, 0x39, 0x3b, 0x3c, 0x3d, 0x3e, 0x3f, 0x41, 0x42, 0x43, 0x44, 0x46, 0x47 };
            var kBytes = new byte[] { 0x31, 0x32, 0x33, 0x34, 0x35, 0x36, 0x37, 0x38, 0x39, 0x38, 0x37, 0x36, 0x35, 0x34, 0x33, 0x32 };


            var cBytes1 = new byte[] { 0x32, 0xBD, 0x3A, 0xE2, 0x45, 0xA2, 0x79, 0xC9, 0x5D, 0x82, 0x93, 0xF7, 0x9C, 0xB8, 0xD0, 0xF6, 0x70, 0x2B, 0xC9, 0x90, 0xF4, 0xA6, 0x1C, 0xEE, 0xB2 };


            mBytes = new byte[] { 0x11, 0x22, 0x33, 0x44, 0xaa, 0xff, 0x55, 0x00 };

            aBytes = new byte[] { 0x31, 0x37, 0x31, 0x31, 0x30, 0x31, 0x35, 0x30, 0x30, 0x38 };

            nBytes = new byte[] { 0x12, 0x15, 0x16, 0x18, 0x19, 0x1a, 0x1b, 0x1c, 0x1e, 0x1f, 0x20, 0x21, 0x23, 0x24, 0x25, 0x26 };

            kBytes = new byte[] { 0x31, 0x32, 0x33, 0x34, 0x35, 0x36, 0x37, 0x38, 0x39, 0x38, 0x37, 0x36, 0x35, 0x34, 0x33, 0x32 };

            var cBytes2 = new byte[] { };

            byte[] c = new byte[mBytes.Length + Ascon128av12.CRYPTO_ABYTES];
            byte[] s = { };

            void Print(string name, byte[] array, int length, int offset)
            {
                Console.Write(name + ": ");
                for (int i = offset; i < length; i++)
                {
                    Console.Write(array[i].ToString("X2") + " ");
                }
                Console.WriteLine();
            }

            Print("k", kBytes, kBytes.Length, 0);
            Print("n", nBytes, nBytes.Length, 0);
            Print("a", aBytes, aBytes.Length, 0);
            Print("m", mBytes, mBytes.Length, 0);

            Ascon128av12.crypto_aead_encrypt(c, out var clen, mBytes, mBytes.Length, aBytes, aBytes.Length, s, nBytes, kBytes);
            Print("c", c, c.Length - 16, 0);
            Print("t", c, Ascon128av12.CRYPTO_ABYTES, c.Length - Ascon128av12.CRYPTO_ABYTES);

            byte[] decryptedM = new byte[mBytes.Length];
            Ascon128av12.crypto_aead_decrypt(decryptedM, out var mlen, null, c, clen, aBytes, aBytes.Length, nBytes, kBytes);
            if (mlen != -1)
            {
                Print("p", decryptedM, mlen, 0);
                string plaintext = Encoding.ASCII.GetString(decryptedM, 0, mlen);
                Console.WriteLine("Decrypted plaintext: " + plaintext);
            }
            else
            {
                Console.WriteLine("Verification failed");
            }
            Console.WriteLine();
            var d1 = c.ToHexString();
            var d2 = c.ToGetString();
            var d3 = c.ToBase64String();
            var d4 = c.ToString();
            var obj = new
            {
                n,
                k,
                a,
                m,
                c,
                cS = Encoding.ASCII.GetString(c),
                s,

            };
            return obj;
        }



    }

    public static partial class LogsExtension
    {
        public static IServiceCollection AddDefaultEgDcLogService(this IServiceCollection services,
    IConfiguration configuration,
    IDcLoggerProcessor loggerProcessor = null)
        {

            //LogHelper.RegisterProcessEvent(EgLogHelper.Log4NetSubscribe);
            //services.AddTransient<ILogService, LogService>();

            //  var configuration = services.BuildServiceProvider().GetService<IConfiguration>();
            var configValue = configuration.GetSectionValue<DcLogOptions>();
            Action<DcLogOptions> configure;
            if (configValue != null && !configValue.HostName.IsNullOrEmpty())
            {
                var hostName = configValue.HostName;
                configure = opt =>
                {
                    opt.LogAppCode = configuration.GetValue<string>("AppSettings:AppCode");
                    opt.LogAppName = configuration.GetValue<string>("AppSettings:AppName");
                    opt.LogAppId = configuration.GetValue<int>("AppSettings:AppId");
                    opt.MinLogLevel = configValue.MinLogLevel;

                    opt.HostName = hostName;
                    opt.Port = configValue.Port;

                    opt.IsOpenDebug = configValue.IsOpenDebug;
                    opt.OpenGZip = configValue.OpenGZip;
                    opt.IsConsoleLog = configValue.IsConsoleLog;
                    opt.DcLogVersion = configValue.DcLogVersion;
                    opt.ContentMaxLength = configValue.ContentMaxLength;
                    opt.QueuedMaxMessageCount = configValue.QueuedMaxMessageCount;


                };
            }
            else
            {
                configure = opt =>
                {
                    opt.LogAppCode = configuration.GetValue<string>("AppSettings:AppCode");
                    opt.LogAppName = configuration.GetValue<string>("AppSettings:AppName");
                    opt.LogAppId = configuration.GetValue<int>("AppSettings:AppId");
                    opt.MinLogLevel = LogLevel.Information;

                    opt.HostName = "192.168.2.103";
                    opt.Port = 2555;


                    opt.IsOpenDebug = false;
                    opt.OpenGZip = false;
                    opt.IsConsoleLog = false;
                    opt.DcLogVersion = "1.0.0";
                    opt.ContentMaxLength = 2000;
                    opt.QueuedMaxMessageCount = 1024;


                };
            }

            if (configure == null)
            {
                throw new ArgumentNullException($"DcLogOptions");
            }
            LogHelper.RegisterProcessEvent(DcLogSubscribe);
            services.AddDcLogger(configure, loggerProcessor);

            return services;

        }
        internal static void DcLogSubscribe(LogHelper.LogInfo? logInfo)
        {
            try
            {
                if (logInfo == null) return;
                // if (!ConfigItems.IsReportToE) return;
#if !NET45
                DcLogHelper.Add(logInfo.Content, logInfo.Level, logInfo.Exceptions, logInfo.SerialNumber,
                    logInfo.Timestamps, logInfo.RequestUrl, logInfo.ServerIp, logInfo.ClientIp);
#endif
            }
            catch (Exception e)
            {
                Console.WriteLine("上报日志异常:{0}", e.ToJsonStr());
                // Log(LogLevels.Warning, $"EsLogAsync({logInfo.ToJsonStr()}) error {e.Message}", e);
            }
        }

    }


    public class ConfigCenterAuth : IConfigCenterAuth
    {
        public bool OpenConfigCenterAuth { get; set; } = true;

        public bool ConfigCenterApiAuth()
        {
            return true;
        }

    }

}

