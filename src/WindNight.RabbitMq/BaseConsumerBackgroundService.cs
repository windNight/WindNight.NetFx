using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json.Extension;
using WindNight.RabbitMq.Abstractions;
using WindNight.RabbitMq.Internal;

namespace WindNight.RabbitMq;

public abstract class BaseConsumerBackgroundService : BackgroundService
{
    private const long DefaultLockTakeKeyExpireMs = 1000 * 60 * 10; //默认 10分钟

    protected readonly IRabbitMqConsumer Consumer;

    public BaseConsumerBackgroundService(IRabbitMqConsumerFactory consumerFactory,
        IRabbitMqConsumerSettings consumerSettings
    )
    {
        if (consumerSettings == null || string.IsNullOrEmpty(consumerSettings.QueueName))
            consumerSettings = DefaultRabbitMqConsumerSettings;
        LogHelper.Info($" RabbitMqConsumerSettings  is {consumerSettings.ToJsonStr()}");
        //初始化MQ消费者队列信息
        Consumer = consumerFactory.GetRabbitMqConsumer(consumerSettings.QueueName, consumerSettings);
    }

    protected abstract string QueueTag { get; }

    protected IRabbitMqConsumerSettings DefaultRabbitMqConsumerSettings
    {
        get
        {
            var config = ConfigItems.RabbitMqConfig.Items.FirstOrDefault(m => m.QueueTag == QueueTag);
            if (config == null) throw new ArgumentNullException("RabbitMqConfig Can not Get from config");
            return new RabbitMqConsumerSettings
            {
                RabbitMqUrl = config.RabbitMqUrl,
                QueueName = config.QueueName,
                LogSwitch = config.LogSwitch,
                PrefetchCount = config.PrefetchCount,
                QueueDurable = config.QueueDurable,
                SleepTime = config.SleepTime,
                ProcessWarnMs = config.ProcessWarnMs
            };
        }
    }


    protected abstract bool HandleOneMessage(string message, string messageMd5);

    //  Stop when stoppingToken is IsCancellationRequested ?
    protected virtual void DoConsumer(CancellationToken stoppingToken)
    {
        Consumer.SetEventingConsumerWithFunc(HandleOneMessage);
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        Task.Run(() => { DoConsumer(stoppingToken); }, stoppingToken);
        return Task.CompletedTask;
    }

    protected virtual void WhileTrueSafe(Action action, string actName)
    {
        while (true)
            try
            {
                action.Invoke();
            }
            catch (Exception ex)
            {
                LogHelper.Error($"Exec Action({actName}) handler errors {ex.Message}", ex);
            }
            finally
            {
                Thread.Sleep(1);
            }
    }

    protected virtual T WhileTrueSafe<T>(Func<T> func, string actName)
    {
        while (true)
            try
            {
                var rlt = func.Invoke();
            }
            catch (Exception ex)
            {
                LogHelper.Error($"Exec Action({actName}) handler errors {ex.Message}", ex);
            }
            finally
            {
                Thread.Sleep(1);
            }
    }

    protected virtual void RunSafe(Action action, string actName)
    {
        try
        {
            action.Invoke();
        }
        catch (Exception ex)
        {
            LogHelper.Error($"Exec Action({actName}) handler errors {ex.Message}", ex);
        }
        finally
        {
            Thread.Sleep(1);
        }
    }

    protected virtual T RunSafe<T>(Func<T> action, string actName)
    {
        try
        {
            return action.Invoke();
        }
        catch (Exception ex)
        {
            LogHelper.Error($"Exec Action({actName}) handler errors {ex.Message}", ex);
            return default;
        }
        finally
        {
            Thread.Sleep(1);
        }
    }

    protected virtual Task TaskRunSafe(Action action, string actName, CancellationToken cancellationToken = default)
    {
        return Task.Run(() =>
        {
            try
            {
                action.Invoke();
            }
            catch (Exception ex)
            {
                LogHelper.Error($"Exec Action({actName}) handler errors {ex.Message}", ex);
            }
        }, cancellationToken);
    }

    protected virtual Task<T> TaskRunSafe<T>(Func<T> func, string actName,
        CancellationToken cancellationToken = default)
    {
        return Task.Run(() =>
        {
            try
            {
                return func.Invoke();
            }
            catch (Exception ex)
            {
                LogHelper.Error($"Exec Action({actName}) handler errors {ex.Message}", ex);
                return default;
            }
        }, cancellationToken);
    }

    //protected virtual bool RedisLockTake(Func<string, string, bool> func, RedisLockPrefixKey preKey, string message, string messageMd5, long expireMs = 0L)
    //{
    //    bool rlt = false;
    //    if (expireMs == 0L) expireMs = DefaultLockTakeKeyExpireMs;
    //    if (_redisLockRedis.RedisLockTake(preKey, messageMd5, expireMs))//异常时 才会用到expireMs
    //    {
    //        try
    //        {
    //            rlt = func.Invoke(message, messageMd5);
    //        }
    //        catch (Exception ex)
    //        {
    //            LogHelper.Error($"Process Message Handler Error {ex.Message}. {message}", ex);
    //        }
    //        finally
    //        {
    //            _redisLockRedis.RedisLockRelease(preKey, messageMd5);
    //        }
    //    }

    //    return rlt;
    //}
}