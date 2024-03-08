using System;
using Common.Storage;
using Serilog;

namespace Common.RabbitMQModule.CustomExceptions
{
    /// <summary>
    /// 自定义重复绑定消费者错误
    /// </summary>
    public class RebindConsumerException : Exception
    {
        public RebindConsumerException(string message) : base($"重复绑定消费者:{message}")
        {
            Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、{nameof(RebindConsumerException)} 重复绑定消费者:{message}");
        }
    }
}