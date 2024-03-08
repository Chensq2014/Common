using Parakeet.NetCore.Dtos;
using Parakeet.NetCore.Entities;
using Parakeet.NetCore.RabbitMQModule.Core;
using System;

namespace Parakeet.NetCore.Consumer.Chongqing.PersistentModule.Consumers
{
    /// <summary>
    /// 考勤持久化消费者
    /// </summary>
    public class GatePersistentConsumer : DataCenterConsumer<ChongqingPersistentModule, GateBase, GateRecordDto>
    {
        public GatePersistentConsumer(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        protected override QueueInfo QueueInfo => new QueueInfo
        {
            Queue = "test",
            RoutingKey = "test"
        };
        protected override string Exchange => "Gate";
    }
}
