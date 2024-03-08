using Microsoft.Extensions.ObjectPool;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Common.RabbitMQModule.Core;
using Common.Storage;
using Serilog;

namespace Common.RabbitMQModule.Client
{
    /// <summary>
    /// ModelPooledObjectPolicy对象池策略 
    /// </summary>
    public class ModelPooledObjectPolicy : IPooledObjectPolicy<ModelWrapper>
    {
        /// <summary>
        /// RabbitMQ.Client 连接管理工厂(由构造函数传入)
        /// </summary>
        private readonly ConnectionFactory _connectionFactory;

        /// <summary>
        /// 对象池管理器容器集合【ConnectionWrapper是IConnection 链接封装扩展类】
        /// </summary>
        private readonly List<ConnectionWrapper> _connections = new List<ConnectionWrapper>();

        /// <summary>
        /// 并发数 允许1个线程调用，当1个线程进入就开启线程锁
        /// </summary>
        private readonly SemaphoreSlim _semaphoreSlim = new SemaphoreSlim(1,1);

        /// <summary>
        /// RabbitMQOptions配置
        /// </summary>
        private readonly RabbitMQOptions _options;

        /// <summary>
        /// 接口提供器
        /// </summary>
        private readonly IServiceProvider _serviceProvider;

        /// <summary>
        /// 构造函数 ModelPooledObjectPolicy
        /// </summary>
        /// <param name="connectionFactory">ConnectionFactory</param>
        /// <param name="options">RabbitMQOptions</param>
        /// <param name="serviceProvider">serviceProvider</param>
        public ModelPooledObjectPolicy(ConnectionFactory connectionFactory, RabbitMQOptions options, IServiceProvider serviceProvider)
        {
            Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、{nameof(ModelPooledObjectPolicy)} 被构造 线程Id：【{Thread.CurrentThread.ManagedThreadId}】");
            _connectionFactory = connectionFactory;
            _options = options;
            _serviceProvider = serviceProvider;
            Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、{nameof(ModelPooledObjectPolicy)} 构造完毕 线程Id：【{Thread.CurrentThread.ManagedThreadId}】");
        }

        /// <summary>
        /// 从对象池连接ConnectionWrapper 返回ModelWrapper
        /// </summary>
        /// <returns>ModelWrapper</returns>
        public ModelWrapper Create()
        {
            Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、{nameof(ModelPooledObjectPolicy)} Create 创建返回一个对象池连接ModelWrapper 线程Id：【{Thread.CurrentThread.ManagedThreadId}】");
            foreach (var connection in _connections)
            {
                var (success, model) = connection.Get();
                if (success)
                {
                    return model;
                }
            }
            _semaphoreSlim.Wait();
            try
            {
                if (_connections.Count > _options.MaxConnection)
                {
                    var closeConnections = _connections.RemoveAll(x => !x.IsConnected);
                    _serviceProvider.GetService<ILogger<ModelPooledObjectPolicy>>().LogInformation($"清除已关闭的连接池数：closeConnections count {closeConnections} 线程Id：【{Thread.CurrentThread.ManagedThreadId}】");
                }

                var connection = new ConnectionWrapper(_connectionFactory.CreateConnection(_options.EndPoints), _options, _serviceProvider);
                var (success, model) = connection.Get();
                _connections.Add(connection);
                return success ? model : throw new OverflowException(nameof(model));
            }
            finally
            {
                _semaphoreSlim.Release();
            }
        }

        /// <summary>
        /// 从对象池中回收/移除ModelWrapper通道代理
        /// </summary>
        /// <param name="modelWrapper">ModelWrapper</param>
        /// <returns>bool是否移除成功</returns>
        public bool Return(ModelWrapper modelWrapper)
        {
            if (modelWrapper.Channel.IsOpen && !modelWrapper.Channel.IsClosed)
            {
                Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、{nameof(ModelWrapper)} Model为打开状态，禁止回收 线程Id：【{Thread.CurrentThread.ManagedThreadId}】");
                return false;
            }

            Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、{nameof(ModelWrapper)} Model为关闭状态，强制回收 线程Id：【{Thread.CurrentThread.ManagedThreadId}】");
            modelWrapper.ForceDispose();
            return true;
        }
    }
}