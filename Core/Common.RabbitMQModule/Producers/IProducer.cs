using System.Threading.Tasks;
using RabbitMQ.Client;

namespace Common.RabbitMQModule.Producers
{
    /// <summary>
    /// 生产者接口 在需要生产者的模块去注册
    /// </summary>
    public interface IProducer
    {
        /// <summary>
        /// 直接异步发布消息
        /// </summary>
        /// <param name="bytes">二进制数组</param>
        /// <returns></returns>
        Task PublishAsync(byte[] bytes);

        /// <summary>
        /// 异步发布消息队列 指定routeKey
        /// </summary>
        /// <param name="bytes">二进制数组</param>
        /// <param name="routeKey">路由键值</param>
        /// <returns></returns>
        Task PublishAsync(byte[] bytes, string routeKey);

        /// <summary>
        /// 发布到消息队列
        /// </summary>
        /// <param name="data">数据对象</param>
        /// <param name="routeKey">路由默认null</param>
        /// <returns></returns>
        Task PublishAsync(object data, string routeKey = null);

        /// <summary>
        /// 异步发布消息队列 指定routeKey、exchange、persistent
        /// </summary>
        /// <param name="bytes">二进制数组</param>
        /// <param name="routeKey">路由键</param>
        /// <param name="exchange">交换机</param>
        /// <param name="persistent">是否持久化默认true</param>
        /// <returns></returns>

        Task PublishAsync(byte[] bytes, string routeKey, string exchange, bool persistent = true);

        /// <summary>
        /// 生产者添加消息到消息队列
        /// </summary>
        /// <param name="data">数据对象</param>
        /// <param name="routeKey">路由健值</param>
        /// <param name="exchange">交换机</param>
        /// <param name="persistent">是否持久化默认true</param>
        /// <returns></returns>
        Task PublishAsync(object data, string routeKey, string exchange,
            bool persistent = true);

        /// <summary>
        /// 生产者声明交换机
        /// </summary>
        /// <param name="exchange">交换机</param>
        /// <param name="type">交换数据类型</param>
        /// <param name="durable">是否持久化</param>
        void ExchangeDeclare(string exchange, string type = ExchangeType.Direct, bool durable = true);
    }
}