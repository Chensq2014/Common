using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Parakeet.NetCore.Extensions;
using Parakeet.NetCore.RabbitMQModule.Extensions;
using Parakeet.NetCore.Storage;
using Serilog;
using Volo.Abp.Modularity.PlugIns;

namespace Parakeet.NetCore.TcpHost
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            CustomConfigurationManager.Init(env);
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、服务注册流程_{nameof(Startup)} Start  ConfigureServices ....");
            services.AddApplication<TcpHostModule>(
                options =>
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

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、中间组装件流程_{nameof(Startup)} Start  Configure ....");
            app.InitializeApplication();
            Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、中间组装件流程_{nameof(Startup)} End  Configure ....");
        }
    }
}
