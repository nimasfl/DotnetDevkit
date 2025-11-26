using DotnetDevkit.Cache.Models;

namespace DotnetDevkit.Cache.Strategies;

internal interface ICacheStrategy
{
    internal Task<CacheData<T>?> GetAsync<T>(string key, CancellationToken cancellationToken);

    internal Task<T> SetAsync<T>(string key, CacheData<T> data, CancellationToken cancellationToken);

    Task RemoveAsync(string key, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default);
}
