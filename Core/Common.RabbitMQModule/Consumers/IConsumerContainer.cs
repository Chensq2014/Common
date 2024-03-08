using System.Collections.Generic;

namespace Common.RabbitMQModule.Consumers
{
    /// <summary>
    /// 消费者容器接口(获取所有消费者)
    /// </summary>
    public interface IConsumerContainer
    {
        /// <summary>
        /// 获取所有消费者
        /// </summary>
        /// <returns></returns>
        List<IConsumer> GetConsumers();
    }
}
