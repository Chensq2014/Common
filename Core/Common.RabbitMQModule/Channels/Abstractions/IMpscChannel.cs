using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Common.RabbitMQModule.Channels.Abstractions
{
    /// <summary>
    /// 多生产者单消费者(基于管道代理)泛型接口（multi producer single consumer）
    /// </summary>
    public interface IMpscChannel<T> : IBaseMpscChannel
    {
        /// <summary>
        /// 绑定消费者处理方法
        /// </summary>
        /// <param name="consumerFunc">消费消息的委托</param>
        void BindConsumerFunc(Func<List<T>, Task> consumerFunc);

        /// <summary>
        /// 绑定消费者处理方法
        /// </summary>
        /// <param name="consumerFunc">消费消息的委托</param>
        /// <param name="maxBatchSize">批量数据处理每次处理的最大数据量</param>
        /// <param name="maxMillisecondsDelay">批量数据接收的最大延时</param>
        void BindConsumerFunc(Func<List<T>, Task> consumerFunc, int maxBatchSize, int maxMillisecondsDelay);

        /// <summary>
        /// 配置批量处理数据的参数
        /// </summary>
        /// <param name="maxBatchSize">批量数据处理每次处理的最大数据量</param>
        /// <param name="maxMillisecondsDelay">批量数据接收的最大延时</param>
        void Config(int maxBatchSize, int maxMillisecondsDelay);

        /// <summary>
        /// 将数据写入管道
        /// 该方法被添加到BasicConsumer.Received事件逻辑中
        /// </summary>
        /// <param name="data">eventArgs(BasicDeliverEventArgs)</param>
        /// <returns>写入状态 bool</returns>
        ValueTask<bool> WriteAsync(T data);
    }
}