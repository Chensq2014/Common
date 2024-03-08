namespace Common.RabbitMQModule.Client
{
    /// <summary>
    /// RabbitMQClient 客户端接口 可获取封装的通道代理类ModelWrapper
    /// 读取配置文件rabbimq配置 new一个connectionFactory工厂，根据工厂创建MQ连接，
    /// </summary>
    public interface IRabbitMQClient
    {
        /// <summary>
        /// 获取封装的RabbitMQ.Client.dll (IModel)channel 通道代理类ModelWrapper(含连接对象池管理器 ConnectionWrapper是IConnection 链接封装扩展类)
        /// </summary>
        /// <returns>ModelWrapper(RabbitMQ.Client.dll (IModel)channel 通道代理) </returns>
        ModelWrapper PullModel();
    }
}