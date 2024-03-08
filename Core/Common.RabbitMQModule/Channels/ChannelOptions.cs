namespace Common.RabbitMQModule.Channels
{
    /// <summary>
    /// 管道代理消费者消费数据数据量配置项
    /// </summary>
    public class ChannelOptions
    {
        /// <summary>
        /// 批量数据处理每次处理的最大数据量
        /// </summary>
        public int MaxBatchSize { get; set; } = 100000;

        /// <summary>
        /// 批量数据接收的最大延时
        /// </summary>
        public int MaxMillisecondsDelay { get; set; } = 1000;
    }
}