using Common.Storage;
using Serilog;
using Volo.Abp.Modularity;

namespace Common
{
    public class CommonSharedModule : AbpModule
    {
        public override void PreConfigureServices(ServiceConfigurationContext context)
        {
            Log.Error($"{{0}}", $"............................................................................................................................");
            Log.Error($"{{0}}", $"..............................................PreConfigureServices..........................................................");
            Log.Error($"{{0}}", $"............................................................................................................................");
            Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(CommonSharedModule)} Start PreConfigureServices ....");
            base.PreConfigureServices(context);
            Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(CommonSharedModule)} End PreConfigureServices ....");
        }

        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            Log.Error($"{{0}}", $"............................................................................................................................");
            Log.Error($"{{0}}", $"..............................................ConfigureServices..........................................................");
            Log.Error($"{{0}}", $"............................................................................................................................");
            Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(CommonSharedModule)} Start ConfigureServices ....");
            base.ConfigureServices(context);
            Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(CommonSharedModule)} End ConfigureServices ....");
        }
    }
}
