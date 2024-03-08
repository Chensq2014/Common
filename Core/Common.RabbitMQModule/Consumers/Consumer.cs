using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Common.RabbitMQModule.Core;
using Common.Storage;
using Serilog;

namespace Common.RabbitMQModule.Consumers
{
    /// <summary>
    /// 消费者基类
    /// </summary>
    public abstract class Consumer : IConsumer
    {
        /// <summary>
        /// 单条事件处理 消费者委托这条线上一直会调用到此委托事件
        /// </summary>
        protected readonly List<Func<(string, ulong), Task>> EventHandlers = new List<Func<(string, ulong), Task>>();

        /// <summary>
        /// 批量事件处理 消费者委托这条线上一直会调用到此委托事件
        /// </summary>
        protected readonly List<Func<List<(string, ulong)>, Task>> BatchEventHandlers = new List<Func<List<(string, ulong)>, Task>>();

        /// <summary>
        /// 初始化消费者
        /// </summary>
        protected Consumer()
        {
            Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、{nameof(Consumer)}  Consumer消费者默认构造函数 什么都没干 线程Id：【{Thread.CurrentThread.ManagedThreadId}】...");
        }

        /// <summary>
        /// 初始化消费者指定处理程序与批量事件处理程序
        /// </summary>
        /// <param name="eventHandlers">单条事件处理委托集合</param>
        /// <param name="batchEventHandlers">批量事件处理委托集合</param>
        protected Consumer(List<Func<(string, ulong), Task>> eventHandlers, List<Func<List<(string, ulong)>, Task>> batchEventHandlers)
        {
            Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、{nameof(Consumer)}  Consumer消费者私有构造函数，添加单/批量 处理委托 线程Id：【{Thread.CurrentThread.ManagedThreadId}】");
            EventHandlers.AddRange(eventHandlers);
            BatchEventHandlers.AddRange(batchEventHandlers);
            Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、{nameof(Consumer)}  Consumer消费者私有构造函数 构造完毕 线程Id：【{Thread.CurrentThread.ManagedThreadId}】");
        }

        /// <summary>
        /// 添加handler 留给子类扩展
        /// </summary>
        /// <param name="handler">单条事件处理委托</param>
        /// <param name="handlerBatch">批量事件处理委托</param>
        public void AddHandler(Func<(string, ulong), Task> handler, Func<List<(string, ulong)>, Task> handlerBatch)
        {
            Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、{nameof(Consumer)}  消费者 添加自定义handler(事件处理委托) 线程Id：【{Thread.CurrentThread.ManagedThreadId}】");
            EventHandlers.Add(handler);
            BatchEventHandlers.Add(handlerBatch);
        }

        /// <summary>
        /// 消息通知【由Process方法调用】
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="tag"></param>
        /// <returns></returns>
        public Task Notice(string bytes, ulong tag)
        {
            Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、{nameof(Consumer)}  消费者 Notice消息通知,顺序执行EventHandlers委托集合 线程Id：【{Thread.CurrentThread.ManagedThreadId}】");
            return Task.WhenAll(EventHandlers.Select(func => func((bytes, tag))));
        }

        /// <summary>
        /// 消息通知列表
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public Task Notice(List<(string, ulong)> list)
        {
            Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、{nameof(Consumer)}  消费者 Notice消息通知 顺序执行BatchEventHandlers委托集合 线程Id：【{Thread.CurrentThread.ManagedThreadId}】");
            return Task.WhenAll(BatchEventHandlers.Select(func => func(list)));
        }

        /// <summary>
        /// 接收消息自定义事件  暂未使用，留给子类扩展联动逻辑
        /// </summary>
        public event EventHandler<DeliverEventArgs> OnReceived;

        /// <summary>
        /// 消费者类全名
        /// </summary>
        public virtual string Name => GetType().FullName;

        /// <summary>
        /// 交换类型  RabbitMQ.Client.ExchangeType
        /// direct(直接交换模式:它会把消息路由到那些binding key与routing key完全匹配的Queue中, routing(binding) key 最大长度 255 bytes)
        /// fanout(广播模式:它会把所有发送到该Exchange的消息路由到所有与它绑定的Queue中，所以不管你的生产者端的bingding key 和 消费者端的routing key)
        /// topic(主题模式：支持路由规则，routing key中可以含 .*/.# 【*匹配一个单词、#匹配多个单词】)
        /// header(根据发送的消息内容中的headers属性进行匹配,在绑定Queue与Exchange时指定一组键值对以及x-match参数，x-match参数是字符串类型，
        /// 可以设置为any或者all。如果设置为any，意思就是只要匹配到了headers表中的任何一对键值即可，all则代表需要全部匹配)
        /// </summary>
        public virtual string ExchangeType { get; set; } = RabbitMQ.Client.ExchangeType.Direct;

        /// <summary>
        /// 路由键
        /// </summary>
        public abstract string RouteKey { get; }

        /// <summary>
        /// 处理程序 暂未使用，留给子类扩展逻辑
        /// </summary>
        /// <param name="body"></param>

        protected virtual void Handler(string body)
        {
            Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、{nameof(Consumer)}  消费者 Handler处理 触发当前自定义Consumer类的接收数据事件：OnReceived?.InvokeSafely  线程Id：【{Thread.CurrentThread.ManagedThreadId}】");
            OnReceived?.InvokeSafely(this, new DeliverEventArgs(body));
        }
    }
}
