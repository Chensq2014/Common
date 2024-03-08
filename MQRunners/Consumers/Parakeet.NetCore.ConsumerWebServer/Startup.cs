using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System.IO;
using Parakeet.NetCore.Storage;
using Serilog;
using Volo.Abp.Modularity.PlugIns;

namespace Parakeet.NetCore.ConsumerWebServer
{
    public class Startup
    {
        public Startup(IWebHostEnvironment env)
        {
            CustomConfigurationManager.Init(env);
        }

        public void ConfigureServices(IServiceCollection services)
        {
            Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、服务注册流程_{nameof(Startup)} Start  ConfigureServices ....");
            services.AddApplication<ConsumerWebServerModule>(options =>
            {
                var baseDirectory = Path.GetDirectoryName(typeof(Program).Assembly.Location);
                var path = $"{baseDirectory}/Plugins";
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                options.PlugInSources.Add(new FolderPlugInSource(path, SearchOption.AllDirectories));
            });
            Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、服务注册流程_{nameof(Startup)} End  ConfigureServices ....");
        }

        public void Configure(IApplicationBuilder app)
        {
            Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、中间组装件流程_{nameof(Startup)} Start  Configure ....");
            //abp的启动module必须引用 AbpAspNetCoreModule 因为这个module才重新注册了 IApplicationBuilder 
            app.InitializeApplication();
            Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、中间组装件流程_{nameof(Startup)} End  Configure ....");
        }
    }
}
