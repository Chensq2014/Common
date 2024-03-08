using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Common.Extensions;
using Common.RabbitMQModule.Channels.Abstractions;
using Common.RabbitMQModule.CustomExceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Common.Storage;
using Serilog;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace Common.RabbitMQModule.Channels
{
    /// <summary>
    /// 多生产者单消费者管道代理泛型（multi producer single consumer）
    /// </summary>
    public class MpscChannel<T> : IMpscChannel<T>
    {
        /// <summary>
        /// 消费者委托序列【委托集合】
        /// </summary>
        private readonly List<IBaseMpscChannel> _consumerSequence = new List<IBaseMpscChannel>();

        /// <summary>
        /// 数据流缓冲区，适合多线程高并发处理
        /// </summary>
        private readonly BufferBlock<T> _buffer = new BufferBlock<T>();

        /// <summary>
        /// 消费者处理数据委托
        /// </summary>
        private Func<List<T>, Task> _consumerFunc;

        /// <summary>
        /// 待消费确认
        /// </summary>
        private Task<bool> _waitToReadTask;

        /// <summary>
        /// 日志接口
        /// </summary>
        private readonly ILogger _logger;

        /// <summary>
        /// 是否在自动消费中 默认值0
        /// 单线程模式：每次线程调用时值为1，调用结束后恢复默认值0
        /// </summary>
        private int _autoConsuming;

        /// <summary>
        /// 批量数据处理每次处理的最大数据量
        /// </summary>
        public int MaxBatchSize { get; set; }

        /// <summary>
        /// 批量数据接收的最大延时
        /// </summary>
        public int MaxMillisecondsDelay { get; set; }

        /// <summary>
        /// 是否已经完成
        /// </summary>
        public bool IsComplete { get; private set; }

        /// <summary>
        /// 是否是子级消费者
        /// </summary>
        public bool IsChildren { get; set; }

        /// <summary>
        /// 多生产单消费者管道代理构造函数
        /// </summary>
        /// <param name="logger">日志</param>
        /// <param name="options">通道配置</param>
        public MpscChannel(ILogger<MpscChannel<T>> logger, IOptions<ChannelOptions> options)
        {
            logger.LogWarning($"{{0}}", $"{CacheKeys.LogCount++}、{nameof(MpscChannel<T>)} 被构造，注入IOptions<ChannelOptions> ILogger<MpscChannel<T>> logger 线程Id：【{Thread.CurrentThread.ManagedThreadId}】");
            _logger = logger;
            MaxBatchSize = options.Value.MaxBatchSize;
            MaxMillisecondsDelay = options.Value.MaxMillisecondsDelay;
            logger.LogWarning($"{{0}}", $"{CacheKeys.LogCount++}、{nameof(MpscChannel<T>)} 构造完毕，注入IOptions<ChannelOptions> ILogger<MpscChannel<T>> logger 线程Id：【{Thread.CurrentThread.ManagedThreadId}】");
        }

        /// <summary>
        /// 绑定消费者处理方法
        /// </summary>
        /// <param name="consumerFunc">消费者处理数据委托</param>
        public void BindConsumerFunc(Func<List<T>, Task> consumerFunc)
        {
            if (_consumerFunc is null)
            {
                _logger.LogWarning($"{{0}}", $"{CacheKeys.LogCount++}、{nameof(MpscChannel<T>)} BindConsumer绑定消费者处理数据委托 线程Id：【{Thread.CurrentThread.ManagedThreadId}】");
                _consumerFunc = consumerFunc;
            }
            else
            {
                _logger.LogWarning($"{{0}}", $"{CacheKeys.LogCount++}、单消费者处理数据委托已经绑定 禁止再次BindConsumer绑定消费者处理数据委托 线程Id：【{Thread.CurrentThread.ManagedThreadId}】");
                //throw new RebindConsumerException(GetType().Name);
            }
        }

        /// <summary>
        /// 绑定消费者处理方法
        /// </summary>
        /// <param name="consumerFunc">消费者处理数据委托</param>
        /// <param name="maxBatchSize">批量数据处理每次处理的最大数据量</param>
        /// <param name="maxMillisecondsDelay">批量数据接收的最大延时</param>
        public void BindConsumerFunc(Func<List<T>, Task> consumerFunc, int maxBatchSize, int maxMillisecondsDelay)
        {
            if (_consumerFunc == null)
            {
                _logger.LogWarning($"{{0}}", $"{CacheKeys.LogCount++}、{nameof(MpscChannel<T>)} BindConsumer 消费者处理数据委托 线程Id：【{Thread.CurrentThread.ManagedThreadId}】");
                _consumerFunc = consumerFunc;
                MaxBatchSize = maxBatchSize;
                MaxMillisecondsDelay = maxMillisecondsDelay;
            }
            else
            {
                _logger.LogWarning($"{{0}}", $"{CacheKeys.LogCount++}、单消费者处理数据委托已经绑定 禁止再次BindConsumer 消费者处理数据委托 线程Id：【{Thread.CurrentThread.ManagedThreadId}】");
                //throw new RebindConsumerException(GetType().Name);
            }
        }

        /// <summary>
        /// 配置消费者批量处理数据的参数
        /// </summary>
        /// <param name="maxBatchSize">批量数据处理每次处理的最大数据量</param>
        /// <param name="maxMillisecondsDelay">批量数据接收的最大延时</param>
        public void Config(int maxBatchSize, int maxMillisecondsDelay)
        {
            _logger.LogWarning($"{{0}}", $"{CacheKeys.LogCount++}、{nameof(MpscChannel<T>)} Config 配置批量处理数据的参数 maxBatchSize:批量数据处理每次处理的最大数据量 maxMillisecondsDelay:批量数据接收的最大延时 线程Id：【{Thread.CurrentThread.ManagedThreadId}】");
            MaxBatchSize = maxBatchSize;
            MaxMillisecondsDelay = maxMillisecondsDelay;
        }

        /// <summary>
        /// 将数据写入管道(增加缓冲区，解决高并发)
        /// 该方法被添加到消费者BasicConsumer.Received事件逻辑中
        /// BasicConsumer.Received += async (sender, eventArgs) => await _mpscChannel.WriteAsync(eventArgs);
        /// </summary>
        /// <param name="data">eventArgs(BasicDeliverEventArgs)</param>
        /// <returns>bool写入状态</returns>
        public async ValueTask<bool> WriteAsync(T data)
        {
            //检查消费者委托是否初始化 消费者处理数据委托
            if (_consumerFunc == null)
            {
                throw new NoBindConsumerException(GetType().Name);
            }

            _logger.LogWarning($"{{0}}", $"{CacheKeys.LogCount++}、{nameof(MpscChannel<T>)} WriteAsync方法被添加到消费者BasicConsumer.Received事件逻辑中，BasicConsumer.Received += async (sender, eventArgs) => await _mpscChannel.WriteAsync(eventArgs);  BasicConsumer.Received事件触发时，如果ActiveAutoConsumer()已经将_buffer中的数据消费完毕(_autoConsuming == 0)那么才会再次启动ActiveAutoConsumer() 线程Id：【{Thread.CurrentThread.ManagedThreadId}】");
            if (!IsChildren && _autoConsuming == 0)
            {
                ActiveAutoConsumer();
            }
            _logger.LogWarning($"{{0}}", $"{CacheKeys.LogCount++}、{nameof(MpscChannel<T>)} 如果_autoConsuming=1或者子级消费者IsChildren=true,将数据写入_buffer(BufferBlock)缓存中去 【ActiveAutoConsumer方法中会去判断_buffer(BufferBlock)中待处理的缓冲区数据】 线程Id：【{Thread.CurrentThread.ManagedThreadId}】");
            if (!_buffer.Post(data))
            {
                return await _buffer.SendAsync(data);
            }

            return true;
        }

        /// <summary>
        /// 激活自动消费
        /// </summary>
        private void ActiveAutoConsumer()
        {
            //使用全局线程队列处理消费端业务
            if (!IsChildren && _autoConsuming == 0)
            {
                //委托(回调函数)排队给线程池处理
                ThreadPool.QueueUserWorkItem(ActiveConsumer);
            }

            async void ActiveConsumer(object state)
            {
                //确保当前实例线程中只有一个消费线程再进行批量消费
                if (Interlocked.CompareExchange(ref _autoConsuming, 1, 0) == 0)
                {
                    try
                    {
                        //_buffer中是否还有可以处理的缓冲数据
                        while (await WaitToReadAsync())
                        {
                            try
                            {
                                _logger.LogWarning($"{{0}}", $"{CacheKeys.LogCount++}、{nameof(MpscChannel<T>)} 经过一系列包装之后，最终手动调用的消费者常规批量消费方法 线程Id：【{Thread.CurrentThread.ManagedThreadId}】");
                                await ManualConsume();
                            }
                            catch (Exception ex)
                            {
                                _logger.LogError(ex, ex.Message);
                            }
                        }
                    }
                    finally
                    {
                        //恢复_autoConsuming为初始值0 以便下个线程调用
                        Interlocked.Exchange(ref _autoConsuming, 0);
                    }
                }
            }
        }

        /// <summary>
        /// 把一个mpscchannel消费者关联到另外一个mpscchannel消费者，
        /// 只要有消息进入，所有关联的消费者都会顺序的进行消息检查和处理
        /// </summary>
        /// <param name="mpscChannel"></param>
        public void JoinConsumerSequence(IBaseMpscChannel mpscChannel)
        {
            if (_consumerSequence.IndexOf(mpscChannel) == -1)
            {
                mpscChannel.IsChildren = true;
                _consumerSequence.Add(mpscChannel);
            }
        }

        /// <summary>
        /// 常规批量消费(消费消息，最终都会调用这个方法,准备好数据执行消费者委托)
        /// </summary>
        /// <returns></returns>
        public async Task ManualConsume()
        {
            if (_waitToReadTask.IsCompleted && _waitToReadTask.Result)
            {
                var dataList = new List<T>();
                var startTime = DateTimeOffset.UtcNow;
                while (_buffer.TryReceive(out var value))
                {
                    //可以查看每次接收到数据value 是什么数据[强类型]在_buffer.send是就支持强类型 建议用NotonSoftJson 序列化
                    _logger.LogWarning($"消费者接收到数据json字符串:{TextJsonConvert.SerializeObject(value)}");
                    dataList.Add(value);
                    if (dataList.Count > MaxBatchSize)
                    {
                        break;
                    }

                    if ((DateTimeOffset.UtcNow - startTime).TotalMilliseconds > MaxMillisecondsDelay)
                    {
                        break;
                    }
                }
                if (dataList.Count > 0)
                {
                    _logger.LogWarning($"{{0}}", $"{CacheKeys.LogCount++}、{nameof(MpscChannel<T>)} 将收到的数据交给 消费者处理数据委托 执行 相当于_consumerFunc.Invoke(dataList), 这个消费者委托_consumerFunc在构造函数中被赋值 线程Id：【{Thread.CurrentThread.ManagedThreadId}】");
                    await _consumerFunc(dataList);
                }
            }
            foreach (var joinConsumer in _consumerSequence)
            {
                //递归调用自己 ManualConsume
                await joinConsumer.ManualConsume();
            }
        }

        /// <summary>
        /// 检查当前管道和子管道中是否还有可以进行消费的数据
        /// </summary>
        /// <returns>是否有数据待消费</returns>
        public async Task<bool> WaitToReadAsync()
        {
            _waitToReadTask = _buffer.OutputAvailableAsync();
            if (_consumerSequence.Count == 0)
            {
                return await _waitToReadTask;
            }

            var taskList = _consumerSequence.Select(c => c.WaitToReadAsync()).ToList();
            taskList.Add(_waitToReadTask);
            return await await Task.WhenAny(taskList);
        }

        /// <summary>
        /// Complete完毕 相关子通道代理 缓冲区_buffer.Complete及联动操作
        /// </summary>
        public void Complete()
        {
            IsComplete = true;
            foreach (var joinConsumer in _consumerSequence)
            {
                joinConsumer.Complete();
            }
            _buffer.Complete();
        }
    }
}