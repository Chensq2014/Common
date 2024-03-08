using System;
using RabbitMQ.Client;
using System.Collections.Generic;
using System.Linq;

namespace Common.RabbitMQModule.Core
{
    /// <summary>
    /// RabbitMQ消息队列配置项
    /// 1、4369 (epmd), 25672 (Erlang distribution)
    ///     Epmd 是 Erlang Port Mapper Daemon 的缩写，在 Erlang 集群中相当于 dns 的作用，绑定在4369端口上
    /// 2、5672, 5671 (AMQP 0-9-1 without and with TLS)
    ///     AMQP 是 Advanced Message Queuing Protocol(高级消息队列协议)的缩写，一个提供统一消息服务的应用层标准高级消息队列协议，
    ///     是应用层协议的一个开放标准，专为面向消息的中间件设计。基于此协议的客户端与消息中间件之间可以传递消息，
    ///     并不受客户端/中间件不同产品、不同的开发语言等条件的限制。Erlang 中的实现有 RabbitMQ 等。
    /// 3、15672 (if management plugin is enabled)
    ///    通过 http://serverip:15672 访问 RabbitMQ 的 Web 管理界面，默认用户名密码都是 guest
    /// 4、61613, 61614 (if STOMP is enabled)
    ///     Stomp 是一个简单的消息文本协议，它的设计核心理念就是简单与可用性，官方文档，实践一下 Stomp 协议需要：
    ///     一个支持 stomp 消息协议的 messaging server(譬如activemq，rabbitmq）；
    ///     一个终端（譬如linux shell);一些基本命令与操作（譬如nc，telnet)
    ///5、1883, 8883 (if MQTT is enabled)
    ///     MQTT 只是 IBM 推出的一个消息协议，基于 TCP/IP 的。两个 App 端发送和接收消息需要中间人
    ///     这个中间人就是消息服务器（比如ActiveMQ/RabbitMQ），三者通信协议就是 MQTT
    ///
    /// AMQP专业术语：（多路复用->在同一个线程中开启多个通道进行操作）
    /// Server：又称broker，接受客户端的链接，实现AMQP实体服务
    /// Connection:连接，应用程序与broker的网络连接
    /// Channel:网络信道，几乎所有的操作都在channel中进行，Channel是进行消息读写的通道。
    ///          客户端可以建立多个channel，每个channel代表一个会话任务。
    /// Message:消息，服务器与应用程序之间传送的数据，由Properties和Body组成.Properties可以对消息进行修饰，
    ///          必须消息的优先级、延迟等高级特性；Body则是消息体内容     。
    /// virtualhost: 虚拟地址，用于进行逻辑隔离，最上层的消息路由。一个virtual host里面可以有若干个Exchange和Queue，
    ///              同一个Virtual Host 里面不能有相同名称的  ///E xchange 或 Queue。
    /// Exchange：交换机，接收消息，根据路由键转单消息到绑定队列
    /// Binding: Exchange和Queue之间的虚拟链接，binding中可以包换routing key
    /// Routing key: 一个路由规则，虚拟机可用它来确定如何路由一个特定消息。（如负载均衡）
    /// </summary>
    public class RabbitMQOptions
    {
        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Vhost 虚拟主机名
        /// 虚拟地址，用于进行逻辑隔离，最上层的消息路由。一个virtual host里面可以有若干个Exchange和Queue，
        /// 同一个Virtual Host 里面不能有相同名称的Exchange 或 Queue。
        /// </summary>
        public string VirtualHost { get; set; }

        /// <summary>
        /// 连接池数
        /// </summary>
        public int PoolSizePerConnection { get; set; } = 200;

        /// <summary>
        /// 最大连接数
        /// </summary>
        public int MaxConnection { get; set; } = 20;

        /// <summary>
        /// 消费者批量处理每次处理的最大消息量
        /// </summary>
        public ushort ConsumerMaxBatchSize { get; set; } = 500;

        /// <summary>
        /// 消费者批量处理每次处理的最大延时 ms
        /// </summary>
        public int ConsumerMaxMillisecondsInterval { get; set; } = 1000;

        /// <summary>
        /// 主机数组 这个居然要带端口 默认5672(注意并不是浏览器连接管理端口15672)
        /// 通过 http://serverip:15672 访问 RabbitMQ 的 Web 管理界面，默认用户名密码都是 guest
        /// 5672, 5671 端口(AMQP 0-9-1 without and with TLS)是AMQP连接端口
        /// AMQP是 Advanced Message Queuing Protocol 的缩写，
        /// 一个提供统一消息服务的应用层标准高级消息队列协议，是应用层协议的一个开放标准，专为面向消息的中间件设计。
        /// 基于此协议的客户端与消息中间件之间可以传递消息，并不受客户端/中间件不同产品、不同的开发语言等条件的限制。Erlang 中的实现有 RabbitMQ 等
        /// </summary>
        public string[] Hosts { get; set; }

        /// <summary>
        ///  5672, 5671 端口(AMQP 0-9-1 without and with TLS)是AMQP连接端口
        /// </summary>
        public int? Port { get; set; } = 5672;

        /// <summary>
        /// 交换机名称
        /// </summary>
        public string Exchange { get; set; }

        /// <summary>
        /// 交换类型
        /// direct(直接交换模式:它会把消息路由到那些binding key与routing key完全匹配的Queue中, routing(binding) key 最大长度 255 bytes)
        /// fanout(广播模式:它会把所有发送到该Exchange的消息路由到所有与它绑定的Queue中，所以不管你的生产者端的bingding key 和 消费者端的routing key)
        /// topic(主题模式：支持路由规则，routing key中可以含  .*/.#  【.号用于分隔单词,*匹配一个单词、#匹配多个单词】)
        /// header(根据发送的消息内容中的headers属性进行匹配,在绑定Queue与Exchange时指定一组键值对以及x-match参数，x-match参数是字符串类型，
        /// 可以设置为any或者all。如果设置为any，意思就是只要匹配到了headers表中的任何一对键值即可，all则代表需要全部匹配)
        /// </summary>
        public string ExchangeType { get; set; }

        /// <summary>
        /// 请求心跳频率
        /// </summary>
        public ushort RequestedHeartbeat { get; set; } = 60;

        /// <summary>
        /// 是否支持自动恢复
        /// </summary>
        public bool AutomaticRecoveryEnabled { get; set; } = true;

        /// <summary>
        /// 路由健
        /// </summary>
        public string RoutingKey { get; set; }

        /// <summary>
        /// 队列名
        /// </summary>
        public string QueueName { get; set; }

        /// <summary>
        /// 节点Id，广播消息时,区分集群队列
        /// </summary>
        public string NodeId { get; set; }

        /// <summary>
        /// amqp协议 tcp协议的终结点
        /// </summary>
        public List<AmqpTcpEndpoint> EndPoints
        {
            get
            {
                var list = new List<AmqpTcpEndpoint>();
                foreach (var host in Hosts)
                {
                    if (host.Contains(":"))
                    {
                        list.Add(AmqpTcpEndpoint.Parse(host));
                        //hostName = host.Split(":").First();
                        //Port??= int.Parse(host.Split(":").Last());
                    }
                    else
                    {
                        list.Add(new AmqpTcpEndpoint
                        {
                            HostName = host,
                            Port = Port ?? 5672
                        });
                    }
                }
                return list;
            }
        }
    }
}
