using Parakeet.NetCore.Dtos;
using Parakeet.NetCore.Entities;
using Parakeet.NetCore.RabbitMQModule.Core;
using System;

namespace Parakeet.NetCore.Consumer.Chongqing.PersistentModule.Consumers
{
    /// <summary>
    /// 环境持久化消费者
    /// </summary>
    public class EnvironmentPersistentConsumer : DataCenterConsumer<ChongqingPersistentModule, EnvironmentBase, EnvironmentRecordDto>
    {
        public EnvironmentPersistentConsumer(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        protected override QueueInfo QueueInfo => new QueueInfo
        {
            Queue = "test",
            RoutingKey = "test"
        };
        protected override string Exchange => "Environment";
    }
}
