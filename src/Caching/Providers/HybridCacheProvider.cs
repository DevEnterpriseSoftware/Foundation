using DevEnterprise.Foundation.Caching.Serialization;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Polly.Caching;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DevEnterprise.Foundation.Caching.Providers
{
  public sealed class HybridCacheProvider<TCache> : IHybridCacheProvider<TCache>, IAsyncCacheProvider<TCache>, ISyncCacheProvider<TCache>
  {
    private const int DefaultMemoryDurationInMinutes = 10;

    private readonly IMemoryCache memoryCache;
    private readonly IDistributedCache distributedCache;
    private readonly ICacheSerializer<TCache> serializer;
    private readonly ILogger<HybridCacheProvider<TCache>> logger;

    public HybridCacheProvider(IMemoryCache memoryCache, IDistributedCache distributedCache)
      : this(memoryCache, distributedCache, null, null)
    {
    }

    public HybridCacheProvider(IMemoryCache memoryCache, IDistributedCache distributedCache, ICacheSerializer<TCache> serializer, ILogger<HybridCacheProvider<TCache>> logger)
    {
      this.memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
      this.distributedCache = distributedCache ?? throw new ArgumentNullException(nameof(distributedCache));
      this.serializer = serializer ?? new JsonSerializer<TCache>();
      this.logger = logger;
    }

    #region IHybridCacheProvider

    public Task<TCache> GetAsync(string key)
    {
      return GetAsync(key, null);
    }

    public async Task<TCache> GetAsync(string key, Func<Task<TCache>> valueFactoryAsync)
    {
      // Check for the item in memory cache first.
      if (memoryCache.TryGetValue(key, out object valueFromCache))
      {
        if (logger?.IsEnabled(LogLevel.Trace) == true)
        {
          logger?.LogTrace(LogEvents.CacheGet, $"Found entry in memory cache for '{key}', refreshing the distributed cache");
        }

        await distributedCache.RefreshAsync(key).ConfigureAwait(false);
        return serializer.Deserialize(valueFromCache.ToString());
      }

      // Nothing found in memory, let's check the distributed cache and update memory if necessary.
      var fromCache = await distributedCache.GetStringAsync(key).ConfigureAwait(false);
      if (!string.IsNullOrEmpty(fromCache))
      {
        if (logger?.IsEnabled(LogLevel.Trace) == true)
        {
          logger?.LogTrace(LogEvents.CacheGet, $"Found entry in distributed for '{key}', setting memory cache value for {DefaultMemoryDurationInMinutes} minutes");
        }

        memoryCache.Set(key, fromCache, DateTimeOffset.UtcNow.AddMinutes(DefaultMemoryDurationInMinutes));
        return serializer.Deserialize(fromCache);
      }

      if (valueFactoryAsync != null)
      {
        var newValue = await valueFactoryAsync().ConfigureAwait(false);
        if (newValue != null)
        {
          var cacheValue = serializer.Serialize(newValue);
          memoryCache.Set(key, cacheValue);
          await distributedCache.SetStringAsync(key, cacheValue).ConfigureAwait(false);

          if (logger?.IsEnabled(LogLevel.Trace) == true)
          {
            logger?.LogTrace(LogEvents.CacheGet, $"New value created for '{key}'");
          }

          return newValue;
        }
      }

      return default;
    }

    public async Task SetAsync(string key, TCache value)
    {
      if (value == null)
      {
        await RemoveAsync(key).ConfigureAwait(false);
      }
      else
      {
        var cacheValue = serializer.Serialize(value);
        memoryCache.Set(key, cacheValue);
        await distributedCache.SetStringAsync(key, cacheValue).ConfigureAwait(false);
      }
    }

    public async Task SetAsync(string key, TCache value, DateTimeOffset absoluteExpiration)
    {
      await SetAsync(key, value, absoluteExpiration, absoluteExpiration).ConfigureAwait(false);
    }

    public async Task SetAsync(string key, TCache value, TimeSpan slidingExpiration)
    {
      await SetAsync(key, value, slidingExpiration, slidingExpiration).ConfigureAwait(false);
    }

    public async Task SetAsync(string key, TCache value, DateTimeOffset memoryAbsoluteExpiration, DateTimeOffset distributedAbsoluteExpiration)
    {
      if (value == null)
      {
        await RemoveAsync(key).ConfigureAwait(false);
      }
      else
      {
        var cacheValue = serializer.Serialize(value);
        memoryCache.Set(key, cacheValue, memoryAbsoluteExpiration);
        await distributedCache.SetStringAsync(key, cacheValue, new DistributedCacheEntryOptions() { AbsoluteExpiration = distributedAbsoluteExpiration }).ConfigureAwait(false);

        if (logger?.IsEnabled(LogLevel.Trace) == true)
        {
          logger?.LogTrace(LogEvents.CacheSet, $"Set cache value for '{key}' in memory for {(DateTimeOffset.UtcNow - memoryAbsoluteExpiration).TotalMinutes} minutes and distributed for {(DateTimeOffset.UtcNow - distributedAbsoluteExpiration).TotalMinutes} minutes");
        }
      }
    }

    public async Task SetAsync(string key, TCache value, TimeSpan memorySlidingExpiration, TimeSpan distributedSlidingExpiration)
    {
      if (value == null)
      {
        await RemoveAsync(key).ConfigureAwait(false);
      }
      else
      {
        var cacheValue = serializer.Serialize(value);
        memoryCache.Set(key, cacheValue, memorySlidingExpiration);
        await distributedCache.SetStringAsync(key, cacheValue, new DistributedCacheEntryOptions() { SlidingExpiration = distributedSlidingExpiration }).ConfigureAwait(false);

        if (logger?.IsEnabled(LogLevel.Trace) == true)
        {
          logger?.LogTrace(LogEvents.CacheSet, $"Set cache value for '{key}' in memory for {memorySlidingExpiration.TotalMinutes} minutes and distributed for {distributedSlidingExpiration.TotalMinutes} minutes");
        }
      }
    }

    public async Task SetAsync(string key, TCache value, DateTimeOffset memoryAbsoluteExpiration, TimeSpan distributedSlidingExpiration)
    {
      if (value == null)
      {
        await RemoveAsync(key).ConfigureAwait(false);
      }
      else
      {
        var cacheValue = serializer.Serialize(value);
        memoryCache.Set(key, cacheValue, memoryAbsoluteExpiration);
        await distributedCache.SetStringAsync(key, cacheValue, new DistributedCacheEntryOptions() { SlidingExpiration = distributedSlidingExpiration }).ConfigureAwait(false);

        if (logger?.IsEnabled(LogLevel.Trace) == true)
        {
          logger?.LogTrace(LogEvents.CacheSet, $"Set cache value for '{key}' in memory for {(DateTimeOffset.UtcNow - memoryAbsoluteExpiration).TotalMinutes} minutes and distributed for {distributedSlidingExpiration.TotalMinutes} minutes");
        }
      }
    }

    public async Task SetAsync(string key, TCache value, TimeSpan memorySlidingExpiration, DateTimeOffset distributedAbsoluteExpiration)
    {
      if (value == null)
      {
        await RemoveAsync(key).ConfigureAwait(false);
      }
      else
      {
        var cacheValue = serializer.Serialize(value);
        memoryCache.Set(key, cacheValue, memorySlidingExpiration);
        await distributedCache.SetStringAsync(key, cacheValue, new DistributedCacheEntryOptions() { AbsoluteExpiration = distributedAbsoluteExpiration }).ConfigureAwait(false);

        if (logger?.IsEnabled(LogLevel.Trace) == true)
        {
          logger?.LogTrace(LogEvents.CacheSet, $"Set cache value for '{key}' in memory for {memorySlidingExpiration.TotalMinutes} minutes and distributed for {(DateTimeOffset.UtcNow - distributedAbsoluteExpiration).TotalMinutes} minutes");
        }
      }
    }

    public async Task RemoveAsync(string key)
    {
      memoryCache.Remove(key);
      await distributedCache.RemoveAsync(key).ConfigureAwait(false);

      if (logger?.IsEnabled(LogLevel.Trace) == true)
      {
        logger?.LogTrace(LogEvents.CacheRemove, $"Removed cache value for '{key}'");
      }
    }

    public void Dispose()
    {
      memoryCache.Dispose();
    }

    #endregion

    #region ISyncCacheProvider

    public void Put(string key, TCache value, Ttl ttl)
    {
      if (ttl.SlidingExpiration)
      {
        SetAsync(key, value, ttl.Timespan).RunSynchronously();
      }
      else
      {
        SetAsync(key, value, DateTimeOffset.UtcNow + ttl.Timespan).RunSynchronously();
      }
    }

    public (bool, TCache) TryGet(string key)
    {
      var result = GetAsync(key).Result;
      return (result != null, result);
    }

    #endregion

    #region IAsyncCacheProvider

    public async Task PutAsync(string key, TCache value, Ttl ttl, CancellationToken cancellationToken, bool continueOnCapturedContext)
    {
      if (ttl.SlidingExpiration)
      {
        await SetAsync(key, value, ttl.Timespan).ConfigureAwait(continueOnCapturedContext);
      }
      else
      {
        await SetAsync(key, value, DateTimeOffset.UtcNow + ttl.Timespan).ConfigureAwait(continueOnCapturedContext);
      }
    }

    public async Task<(bool, TCache)> TryGetAsync(string key, CancellationToken cancellationToken, bool continueOnCapturedContext)
    {
      var result = await GetAsync(key).ConfigureAwait(continueOnCapturedContext);
      return (result != null, result);
    }

    #endregion
  }
}
