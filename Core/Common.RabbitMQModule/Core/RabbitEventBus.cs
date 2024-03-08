using Common.RabbitMQModule.Consumers;
using Common.Storage;
using Serilog;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Common.RabbitMQModule.Producers;

namespace Common.RabbitMQModule.Core
{
    /// <summary>
    /// RabbitMQEventBus RabbitMQ事件总线 RabbitMQ单生产者绑多个消费者管理类
    /// RabbitMQEventBus是IRabbitMQEventBusContainer的进一步封装扩展类
    /// (扩展绑定/解绑生产者，消费者，交换机，路由，是否持久化等信息)
    /// 1、基于事件的消息队列容器/生产者 初始化
    /// 2、消息类型 交换机 路由键 是否持久化设置
    /// 3、消费者Consumer配置项信息 是否
    /// 4、负责绑定解绑生产者、绑定解绑消费者等
    /// </summary>
    public class RabbitMQEventBus
    {
        /// <summary>
        /// 基于事件的消息队列容器/生产者
        /// </summary>
        public IRabbitMQEventBusContainer Container { get; }

        /// <summary>
        /// 交换机
        /// </summary>
        public string Exchange { get; }

        /// <summary>
        /// 路由键
        /// </summary>
        public string RoutingKey { get; }

        /// <summary>
        /// 类型
        /// direct(直接交换模式:它会把消息路由到那些binding key与routing key完全匹配的Queue中, routing(binding) key 最大长度 255 bytes)
        /// fanout(广播模式:它会把所有发送到该Exchange的消息路由到所有与它绑定的Queue中，所以不管你的生产者端的bingding key 和 消费者端的routing key)
        /// topic(主题模式：支持路由规则，routing key中可以含  .*/.#  【.号用于分隔单词,*匹配一个单词、#匹配多个单词】)
        /// header(根据发送的消息内容中的headers属性进行匹配,在绑定Queue与Exchange时指定一组键值对以及x-match参数，x-match参数是字符串类型，
        /// 可以设置为any或者all。如果设置为any，意思就是只要匹配到了headers表中的任何一对键值即可，all则代表需要全部匹配)
        /// </summary>
        public string Type { get; }

        /// <summary>
        /// Consumer配置项信息 是否自动ack  消息处理失败是否重回队列还是不停重发
        /// </summary>
        public ConsumerOptions ConsumerConfig { get; set; }

        /// <summary>
        /// 生产者名称--仅单生产者使用
        /// </summary>
        public string ProducerName { get; set; }

        ///// <summary>
        ///// 生产者字典 key为 ProducerGroupName 可支持多生产者,
        ///// 但极少不这样用 多生产者增加业务复杂度,可根据业务需要进行扩展
        ///// </summary>
        //public ConcurrentDictionary<string, RabbitMQProducer> ProducerDics { get; set; } = new ConcurrentDictionary<string, RabbitMQProducer>();

        /// <summary>
        /// 消息是否持久化
        /// </summary>
        public bool Persistent { get; set; }

        /// <summary>
        /// 消费者集合
        /// </summary>
        public List<RabbitMQConsumer> Consumers { get; set; } = new List<RabbitMQConsumer>();

        /// <summary>
        /// 初始化生产消费者管理器
        /// </summary>
        /// <param name="eventBusContainer">IRabbitMQEventBusContainer </param>
        /// <param name="exchange">交换机</param>
        /// <param name="routingKey">路由键</param>
        /// <param name="type">交换数据类型</param>
        /// <param name="autoAck">是否自动应答</param>
        /// <param name="requeue">消费失败是否重新入队</param>
        /// <param name="persistent">消息是否持久化</param>
        public RabbitMQEventBus(
            IRabbitMQEventBusContainer eventBusContainer,
            string exchange, string routingKey, string type = "direct", bool autoAck = false, bool requeue = true, bool persistent = false)
        {
            Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、{nameof(RabbitMQEventBus)} 被构造 传入IRabbitMQEventBusContainer  exchange, routKey, type,  autoAck ,  requeue , persistent 线程Id：【{Thread.CurrentThread.ManagedThreadId}】");
            if (string.IsNullOrEmpty(exchange))
            {
                throw new ArgumentNullException(nameof(exchange));
            }

            if (string.IsNullOrEmpty(routingKey))
            {
                throw new ArgumentNullException(nameof(routingKey));
            }

            Container = eventBusContainer;
            Exchange = exchange;
            RoutingKey = routingKey;
            Type = type;
            Persistent = persistent;
            ConsumerConfig = new ConsumerOptions
            {
                AutoAck = autoAck,
                Requeue = requeue,
            };

            Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、{nameof(RabbitMQEventBus)} 被构造完毕 线程Id：【{Thread.CurrentThread.ManagedThreadId}】");
        }

