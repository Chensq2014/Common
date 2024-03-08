using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Common.RabbitMQModule.Client;
using Common.RabbitMQModule.Core;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Common.Storage;
namespace Common.RabbitMQModule.Consumers
{
    /// <summary>
    /// 消费者管理器ConsumerManager(IHostedService)定时运行 
    /// </summary>
    public class ConsumerManager : IHostedService, IDisposable
    {
        /// <summary>
        /// 日志
        /// </summary>
        private readonly ILogger<ConsumerManager> _logger;

        /// <summary>
        /// 客户端
        /// </summary>
        private readonly IRabbitMQClient _client;

        /// <summary>
        /// 事件消息队列容器
        /// </summary>
        private readonly IRabbitMQEventBusContainer _rabbitEventBusContainer;

        /// <summary>
        /// 接口提供器
        /// </summary>
        private readonly IServiceProvider _provider;

        /// <summary>
        /// 运行中的消费者集合 线程安全字典
        /// </summary>
        private readonly ConcurrentDictionary<string, ConsumerRunner> _consumerRunners = new ConcurrentDictionary<string, ConsumerRunner>();

        /// <summary>
        /// 定时锁
        /// </summary>
        private int _monitorTimeLock;

        /// <summary>
        /// 健康检查锁
        /// </summary>
        private int _heathCheckTimerLock;

        /// <summary>
        /// 是否释放
        /// </summary>
        private bool _disposing = false;

        /// <summary>
        /// MonitorTime
        /// </summary>
        public const int MonitorTime = 60 * 2 * 1000;//2分钟定时运行一次

        /// <summary>
        /// CheckTime
        /// </summary>
        public const int CheckTime = 60 * 1000;//1分钟进行一次检测

        /// <summary>
        /// 健康检查时间间隔
        /// </summary>
        public System.Threading.Timer HeathCheckTimer { get; set; }

        /// <summary>
        /// 定时运行时间间隔
        /// </summary>
        public System.Threading.Timer MonitorTimer { get; set; }

        ///// <summary>
        ///// 定时运行时间间隔
        ///// </summary>
        //public System.Timers.Timer MonitorTimer { get; set; }

        /// <summary>
        /// 消费者管理器构造函数
        /// </summary>
        /// <param name="logger">日志</param>
        /// <param name="client">IRabbitMQClient</param>
        /// <param name="provider">IServiceProvider</param>
        /// <param name="rabbitEventBusContainer">IRabbitMQEventBusContainer</param>
        public ConsumerManager(ILogger<ConsumerManager> logger, IRabbitMQClient client, IServiceProvider provider, IRabbitMQEventBusContainer rabbitEventBusContainer)
        {
            logger.LogWarning($"{{0}}", $"{CacheKeys.LogCount++}、{nameof(ConsumerManager)} IHostedService 构造函数 依赖注入ILogger IRabbitMQClient IServiceProvider IRabbitMQEventBusContainer 线程Id：【{Thread.CurrentThread.ManagedThreadId}】");

            _provider = provider;
            _rabbitEventBusContainer = rabbitEventBusContainer;
            _client = client;
            _logger = logger;
            _logger.LogWarning($"{{0}}", $"{CacheKeys.LogCount++}、{nameof(ConsumerManager)} 构造完毕 依赖注入ILogger IRabbitMQClient IServiceProvider IRabbitMQEventBusContainer 线程Id：【{Thread.CurrentThread.ManagedThreadId}】");
        }

