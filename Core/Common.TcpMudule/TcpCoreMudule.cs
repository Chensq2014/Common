using Common.Dtos.Tcp;
using Common.TcpMudule.Interfaces;
using Common.TcpMudule.Sockets;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp;
using Volo.Abp.Modularity;

namespace Common.TcpMudule
{
    /// <summary>
    /// RabbitMQ消息队列模块 公共类库模块
    /// </summary>
    [DependsOn(typeof(CommonSharedModule))]
    public class TcpCoreMudule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            var configuration = context.Services.GetConfiguration();
            context.Services.Configure<TcpOptionDto>(configuration.GetSection(Magics.TCPOPTIONS));
            context.Services.AddSingleton<ITcpServer, AsyncIOCPServer>();
            base.ConfigureServices(context);
        }

        public override void OnApplicationInitialization(ApplicationInitializationContext context)
        {
            base.OnApplicationInitialization(context);
        }
    }
}
