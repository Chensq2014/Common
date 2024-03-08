using Microsoft.Extensions.DependencyInjection;
using Common.RabbitMQModule.Channels;
using Common.RabbitMQModule.Channels.Abstractions;
using Common.RabbitMQModule.Client;
using Common.RabbitMQModule.Consumers;
using Common.RabbitMQModule.Core;
using Common.RabbitMQModule.Producers;
using System;
using System.Threading;
using Common.RabbitMQModule.Tests;
using Common.Storage;
using Serilog;

namespace Common.RabbitMQModule.Extensions
{
    /// <summary>
    /// 提供本模块(RabbitMQModule) 接口注入、生产者Publish消息等静态方法
    /// </summary>
    public static class ServiceExtensions
    {
        /// <summary>
        /// RabbitMQModule RabbitMQ接口注册、配置项
        /// </summary>
        /// <param name="services"></param>
        public static void AddRabbitMQ(this IServiceCollection services)
        {
            Log.Information($"{{0}}", $"{CacheKeys.LogCount++}、1、开始配置 RabbitMQOptions、IMpscChannel、RabbitMQClient、EventBusContainer、ConsumerManager....ConfigureServices中的流程日志 线程Id：【{Thread.CurrentThread.ManagedThreadId}】");
            services.AddRabbitMQ(options =>
            {
                Log.Information($"{{0}}", $"{CacheKeys.LogCount++}、5、AddRabbitMQ执行后的委托(自定义扩展的委托)，这里可覆盖 RabbitMQ Options配置(流程之前的所有配置均可覆盖) 依赖注入替换服务等 配置自定义{nameof(RabbitMQOptions)}---hostBuilder.Build()--> BuildCommonServices--执行所有ConfigureServices委托时)....如果这个委托是子线程，日志靠后正常 ConfigureServices中的{options.GetType().Name}委托日志 线程Id：【{Thread.CurrentThread.ManagedThreadId}】");
            });
            Log.Information($"{{0}}", $"{CacheKeys.LogCount++}、4、配置完毕，配置IRabbitMQEventBusContainer、IProducerContainer 接口单例均为EventBusContainer....线程Id：【{Thread.CurrentThread.ManagedThreadId}】");
        }

        /// <summary>
        /// RabbitMQModule RabbitMQ接口注册、配置项,额外重载一个委托(参数为RabbitMQOptions配置)
        /// </summary>
        /// <param name="services"></param>
        /// <param name="rabbitConfigAction"></param>
        public static void AddRabbitMQ(this IServiceCollection services, Action<RabbitMQOptions> rabbitConfigAction)
        {
            Log.Information($"{{0}}", $"{CacheKeys.LogCount++}、2、执行AddRabbitMQ，这里初始化 RabbitMQ Options配置....ConfigureServices中的流程日志，按顺序执行的【注册多生产者单消费者泛型通道：IMpscChannel<>、注册RabbitMQ客户端：IRabbitMQClient、注册事件总线/生产者：IRabbitMQEventBusContainer/IProducerContainer (事件总线和生产者接口均由EventBusContainer类实现单例注入)、定时运行 消费者管理器：ConsumerManager、添加日志注册等】ConfigureServices中的流程日志 线程Id：【{Thread.CurrentThread.ManagedThreadId}】");
            var configration = services.GetConfiguration();
            services.Configure<RabbitMQOptions>(configration.GetSection(Magics.RABBITMQOPTIONS));
            ////注册自定义测试接口 按照abp的命名规范来可以自动依赖注入
            //services.AddTransient<IMQTest, MQTest>();

            //注册多生产者单消费者泛型通道
            services.AddTransient(typeof(IMpscChannel<>), typeof(MpscChannel<>));
            //注册RabbitMQ客户端
            services.AddSingleton<IRabbitMQClient, RabbitMQClient>();
            //注册事件总线/生产者-消费者容器/管理器
            services.AddSingleton<IRabbitMQEventBusContainer, EventBusContainer>();
            services.AddSingleton<IProducerContainer, EventBusContainer>();
            //EventBusContainer同时成为两个接口的单例
            //EventBusContainer继承了IRabbitMQEventBusContainer:IConsumerContainer(继承消费者容器),IProducerContainer接口(生产者容器) 
            //services.AddSingleton(serviceProvider => serviceProvider.GetService<IRabbitMQEventBusContainer>() as IProducerContainer);
            //定时运行 消费者管理器
            services.AddHostedService<ConsumerManager>();
            //services.AddLogging();//netcore框架已经默认配置了
            services.Configure(rabbitConfigAction);//(所有模块ConfigureServices结束之后,hostBuilder.Build()后)才执行这个委托(参数)

            Log.Information($"{{0}}", $"{CacheKeys.LogCount++}、3、执行AddRabbitMQ初始化完毕....ConfigureServices中的流程日志 线程Id：【{Thread.CurrentThread.ManagedThreadId}】");
        }
    }
}
