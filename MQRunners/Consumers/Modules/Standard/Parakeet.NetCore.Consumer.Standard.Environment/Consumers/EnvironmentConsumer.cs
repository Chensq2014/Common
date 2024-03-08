using Microsoft.Extensions.DependencyInjection;
using Parakeet.NetCore.Consumer.Standard.Interfaces;
using Parakeet.NetCore.Dtos;
using Parakeet.NetCore.RabbitMQModule.Core;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Parakeet.NetCore.Consumer.Standard.EnvironmentModule.Consumers
{
    /// <summary>
    ///standard模块 环境消费者
    /// </summary>
    public class EnvironmentConsumer : ForwardConsumer<EnvironmentRecordDto>
    {
        private readonly IEnvironmentRecordHttpForward _httpForward;
        public EnvironmentConsumer(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            _httpForward = serviceProvider.GetRequiredService<IEnvironmentRecordHttpForward>();
            _httpForward.Init(source =>
            {
                source.HttpMethod = HttpMethod.Post;
                source.HttpClientName = nameof(EnvironmentModule);
            });
        }

        protected override QueueInfo QueueInfo => new QueueInfo
        {
            Queue = "test",
            RoutingKey = "test"
        };
        protected override string Exchange => "environment";
        protected override async Task EventProcess(WrapperData<EnvironmentRecordDto> wrapperData)
        {
            await _httpForward.Push(wrapperData);
        }

        protected override async Task BatchEventProcess(List<WrapperData<EnvironmentRecordDto>> wrapperDataList)
        {
            await _httpForward.BatchPush(wrapperDataList);
        }
    }
}
