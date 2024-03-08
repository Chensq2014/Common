using System;
using Common.Storage;
using Serilog;

namespace Common.RabbitMQModule.CustomExceptions
{
    /// <summary>
    /// 自定义未绑定消费者处理数据委托错误消息
    /// </summary>
    public class NoBindConsumerException : Exception
    {
        public NoBindConsumerException(string message) : base($"未绑定消费者处理数据的委托:{message}")
        {
            Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、{nameof(NoBindConsumerException)} 未绑定消费者:{message}");
        }
    }
}