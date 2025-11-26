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


    /// <summary>
    /// Get value from cache or add it with the factory. Uses distributed lock to prevent thundering herd.
    /// Must never throw if Redis is down; it will compute the factory value and return it.
    /// </summary>
    /// <param name="key"></param>
    /// <param name="factory"></param>
    /// <param name="absoluteExpiry"></param>
    /// <param name="cancellationToken"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    Task<T> GetOrAddAsync<T>(string key, Func<CancellationToken, Task<T>> factory, TimeSpan? absoluteExpiry = null,
        CancellationToken cancellationToken = default);
}
