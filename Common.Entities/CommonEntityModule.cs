using Common.Storage;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Serilog;
using Volo.Abp.Modularity;

namespace Common;

[DependsOn(
    typeof(CommonSharedModule)
)]
public class CommonEntityModule : AbpModule
{
    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        Log.Debug($"{{0}}", $"............................................................................................................................");
        Log.Debug($"{{0}}", $"..............................................PreConfigureServices..........................................................");
        Log.Debug($"{{0}}", $"............................................................................................................................");
        Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(CommonEntityModule)} Start PreConfigureServices ....");
        base.PreConfigureServices(context);
        Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(CommonEntityModule)} End PreConfigureServices ....");
    }


    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Log.Debug($"{{0}}", $"............................................................................................................................");
        Log.Debug($"{{0}}", $"..............................................ConfigureServices..........................................................");
        Log.Debug($"{{0}}", $"............................................................................................................................");
        Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(CommonEntityModule)} Start ConfigureServices ....");
        base.ConfigureServices(context);
        Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(CommonEntityModule)} End ConfigureServices ....");
    }
}
