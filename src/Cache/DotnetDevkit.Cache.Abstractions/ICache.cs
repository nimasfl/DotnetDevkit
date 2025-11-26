namespace DotnetDevkit.Cache.Abstractions;

using System;
using System.Threading;
using System.Threading.Tasks;

public interface ICache
{
    Task<CacheResult<T>> GetAsync<T>(string key, CancellationToken cancellationToken = default);

    Task SetAsync<T>(string key, T value, TimeSpan? absoluteExpiry = null,
        CancellationToken cancellationToken = default);

    Task RemoveAsync(string key, CancellationToken cancellationToken = default);

    /// Get value from cache or create it with the factory. Uses distributed lock to prevent thundering herd.
    /// Must never throw if Redis is down; it will compute the factory value and return it.
    Task<CacheResult<T>> GetOrAddAsync<T>(string key, Func<CancellationToken, Task<T>> factory, TimeSpan? absoluteExpiry = null,
        CancellationToken cancellationToken = default);
}
