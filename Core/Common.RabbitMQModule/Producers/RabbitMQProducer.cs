using Common.RabbitMQModule.Client;
using Common.RabbitMQModule.Core;
using System;
using System.Threading;
using System.Threading.Tasks;
using Common.Extensions;
using Common.Storage;
using RabbitMQ.Client;
using Serilog;

namespace Common.RabbitMQModule.Producers
{
    /// <summary>
    /// 实现生产者接口IProducer   包装一层RabbitMQEventBus(RabbitMQ生产者，消费者绑定管理类)
    /// </summary>
    public class RabbitMQProducer : IProducer
    {
        /// <summary>
        /// 生产者名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// RabbitMQ单生产者多消费者绑定管理类
        /// </summary>
        private readonly RabbitMQEventBus _eventBus;

        /// <summary>
        /// RabbitMQClient 客户端
        /// </summary>
        public IRabbitMQClient RabbitMQClient { get; }

        /// <summary>
        /// RabbitMQ单生产者，消费者绑管理类
        /// </summary>
        /// <param name="rabbitMQClient">RabbitMQClient 客户端</param>
        /// <param name="eventBus"> RabbitMQ单生产者多消费者绑定管理类</param>
        public RabbitMQProducer(
            IRabbitMQClient rabbitMQClient,
            RabbitMQEventBus mqEventBus)
        {
            Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、{nameof(RabbitMQProducer)} 被构造 传入IRabbitMQClient RabbitMQEventBus 线程Id：【{Thread.CurrentThread.ManagedThreadId}】");
            _eventBus = mqEventBus;
            Name = mqEventBus.ProducerName;
            RabbitMQClient = rabbitMQClient;
        }

        /// <summary>
        /// RabbitMQ生产者 包装一层RabbitMQEventBus(RabbitMQ生产者，消费者绑定管理类)
        /// </summary>
        /// <param name="rabbitMQClient">RabbitMQClient 客户端连接 通道代理</param>
        public RabbitMQProducer(IRabbitMQClient rabbitMQClient)
        {
            Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、{nameof(RabbitMQProducer)} 被构造 传入IRabbitMQClient 线程Id：【{Thread.CurrentThread.ManagedThreadId}】");
            RabbitMQClient = rabbitMQClient;
        }

        /// <summary>
        /// push二进制消息到交换机异步
        /// </summary>
        /// <param name="bytes">二进制数组</param>
        /// <returns></returns>
        public Task PublishAsync(byte[] bytes)
        {
            if (_eventBus == null)
            {
                throw new ArgumentNullException($"{nameof(RabbitMQEventBus)}不能为null");
            }

            Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、{nameof(RabbitMQProducer)} RabbitMQClient.PullModel()获取通道代理封装类{nameof(ModelWrapper)},然后再使用通道代理封装类获取通道代理封装类{nameof(ModelWrapper)} Publish 线程Id：【{Thread.CurrentThread.ManagedThreadId}】");
            using var model = RabbitMQClient.PullModel();
            model.Publish(bytes, _eventBus.Exchange, _eventBus.RoutingKey, _eventBus.Persistent);
            return Task.CompletedTask;
        }

        /// <summary>
        /// push二进制消息到指定routeKey交换机 异步
        /// </summary>
        /// <param name="bytes">二进制数组</param>
        /// <param name="routingKey">路由键</param>
        /// <returns></returns>
        public Task PublishAsync(byte[] bytes, string routingKey)
        {
            if (_eventBus == null)
            {
                throw new ArgumentNullException($"{nameof(RabbitMQEventBus)}不能为null");
            }

            using var model = RabbitMQClient.PullModel();
            model.Publish(bytes, _eventBus.Exchange, routingKey ?? _eventBus.RoutingKey, _eventBus.Persistent);
            return Task.CompletedTask;
        }

        /// <summary>
        /// 发布到消息队列
        /// </summary>
        /// <param name="data">object数据对象</param>
        /// <param name="routingKey">路由键默认null</param>
        /// <returns></returns>
        public Task PublishAsync(object data, string routingKey = null)
        {
            //var json = data.Serialize();
            //return Encoding.UTF8.GetBytes(json);
            var body = data.SerializeToUtf8Bytes();//System.Text.Json 的Json序列化
            return PublishAsync(body, routingKey);
        }

        /// <summary>
        /// push消息到指定交换机routKey并默认存储 
        /// </summary>
        /// <param name="bytes">二进制数组</param>
        /// <param name="routeKey">路由键</param>
        /// <param name="exchange">交换机</param>
        /// <param name="persistent">是否持久化默认true</param>
        /// <returns></returns>
        public Task PublishAsync(byte[] bytes, string routeKey, string exchange, bool persistent = true)
        {
            using var model = RabbitMQClient.PullModel();
            model.Publish(bytes, exchange, routeKey ?? _eventBus.RoutingKey, persistent);
            return Task.CompletedTask;
        }

        /// <summary>
        /// 生产者添加消息到消息队列
        /// </summary>
        /// <param name="data">数据对象</param>
        /// <param name="routeKey">路由健值</param>
        /// <param name="exchange">交换机</param>
        /// <param name="persistent">是否持久化默认true</param>
        /// <returns></returns>
        public Task PublishAsync(object data, string routeKey, string exchange, bool persistent = true)
        {
            var body = data.SerializeToUtf8Bytes();//System.Text.Json 的Json序列化
            return PublishAsync(body, routeKey, exchange, persistent);
        }


        /// <summary>
        /// 生产者声明交换机
        /// </summary>
        /// <param name="exchange">交换机</param>
        /// <param name="type">交换数据类型</param>
        /// <param name="durable">是否持久化</param>
        public void ExchangeDeclare(string exchange, string type = ExchangeType.Direct, bool durable = true)
        {
            using var model = RabbitMQClient.PullModel();
            model.Channel.ExchangeDeclare(exchange: exchange, type: type, durable: durable);
        }
    }
}
