namespace Common.RabbitMQModule.Core
{
    /// <summary>
    /// Consumer配置项信息
    /// </summary>
    public class ConsumerOptions
    {
        /// <summary>
        /// 是否自动ack 【ack:acknowledg】 应答 Delivery Acknowledgements 交货确认
        /// </summary>
        public bool AutoAck { get; set; }

        /// <summary>
        /// 消息处理失败是否重回队列还是不停重发
        /// </summary>
        public bool Requeue { get; set; }
    }
}
