using System;
using System.Security.Cryptography.Extensions;
using System.Threading;
using Newtonsoft.Json.Extension;
using RabbitMQ.Client;
using WindNight.RabbitMq.Abstractions;
using WindNight.RabbitMq.Internal;

namespace WindNight.RabbitMq;

/// <summary>
///     生产者类
/// </summary>
public class Producer // : IDisposable
{
    private BasicLibrary basicLibrary;

    private bool breakRepairLoop;

    private string encrypturi;

    private readonly object lockObj;

    /// <summary>
    ///     本地文件修复线程
    /// </summary>
    private Thread loopRepairExceptionThread;

    private IModel model;

    private readonly ProducerConfigInfo producerConfigInfo;

    private readonly Producer spareProduce;
    private readonly string uri;

    /// <summary>
    ///     临时消息本地存储器
    /// </summary>
    private IMessageWrapper wrapper;

    private string EncryptUri
    {
        get
        {
            if (encrypturi.IsNullOrEmpty()) encrypturi = RSAEncrypt(uri);

            return encrypturi;
        }
    }

    /// <summary>
    ///     通道连接信息
    /// </summary>
    private IModel Model
    {
        get
        {
            if (model == null)
                lock (lockObj)
                {
                    if (model == null)
                        CreateModel();
                }

            return model;
        }
    }

    /// <summary>
    ///     初始化通道
    /// </summary>
    /// <returns></returns>
    private void CreateModel()
    {
        basicLibrary = new BasicLibrary(uri);
        model = basicLibrary.CreateProducerChannelByConfig(producerConfigInfo);
    }

    /// <summary>
    ///     发送消息(不自动重试,不融断,实时返回发送结果)
    /// </summary>
    /// <param name="message">消息</param>
    /// <param name="routingKey">路由键</param>
    /// <param name="isMessageDurable">消息是否持久化</param>
    /// <returns>true=成功</returns>
    public bool SendWithNotRetry(string message, string routingKey, bool isMessageDurable = true)
    {
        var messageBodyBytes = CommonLibrary.BinarySerialize(message);
        return SendWithNotRetry(messageBodyBytes, routingKey, isMessageDurable);
    }

    /// <summary>
    ///     发送消息(不自动重试,不融断,实时返回发送结果)
    /// </summary>
    /// <param name="message">消息</param>
    /// <param name="routingKey">路由键</param>
    /// <param name="isMessageDurable">消息是否持久化</param>
    /// <returns>true=成功</returns>
    public bool SendWithNotRetry(byte[] messageBodyBytes, string routingKey, bool isMessageDurable = true)
    {
        try
        {
            var config = new BasicProperties { Durable = isMessageDurable };
            var configInfo = CommonLibrary.CreateBasicProperties(Model, config);
            Model.BasicPublish(producerConfigInfo.ExchangeName, routingKey, configInfo, messageBodyBytes);
            return true;
        }
        catch (Exception ex)
        {
            var errLog = $"ExchangeName:{producerConfigInfo.ExchangeName},RoutingKey:{routingKey},发送失败";
            LogHelper.Error(errLog, ex);
            return false;
        }
    }

    /// <summary>
    ///     发送消息
    /// </summary>
    /// <param name="message">消息</param>
    /// <param name="routingKey">路由键</param>
    /// <param name="isMessageDurable">消息是否持久化</param>
    /// <returns>true=成功</returns>
    public bool Send(string message, string routingKey, bool isMessageDurable = true)
    {
        return Send(message, routingKey, new BasicProperties { Durable = isMessageDurable }, null);
    }

    /// <summary>
    ///     发送消息
    /// </summary>
    /// <param name="message">消息</param>
    /// <param name="routingKey">路由键</param>
    /// <param name="basicProperties">基础属性配置</param>
    /// <returns>true=成功</returns>
    public bool Send(string message, string routingKey, BasicProperties basicProperties)
    {
        return Send(message, routingKey, basicProperties, null);
    }

