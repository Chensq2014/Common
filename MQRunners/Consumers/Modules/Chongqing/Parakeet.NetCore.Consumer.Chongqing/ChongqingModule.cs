using Microsoft.Extensions.DependencyInjection;
using Parakeet.NetCore.Consumer;
using Parakeet.NetCore.Consumer.Chongqing.Dtos;
using Parakeet.NetCore.Storage;
using Serilog;
using Volo.Abp;
using Volo.Abp.Modularity;

namespace Parakeet.NetCore.Consumer.Chongqing
{
    /// <summary>
    /// Chongqing(重庆)区域公共模块
    /// </summary>
    [DependsOn(typeof(ConsumerModule))]
    public class ChongqingModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(ChongqingModule)} Start  ConfigureServices ....");

            var configuration = context.Services.GetConfiguration();
            context.Services.Configure<TokenConfigDto>(configuration.GetSection("ForwardBaseAddress:ChongqingTokenConfig"));

            base.ConfigureServices(context);
            Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(ChongqingModule)} End  ConfigureServices ....");
        }
        public override void OnApplicationInitialization(ApplicationInitializationContext context)
        {
            Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(ChongqingModule)} Start  OnApplicationInitialization ....");
            var app = context.GetApplicationBuilder();
            var env = context.GetEnvironment();
            var configuration = context.GetConfiguration();

            base.OnApplicationInitialization(context);
            Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(ChongqingModule)} End  OnApplicationInitialization ....");
        }
    }
}
