using System;
using Common.Storage;
using Serilog;

namespace Common.RabbitMQModule.CustomExceptions
{
    /// <summary>
    /// 自定义重复绑定生产者异常
    /// </summary>
    public class EventBusRepeatBindingProducerException : Exception
    {
        public EventBusRepeatBindingProducerException(string name) : base($"重复绑定生产者:{name}")
        {
            Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、{nameof(EventBusRepeatBindingProducerException)} 重复绑定生产者:{name}");
        }
    }
}