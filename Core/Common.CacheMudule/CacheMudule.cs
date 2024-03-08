using System.Collections.Generic;
using EasyCaching.Core.Configurations;
using EasyCaching.Redis;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Common.Dtos;
using Common.Helpers;
using Common.Interfaces;
using Common.Storage;
using ServiceStack;
using Volo.Abp;
using Volo.Abp.Caching.StackExchangeRedis;
using Volo.Abp.Modularity;

namespace Common.CacheMudule
{
    /// <summary>
    /// Redis缓存模块
    /// </summary>
    [DependsOn(typeof(AbpCachingStackExchangeRedisModule))]
    public class CacheMudule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            var configuration = context.Services.GetConfiguration();
            //注册ServiceStack 自定义实现类 使用
            context.Services.Configure<RedisConnOptions>(configuration.GetSection(Magics.REDISCONN));
            context.Services.TryAddSingleton<IDistributedCache,ServiceStackRedisCache>();
            //context.Services.Replace(ServiceDescriptor.Singleton<IDistributedCache, ServiceStackRedisCache>());

            //context.Services.AddEasyCaching(options =>
            //{
            //    //StackExchange.Redis
            //    options.UseRedis(redisOptions =>
            //    {
            //        redisOptions.DBConfig = new RedisDBOptions
            //        {
            //            //Database=0
            //            //IsSsl = false,
            //            //SslHost = "",
            //            Username = "",
            //            //Password = "",
            //            Configuration = configuration["Redis:Configuration"],
            //            ConnectionTimeout = 5000
            //        };
            //        redisOptions.SerializerName = "parakeet";
            //    });
            //    options.WithMessagePack("parakeet");
            //});
            context.Services.TryAddSingleton(typeof(ICacheContainer<,>), typeof(CacheContainer<,>));
        }

        public override void OnApplicationInitialization(ApplicationInitializationContext context)
        {
            base.OnApplicationInitialization(context);
        }
    }
}
