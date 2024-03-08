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
    /// standard����ģ��
    /// </summary>
    [DependsOn(typeof(ProducerModule))]
    public class StandardModule : AbpModule
    {
        public override void PreConfigureServices(ServiceConfigurationContext context)
        {
            Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}��Module����˳��_{nameof(StandardModule)} Start  PreConfigureServices ....");
            //PreConfigure<IMvcBuilder>(mvcBuilder =>
            //{
            //    mvcBuilder.AddApplicationPartIfNotExists(typeof(StandardModule).Assembly);
            //});
            base.PreConfigureServices(context);
            Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}��Module����˳��_{nameof(StandardModule)} End  PreConfigureServices ....");
        }

        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}��Module����˳��_{nameof(StandardModule)} Start  ConfigureServices ....");
            //var configuration = context.Services.GetConfiguration();
            //context.Services.AddHttpApi<IImageApi>(options =>
            //{
            //    options.HttpHost = new Uri(configuration.GetValue<string>("HttpOptions:ImageUrl"));
            //});
            //context.Services.BuildServiceProvider();
            base.ConfigureServices(context);
            Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}��Module����˳��_{nameof(StandardModule)} End  ConfigureServices ....");
        }



        public override void OnApplicationInitialization(ApplicationInitializationContext context)
        {
            Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}��Module����˳��_{nameof(StandardModule)} Start  OnApplicationInitialization ....");
            var app = context.GetApplicationBuilder();
            var env = context.GetEnvironment();
            var configuration = context.GetConfiguration();

            Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}�� ��ȡIRabbitMQEventBusContainer Ȼ��AutoRegister �Զ�ע�Ტ�󶨵�ǰģ�������/������....");
            var rabbitEventBusContainer = context.ServiceProvider.GetService<IRabbitMQEventBusContainer>();
            rabbitEventBusContainer.AutoRegister(new[] { typeof(StandardModule).Assembly });

            Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}��Module����˳��_{nameof(StandardModule)} End  OnApplicationInitialization ....");
        }

    }
}
