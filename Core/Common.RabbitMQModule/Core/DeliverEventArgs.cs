using System;
using System.Threading;
using Common.Storage;
using Serilog;

namespace Common.RabbitMQModule.Core
{
    /// <summary>
    /// 自定义系统分发事件类 二进制
    /// Delivery 送货，DeliverEventArgs 【交货涉及的事件】
    /// 参考：BasicDeliverEventArgs(这才是标准的rabbimq 分发事件)
    /// </summary>
    public class DeliverEventArgs : EventArgs
    {
        /// <summary>
        /// 自定义系统分发事件类 二进制
        /// </summary>
        public DeliverEventArgs()
        {
            Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、{nameof(DeliverEventArgs)} 默认构造函数 被构造 线程Id：【{Thread.CurrentThread.ManagedThreadId}】");
        }

        /// <summary>
        /// 自定义系统分发事件类 二进制
        /// </summary>
        /// <param name="body"></param>
        public DeliverEventArgs(string body)
        {
            Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、{nameof(DeliverEventArgs)} 被构造 自定义系统分发事件类 二进制字符串 线程Id：【{Thread.CurrentThread.ManagedThreadId}】");
            Body = body;
        }

        /// <summary>
        /// 二进制Body
        /// </summary>
        public string Body { get; set; }
    }
}
