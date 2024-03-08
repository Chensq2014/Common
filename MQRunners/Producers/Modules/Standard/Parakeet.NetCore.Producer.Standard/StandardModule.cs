using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Parakeet.NetCore.EntityFrameworkCore.Repositories;
using Parakeet.NetCore.Interfaces;
using Parakeet.NetCore.Producer;
using Parakeet.NetCore.RabbitMQModule.Core;
using Parakeet.NetCore.Storage;
using Serilog;
using Volo.Abp;
using Volo.Abp.Modularity;

namespace Parakeet.NetCore.Producer.Standard
{
    /// <summary>
    /// standard公共模块
    /// </summary>
    [DependsOn(typeof(ProducerModule))]
    public class StandardModule : AbpModule
    {
        public override void PreConfigureServices(ServiceConfigurationContext context)
        {
            Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(StandardModule)} Start  PreConfigureServices ....");
            //PreConfigure<IMvcBuilder>(mvcBuilder =>
            //{
            //    mvcBuilder.AddApplicationPartIfNotExists(typeof(StandardModule).Assembly);
            //});
            base.PreConfigureServices(context);
            Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(StandardModule)} End  PreConfigureServices ....");
        }

        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(StandardModule)} Start  ConfigureServices ....");
            //var configuration = context.Services.GetConfiguration();
            //context.Services.AddHttpApi<IImageApi>(options =>
            //{
            //    options.HttpHost = new Uri(configuration.GetValue<string>("HttpOptions:ImageUrl"));
            //});
            //context.Services.BuildServiceProvider();
            base.ConfigureServices(context);
            Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(StandardModule)} End  ConfigureServices ....");
        }



        public override void OnApplicationInitialization(ApplicationInitializationContext context)
        {
            Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(StandardModule)} Start  OnApplicationInitialization ....");
            var app = context.GetApplicationBuilder();
            var env = context.GetEnvironment();
            var configuration = context.GetConfiguration();

            Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、 获取IRabbitMQEventBusContainer 然后AutoRegister 自动注册并绑定当前模块的生产/消费者....");
            var rabbitEventBusContainer = context.ServiceProvider.GetService<IRabbitMQEventBusContainer>();
            rabbitEventBusContainer.AutoRegister(new[] { typeof(StandardModule).Assembly });

            Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(StandardModule)} End  OnApplicationInitialization ....");
        }

    }
}
