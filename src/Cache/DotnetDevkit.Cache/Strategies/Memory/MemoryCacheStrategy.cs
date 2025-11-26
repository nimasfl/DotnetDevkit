using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using DotnetDevkit.Cache.Models;

namespace DotnetDevkit.Cache.Strategies.Memory;

internal sealed class MemoryCacheStrategy : ICacheStrategy
{
    private static readonly ConcurrentDictionary<string, SemaphoreSlim> Locks = new();
    private static readonly Dictionary<string, CacheData> Cache = new();
    public static CacheStrategy GetCacheStrategy() => CacheStrategy.Memory;

    public Task<CacheData<T>?> GetAsync<T>(string key, CancellationToken ct)
    {
        CacheData<T>? data = null;
        if (HasValidCacheData(key, out var existingData) && existingData.Value is T value)
        {
            data = new CacheData<T>(existingData.Context, value);
        }

        return Task.FromResult(data);
    }

    public async Task<T> SetAsync<T>(string key, CacheData<T> data, CancellationToken ct)
    {
        var semaphore = Locks.GetOrAdd(key, _ => new SemaphoreSlim(1, 1));
        await semaphore.WaitAsync(ct);

        try
        {
            Cache[key] = new CacheData(data.Context, data.Value);
            return data.Value;
        }
        finally
        {
            semaphore.Release();
            Locks.TryRemove(key, out _);
        }
    }

    private static bool HasValidCacheData(string key, [MaybeNullWhen(false)] out CacheData data)
    {
        return Cache.TryGetValue(key, out data) && data.Context.IsExpired() is false;
    }

    public Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        Cache.Remove(key, out _);
        return Task.CompletedTask;
    }

    public Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(Cache.ContainsKey(key));
    }
}
