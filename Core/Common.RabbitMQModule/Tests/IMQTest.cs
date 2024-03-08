using RabbitMQ.Client;
using Volo.Abp.DependencyInjection;

namespace Common.RabbitMQModule.Tests
{
    /// <summary>
    /// MQ的测试接口 注入容器 AbpVnext module里面的实现类的抽象接口只要继承了 ITransientDependency 接口
    /// 且抽象接口与实现类按照IService与Service规范命名即可自动依赖注入
    /// 命名规范：接口IService  实现类[XXX]Service 以Service结尾即可
    /// IServvice必须继承ITransientDependency/ISingletonDependency/IScopedDependency
    /// </summary>
    public interface IMQTest : IScopedDependency
    {
        /// <summary>
        /// 获取连接
        /// </summary>
        /// <returns>RabbitMQ.Client.IConnection</returns>
        IConnection GetConnection();
    }
}