    internal bool Send(string message, string routingKey, BasicProperties basicProperties, MessageLocal messageLocal)
    {
        try
        {
            var configInfo = basicProperties == null
                ? null
                : CommonLibrary.CreateBasicProperties(Model, basicProperties);
            var messageBodyBytes = CommonLibrary.BinarySerialize(message);
            Model.BasicPublish(producerConfigInfo.ExchangeName, routingKey, configInfo, messageBodyBytes);
            //#endif

            return true;
        }
        catch (Exception ex)
        {
            var log = "";
            if (spareProduce != null)
                log = $"发送失败:本次消息推送到中转服务处理 ExchangeName:{producerConfigInfo.ExchangeName},RoutingKey:{routingKey} ";
            else
                log = $"发送失败:本次消息记录到本地临时文件 ExchangeName:{producerConfigInfo.ExchangeName},RoutingKey:{routingKey} ";

            LogHelper.Error(log, ex);

            SaveException(message, routingKey, producerConfigInfo, messageLocal);
            return false;
        }
    }

    #region 加密

    private static string RSAEncrypt(string content)
    {
        // read from config
        var publicKey =
            "BgIAAACkAABSU0ExAAQAAAEAAQDhywxiz16bJ1YSx187lNqNz8ltNXhpivkt2WJGEpraUvHKhdF6h5rcses7gxOhAAg38/ZZlZq26Ssm2v791c8+DZ0CAkDfGG7GwbVmV2k2hLU6IB0Owof2IroMvR2mBkxGMPRcOfk/3JMasY451oOo7t3XlHmvtpTcZTrMDMDNzw==";
        return content.RSAEncrypt(publicKey);
        //RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
        //rsa.ImportCspBlob(Convert.FromBase64String(publicKey));
        //byte[] cipherbytes = rsa.Encrypt(content.ToBytes(), false);
        //return Convert.ToBase64String(cipherbytes);
    }

    #endregion

    public void Dispose()
    {
        ///退出修复线程
        breakRepairLoop = true;
    }

    /// <summary>
    ///     释放资源
    /// </summary>
    private void DisposeResource()
    {
        if (basicLibrary != null)
        {
            try
            {
                basicLibrary.Dispose();
            }
            catch (Exception ex)
            {
                LogHelper.Error("basicLibrary.Dispose()", ex);
            }

            basicLibrary = null;
        }

        if (wrapper != null)
        {
            try
            {
                wrapper.Dispose();
            }
            catch (Exception ex)
            {
                LogHelper.Error("wrapper.Dispose()", ex);
            }

            wrapper = null;
        }
    }

    #region 构造函数

    ~Producer()
    {
        Dispose();
        GC.Collect();
    }

    /// <summary>
    ///     生产者初始化
    /// </summary>
    /// <param name="uri">amqp地址</param>
    /// <param name="producerConfigInfo">生产者配置信息</param>
    /// <param name="messageWrapper"> 消息包装器 </param>
    public Producer(string uri, ProducerConfigInfo producerConfigInfo, IMessageWrapper messageWrapper = null)
    {
        if (producerConfigInfo.FileName.IsNullOrEmpty()) producerConfigInfo.FileName = producerConfigInfo.ExchangeName;
        this.uri = uri;
        lockObj = new object();

        this.producerConfigInfo = producerConfigInfo;
        wrapper = new DefaultMessageWrapper(producerConfigInfo.FileName);
        LoopRepairException();


        if (!producerConfigInfo.SpareMqUri.IsNullOrEmpty())
        {
            if (producerConfigInfo.SpareExchangeName.IsNullOrEmpty() ||
                producerConfigInfo.SpareRoutingKey.IsNullOrEmpty())
                throw new ArgumentNullException("SpareExchangeName or SpareRoutingKey can not be empty");

            spareProduce = new Producer(producerConfigInfo.SpareMqUri, new ProducerConfigInfo
            {
                ExchangeDurable = true,
                ExchangeName = producerConfigInfo.SpareExchangeName,
                ExchangeTypeCode = ExchangeTypeCodeEnum.Topic,
                FileName = $"{producerConfigInfo.ExchangeName}_{producerConfigInfo.SpareExchangeName}"
            });
        }
    }

    #endregion


    #region 异常数据处理

    /// <summary>
    ///     检查本地消息
    /// </summary>
    /// <param name="messageLocal"></param>
    /// <returns></returns>
    private bool CheckLocalMessage(MessageLocal messageLocal, IMessageWrapper wrapper)
    {
        if (messageLocal.NeedGiveUp) return false;

        return true;
    }

