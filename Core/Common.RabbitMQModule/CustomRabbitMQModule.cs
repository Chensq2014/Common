using Common.RabbitMQModule.Extensions;
using Common.Storage;
using Serilog;
using Volo.Abp;
using Volo.Abp.Modularity;

namespace Common.RabbitMQModule
{
    /// <summary>
    /// RabbitMQ消息队列模块 公共类库模块
    /// </summary>
    [DependsOn(typeof(CommonSharedModule))]
    public class CustomRabbitMQModule : AbpModule
    {
        public override void PreConfigureServices(ServiceConfigurationContext context)
        {
            Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(CustomRabbitMQModule)} Start PreConfigureServices ....");
            base.PreConfigureServices(context);
            Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(CustomRabbitMQModule)} End PreConfigureServices ....");
        }

        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(CustomRabbitMQModule)} Start  ConfigureServices ....");
            
            context.Services.AddRabbitMQ();
            base.ConfigureServices(context);
            Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(CustomRabbitMQModule)} End  ConfigureServices ....");
        }

        public override void PostConfigureServices(ServiceConfigurationContext context)
        {
            Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(CustomRabbitMQModule)} Start PostConfigureServices ....");
            base.PostConfigureServices(context);
            Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(CustomRabbitMQModule)} End PostConfigureServices ....");
        }

        public override void OnPreApplicationInitialization(ApplicationInitializationContext context)
        {
            Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(CustomRabbitMQModule)} Start OnPreApplicationInitialization ....");
            base.OnPreApplicationInitialization(context);
            Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(CustomRabbitMQModule)} End OnPreApplicationInitialization ....");
        }
        public override void OnApplicationInitialization(ApplicationInitializationContext context)
        {
            Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(CustomRabbitMQModule)} Start OnApplicationInitialization ....");
            base.OnApplicationInitialization(context);
            Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(CustomRabbitMQModule)} End OnApplicationInitialization ....");
        }

        public override void OnPostApplicationInitialization(ApplicationInitializationContext context)
        {
            Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(CustomRabbitMQModule)} Start OnPostApplicationInitialization ....");
            base.OnPostApplicationInitialization(context);
            Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(CustomRabbitMQModule)} End OnPostApplicationInitialization ....");
        }

        public override void OnApplicationShutdown(ApplicationShutdownContext context)
        {
            Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(CustomRabbitMQModule)} Start OnApplicationShutdown ....");
            base.OnApplicationShutdown(context);
            Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(CustomRabbitMQModule)} End OnApplicationShutdown ....");
        }
    }
}
