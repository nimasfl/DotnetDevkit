using DotnetDevkit.Cache.Models;

namespace DotnetDevkit.Cache;

public interface ICacheProvider
{
    Task<T> GetOrAddAsync<T>(string rawKey, Func<CacheContext, CancellationToken, Task<T>> func,
        CancellationToken cancellationToken = default);

    Task RemoveAsync(string key, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default);
    Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken);

    Task SetAsync<T>(string key, CacheData<T> data, CancellationToken cancellationToken);
}
