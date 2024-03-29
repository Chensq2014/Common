﻿//using EasyCaching.CSRedis;
//using Microsoft.Extensions.Caching.Distributed;
//using Microsoft.Extensions.Caching.Redis;
//using Microsoft.Extensions.DependencyInjection;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using CSRedis;
//using EasyCaching.Core.Configurations;
//using EasyCaching.Redis;
//using Microsoft.Extensions.DependencyInjection.Extensions;
//using Common.Helper;
//using Common.Interfaces;
//using Common.Storage;
//using Serilog;
//using Volo.Abp.Caching;
//using Nest;

//namespace Common.Extensions
//{
//    /// <summary>
//    /// 缓存注册配置扩展
//    ///nosql:not only sql 不仅仅是sql语句，有各自操作的api
//    ///redis:remote dictionary server 远程字典服务器  开源的服务(freeBSD)
//    ///string hash list set zset bitmaps hyperloglss streams 八大数据结构
//    ///方便扩展，大数据高并发，八大数据结构，分布式存储
//    ///单线程原子性：一个任务一个人做。（redis单线程不代表整个redis服务中只有一个线程）
//    ///单线程多进程pk多线程：
//    ///单线程多进程：一个任务有好多个人去做(启动多个redis实例如6379，6380两个端口启动redis)，
//    ///多线程：有一个人去指导这些人去合理安排的的干,多线程要实现原子性，涉及到各种锁，上下文的切换性能的消耗
//    ///典型单线程多进程服务:nginx
//    ///io多路复用/异步
//    ///多路复用：打个单子，然后把单子给后厨，然后你稍等，菜做完了喊你，你来拿
//    ///异步：打个单子，让你留下自己的地址，然后把单子给后厨，菜做完了，直接送达地址
//    /// </summary>
//    public static class CacheExtensions
//    {
//        /// <summary>
//        /// 添加RedisCache 缓存
//        /// </summary>
//        /// <param name="services"></param>
//        /// <param name="redisConnectionString"></param>
//        public static void AddCsRedisCache(this IServiceCollection services, string redisConnectionString = "")
//        {
//            //var configuration = services.GetConfiguration();
//            //configration.GetValue<string>("Redis:CsRedisConfiguration");
//            //redisConnectionString ??= configuration["Redis:CsRedisConfiguration"];
//            redisConnectionString ??= EnvironmentHelper.GetCsRedisDbConnections().First();

//            #region 多redis数据库支持

//            //csredis多数据库支持
//            //var connectionString = "127.0.0.1:6379,password=123,poolsize=10,ssl=false,writeBuffer=10240,prefix=parakeet";
//            var csRedisArray = new CSRedisClient[EnvironmentHelper.GetCsRedisDbConnections().Count]; //csRedisArray要用redisHelper类管理起来,定义成集合单例
//            for (var index = 0; index < csRedisArray.Length; index++)
//            {
//                csRedisArray[index] = new CSRedisClient(EnvironmentHelper.GetCsRedisDbConnections()[index]);
//                ////访问数据库index的数据
//                //redisArray[index].Get("test1");
//            }
//            #endregion
//            Log.Logger.Information($"{{0}}", $"{CacheKeys.LogCount++}、配置CsRedisCache redisConnectionString:{redisConnectionString}....");
//            RedisHelper.Initialization(new CSRedisClient(redisConnectionString));

//            //IDistributedCache接口 也让CSRedisCache 来实现
//            services.TryAddSingleton<IDistributedCache>(new CSRedisCache(RedisHelper.Instance));
//            //services.Replace(ServiceDescriptor.Singleton<IDistributedCache, CSRedisCache> ());
//            services.AddEasyCaching(options =>
//            {
//                Log.Logger.Error($"{{0}}", $"{CacheKeys.LogCount++}、AddEasyCaching配置{nameof(EasyCachingOptions)}....");
//                options.UseCSRedis(redisOptions =>
//                {
//                    Log.Logger.Error($"配置AddEasyCaching {nameof(redisOptions)} SerializerName:{redisOptions.SerializerName},WithMessagePack:{redisOptions.SerializerName}....");
//                    redisOptions.DBConfig = new CSRedisDBOptions
//                    {
//                        ConnectionStrings = new List<string> { redisConnectionString },
//                    };
//                    redisOptions.SerializerName = "parakeet";
//                });
                
