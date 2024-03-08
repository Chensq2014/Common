using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Threading;
using Common.RabbitMQModule.Core;
using Common.Storage;
using Serilog;

namespace Common.RabbitMQModule.Client
{
    /// <summary>
    /// 连接池管理器 ConnectionWrapper是IConnection 链接封装扩展类
    /// (含通道代理扩展类(ModelWrapper)集合)
    /// </summary>
    public class ConnectionWrapper
    {
        /// <summary>
        /// 通道代理扩展类集合
        /// </summary>
        private readonly List<ModelWrapper> _models = new List<ModelWrapper>();

        /// <summary>
        /// RabbitMQ.Client.dll 获取数据连接接口
        /// </summary>
        protected IConnection Connection;

        /// <summary>
        /// System.Threading.SemaphoreSlim  控制线程并发数 只允许同时1个线程调用
        /// </summary>
        private readonly SemaphoreSlim _semaphoreSlim = new SemaphoreSlim(1);

        /// <summary>
        /// 日志
        /// </summary>
        private readonly ILogger<ConnectionWrapper> _logger;

        /// <summary>
        /// 是否关闭
        /// </summary>
        public bool IsClose { get; set; }

        /// <summary>
        /// 是否已连接
        /// </summary>
        public bool IsConnected => (Connection?.IsOpen ?? false) && !IsClose;

        /// <summary>
        /// RabbitMQOptions 从配置文件中读取
        /// </summary>
        public RabbitMQOptions Options { get; }

        /// <summary>
        /// 创建连接对象池管理器
        /// </summary>
        /// <param name="connection">RabbitMQ.Client.IConnection 连接对象</param>
        /// <param name="options">RabbitMQOptions配置文件配置</param>
        /// <param name="serviceProvider">serviceProvider框架容器接口提供器</param>
        public ConnectionWrapper(IConnection connection, RabbitMQOptions options, IServiceProvider serviceProvider)
        {
            Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、{nameof(ConnectionWrapper)} 被构造，注入IConnection(RabbitMQ.Client.dll 获取数据连接接口) IServiceProvider RabbitMQOptions配置 线程Id：【{Thread.CurrentThread.ManagedThreadId}】");
            Connection = connection;
            Connection.ConnectionShutdown += OnConnectionShutdown;
            Connection.CallbackException += OnCallbackException;
            Connection.ConnectionBlocked += OnConnectionBlocked;
            _logger = serviceProvider.GetService<ILogger<ConnectionWrapper>>();
            Options = options;

            _logger.LogWarning($"{{0}}", $"{CacheKeys.LogCount++}、{nameof(ConnectionWrapper)} 构造完毕，注入IConnection(RabbitMQ.Client.dll 获取数据连接接口) IServiceProvider RabbitMQOptions配置 线程Id：【{Thread.CurrentThread.ManagedThreadId}】");
        }

        /// <summary>
        /// 析构函数 同时把当前连接dispose掉
        /// </summary>
        ~ConnectionWrapper()
        {
            Connection.Dispose();
        }

        /// <summary>
        /// 从ConnectionWrapper(连接对象池管理器)中获取一个通道代理  ModelWrapper(ConnectionWrapper(Connection))
        /// </summary>
        /// <returns></returns>
        public (bool success, ModelWrapper model) Get()
        {
            _semaphoreSlim.Wait();
            try
            {
                if (_models.Count < Options.PoolSizePerConnection)
                {
                    if (!Connection.IsOpen)
                    {
                        return (false, default);
                    }
                    _logger.LogWarning($"{{0}}", $"{CacheKeys.LogCount++}、new一个{nameof(ModelWrapper)} 传入参数connectionWrapper：当前ConnectionWrapper IModle:Connection.CreateModel() 线程Id：【{Thread.CurrentThread.ManagedThreadId}】");
                    //channel=this.Connection.CreateModel() 连接上创建通道代理
                    var model = new ModelWrapper(this, Connection.CreateModel());
                    _models.Add(model);
                    return (true, model);
                }
            }
            finally
            {
                _semaphoreSlim.Release();
            }
            return (false, default);
        }

        /// <summary>
        /// 从通道代理扩展类(ModelWrapper)集合中移除当前ModelWrapper RabbitMQ.Client.dll (IModel)channel 通道代理扩展类
        /// </summary>
        /// <param name="model">RabbitMQ.Client.dll (IModel)channel 通道代理扩展类</param>
        public void Return(ModelWrapper model)
        {
            _models.Remove(model);
        }

        /// <summary>
        /// 连接中断事件
        /// </summary>
        /// <param name="sender">事件的承载者，当前连接connection</param>
        /// <param name="eventArgs">ConnectionBlockedEventArgs</param>
        private void OnConnectionBlocked(object sender, ConnectionBlockedEventArgs eventArgs)
        {
            _logger.LogWarning($"A RabbitMQ connection is blocked. Trying to re-connect...{eventArgs.Reason} 线程Id：【{Thread.CurrentThread.ManagedThreadId}】");
            IsClose = true;
        }

        /// <summary>
        /// 连接异常事件
        /// </summary>
        /// <param name="sender">事件的承载者，当前连接connection</param>
        /// <param name="eventArgs">CallbackExceptionEventArgs</param>
        private void OnCallbackException(object sender, CallbackExceptionEventArgs eventArgs)
        {
            _logger.LogWarning($"A RabbitMQ connection throw exception. Trying to re-connect...{eventArgs.Exception.Message} 线程Id：【{Thread.CurrentThread.ManagedThreadId}】");
            IsClose = true;
        }

        /// <summary>
        /// 连接关闭事件
        /// </summary>
        /// <param name="sender">事件的承载者，当前连接connection</param>
        /// <param name="eventArgs">ShutdownEventArgs</param>
        private void OnConnectionShutdown(object sender, ShutdownEventArgs eventArgs)
        {
            _logger.LogWarning($"A RabbitMQ connection is on shutdown. Trying to re-connect...{eventArgs.ReplyText} 线程Id：【{Thread.CurrentThread.ManagedThreadId}】");
            IsClose = true;
        }
    }
}