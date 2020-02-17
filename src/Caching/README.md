# Caching

This caching library provides a hybrid memory and distributed cache mechanism over the standard IMemoryCache and IDistributedCache interfaces. Cached items are stored in memory for fast access uses a distributed backplane for longer term caching and to share across multiple clients. This caching library is also a Polly cache provider so can be used in cache policy configuration.

---

## Tools

[Redis on Windows](https://github.com/MicrosoftArchive/redis/releases) - Run a local Redis server and connect to it using `127.0.0.1:6379,ssl=False,allowAdmin=True,connectTimeout=30000,syncTimeout=15000` as your connection string.

[Redis Desktop Manager](https://redisdesktop.com/) - A simple and free GUI client to connect to Redis and view cache keys and values.

---

## References

- Caching Best Practices: https://docs.microsoft.com/en-us/azure/architecture/best-practices/caching
