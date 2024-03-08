using Microsoft.Extensions.ObjectPool;
using Microsoft.Extensions.Options;
using Common.RabbitMQModule.Core;
using Common.Storage;
using RabbitMQ.Client;
using Serilog;
using System;
using System.Threading;

namespace Common.RabbitMQModule.Client
{
    /// <summary>
    /// RabbitMQ客户端:实现IRabbitMQClient接口
    /// 读取配置文件 根据连接配置创建MQ连接，缓存连接池，通道代理
    /// </summary>
    public class RabbitMQClient : IRabbitMQClient
    {
        /// <summary>
        /// 默认通道代理封装类(ModelWrapper)缓存 Microsoft.Extensions.ObjectPool.dll 
        /// </summary>
        private readonly DefaultObjectPool<ModelWrapper> _pool;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="config">RabbitMQOptions 配置文件账号连接配置</param>
        /// <param name="serviceProvider">serviceProvider 框架接口提供器</param>
        public RabbitMQClient(IOptions<RabbitMQOptions> config, IServiceProvider serviceProvider)
        {
            Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、ConsumerManager HostService 后台自动启动时，依赖注入了IRabbitMQClient接口，所以这里{nameof(RabbitMQClient)} 被首先构造,根据RabbitMQOptions配置 new一个连接工厂ConnectionFactory,初始化对象池 DefaultObjectPool<ModelWrapper> 线程Id：【{Thread.CurrentThread.ManagedThreadId}】");

            var options = config.Value;
            var connectionFactory = new ConnectionFactory
            {
                //HostName = options.Hosts.FirstOrDefault(),
                //Port = options.Port ?? 5672,//默认端口就是5672
                UserName = options.UserName,
                Password = options.Password,
                VirtualHost = options.VirtualHost,
                AutomaticRecoveryEnabled = false//自动recovery
            };
            _pool = new DefaultObjectPool<ModelWrapper>(new ModelPooledObjectPolicy(connectionFactory, options, serviceProvider));
            Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、{nameof(RabbitMQClient)} 构造完毕,根据RabbitMQOptions配置 new一个连接工厂ConnectionFactory,初始化对象池 DefaultObjectPool<ModelWrapper>(new ModelPooledObjectPolicy(connectionFactory, options, serviceProvider)) 线程Id：【{Thread.CurrentThread.ManagedThreadId}】");
        }

        /// <summary>
        /// 获取ModelWrapper:RabbitMQ.Client.dll (IModel)channel 通道代理 由连接对象IConnection 创建
        /// </summary>
        /// <returns>ModelWrapper:RabbitMQ.Client.dll (IModel)channel 通道代理 由连接对象IConnection 创建</returns>
        public ModelWrapper PullModel()
        {
            Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、{nameof(RabbitMQClient)}  DefaultObjectPool<ModelWrapper>执行PullModel获取对象池ModelWrapper 线程Id：【{Thread.CurrentThread.ManagedThreadId}】");

            ModelWrapper modelWrapper;
            bool invalid;
            do
            {
                modelWrapper = _pool.Get();//从对象缓存池中获取 ModelWrapper(Channel通道代理扩展类)
                if (modelWrapper.Pool == null)
                {
                    modelWrapper.Pool = _pool;
                }

                if (modelWrapper.Channel.IsClosed || !modelWrapper.Channel.IsOpen)
                {
                    invalid = true;
                    modelWrapper.ForceDispose();
                }
                else
                {
                    invalid = false;
                }
            } while (invalid);

            return modelWrapper;
        }
    }
}