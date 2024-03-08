using Microsoft.Extensions.ObjectPool;
using RabbitMQ.Client;
using System;
using System.Threading;
using Common.Storage;
using Serilog;

namespace Common.RabbitMQModule.Client
{
    /// <summary>
    /// 封装通道代理类ModelWrapper(含连接对象池管理器 ConnectionWrapper是IConnection 链接封装扩展类)
    /// RabbitMQ.Client.dll (IModel)channel 通道代理 由连接对象IConnection 创建
    /// </summary>
    public class ModelWrapper : IDisposable
    {
        /// <summary>
        /// RabbitMQ.Client 标记消息是否持久化基本属性
        /// </summary>
        private readonly IBasicProperties _persistentProperties;
        private readonly IBasicProperties _noPersistentProperties;

        /// <summary>
        /// Microsoft.Extensions.ObjectPool.dll 对象缓存池
        /// 用于ModelWrapper(IModel-Channel)的重用，节省实例化ModelWrapper(IModel-Channel)的开销
        /// </summary>
        public DefaultObjectPool<ModelWrapper> Pool { get; set; }

        /// <summary>
        /// 连接对象池管理器 ConnectionWrapper是IConnection 链接封装扩展类
        /// </summary>
        public ConnectionWrapper ConnectionWrapper { get; set; }

        /// <summary>
        /// RabbitMQ.Client.dll (IModel)channel 通道代理
        /// used to scope the lifetime of a channel when appropriate
        /// </summary>
        public IModel Channel { get; set; }

        /// <summary>
        /// 封装通道代理类ModelWrapper(含连接对象池管理器 ConnectionWrapper是IConnection 链接封装扩展类)
        /// RabbitMQ.Client.dll (IModel)channel 通道代理 由连接对象IConnection 创建
        /// </summary>
        /// <param name="connectionWrapper">是IConnection 链接封装扩展类</param>
        /// <param name="model">(IModel)channel通道代理</param>
        public ModelWrapper(ConnectionWrapper connectionWrapper, IModel model)
        {
            Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、{nameof(ModelWrapper)} 被构造 线程Id：【{Thread.CurrentThread.ManagedThreadId}】");
            ConnectionWrapper = connectionWrapper;
            Channel = model;
            _persistentProperties = Channel.CreateBasicProperties();
            _persistentProperties.Persistent = true;
            _noPersistentProperties = Channel.CreateBasicProperties();
            _noPersistentProperties.Persistent = false;
            Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、{nameof(ModelWrapper)} 构造完毕 线程Id：【{Thread.CurrentThread.ManagedThreadId}】");
        }

        /// <summary>
        /// publish数据到 指定交换机exchange,指定routekey 所有生产者发布消息 都会经过这个Publish方法
        /// </summary>
        /// <param name="msg">消息(字节数组)</param>
        /// <param name="exchange">交换机 字符串</param>
        /// <param name="routingKey">路由键 字符串</param>
        /// <param name="persistent">是否持久化 bool</param>
        public void Publish(byte[] msg, string exchange, string routingKey, bool persistent = true)
        {
            Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、{nameof(ModelWrapper)} Publish方法，通道代理最终在这里BasicPublish发布消息到 【指定交换机_路由键:{exchange}_{routingKey}】,指定routekey 线程Id：【{Thread.CurrentThread.ManagedThreadId}】");
            Channel.BasicPublish(exchange, routingKey, persistent ? _persistentProperties : _noPersistentProperties, msg);
        }

        #region Dispose

        /// <summary>
        /// 强制关闭
        /// </summary>
        public void ForceDispose()
        {
            Channel.Close();
            Channel.Dispose();
            //从连接管理器的通道代理扩展类(ModelWrapper)集合中移除当前通道代理ModelWrapper
            ConnectionWrapper.Return(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                Pool.Return(this);
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }

        #endregion Dispose
    }
}