using System.Threading.Tasks;

namespace Common.RabbitMQModule.Channels.Abstractions
{
    /// <summary>
    /// 多生产者单消费者(基于管道代理)基础接口（multi producer single consumer）
    /// </summary>
    public interface IBaseMpscChannel
    {
        /// <summary>
        /// 是否是子级消费者
        /// </summary>
        bool IsChildren { get; set; }

        /// <summary>
        /// 是否已经完成
        /// </summary>
        bool IsComplete { get; }

        /// <summary>
        /// 把一个mpscchannel消费者关联到另外一个mpscchannel消费者，
        /// 只要有消息进入，所有关联的消费者都会顺序的进行消息检查和处理
        /// </summary>
        /// <param name="mpscChannel"></param>
        void JoinConsumerSequence(IBaseMpscChannel mpscChannel);

        /// <summary>
        /// 检查当前管道和子管道中是否还有可以进行消费的数据
        /// </summary>
        /// <returns>是否有数据待消费</returns>
        Task<bool> WaitToReadAsync();

        /// <summary>
        /// 常规消费 调用消费者消费数据的委托
        /// </summary>
        /// <returns></returns>
        Task ManualConsume();

        /// <summary>
        /// 完毕及联动操作 相关子通道代理 缓冲区_buffer.Complete及联动操作
        /// </summary>
        void Complete();
    }
}