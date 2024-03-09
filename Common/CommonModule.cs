using Common.Storage;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Serilog;
using Volo.Abp.Modularity;

namespace Common;

[DependsOn(
    typeof(CommonEntityModule)
)]
public class CommonModule : AbpModule
{
    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        Log.Error($"{{0}}", $"............................................................................................................................");
        Log.Error($"{{0}}", $"..............................................PreConfigureServices..........................................................");
        Log.Error($"{{0}}", $"............................................................................................................................");
        Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(CommonModule)} Start PreConfigureServices ....");
        base.PreConfigureServices(context);
        Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(CommonModule)} End PreConfigureServices ....");
    }

    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Log.Error($"{{0}}", $"............................................................................................................................");
        Log.Error($"{{0}}", $"..............................................ConfigureServices..........................................................");
        Log.Error($"{{0}}", $"............................................................................................................................");
        Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(CommonModule)} Start ConfigureServices ....");
        base.ConfigureServices(context);
        Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(CommonModule)} End ConfigureServices ....");
    }
}
