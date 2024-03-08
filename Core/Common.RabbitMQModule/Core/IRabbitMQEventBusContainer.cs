using System.Collections.Generic;
using Common.RabbitMQModule.Consumers;
using Common.RabbitMQModule.Producers;
using System.Reflection;
using System.Threading.Tasks;

namespace Common.RabbitMQModule.Core
{
    /// <summary>
    /// 基于事件的RabbitMQ容器/生产者-消费者容器
    /// </summary>
    public interface IRabbitMQEventBusContainer : IConsumerContainer
    {
        /// <summary>
        /// 根据自定义生产者属性添加生产者
        /// </summary>
        /// <param name="producerAttribute">生产者属性</param>
        void AddProducer(ProducerAttribute producerAttribute);

        /// <summary>
        /// 自动注册 基于事件的RabbitMQ容器/生产者
        /// </summary>
        /// <returns></returns>
        Task AutoRegister();

        /// <summary>
        /// 程序集自动注册 基于事件的RabbitMQ容器/生产者
        /// </summary>
        /// <param name="assemblies">需要注册生产/消费者的程序集</param>
        /// <returns></returns>
        Task AutoRegister(Assembly[] assemblies);

        /// <summary>
        /// 创建基于事件的RabbitMQ容器/生产者
        /// </summary>
        /// <param name="exchange">交换机 字符串</param>
        /// <param name="routingKey">路由键 字符串</param>
        /// <param name="type">消息类型</param>
        /// <param name="autoAck">是否自动应答</param>
        /// <param name="requeue">消费失败是否重新入列</param>
        /// <param name="persistent">是否持久化</param>
        /// <returns>RabbitMQEventBus生产者-消费者容器</returns>
        RabbitMQEventBus CreateEventBus(string exchange, string routingKey, string type = RabbitMQ.Client.ExchangeType.Direct, bool autoAck = false, bool requeue = true, bool persistent = false);


        /// <summary>
        /// 通道代理声明交换机 ExchangeDeclare
        /// </summary>
        /// <param name="eventBus">RabbitMQEventBus生产者-消费者容器</param>
        void Work(RabbitMQEventBus eventBus);

        /// <summary>
        /// 通道代理声明交换机 ExchangeDeclare
        /// </summary>
        /// <param name="eventBuses">RabbitMQEventBus生产者-消费者容器集合</param>
        void Works(List<RabbitMQEventBus> eventBuses);
    }
}
