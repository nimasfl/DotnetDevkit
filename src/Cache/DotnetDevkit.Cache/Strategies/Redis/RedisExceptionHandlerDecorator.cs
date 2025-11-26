using DotnetDevkit.Cache.Abstractions;
using DotnetDevkit.Cache.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace DotnetDevkit.Cache.Strategies.Redis;

internal class RedisExceptionHandlerDecorator(
    RedisCacheStrategy redisCacheStrategy,
    IOptions<CacheProviderOptions> options,
    ILogger<RedisExceptionHandlerDecorator> logger
) : ICacheStrategy
{
    public async Task<CacheData<T>?> GetAsync<T>(string key, CancellationToken cancellationToken)
    {
        TryConnectionAsync();
        try
        {
            return await redisCacheStrategy.GetAsync<T>(key, cancellationToken);
        }
        catch (RedisException e)
        {
            if (logger.IsEnabled(LogLevel.Error))
            {
                logger.LogError(e, "Redis Cache GetAsync: {Message}", e.Message);
            }
        }

        return null;
    }

    public async Task<T> SetAsync<T>(string key, CacheData<T> data, CancellationToken cancellationToken)
    {
        TryConnectionAsync();
        try
        {
            return await redisCacheStrategy.SetAsync(key, data, cancellationToken);
        }
        catch (RedisException e)
        {
            if (logger.IsEnabled(LogLevel.Error))
            {
                logger.LogError(e, "Redis Cache SetAsync: {Message}", e.Message);
            }
        }

        return data.Value;
    }

    public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        TryConnectionAsync();
        try
        {
            await redisCacheStrategy.RemoveAsync(key, cancellationToken);
        }
        catch (RedisException e)
        {
            if (logger.IsEnabled(LogLevel.Error))
            {
                logger.LogError(e, "Redis Cache RemoveAsync: {Message}", e.Message);
            }
        }
    }

    public async Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default)
    {
        TryConnectionAsync();
        try
        {
            return await redisCacheStrategy.ExistsAsync(key, cancellationToken);
        }
        catch (RedisException e)
        {
            if (logger.IsEnabled(LogLevel.Error))
            {
                logger.LogError(e, "Redis Cache RemoveAsync: {Message}", e.Message);
            }
        }

        return false;
    }

    private async void TryConnectionAsync()
    {
        try
        {
            await Task.Run(() => { RedisCacheStrategy.TryConnect(options.Value.RedisConfiguration); });
        }
        catch
        {
            // ignore exceptions
        }
    }
}
