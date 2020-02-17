using Microsoft.Extensions.Logging;

namespace DevEnterprise.Foundation.Caching
{
  internal static class LogEvents
  {
    public static readonly EventId CacheGet = new EventId(4000);
    public static readonly EventId CacheSet = new EventId(4001);
    public static readonly EventId CacheRemove = new EventId(4002);
  }
}
