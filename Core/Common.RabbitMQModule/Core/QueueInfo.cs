using System.Collections.Generic;

namespace Common.RabbitMQModule.Core
{
    /// <summary>
    /// 队列信息
    /// </summary>
    public class QueueInfo
    {
        /// <summary>
        /// 队列名
        /// </summary>
        public string Queue { get; set; }

        /// <summary>
        /// 交换数据的路由键值
        /// </summary>
        public string RoutingKey { get; set; }

        /// <summary>
        /// 交换机信息
        /// </summary>
        public string Exchange { get; set; }

        /// <summary>
        /// 队列参数
        /// </summary>
        public Dictionary<string, object> Arguments { get; set; } = new Dictionary<string, object>();

        /// <summary>
        /// 队列_路由键_交换机
        /// </summary>
        /// <returns></returns>

        public override string ToString()
        {
            return $"{Queue}_{RoutingKey}_{Exchange}";
        }
    }
}
