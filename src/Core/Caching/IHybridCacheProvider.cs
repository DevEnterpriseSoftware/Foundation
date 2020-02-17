using System;
using System.Threading.Tasks;

namespace DevEnterprise.Foundation.Caching
{
  public interface IHybridCacheProvider<TCache> : ICacheProvider<TCache>
  {
    Task SetAsync(string key, TCache value, DateTimeOffset memoryAbsoluteExpiration, DateTimeOffset distributedAbsoluteExpiration);

    Task SetAsync(string key, TCache value, TimeSpan memorySlidingExpiration, TimeSpan distributedSlidingExpiration);

    Task SetAsync(string key, TCache value, DateTimeOffset memoryAbsoluteExpiration, TimeSpan distributedSlidingExpiration);

    Task SetAsync(string key, TCache value, TimeSpan memorySlidingExpiration, DateTimeOffset distributedAbsoluteExpiration);
  }
}
