using Common.RabbitMQModule.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Common.Storage;
using Serilog;

namespace Common.RabbitMQModule.Consumers
{
    /// <summary>
    /// RabbitMQ消费者 基类Consumer
    /// </summary>
    public class RabbitMQConsumer : Consumer
    {
        /// <summary>
        /// RabbitMQ消费者
        /// </summary>
        public RabbitMQConsumer()
        {
            Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、{nameof(RabbitMQConsumer)}  消费者默认构造函数  被构造 线程Id：【{Thread.CurrentThread.ManagedThreadId}】...");
        }

        /// <summary>
        /// 初始化消费者指定处理程序与批量事件处理程序
        /// </summary>
        /// <param name="eventHandlers">子类消费者提供的单条数据消费委托</param>
        /// <param name="batchEventHandlers">子类消费者提供的批量数据消费委托</param>
        public RabbitMQConsumer(List<Func<(string, ulong), Task>> eventHandlers, List<Func<List<(string, ulong)>, Task>> batchEventHandlers) : base(eventHandlers, batchEventHandlers)
        {
            Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、{nameof(RabbitMQConsumer)}  消费者 带参构造函数 被构造 线程Id：【{Thread.CurrentThread.ManagedThreadId}】...");
        }


        /// <summary>
        /// 交换机
        /// </summary>
        public string EventBusExchange { get; set; }

        /// <summary>
        /// 消息队列持久化，默认为true
        /// </summary>
        public bool Persistent { get; set; } = true;

        /// <summary>
        /// 生产者名
        /// </summary>
        public string ProducerName { get; set; }

        /// <summary>
        /// 考虑后期支持一个消费者同时绑定多个队列的情况，这里定义为集合
        /// </summary>
        public List<QueueInfo> QueueList { get; set; } = new List<QueueInfo>();

        /// <summary>
        /// 消费者配置 子类可自定义此消费者配置
        /// </summary>
        public ConsumerOptions Config { get; set; } = new ConsumerOptions();

        /// <summary>
        /// 已消费确认的Tag
        /// </summary>
        public List<ulong> AckedTags { get; set; } = new List<ulong>();

        /// <summary>
        /// 批量处理消息部分失败
        /// </summary>
        public bool BathHandlePartialFailed { get; set; }

        /// <summary>
        /// 路由键
        /// </summary>

        public override string RouteKey => QueueList.FirstOrDefault()?.RoutingKey;
    }
}
