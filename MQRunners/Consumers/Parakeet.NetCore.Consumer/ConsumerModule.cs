using System;
using Microsoft.Extensions.DependencyInjection;
using Parakeet.NetCore.Devices.Dtos;
using Parakeet.NetCore.EntityFrameworkCore;
using Parakeet.NetCore.RabbitMQModule;
using Parakeet.NetCore.Storage;
using Serilog;
using Volo.Abp;
using Volo.Abp.AspNetCore;
using Volo.Abp.Autofac;
using Volo.Abp.AutoMapper;
using Volo.Abp.Modularity;

namespace Parakeet.NetCore.Consumer
{
    /// <summary>
    /// 公共消费者模块 引用NetCoreEntityFrameworkCoreModule 是为了使用EFCore Repository
    /// </summary>
    [DependsOn(
        typeof(CustomRabbitMQModule),
        typeof(AbpAspNetCoreModule)
        , typeof(NetCoreEntityFrameworkCoreModule),
        typeof(AbpAspNetCoreModule),
        typeof(AbpAutoMapperModule),
        typeof(AbpAutofacModule))]
    public class ConsumerModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(ConsumerModule)} Start  ConfigureServices ....");
            //使用自定义别名的httpClient 添加重试、熔断器、超时等策略配置 从配置文件ForwardConfig节点中读取
            context.Services.Configure<ForwardConfigDto>(context.Services.GetConfiguration().GetSection("ForwardConfig"));

            base.ConfigureServices(context);
            Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(ConsumerModule)} End  ConfigureServices ....");
        }

        public override void OnApplicationInitialization(ApplicationInitializationContext context)
        {
            Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(ConsumerModule)} Start  OnApplicationInitialization ....");
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
            Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(ConsumerModule)} End  OnApplicationInitialization ....");
        }
    }
}