//                options.WithMessagePack("parakeet");
//            });
//            services.TryAddSingleton(typeof(ICacheContainer<,>), typeof(CacheContainer<,>));

//        }

//        /// <summary>
//        /// 添加ServiceStack 缓存
//        /// </summary>
//        /// <param name="services"></param>
//        /// <param name="redisConnectionString"></param>
//        public static void AddServiceStackRedisCache(this IServiceCollection services, string redisConnectionString = "")
//        {
            
//        }

//        /// <summary>
//        /// 添加RedisCache 缓存
//        /// </summary>
//        /// <param name="services"></param>
//        /// <param name="redisConnectionString"></param>
//        public static void AddStackExchangeRedisCache(this IServiceCollection services, string redisConnectionString = "")
//        {
//            //Log.Logger.Information($"{{0}}", $"{CacheKeys.LogCount++}、配置StackExchangeRedis....");
//            var configuration = services.GetConfiguration();
//            //services.AddStackExchangeRedisCache(options =>
//            //{
//            //    options.Configuration = redisConnectionString ?? configuration["Redis:ConnectionStrings"];//ConnectionStrings//Configuration
//            //    options.InstanceName = configuration["Redis:InstanceName"];
//            //});
//            services.AddEasyCaching(options =>
//            {
//                //StackExchange.Redis
//                options.UseRedis(redisOptions =>
//                {
//                    redisOptions.DBConfig = new RedisDBOptions
//                    {
//                        //Database=0
//                        //IsSsl = false,
//                        //SslHost = "",
//                        Username = "",
//                        //Password = "",
//                        Configuration = configuration["Redis:Configuration"],
//                        ConnectionTimeout = 5000
//                    };
//                    redisOptions.SerializerName = "parakeet";
//                });
//                options.WithMessagePack("parakeet");
//            });
//            services.TryAddSingleton(typeof(ICacheContainer<,>), typeof(CacheContainer<,>));
//        }

//        /// <summary>
//        /// 配置分布式缓存Options 也可以在配置文件中设置
//        /// </summary>
//        /// <param name="services"></param>
//        /// <param name="redisConnectionString"></param>
//        public static void ConfigAbpDistributeCacheOptions(this IServiceCollection services, string redisConnectionString = "")
//        {
//            var configuration = services.GetConfiguration();
//            redisConnectionString = string.IsNullOrWhiteSpace(redisConnectionString) ? configuration["Redis:CsRedisConfiguration"]:string.Empty;
//            //services.AddDistributedMemoryCache();
//            //配置分布式缓存，那么系统使用session时保存的session就会存在redis里，确保分布式后session的正常使用
//            services.AddDistributedRedisCache(options =>
//            {
//                options.Configuration = redisConnectionString;
//                options.InstanceName = configuration["Redis:InstanceName"];
                
//                Log.Logger.Error($"{{0}}", $"{CacheKeys.LogCount++}、AddDistributedRedisCache配置{nameof(RedisCacheOptions)} redisConnectionString:{redisConnectionString} InstanceName:{configuration["Redis:InstanceName"]}....");
//            });

//            services.Configure<AbpDistributedCacheOptions>(options =>
//            {
//                options.KeyPrefix = "NetCore_"; //默认key前缀  与
//                //缓存默认过期时间设置为2h
//                options.GlobalCacheEntryOptions.SlidingExpiration = TimeSpan.FromHours(2);

//                Log.Logger.Error($"{{0}}", $"{CacheKeys.LogCount++}、配置{nameof(AbpDistributedCacheOptions)} GlobalCacheEntryOptions.SlidingExpiration:{TimeSpan.FromHours(2)}....");
//            });
//        }
//    }

//}