    /// <summary>
    ///     开始循环修复异常
    /// </summary>
    private void LoopRepairException()
    {
        loopRepairExceptionThread = new Thread(p =>
        {
            var sleepTicks = 1;
            var sender = (Producer)p;
            while (true)
                try
                {
                    if (breakRepairLoop) break;

                    var localFile = false; //本地存在文件
                    var repairCount = 0; //修复数量
                    MessageLocal msgLocal;

                    while ((msgLocal = sender.wrapper.ReadLine()) != null)
                    {
                        localFile = true;

                        if (!CheckLocalMessage(msgLocal, sender.wrapper)) //丢弃消息
                            continue;

                        if (!sender.Send(msgLocal.Message, msgLocal.RoutingKey, null, msgLocal))
                            break;

                        repairCount++;
                        Thread.Sleep(1);
                    }

                    //异常或者没有修复数据的时候，叠加延时
                    //修复数量
                    if (repairCount > 0)
                    {
                        sleepTicks = 1;
                    }
                    else if (localFile) //存在本地文件
                    {
                        if (sleepTicks < 10)
                            sleepTicks++;
                    }
                    else //不存在本地文件
                    {
                        if (sleepTicks < 20)
                            sleepTicks++;
                    }

                    if (repairCount > 0)
                        RecordLog.Debug($"修复线程: {sender.producerConfigInfo.ExchangeName} 修复本地文件数量:{repairCount}");

                    RecordLog.Debug($"修复线程: {sender.producerConfigInfo.ExchangeName} 延时:{sleepTicks}秒");
                    Thread.Sleep(sleepTicks * 1000);
                }
                catch (ThreadAbortException ex)
                {
                    breakRepairLoop = true;
                    LogHelper.Error("LoopRepairException:ThreadAbortException", ex);
                }
                catch (Exception ex)
                {
                    LogHelper.Error("LoopRepairException", ex);
                }

            if (breakRepairLoop) DisposeResource();
        });
        loopRepairExceptionThread.Start(this);
    }

    /// <summary>
    ///     临时存储异常时的消息
    /// </summary>
    private void SaveException(string message, string routingKey, ProducerConfigInfo producerConfigInfo,
        MessageLocal messageLocal)
    {
        if (spareProduce != null)
            SaveExceptionSpareProcess(message, routingKey, producerConfigInfo);
        else
            SaveExceptionLocalFile(message, routingKey, producerConfigInfo, messageLocal);
    }

    /// <summary>
    ///     处理异常
    /// </summary>
    private void SaveExceptionSpareProcess(string message, string routingKey, ProducerConfigInfo producerConfigInfo)
    {
        var msg = new
        {
            Exchange = producerConfigInfo.ExchangeName,
            RoutingKey = routingKey,
            Msg = message,
            Uri = EncryptUri
        }.ToJsonStr();
        spareProduce.Send(msg, producerConfigInfo.SpareRoutingKey);
    }

    /// <summary>
    ///     保存本地文件
    /// </summary>
    /// <param name="message"></param>
    /// <param name="routingKey"></param>
    /// <param name="producerConfigInfo"></param>
    /// <param name="basicProperties"></param>
    private void SaveExceptionLocalFile(string message, string routingKey, ProducerConfigInfo producerConfigInfo,
        MessageLocal messageLocal)
    {
        try
        {
            var now = DateTime.Now;
            var msgLocal = new MessageLocal
            {
                Message = message,
                RoutingKey = routingKey,
                IsEncrypt = true,
                CreateTime = now.ConvertToUnixTime(),
                ProducerConfigInfo = producerConfigInfo
            };

            if (messageLocal != null)
            {
                msgLocal.CreateTime = messageLocal.CreateTime;
                msgLocal.RetryNum = messageLocal.RetryNum;
            }

            msgLocal.RetryNum++;
            msgLocal.LastRetryTime = now.ConvertToUnixTime();
            wrapper.Write(msgLocal);
        }
        catch (Exception ex)
        {
            var errLog =
                $"保存本地文件异常 ExchangeName:{producerConfigInfo.ExchangeName},RoutingKey:{routingKey},Message:{message}";
            LogHelper.Error(errLog, ex);
            throw ex;
        }
    }

    #endregion
}