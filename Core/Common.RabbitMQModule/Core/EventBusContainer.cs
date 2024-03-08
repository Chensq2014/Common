using Common.Extensions;
using Common.Helpers;
using Common.RabbitMQModule.Client;
using Common.RabbitMQModule.Consumers;
using Common.RabbitMQModule.Producers;
using Common.Storage;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using Serilog;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;


namespace Common.RabbitMQModule.Core
{
    /// <summary>
    /// 基于事件总线RabbitMQ容器/生产者-消费者容器
    /// 实现接口IRabbitMQEventBusContainer(又继承自IConsumerContainer) IProducerContainer
    /// </summary>
    public class EventBusContainer : IRabbitMQEventBusContainer, IProducerContainer
    {
        /// <summary>
        /// 线程安全的 RabbitMQ容器 RabbitMQEventBus字典 存放【RabbitMQEventBus是IRabbitMQEventBusContainer-IConsumerContainer的封装扩展类】
        /// </summary>
        private readonly ConcurrentDictionary<string, RabbitMQEventBus> _eventBusDictionary = new ConcurrentDictionary<string, RabbitMQEventBus>();

        /// <summary>
        /// RabbitMQ容器集合 RabbitMQEventBus是IRabbitMQEventBusContainer的进一步封装扩展类
        /// (扩展绑定/解绑生产者，消费者，交换机，路由，是否持久化等信息)
        /// </summary>
        private readonly List<RabbitMQEventBus> _eventBusList = new List<RabbitMQEventBus>();

        /// <summary>
        /// RabbitMQ客户端
        /// </summary>
        private readonly IRabbitMQClient _rabbitMQClient;

        /// <summary>
        /// 接口提供器IServiceProvider
        /// </summary>
        private readonly IServiceProvider _serviceProvider;

        /// <summary>
        /// 线程锁
        /// </summary>
        private static readonly object _locker = new object();

        /// <summary>
        /// 线程安全生产者字典容器
        /// </summary>
        private readonly ConcurrentDictionary<string, IProducer> _producerDic = new ConcurrentDictionary<string, IProducer>();

        /// <summary>
        /// 生产者属性列表
        /// </summary>
        public readonly List<ProducerAttribute> ProducerAttributes = new List<ProducerAttribute>();

        /// <summary>
        /// 日志
        /// </summary>
        public readonly ILogger<EventBusContainer> _logger;

        /// <summary>
        /// 注入客户端
        /// </summary>
        /// <param name="rabbitMQClient">客户端</param>
        /// <param name="serviceProvider"></param>
        public EventBusContainer(IRabbitMQClient rabbitMQClient, IServiceProvider serviceProvider)
        {
            Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、ConsumerManager 注入IRabbitMQClient，IRabbitMQEventBusContainer接口(注入IRabbitMQClient IServiceProvider)，所以在IRabbitMQClient被构造后{nameof(EventBusContainer)} (IRabbitMQEventBusContainer)才被构造 线程Id：【{Thread.CurrentThread.ManagedThreadId}】");
            _rabbitMQClient = rabbitMQClient;
            _serviceProvider = serviceProvider;
            _logger = serviceProvider.GetService<ILogger<EventBusContainer>>();
            _logger.LogWarning($"{{0}}", $"{CacheKeys.LogCount++}、{nameof(EventBusContainer)} 构造完毕 注入IRabbitMQClient IServiceProvider 线程Id：【{Thread.CurrentThread.ManagedThreadId}】");
        }

        /// <summary>
        /// 添加生产者属性到生产者属性列表 这个由其它模块启动时，注入IRabbitMQEventBusContainer接口后，调用AddProducer添加
        /// 一般在注册接口后程序build后，run之前abp模块一般在OnPreApplicationInitialization 应用程序启动前进行生产者注册配置
        /// 当然也可以在启动后的任何时候进行注册，只是建议模块启动时这么做
        /// </summary>
        /// <param name="producerAttribute">生产者属性</param>
        public void AddProducer(ProducerAttribute producerAttribute)
        {
            _logger.LogWarning($"{{0}}", $"{CacheKeys.LogCount++}、{nameof(EventBusContainer)} AddProducer 传入ProducerAttribute添加生产者 线程Id：【{Thread.CurrentThread.ManagedThreadId}】");
            if (ProducerAttributes.Any(x => x.GroupName == producerAttribute.GroupName && x.Exchange == producerAttribute.Exchange))
            {
                _logger.LogWarning($"{{0}}", $"{CacheKeys.LogCount++}、{nameof(EventBusContainer)} AddProducer GroupName与Exchange相同则认位是同一个Producer 此处{producerAttribute.GroupName}_{producerAttribute.Exchange}已被添加过，不再重复添加 线程Id：【{Thread.CurrentThread.ManagedThreadId}】");
                return;
            }

            ProducerAttributes.Add(producerAttribute);
        }