        /// <summary>
        /// 定期执行Start
        /// </summary>
        /// <returns></returns>
        private async Task Start()
        {
            _logger.LogWarning($"{{0}}", $"{CacheKeys.LogCount++}、{nameof(ConsumerManager)} IHostedService Start 线程Id：【{Thread.CurrentThread.ManagedThreadId}】");

            try
            {
                //原子操作 单线程
                //比较location1与comparand，如果不相等，什么都不做；
                //如果location1与comparand相等，则用value替换location1的值。
                //无论比较结果相等与否，返回值都是location1中原有的值。
                //(第一次进入:_monitorTimeLock=compareand=0,此时将value(1)赋值给_monitorTimeLock，
                //当快语句执行完毕后，_monitorTimeLock 赋值为原始值0)
                //这样第二次调用时又重复以上流程(达到了线程锁的效果)
                if (Interlocked.CompareExchange(location1: ref _monitorTimeLock, value: 1, comparand: 0) == 0)
                {
                    //每次启动后，检查系统运行中的消费者有哪些 如果是自定义RabbitMQConsumer的话，初始化为ConsumerRunner，
                    //并放在自定义consumers线程安全字典中，接着调用所有consumerRunner.Run方法，主动消费数据。
                    var consumers = _rabbitEventBusContainer.GetConsumers();
                    foreach (var consumer in consumers)
                    {
                        if (consumer is RabbitMQConsumer value)
                        {
                            foreach (var queue in value.QueueList)
                            {
                                var key = queue.ToString();
                                if (!_consumerRunners.ContainsKey(key))
                                {
                                    _logger.LogWarning($"{{0}}", $"{CacheKeys.LogCount++}、{nameof(ConsumerManager)}后台任务启动Start时，会new 一个{nameof(ConsumerRunner)} 构造函数传入 依赖注入的IRabbitMQClient, IServiceProvider, RabbitMQConsumer, QueueInfo:{queue.ToString()}，并把这个runner放入线程安全字典，下一次直接从字典里面获取 线程Id：【{Thread.CurrentThread.ManagedThreadId}】");
                                    var runner = new ConsumerRunner(_client, _provider, value, queue);
                                    _consumerRunners.TryAdd(key, runner);
                                }
                            }
                        }
                    }

                    //调用所有consumerRunner.Run方法，主动消费数据
                    foreach (var consumerRunner in _consumerRunners)
                    {
                        await consumerRunner.Value.Run();
                    }

                    //所有逻辑执行完毕后，_monitorTimeLock恢复为它最原始的值，
                    //达到原子操作(为下一次调用恢复所有变量)
                    Interlocked.Exchange(ref _monitorTimeLock, 0);
                    if (_logger.IsEnabled(LogLevel.Information))
                    {
                        _logger.LogInformation($"消费者管理器后台任务开始工作(EventBus Background Service is working.Start) 线程Id：【{Thread.CurrentThread.ManagedThreadId}】");
                    }
                }
            }
            catch (Exception exception)
            {
                _logger.LogError(exception.InnerException ?? exception, nameof(Start));
                Interlocked.Exchange(ref _monitorTimeLock, 0);
            }
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation($"消费者管理器后台任务开始工作(EventBus Background Service is working.StartAsync) 线程Id：【{Thread.CurrentThread.ManagedThreadId}】");
            }

            await Start();
            //MonitorTimer = new System.Timers.Timer{ Interval = (double)MonitorTime};//new Timer { Interval = 2 * 60 * 1000 };//
            MonitorTimer = new Timer(async state => { await Start(); }, null, MonitorTime, MonitorTime);
            HeathCheckTimer = new Timer(state => { HeathCheck().Wait(cancellationToken); }, null, CheckTime, CheckTime);
        }


        /// <summary>
        /// 健康检查
        /// </summary>
        /// <returns></returns>
        private async Task HeathCheck()
        {
            try
            {
                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation($"消费者管理器后台任务健康检查(EventBus Background Service is checking.) 线程Id：【{Thread.CurrentThread.ManagedThreadId}】");
                }

                if (Interlocked.CompareExchange(ref _heathCheckTimerLock, 1, 0) == 0)
                {
                    await Task.WhenAll(_consumerRunners.Values.Select(runner => runner.HeathCheck()));
                    Interlocked.Exchange(ref _heathCheckTimerLock, 0);
                }
            }
            catch (Exception exception)
            {
                _logger.LogError(exception.InnerException ?? exception, nameof(HeathCheck));
                Interlocked.Exchange(ref _heathCheckTimerLock, 0);
            }
        }

        /// <summary>
        /// 停止
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task StopAsync(CancellationToken cancellationToken)
        {
            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation($"消费者管理器后台任务停止(EventBus Background Service is stopping.) 线程Id：【{Thread.CurrentThread.ManagedThreadId}】");
            }

            Dispose();
            return Task.CompletedTask;
        }


        /// <summary>
        /// Dispose 释放资源
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing && !_disposing)
            {
                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation($"消费者管理器后台任务终止,正在回收资源(EventBus Background Service is disposing.) 线程Id：【{Thread.CurrentThread.ManagedThreadId}】");
                }

                foreach (var runner in _consumerRunners.Values)
                {
                    runner.Close();
                }

                MonitorTimer?.Dispose();
                HeathCheckTimer?.Dispose();

                _disposing = true;
            }
        }

        /// <summary>
        /// 默认Dispose
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}