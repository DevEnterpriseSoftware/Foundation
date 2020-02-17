using System;
using DevEnterprise.Foundation.Caching.Providers;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using NSubstitute;

namespace Test
{
  public sealed class CacheProviderTests : IDisposable
  {
    private readonly IMemoryCache testMemoryCache;
    private readonly IDistributedCache testDistributedCache;
    private readonly HybridCacheProvider<int> testHybridCache;

    public CacheProviderTests()
    {
      testMemoryCache = Substitute.For<IMemoryCache>();
      testDistributedCache = Substitute.For<IDistributedCache>();
      testHybridCache = new HybridCacheProvider<int>(testMemoryCache, testDistributedCache);
    }

    public void Dispose()
    {
      testHybridCache.Dispose();
    }
  }
}