        /// <summary>
        /// 初始化生产消费者管理器 添加消费者
        /// </summary>
        /// <param name="handler">单数据处理委托</param>
        /// <param name="batchHandler">批量数据处理委托</param>
        /// <param name="queue">队列信息</param>
        /// <returns>RabbitMQEventBus 单生产者绑多个消费者管理类</returns>
        public RabbitMQEventBus AddConsumer(
            Func<(string bytes, ulong tag), Task> handler,
            Func<List<(string bytes, ulong tag)>, Task> batchHandler,
            string queue = null)
        {
            Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、{nameof(RabbitMQEventBus)} AddConsumer 准备需要的委托和队列 添加消费者 线程Id：【{Thread.CurrentThread.ManagedThreadId}】");
            var queueList = new List<QueueInfo> { new QueueInfo
            {
                //Arguments = null,
                RoutingKey = RoutingKey,
                Queue = queue ?? $"{RoutingKey}_queue" ,
                Exchange = Exchange
            } };
            var consumer = new RabbitMQConsumer(
                new List<Func<(string bytes, ulong tag), Task>> { handler },
                new List<Func<List<(string bytes, ulong tag)>, Task>> { batchHandler })
            {
                EventBusExchange = Exchange,
                ExchangeType = Type,
                QueueList = queueList,
                Config = ConsumerConfig
            };
            Consumers.Add(consumer);
            return this;
        }

        /// <summary>
        /// 添加消费者
        /// </summary>
        /// <param name="consumer"></param>
        /// <returns>RabbitMQEventBus 单生产者绑多个消费者管理类</returns>
        public RabbitMQEventBus AddConsumer(RabbitMQConsumer consumer)
        {
            Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、{nameof(RabbitMQEventBus)} AddConsumer 添加消费者{consumer.Name} 线程Id：【{Thread.CurrentThread.ManagedThreadId}】");
            Consumers.Add(consumer);
            return this;
        }

        ///// <summary>
        ///// 添加多生产者
        ///// </summary>
        ///// <param name="producer">RabbitMQProducer 生产者</param>
        ///// <returns>RabbitMQEventBus 多生产者绑多消费者管理类</returns>
        //public RabbitMQEventBus AddProducer(RabbitMQProducer producer)
        //{
        //    ProducerDics.TryAdd(producer.Name,producer);
        //    return this;
        //}

        #region 单生产者

        /// <summary>
        /// 绑定RabbitMQ生产者，消费者绑管理类
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>RabbitMQEventBus 当前生产-消费管理类</returns>
        public RabbitMQEventBus BindProducer<T>()
        {
            return BindProducer(typeof(T));
        }

        /// <summary>
        /// 绑定RabbitMQ生产者，消费者绑管理类
        /// </summary>
        /// <param name="type"></param>
        /// <returns>RabbitMQEventBus 当前生产-消费管理类</returns>
        public RabbitMQEventBus BindProducer(Type type)
        {
            return BindProducer(type.FullName);
        }

        /// <summary>
        /// 绑定RabbitMQ生产者，消费者绑管理类
        /// </summary>
        /// <param name="name">生产者类型名/生产者名</param>
        /// <returns>RabbitMQEventBus单生产者绑多个消费者管理类</returns>
        public RabbitMQEventBus BindProducer(string name)
        {
            Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、{nameof(RabbitMQEventBus)} BindProducer 线程Id：【{Thread.CurrentThread.ManagedThreadId}】");
            if (ProducerName.IsNullOrWhiteSpace())
            {
                ProducerName = name;
            }
            else
            {
                Log.Warning($"{nameof(RabbitMQEventBus)} 已经绑定过生产者,禁止重复绑定 线程Id：【{Thread.CurrentThread.ManagedThreadId}】");
                //throw new EventBusRepeatBindingProducerException(name);
            }

            return this;
        }

        #endregion

        /// <summary>
        /// 容器/生产者 注册交换机 同一个exchange routingKey不能重复调用，不能重复声明交换机【消费者模块的队列绑定也是如此】
        /// </summary>
        public void Enable()
        {
            Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、{nameof(RabbitMQEventBus)} Enable 容器/生产者 注册交换机 线程Id：【{Thread.CurrentThread.ManagedThreadId}】");
            Container.Work(this);
            //Container.Works(new List<RabbitMQEventBus>() { this });
        }
    }
}
