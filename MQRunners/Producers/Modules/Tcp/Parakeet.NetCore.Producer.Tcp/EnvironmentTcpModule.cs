using Microsoft.Extensions.DependencyInjection;
using Parakeet.NetCore.Entities;
using Parakeet.NetCore.Producer.Tcp.Commands;
using Parakeet.NetCore.RabbitMQModule.Core;
using Parakeet.NetCore.Storage;
using Parakeet.NetCore.TcpMudule;
using Parakeet.NetCore.TcpMudule.Interfaces;
using Serilog;
using Volo.Abp;
using Volo.Abp.Modularity;

namespace Parakeet.NetCore.Producer.Tcp
{
    [DependsOn(typeof(ProducerModule),
        typeof(TcpCoreMudule))]
    public class EnvironmentTcpModule : AbpModule
    {
        public override void PreConfigureServices(ServiceConfigurationContext context)
        {
            Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(EnvironmentTcpModule)} Start  PreConfigureServices ....");

            context.Services.AddScoped<IPacketHandler, Handlers.PacketHandler>();
            context.Services.AddScoped<ICommand, PacketParseCommand>();
            //context.Services.AddScoped<ICommand, PacketUploadCommand>();
            context.Services.AddScoped<ICommand, TimeSyncCommand>();
            context.Services.AddScoped<ICommand, UnknownCommand>();

            base.PreConfigureServices(context);
            Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(EnvironmentTcpModule)} End  PreConfigureServices ....");
        }

        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(EnvironmentTcpModule)} Start  ConfigureServices ....");
            //var configuration = context.Services.GetConfiguration();
            //context.Services.AddHttpApi<IImageApi>(options =>
            //{
            //    options.HttpHost = new Uri(configuration.GetValue<string>("HttpOptions:ImageUrl"));
            //});
            //context.Services.BuildServiceProvider();

            base.ConfigureServices(context);
            Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(EnvironmentTcpModule)} End  ConfigureServices ....");
        }



        public override void OnApplicationInitialization(ApplicationInitializationContext context)
        {
            Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(EnvironmentTcpModule)} Start  OnApplicationInitialization ....");
            var app = context.GetApplicationBuilder();
            var env = context.GetEnvironment();
            var configuration = context.GetConfiguration();

            Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、 获取IRabbitMQEventBusContainer 然后AutoRegister 自动注册并绑定当前模块的生产/消费者....");
            var rabbitEventBusContainer = context.ServiceProvider.GetService<IRabbitMQEventBusContainer>();
            rabbitEventBusContainer.AutoRegister(new[] { typeof(EnvironmentTcpModule).Assembly });

            Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(EnvironmentTcpModule)} End  OnApplicationInitialization ....");
        }
    }
}
