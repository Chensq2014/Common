using Common.Helpers;
using Common.Storage;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Serilog;
using Volo.Abp.Guids;
using Volo.Abp.Modularity;

namespace Common
{
    public class CommonSharedModule : AbpModule
    {
        public override void PreConfigureServices(ServiceConfigurationContext context)
        {
            Log.Debug($"{{0}}", $"............................................................................................................................");
            Log.Debug($"{{0}}", $"..............................................PreConfigureServices..........................................................");
            Log.Debug($"{{0}}", $"............................................................................................................................");
            Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(CommonSharedModule)} Start PreConfigureServices ....");

            //var configuration = context.Services.GetConfiguration();
            //context.Services.Configure<AbpSequentialGuidGeneratorOptions>(configuration.GetSection("GuidConfig"));

            //配置SequentialGuidGenerator的默认类型选项 Configure 最终都是调用context.Services.Configure
            Configure<AbpSequentialGuidGeneratorOptions>(options =>
            {
                options.DefaultSequentialGuidType = (SequentialGuidType)EnvironmentHelper.DefaultSequentialGuidType;

                //options.DefaultSequentialGuidType = SequentialGuidType.SequentialAsString;//mysql或pgsql的有序guid配置
                //options.DefaultSequentialGuidType = SequentialGuidType.SequentialAsBinary;//oracle
                //options.DefaultSequentialGuidType = SequentialGuidType.SequentialAtEnd;//sqlserver
            });

            //context.Services.TryAddTransient<IGuidGenerator, SequentialGuidGenerator>();//默认就是  SequentialGuidGenerator 实现的，不用再注册
            base.PreConfigureServices(context);
            Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(CommonSharedModule)} End PreConfigureServices ....");
        }

        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            Log.Debug($"{{0}}", $"............................................................................................................................");
            Log.Debug($"{{0}}", $"..............................................ConfigureServices..........................................................");
            Log.Debug($"{{0}}", $"............................................................................................................................");
            Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(CommonSharedModule)} Start ConfigureServices ....");
            base.ConfigureServices(context);
            Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(CommonSharedModule)} End ConfigureServices ....");
        }
    }
}
