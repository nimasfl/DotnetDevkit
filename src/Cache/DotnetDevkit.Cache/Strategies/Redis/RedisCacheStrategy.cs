using System.Text;
using DotnetDevkit.Cache.Models;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RedLockNet.SERedis;
using StackExchange.Redis;

namespace DotnetDevkit.Cache.Strategies.Redis;

internal class RedisCacheStrategy : ICacheStrategy
{
    private readonly RedisCacheProviderOptions _options;
    private readonly ILogger<ICacheProvider> _logger;
    private static ConnectionMultiplexer? _connectionMultiplexer;
    private static RedLockFactory? _redLockFactory;
    private static bool _isInitialized;

    private readonly IDatabase? _redis;

    public RedisCacheStrategy(
        IOptions<CacheProviderOptions> options,
        ILogger<ICacheProvider> logger
    )
    {
        ArgumentNullException.ThrowIfNull(options.Value.RedisConfiguration);
        _options = options.Value.RedisConfiguration;

        var redisConnectionMissing = _options.Configuration is null &&
                                     _options.ConfigurationOptions is null;

        if (redisConnectionMissing)
        {
            throw new ArgumentException("Redis configuration is missing.");
        }

        if (_isInitialized is false)
        {
            _isInitialized = true;
            TryConnect(_options);
        }

        _logger = logger;
        _redis = _connectionMultiplexer?.GetDatabase();
    }

    public static CacheStrategy GetCacheStrategy() => CacheStrategy.Redis;
    public int GetPriority() => 50;


    public async Task<CacheData<T>?> GetAsync<T>(string key, CancellationToken cancellationToken)
    {
        if (_redis is null)
        {
            return null;
        }

        CacheData<T>? data = null;
        var stringValue = await _redis.StringGetAsync(key);
        if (stringValue.HasValue && string.IsNullOrEmpty(stringValue) is false)
        {
            data = JsonConvert.DeserializeObject<CacheData<T>>(stringValue!, CacheProvider.SerializerSettings);
        }

        return data;
    }

    public async Task<T> SetAsync<T>(string key, CacheData<T> data, CancellationToken cancellationToken)
    {
        if (_redis is null || _redLockFactory is null)
        {
            return data.Value;
        }

        var resource = $"lock:{key}";
        var expiry = TimeSpan.FromMilliseconds(_options.LockExpiryMs);
        var wait = TimeSpan.FromMilliseconds(_options.LockWaitMs);
        var retry = TimeSpan.FromMilliseconds(_options.LockRetryIntervalMs);
        await using var redLock =
            await _redLockFactory.CreateLockAsync(resource, expiry, wait, retry, cancellationToken);

        if (redLock.IsAcquired is false)
        {
            throw new RedisCommandException("Unable to acquire lock");
        }

        var cachedValue = await GetAsync<T>(key, cancellationToken);
        if (cachedValue is not null && cachedValue.Context.SlidingTimespan is null)
        {
            return cachedValue.Value;
        }

        ArgumentNullException.ThrowIfNull(data.Context.AbsoluteExpiration);
        var duration = data.Context.AbsoluteExpiration.Value - DateTime.UtcNow;

        if (duration < TimeSpan.FromSeconds(1))
        {
            return data.Value;
        }

        await _redis.StringSetAsync(
            key: key,
            value: Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(data, CacheProvider.SerializerSettings)),
            expiry: data.Context.AbsoluteExpiration.Value - DateTime.UtcNow
        );

        return data.Value;
    }

    public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        if (_redis is null)
        {
            return;
        }

        try
        {
            await _redis.KeyDeleteAsync(key);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "{Message}", e.Message);
        }
    }

    public async Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default)
    {
        if (_redis is null)
        {
            return false;
        }

        try
        {
            return await _redis.KeyExistsAsync(key);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "{Message}", e.Message);
            return false;
        }
    }

    public static void TryConnect(RedisCacheProviderOptions? options)
    {
        ArgumentNullException.ThrowIfNull(options);

        if (_connectionMultiplexer is not null && _redLockFactory is not null)
        {
            return;
        }

        var configuration = options.Configuration;
        var configurationOptions = options.ConfigurationOptions;

        try
        {
            if (configuration is not null)
            {
                _connectionMultiplexer = ConnectionMultiplexer.Connect(configuration);
            }
            else if (configurationOptions is not null)
            {
                _connectionMultiplexer = ConnectionMultiplexer.Connect(configurationOptions);
            }
            else
            {
                throw new ArgumentNullException(nameof(RedisCacheOptions.Configuration),
                    "redis configuration is missing");
            }

            _redLockFactory = RedLockFactory.Create([_connectionMultiplexer]);
        }
        catch (RedisException)
        {
            // ignore redis exceptions
        }
    }
}
