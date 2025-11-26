using DotnetDevkit.Cache.Abstractions;
using Microsoft.Extensions.Logging;

namespace DotnetDevkit.Cache;

public class RedisCacheReconnector(RedisCache cache, ILogger<RedisCacheReconnector> logger) : ICache
{
    public async Task<CacheResult<T>> GetAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        try
        {
            if (cache._redis is { IsConnected: true })
            {
                return await cache.GetAsync<T>(key, cancellationToken);
            }
        }
        catch (Exception ex)
        {
            if (logger.IsEnabled(LogLevel.Error))
            {
                logger.LogError(ex, "Error while using cache. {CacheMethod} {CacheKey}: {ExceptionMessage}",
                    nameof(GetAsync), key, ex.Message);
            }
        }

        FireAndForgetReconnect();
        return CacheResult<T>.None;
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? absoluteExpiry = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (cache._redis is { IsConnected: true })
            {
                await cache.SetAsync(key, value, absoluteExpiry, cancellationToken);
                return;
            }
        }
        catch (Exception ex)
        {
            if (logger.IsEnabled(LogLevel.Error))
            {
                logger.LogError(ex, "Error while using cache. {CacheMethod} {CacheKey}: {ExceptionMessage}",
                    nameof(SetAsync), key, ex.Message);
            }
        }

        FireAndForgetReconnect();
    }

    public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        try
        {
            if (cache._redis is { IsConnected: true })
            {
                await cache.RemoveAsync(key, cancellationToken);
                return;
            }
        }
        catch (Exception ex)
        {
            if (logger.IsEnabled(LogLevel.Error))
            {
                logger.LogError(ex, "Error while using cache. {CacheMethod} {CacheKey}: {ExceptionMessage}",
                    nameof(RemoveAsync), key, ex.Message);
            }
        }

        FireAndForgetReconnect();
    }

    public async Task<CacheResult<T>> GetOrAddAsync<T>(string key, Func<CancellationToken, Task<T>> factory,
        TimeSpan? absoluteExpiry = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (cache._redis is { IsConnected: true })
            {
                return await cache.GetOrAddAsync(key, factory, absoluteExpiry, cancellationToken);
            }
        }
        catch (Exception ex)
        {
            if (logger.IsEnabled(LogLevel.Error))
            {
                logger.LogError(ex, "Error while using cache. {CacheMethod} {CacheKey}: {ExceptionMessage}",
                    nameof(RemoveAsync), key, ex.Message);
            }
        }

        FireAndForgetReconnect();
        return CacheResult<T>.None;
    }

    private void FireAndForgetReconnect()
    {
        if (cache._connectionFactory != null)
        {
            Task.Run(cache.TryConnectAndInitAsync);
        }
    }


}
