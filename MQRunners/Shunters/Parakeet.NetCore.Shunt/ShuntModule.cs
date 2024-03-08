using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Parakeet.NetCore.EntityFrameworkCore;
using Parakeet.NetCore.Extensions;
using Parakeet.NetCore.RabbitMQModule.Client;
using Parakeet.NetCore.RabbitMQModule.Core;
using Parakeet.NetCore.RabbitMQModule.Producers;
using Parakeet.NetCore.Storage;
using Serilog;
using Volo.Abp;
using Volo.Abp.AspNetCore;
using Volo.Abp.Autofac;
using Volo.Abp.AutoMapper;
using Volo.Abp.Modularity;

namespace Parakeet.NetCore.Shunt
{
    /// <summary>
    /// 分流公共模块
    /// </summary>
    [DependsOn(typeof(NetCoreEntityFrameworkCoreModule),
        typeof(AbpAspNetCoreModule),
        typeof(AbpAutoMapperModule),
        typeof(AbpAutofacModule))]
    public class ShuntModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(ShuntModule)} Start  ConfigureServices ....");
            var configuration = context.Services.GetConfiguration();
            Configure<AbpAutoMapperOptions>(options => { options.AddMaps<ShuntModule>(true); });
            var shuntMQOption = context.Services.ConfigureSingleton(configuration.GetSection(Magics.SHUNTRABBITMQOPTIONS),new RabbitMQOptions());
            context.Services.AddSingleton<IProducer>(provider =>
            {
                var client = new RabbitMQClient(new OptionsWrapper<RabbitMQOptions>(shuntMQOption), provider);
                return new ShuntProducer(client);//shunt模块的生产者  注册为单例唯一
            });
            base.ConfigureServices(context);
            Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(ShuntModule)} End  ConfigureServices ....");
        }

        public override void OnApplicationInitialization(ApplicationInitializationContext context)
        {
            Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(ShuntModule)} Start  OnApplicationInitialization ....");

            var app = context.GetApplicationBuilder();
            var env = context.GetEnvironment();
            var configuration = context.GetConfiguration();
            //AsyncHelper.RunSync(async () =>
            //{
            //    //缓存预热
            //    //await context.ServiceProvider.WarmUpDevice();
            //    //await context.ServiceProvider.WarmUpPacketHandler();
            //    await Task.CompletedTask;
            //});
            base.OnApplicationInitialization(context);
            Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(ShuntModule)} End  OnApplicationInitialization ....");
        }
    }
}
