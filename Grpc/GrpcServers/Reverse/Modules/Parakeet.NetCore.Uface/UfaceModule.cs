using System;
using Microsoft.Extensions.DependencyInjection;
using Parakeet.NetCore.Equipment;
using Parakeet.NetCore.GrpcEFCore;
using Parakeet.NetCore.RabbitMQModule.Core;
using Parakeet.NetCore.RabbitMQModule.Producers;
using Parakeet.NetCore.Storage;
using Serilog;
using Volo.Abp;
using Volo.Abp.Modularity;

namespace Parakeet.NetCore.Uface
{
    [DependsOn(typeof(GrpcEFCoreModule))]
    public class UfaceModule : AbpModule
    {
        public override void PreConfigureServices(ServiceConfigurationContext context)
        {
            Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(UfaceModule)} Start PreConfigureServices ....");
            base.PreConfigureServices(context);
            Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(UfaceModule)} End PreConfigureServices ....");
        }

        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(UfaceModule)} Start ConfigureServices ....");
            base.ConfigureServices(context);
            Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(UfaceModule)} End ConfigureServices ....");
        }

        public override void PostConfigureServices(ServiceConfigurationContext context)
        {
            Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(UfaceModule)} Start PostConfigureServices ....");
            base.PostConfigureServices(context);
            Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(UfaceModule)} End PostConfigureServices ....");
        }

        public override void OnPreApplicationInitialization(ApplicationInitializationContext context)
        {
            Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(UfaceModule)} Start OnPreApplicationInitialization ....");
            var eventBusContainer = context.ServiceProvider.GetRequiredService<IRabbitMQEventBusContainer>();
            eventBusContainer.AddProducer(new ProducerAttribute(AppConstants.STANDARD, AppConstants.GATE_EXCHANGE));
            eventBusContainer.AddProducer(new ProducerAttribute(AppConstants.SICHUAN, AppConstants.GATE_EXCHANGE));
            eventBusContainer.AddProducer(new ProducerAttribute(AppConstants.CHONGQING, AppConstants.GATE_EXCHANGE));
            eventBusContainer.AddProducer(new ProducerAttribute(AppConstants.HUNAN, AppConstants.GATE_EXCHANGE));
            base.OnPreApplicationInitialization(context);
            Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(UfaceModule)} End OnPreApplicationInitialization ....");
        }
        public override void OnApplicationInitialization(ApplicationInitializationContext context)
        {
            Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(UfaceModule)} Start OnApplicationInitialization ....");
            var rabbitEventBusContainer = context.ServiceProvider.GetService<IRabbitMQEventBusContainer>();
            rabbitEventBusContainer.AutoRegister(new[] { typeof(UfaceModule).Assembly });
            base.OnApplicationInitialization(context);
            Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(UfaceModule)} End OnApplicationInitialization ....");
        }

        public override void OnPostApplicationInitialization(ApplicationInitializationContext context)
        {
            Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(UfaceModule)} Start OnPostApplicationInitialization ....");
            base.OnPostApplicationInitialization(context);
            Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(UfaceModule)} End OnPostApplicationInitialization ....");
        }

        public override void OnApplicationShutdown(ApplicationShutdownContext context)
        {
            Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(UfaceModule)} Start OnApplicationShutdown ....");
            base.OnApplicationShutdown(context);
            Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(UfaceModule)} End OnApplicationShutdown ....");
        }
    }
}
