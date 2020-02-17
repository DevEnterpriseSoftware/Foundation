using System;
using DevEnterprise.Foundation.Caching.Providers;
using Microsoft.Extensions.Caching.SqlServer;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using Microsoft.Extensions.DependencyInjection;

namespace DevEnterprise.Foundation.Caching.Extensions
{
  public static class CacheExtensions
  {
    public static IServiceCollection AddHybridDistributedSqlServerCache(this IServiceCollection services, Action<SqlServerCacheOptions> setupAction)
    {
      return services.AddDistributedSqlServerCache(setupAction)
                     .AddScoped(typeof(IHybridCacheProvider<>), typeof(HybridCacheProvider<>));
    }

    public static IServiceCollection AddHybridRedisCache(this IServiceCollection services, Action<RedisCacheOptions> setupAction)
    {
      return services.AddStackExchangeRedisCache(setupAction)
                     .AddScoped(typeof(IHybridCacheProvider<>), typeof(HybridCacheProvider<>));
    }
  }
}