        /// <summary>
        /// 自动注册 生产/消费者
        /// </summary>
        /// <returns></returns>
        public async Task AutoRegister()
        {
            var assemblies = AssemblyHelper.GetAssemblies(_logger);
            await AutoRegister(assemblies.ToArray());
        }

        /// <summary>
        /// 根据程序集自动注册
        /// </summary>
        /// <param name="assemblies">需要注册生产/消费者的程序集</param>
        /// <returns></returns>
        public Task AutoRegister(Assembly[] assemblies)
        {
            _logger.LogWarning($"{{0}}", $"{CacheKeys.LogCount++}、{nameof(EventBusContainer)} AutoRegister");
            var consumers = new List<RabbitMQConsumer>();
            //扫描程序集中的 带生产者属性的 生产者 （producerGroupName生产者名或组名来区分生产者）
            var producerScanList = new List<(string producerGroupName, ProducerAttribute producerAttribute)>();
            foreach (var producerAttribute in ProducerAttributes)
            {
                producerScanList.Add((producerAttribute.GroupName.HasValue() ? $"{producerAttribute.GroupName}.{producerAttribute.Exchange}" : producerAttribute.Exchange, producerAttribute));
            }

            foreach (var type in assemblies.SelectMany(x => x.GetTypes()))
            {
                foreach (var attribute in type.GetCustomAttributes(false))
                {
                    if (attribute is ProducerAttribute producerAttribute)
                    {
                        producerScanList.Add((producerAttribute.GroupName.HasValue() ? $"{producerAttribute.GroupName}.{type.Name}" : type.Name, producerAttribute));
                        break;
                    }
                }

                foreach (var methodInfo in type.GetMethods())
                {
                    foreach (var attribute in methodInfo.GetCustomAttributes(false))
                    {
                        if (attribute is ProducerAttribute producerAttribute)
                        {
                            producerScanList.Add((producerAttribute.GroupName.HasValue() ? $"{producerAttribute.GroupName}.{methodInfo.Name}" : methodInfo.Name, producerAttribute));
                            break;
                        }
                    }
                }

                if (!type.IsAbstract && typeof(RabbitMQConsumer).IsAssignableFrom(type) && type.FullName != typeof(RabbitMQConsumer).FullName)
                {
                    consumers.Add((RabbitMQConsumer)Activator.CreateInstance(type, _serviceProvider));
                }
            }

            #region 先添加生产者，并注册交换机 多线程加锁
            lock (_locker)
            {
                foreach (var (producerName, producerAttribute) in producerScanList)
                {
                    //消费者sourceName绑定其ProduceAttribute上的 Exchange RouteKey
                    //可能sourceName不同就有多个 eventBus
                    //但consumer获取生产者时，按同一个exchange与routkey 只找其中一个eventBus
                    var eventBus = CreateEventBus(producerAttribute.Exchange ?? producerName, producerAttribute.RouteKey ?? producerName, producerAttribute.Type, producerAttribute.AutoAck, true, producerAttribute.Persistent).BindProducer(producerName);
                    eventBus.Enable();
                }
            }
            #endregion

            #region 再添加消费者，如果有新的eventBus，需要创建新的eventBus并未注册交换机  注意，消费者消费数据时才会去绑定队列到交换机上

            foreach (var consumer in consumers)
            {
                foreach (var queueInfo in consumer.QueueList)
                {
                    //同一个exchange与routkey 只找其中一个eventBus...
                    var eventBus = _eventBusList.FirstOrDefault(x => x.Exchange == consumer.EventBusExchange && x.RoutingKey == queueInfo.RoutingKey);
                    if (eventBus == null)
                    {
                        eventBus = CreateEventBus(consumer.EventBusExchange, queueInfo.RoutingKey, consumer.ExchangeType, consumer.Config.AutoAck, consumer.Config.Requeue, consumer.Persistent).BindProducer(consumer.ProducerName ?? consumer.GetType().FullName);
                        eventBus.Enable();
                    }
                    eventBus.AddConsumer(consumer);
                }
            }
            #endregion

            return Task.CompletedTask;
        }

        /// <summary>
        /// 获取消费者集合
        /// </summary>
        /// <returns>消费者集合</returns>
        public List<IConsumer> GetConsumers()
        {
            _logger.LogWarning($"{{0}}", $"{CacheKeys.LogCount++}、ConsumerManager start 后会调用{nameof(EventBusContainer)} GetConsumers,获取消费者集合 线程Id：【{Thread.CurrentThread.ManagedThreadId}】");
            var result = new List<IConsumer>();
            foreach (var eventBus in _eventBusList)
            {
                result.AddRange(eventBus.Consumers);
            }
            return result;
        }

