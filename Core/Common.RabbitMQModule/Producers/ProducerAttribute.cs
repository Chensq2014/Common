using System;
using Common.Storage;
using Serilog;

namespace Common.RabbitMQModule.Producers
{
    /// <summary>
    /// 标记生产者,被标记的类或方法将被定义为生产者，其类名或方法将作为生产者的名称
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public class ProducerAttribute : Attribute
    {
        /// <summary>
        /// 初始化生产者属性
        /// </summary>
        /// <param name="groupName">生产者组名称</param>
        /// <param name="exchange">mq交换器名称，如果不填写默认为该attribute所绑定的类名或方法名称</param>
        /// <param name="routingKey">mq的路由Key,如果不填写默认为该attribute所绑定的类名或方法名称</param>
        /// <param name="persistent">是否持久化 默认true</param>
        /// <param name="autoAck">是否自动确认消息被消费 默认false(false:消费失败的空闲时再自动重新消费)</param>
        /// <param name="type"> RabbitMQ.Client.ExchangeType 类型
        ///     direct(直接交换模式:它会把消息路由到那些binding key与routing key完全匹配的Queue中, routing(binding) key 最大长度 255 bytes)
        ///     fanout(广播模式:它会把所有发送到该Exchange的消息路由到所有与它绑定的Queue中，所以不管你的生产者端的bingding key 和 消费者端的routing key)
        ///     topic(主题模式：支持路由规则，routing key中可以含 */#*匹配一个单词、#匹配多个单词)
        ///     header(根据发送的消息内容中的headers属性进行匹配,在绑定Queue与Exchange时指定一组键值对以及x-match参数，x-match参数是字符串类型，
        ///     可以设置为any或者all。如果设置为any，意思就是只要匹配到了headers表中的任何一对键值即可，all则代表需要全部匹配)
        /// </param>
        public ProducerAttribute(string groupName, string exchange, string routingKey = null, string type = RabbitMQ.Client.ExchangeType.Direct, bool persistent = true, bool autoAck = false)
        {
            Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、{nameof(ProducerAttribute)} 初始化生产者属性: GroupName:{groupName}  Exchange:{exchange}  RouteKey:{routingKey} Type:{type} Persistent:{persistent} AutoAck:{autoAck}");
            GroupName = groupName;
            Exchange = exchange;
            RouteKey = routingKey;
            Type = type;
            Persistent = persistent;
            AutoAck = autoAck;
        }

        /// <summary>
        /// 初始化生产者属性
        /// </summary>
        /// <param name="area"></param>
        /// <param name="deviceType"></param>
        /// <param name="exchange">mq交换器名称，如果不填写默认为该attribute所绑定的类名或方法名称</param>
        /// <param name="routingKey">mq的路由Key,如果不填写默认为该attribute所绑定的类名或方法名称</param>
        /// <param name="persistent">是否持久化 默认true</param>
        /// <param name="autoAck">是否自动确认消息被消费 默认false(false:消费失败的空闲时再自动重新消费)</param>
        /// <param name="type"> RabbitMQ.Client.ExchangeType 类型
        ///     direct(直接交换模式:它会把消息路由到那些binding key与routing key完全匹配的Queue中, routing(binding) key 最大长度 255 bytes)
        ///     fanout(广播模式:它会把所有发送到该Exchange的消息路由到所有与它绑定的Queue中，所以不管你的生产者端的bingding key 和 消费者端的routing key)
        ///     topic(主题模式：支持路由规则，routing key中可以含 */#*匹配一个单词、#匹配多个单词)
        ///     header(根据发送的消息内容中的headers属性进行匹配,在绑定Queue与Exchange时指定一组键值对以及x-match参数，x-match参数是字符串类型，
        ///     可以设置为any或者all。如果设置为any，意思就是只要匹配到了headers表中的任何一对键值即可，all则代表需要全部匹配)
        /// </param>
        public ProducerAttribute(string area, int deviceType, string exchange, string routingKey = null, string type = RabbitMQ.Client.ExchangeType.Direct, bool persistent = true, bool autoAck = false)
        {
            Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、{nameof(ProducerAttribute)} 初始化生产者属性:area:{area} deviceType:{deviceType}  Exchange:{exchange}  RouteKey:{routingKey} Type:{type} Persistent:{persistent} AutoAck:{autoAck}");
            GroupName = $"{area}.{deviceType}";
            Exchange = exchange;
            RouteKey = routingKey;
            Type = type;
            Persistent = persistent;
            AutoAck = autoAck;
        }


        /// <summary>
        /// 分组名称
        /// </summary>
        public string GroupName { get; set; }

        /// <summary>
        /// 交换机
        /// </summary>
        public string Exchange { get; }

        /// <summary>
        /// 路由键
        /// </summary>
        public string RouteKey { get; }

        /// <summary>
        /// 消息队列类型 RabbitMQ.Client.ExchangeType 类型
        /// direct(直接交换模式:它会把消息路由到那些binding key与routing key完全匹配的Queue中, routing(binding) key 最大长度 255 bytes)
        /// fanout(广播模式:它会把所有发送到该Exchange的消息路由到所有与它绑定的Queue中，所以不管你的生产者端的bingding key 和 消费者端的routing key)
        /// topic(主题模式：支持路由规则，routing key中可以含 */#*匹配一个单词、#匹配多个单词)
        /// header(根据发送的消息内容中的headers属性进行匹配,在绑定Queue与Exchange时指定一组键值对以及x-match参数，x-match参数是字符串类型，
        /// 可以设置为any或者all。如果设置为any，意思就是只要匹配到了headers表中的任何一对键值即可，all则代表需要全部匹配)
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// 自动确认消费消息
        /// </summary>
        public bool AutoAck { get; set; }

        /// <summary>
        /// 是否持久化
        /// </summary>

        public bool Persistent { get; set; }
    }
}