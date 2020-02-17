using System;
using System.Threading.Tasks;

namespace DevEnterprise.Foundation.Caching
{
  public interface ICacheProvider<TCache> : IDisposable
  {
    Task<TCache> GetAsync(string key);

    Task<TCache> GetAsync(string key, Func<Task<TCache>> valueFactoryAsync);

    Task SetAsync(string key, TCache value);

    Task SetAsync(string key, TCache value, DateTimeOffset absoluteExpiration);

    Task SetAsync(string key, TCache value, TimeSpan slidingExpiration);

    Task RemoveAsync(string key);
  }
}
