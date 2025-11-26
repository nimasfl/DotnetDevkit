using DotnetDevkit.Cache.Converters;
using DotnetDevkit.Cache.Models;
using DotnetDevkit.Cache.Strategies;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace DotnetDevkit.Cache;

internal sealed class CacheProvider(
    ICacheStrategy cacheStrategy,
    ILogger<ICacheProvider> logger,
    IOptions<CacheProviderOptions> options)
    : ICacheProvider
{
    internal static readonly JsonSerializerSettings SerializerSettings = new()
    {
        DefaultValueHandling = DefaultValueHandling.Include,
        ContractResolver = new CustomContractResolver()
    };

    public async Task<T> GetOrAddAsync<T>(string rawKey, Func<CacheContext, CancellationToken, Task<T>> func,
        CancellationToken cancellationToken = default)
    {
        var key = GetPrefixedKey(rawKey);
        var data = await cacheStrategy.GetAsync<T>(key, cancellationToken);

        if (data is null || data.Value is null)
        {
            var cacheContext = new CacheContext();
            var value = await func(cacheContext, cancellationToken);

            if (value is null)
            {
                return value;
            }

            try
            {
                await cacheStrategy.SetAsync(key, new CacheData<T>(cacheContext, value), cancellationToken);
            }
            catch (Exception e)
            {
                if (logger.IsEnabled(LogLevel.Error))
                {
                    logger.LogError(e, "Error while setting cache data using provider: {Provider}. {Message}",
                        cacheStrategy,
                        e.Message
                    );
                }
            }

            return value;
        }

        SlideIfEnabled(key, data);


        return data.Value;
    }

    public async Task RemoveAsync(string rawKey, CancellationToken cancellationToken = default)
    {
        var key = GetPrefixedKey(rawKey);
        await cacheStrategy.RemoveAsync(key, cancellationToken);
    }

    public async Task<bool> ExistsAsync(string rawKey, CancellationToken cancellationToken = default)
    {
        var key = GetPrefixedKey(rawKey);
        return await cacheStrategy.ExistsAsync(key, cancellationToken);
    }

    public async Task<T?> GetAsync<T>(string rawKey, CancellationToken cancellationToken)
    {
        var key = GetPrefixedKey(rawKey);
        try
        {
            var cachedData = await cacheStrategy.GetAsync<T?>(key, cancellationToken);
            if (cachedData is not null && cachedData.Value is not null)
            {
                SlideIfEnabled(key, cachedData);
                return cachedData.Value;
            }
        }
        catch (Exception e)
        {
            if (logger.IsEnabled(LogLevel.Error))
            {
                logger.LogError(e, "Error while getting cached data using provider: {Provider}. {Message}",
                    cacheStrategy,
                    e.Message
                );
            }
        }

        return (T?)(object?)null;
    }

    public async Task SetAsync<T>(string rawKey, CacheData<T> data, CancellationToken cancellationToken)
    {
        var key = GetPrefixedKey(rawKey);
        try
        {
            await cacheStrategy.SetAsync<T?>(key, data!, cancellationToken);
        }
        catch (Exception e)
        {
            if (logger.IsEnabled(LogLevel.Error))
            {
                logger.LogError(e, "Error while getting cached data using provider: {Provider}. {Message}",
                    cacheStrategy,
                    e.Message
                );
            }
        }
    }

    private async void SlideIfEnabled<T>(string key, CacheData<T> data)
    {
        try
        {
            if (data.Context.SlidingTimespan is null)
            {
                return;
            }

            data.Context.SlideExpirationDate();

            await SetAsync(key, data, CancellationToken.None);
        }
        catch (Exception e)
        {
            if (logger.IsEnabled(LogLevel.Error))
            {
                logger.LogError(e, "{Message}", e.Message);
            }
        }
    }

    private string GetPrefixedKey(string key) => options.Value.Prefix + key;
}