        /// <summary>
        /// 创建生产消费者管理器
        /// </summary>
        /// <param name="exchange">交换机 字符串</param>
        /// <param name="routingKey">路由键 字符串</param>
        /// <param name="type"> 交换数据类型，字符串
        ///     direct(直接交换模式:它会把消息路由到那些binding key与routing key完全匹配的Queue中, routing(binding) key 最大长度 255 bytes)
        ///     fanout(广播模式:它会把所有发送到该Exchange的消息路由到所有与它绑定的Queue中，所以不管你的生产者端的bingding key 和 消费者端的routing key)
        ///     topic(主题模式：支持路由规则，routing key中可以含  .*/.#  【.号用于分隔单词,*匹配一个单词、#匹配多个单词】)
        ///     header(根据发送的消息内容中的headers属性进行匹配,在绑定Queue与Exchange时指定一组键值对以及x-match参数，x-match参数是字符串类型，
        ///     可以设置为any或者all。如果设置为any，意思就是只要匹配到了headers表中的任何一对键值即可，all则代表需要全部匹配)
        /// </param>
        /// <param name="autoAck">是否自动应答 bool</param>
        /// <param name="requeue">消费失败是否重新入队 bool</param>
        /// <param name="persistent">是否持久化 bool</param>
        /// <returns>RabbitMQEventBus</returns>
        public RabbitMQEventBus CreateEventBus(string exchange, string routingKey, string type = RabbitMQ.Client.ExchangeType.Direct, bool autoAck = false, bool requeue = true, bool persistent = false)
        {
            _logger.LogWarning($"{{0}}", $"{CacheKeys.LogCount++}、{nameof(EventBusContainer)} CreateEventBus new一个 RabbitMQEventBus 线程Id：【{Thread.CurrentThread.ManagedThreadId}】");
            return new RabbitMQEventBus(this, exchange, routingKey, type, autoAck, requeue, persistent);
        }

        /// <summary>
        /// 通道代理声明交换机 ExchangeDeclare
        /// </summary>
        /// <param name="eventBus">RabbitMQEventBus生产者-消费者容器</param>
        public void Work(RabbitMQEventBus eventBus)
        {
            Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、{nameof(EventBusContainer)} Work 容器/生产者 注册交换机 线程Id：【{Thread.CurrentThread.ManagedThreadId}】");
            if (_eventBusDictionary.TryAdd(eventBus.ProducerName, eventBus))
            {
                _eventBusList.Add(eventBus);
                using var modelWrapper = _rabbitMQClient.PullModel();
                _logger.LogWarning($"{{0}}", $"{CacheKeys.LogCount++}、{nameof(EventBusContainer)} Work【通道封装类modelWrapper的通道代理声明交换机 ExchangeDeclare{eventBus.Exchange}_{eventBus.Type}(durable:true)】 线程Id：【{Thread.CurrentThread.ManagedThreadId}】");
                modelWrapper.Channel.ExchangeDeclare(exchange: eventBus.Exchange, type: eventBus.Type, durable: true);
            }
        }

        /// <summary>
        /// 多生产者注册交换机
        /// 注意一定要exchange name不一样的生产者 不然会重复声明交换机
        /// </summary>
        /// <param name="eventBuses">RabbitMQEventBus生产者-消费者容器 集合</param>
        public void Works(List<RabbitMQEventBus> eventBuses)
        {
            foreach (var eventBus in eventBuses)
            {
                Work(eventBus);
            }
        }

        /// <summary>
        /// 根据生产者类型获取生产者
        /// </summary>
        /// <typeparam name="T">生产者类型</typeparam>
        /// <returns>IProducer(RabbitMQProducer)</returns>
        public IProducer GetProducer<T>()
        {
            return GetProducer(typeof(T));
        }

        /// <summary>
        /// 根据生产者类型获取生产者
        /// </summary>
        /// <param name="type">生产者类型</param>
        /// <returns>IProducer(RabbitMQProducer)</returns>
        public IProducer GetProducer(Type type)
        {
            return GetProducer(type.FullName);
        }

        /// <summary>
        /// 根据生产者类型名/注册名称 获取生产者
        /// </summary>
        /// <param name="name">生产者名称</param>
        /// <returns>IProducer(RabbitMQProducer)</returns>
        public IProducer GetProducer(string name)
        {
            if (_eventBusDictionary.TryGetValue(name, out var eventBus))
            {
                Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、{nameof(EventBusContainer)} GetProducer方法 没有找到生产者就会new 一个RabbitMQProducer返回,并把新容器{nameof(EventBusContainer)}添加到字典，下次直接可从字典里获取生产者 线程Id：【{Thread.CurrentThread.ManagedThreadId}】");
                return _producerDic.GetOrAdd(name, key => new RabbitMQProducer(_rabbitMQClient, eventBus));
            }

            throw new NotImplementedException($"{nameof(IProducer)} of {name}");
        }

        /// <summary>
        /// 获取生产者
        /// </summary>
        /// <param name="groupName">生产者分组名</param>
        /// <param name="name">生产者名称</param>
        /// <returns>IProducer(RabbitMQProducer)</returns>
        public IProducer GetProducer(string groupName, string name)
        {
            name = $"{groupName}.{name}";
            return GetProducer(name);
        }
    }
}
