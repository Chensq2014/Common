using System;

namespace Common.RabbitMQModule.Producers
{
    /// <summary>
    /// 获取生产者容器接口 提供给子模块 如：[Controller的构造函数依赖注入]
    /// </summary>
    public interface IProducerContainer
    {
        /// <summary>
        /// 获取生产者容器
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <returns>IProducer</returns>
        IProducer GetProducer<T>();

        /// <summary>
        /// 根据Type获取生产者容器
        /// </summary>
        /// <param name="type">类型Type</param>
        /// <returns>IProducer</returns>
        IProducer GetProducer(Type type);

        /// <summary>
        /// 根据名称获取生产者容器
        /// </summary>
        /// <param name="name">名称</param>
        /// <returns>IProducer</returns>
        IProducer GetProducer(string name);

        /// <summary>
        /// 根据分组名称与名称获取生产者容器
        /// </summary>
        /// <param name="groupName">分组名</param>
        /// <param name="name">名称</param>
        /// <returns>IProducer</returns>
        IProducer GetProducer(string groupName, string name);
    }
}