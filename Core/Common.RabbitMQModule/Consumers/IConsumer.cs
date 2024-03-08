using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Common.RabbitMQModule.Core;

namespace Common.RabbitMQModule.Consumers
{
    /// <summary>
    /// 消费者接口
    /// </summary>
    public interface IConsumer
    {

        /// <summary>
        /// 消息通知
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="tag"></param>
        /// <returns></returns>
        Task Notice(string bytes, ulong tag);

        /// <summary>
        /// 消息通知列表
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        Task Notice(List<(string bytes, ulong tag)> list);

        /// <summary>
        /// 添加handler
        /// </summary>
        /// <param name="handler"></param>
        /// <param name="handlerBatch"></param>
        void AddHandler(Func<(string bytes, ulong tag), Task> handler, Func<List<(string bytes, ulong tag)>, Task> handlerBatch);
        
        /// <summary>
        /// 消费者类全名
        /// </summary>
        string Name { get; }

        /// <summary>
        /// 交换类型  RabbitMQ.Client.ExchangeType
        /// direct(直接交换模式:它会把消息路由到那些binding key与routing key完全匹配的Queue中, routing(binding) key 最大长度 255 bytes)
        /// fanout(广播模式:它会把所有发送到该Exchange的消息路由到所有与它绑定的Queue中，所以不管你的生产者端的bingding key 和 消费者端的routing key)
        /// topic(主题模式：支持路由规则，routing key中可以含 */#*匹配一个单词、#匹配多个单词)
        /// header(根据发送的消息内容中的headers属性进行匹配,在绑定Queue与Exchange时指定一组键值对以及x-match参数，x-match参数是字符串类型，
        /// 可以设置为any或者all。如果设置为any，意思就是只要匹配到了headers表中的任何一对键值即可，all则代表需要全部匹配)
        /// </summary>
        string ExchangeType { get; set; }

        /// <summary>
        /// 路由键
        /// </summary>
        string RouteKey { get; }

        /// <summary>
        /// 接收事件 暂未使用
        /// </summary>

        event EventHandler<DeliverEventArgs> OnReceived;
    }
}
