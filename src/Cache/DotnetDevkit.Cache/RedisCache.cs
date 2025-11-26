using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RedLockNet;
using RedLockNet.SERedis;
using RedLockNet.SERedis.Configuration;
using StackExchange.Redis;
using System.Text.Json;
using DotnetDevkit.Cache.Abstractions;

namespace DotnetDevkit.Cache;

public class RedisCache(
    IOptions<SafeRedisCacheOptions> options,
    Func<Task<ConnectionMultiplexer?>>? connectionFactory,
    ILogger<RedisCache> logger)
    : ICache, IDisposable
{
    private volatile ConnectionMultiplexer? _redis;
    private volatile IDistributedLockFactory? _redLockFactory;
    private readonly SemaphoreSlim _connectLock = new(1, 1);
    private readonly JsonSerializerOptions _jsonOptions = new();

    public bool IsConnected => _redis is { IsConnected: true };

    public async Task EnsureConnectedAsync(CancellationToken cancellationToken = default)
    {
        if (connectionFactory == null)
        {
            return;
        }

        await _connectLock.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            if (IsConnected)
            {
                return;
            }

            ConnectionMultiplexer? candidate = null;
            try
            {
                candidate = await connectionFactory().ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Exception thrown while obtaining ConnectionMultiplexer from factory.");
            }

            if (candidate is { IsConnected: true })
            {
                var previous = _redis;
                _redis = candidate;

                try
                {
                    previous?.Dispose();
                }
                catch (Exception ex)
                {
                    logger.LogDebug(ex, "Error disposing previous ConnectionMultiplexer.");
                }

                try
                {
                    InitializeRedLock();
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "RedLock initialization failed after establishing Redis connection.");
                    _redLockFactory = null;
                }

                logger.LogInformation("Redis connected and RedLock initialized.");
            }
            else
            {
                logger.LogDebug("Connection factory returned null or disconnected instance.");
            }
        }
        finally
        {
            _connectLock.Release();
        }
    }

    private void InitializeRedLock()
    {
        var redis = _redis;
        if (redis is { IsConnected: true })
        {
            var multiplexers = new[] { new RedLockMultiplexer(redis) };

            if (_redLockFactory is IDisposable prevDisposable)
            {
                try
                {
                    prevDisposable.Dispose();
                }
                catch (Exception)
                {
                    // ignore
                }
            }

            _redLockFactory = RedLockFactory.Create(multiplexers);
        }
        else
        {
            _redLockFactory = null;
        }
    }

    public async Task<CacheResult<T>> GetAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        try
        {
            var redis = _redis;
            if (redis is { IsConnected: true })
            {
                var db = redis.GetDatabase();
                var raw = await db.StringGetAsync(key).ConfigureAwait(false);
                if (raw.HasValue)
                {
                    var rawStr = raw.ToString();
                    if (rawStr != null)
                    {
                        var deserialized = JsonSerializer.Deserialize<T>(rawStr, _jsonOptions);
                        if (deserialized != null || typeof(T).IsValueType)
                        {
                            return CacheResult<T>.Some(deserialized!);
                        }
                    }
                }
            }
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            logger.LogDebug(ex, "GetAsync failed for key {Key}.", key);
        }

        return CacheResult<T>.None;
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? absoluteExpiry = null,
        CancellationToken cancellationToken = default)
    {
        var expiry = absoluteExpiry ?? TimeSpan.FromMilliseconds(options.Value.DefaultExpiryMs);

        try
        {
            var redis = _redis;
            if (redis is { IsConnected: true })
            {
                var db = redis.GetDatabase();
                var json = JsonSerializer.Serialize(value, _jsonOptions);
                await db.StringSetAsync(key, json, expiry).ConfigureAwait(false);
            }
            else
            {
                logger.LogDebug("SetAsync skipped because Redis is not connected. Key={Key}", key);
            }
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            logger.LogDebug(ex, "SetAsync failed for key {Key}.", key);
        }
    }

    public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        try
        {
            var redis = _redis;
            if (redis is { IsConnected: true })
            {
                var db = redis.GetDatabase();
                await db.KeyDeleteAsync(key).ConfigureAwait(false);
            }
            else
            {
                logger.LogDebug("RemoveAsync skipped because Redis is not connected. Key={Key}", key);
            }
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            logger.LogDebug(ex, "RemoveAsync failed for key {Key}.", key);
        }
    }

    public async Task<T> GetOrAddAsync<T>(string key, Func<CancellationToken, Task<T>> factory,
        TimeSpan? absoluteExpiry = null, CancellationToken cancellationToken = default)
    {
        var existing = await GetAsync<T>(key, cancellationToken).ConfigureAwait(false);
        if (existing.HasValue) return existing.Value;

        var redis = _redis;
        if (redis is not { IsConnected: true })
        {
            return await ExecuteFactorySafely(factory, cancellationToken).ConfigureAwait(false);
        }

        var expiry = absoluteExpiry ?? TimeSpan.FromMilliseconds(options.Value.DefaultExpiryMs);
        var lockKey = $"{key}:lock";

        if (_redLockFactory != null)
        {
            try
            {
                await using var redLock = await _redLockFactory.CreateLockAsync(lockKey,
                        TimeSpan.FromMilliseconds(options.Value.LockExpiryMs),
                        TimeSpan.FromMilliseconds(options.Value.LockWaitMs),
                        TimeSpan.FromMilliseconds(200))
                    .ConfigureAwait(false);

                if (redLock.IsAcquired)
                {
                    var afterAcquire = await GetAsync<T>(key, cancellationToken).ConfigureAwait(false);
                    if (afterAcquire.HasValue) return afterAcquire.Value;

                    var result = await ExecuteFactorySafely(factory, cancellationToken).ConfigureAwait(false);
                    await SetAsync(key, result, expiry, cancellationToken).ConfigureAwait(false);
                    return result;
                }
                else
                {
                    var sw = System.Diagnostics.Stopwatch.StartNew();
                    while (sw.Elapsed < TimeSpan.FromMilliseconds(options.Value.LockWaitMs))
                    {
                        await Task.Delay(100, cancellationToken).ConfigureAwait(false);
                        var peek = await GetAsync<T>(key, cancellationToken).ConfigureAwait(false);
                        if (peek.HasValue) return peek.Value;
                    }

                    return await ExecuteFactorySafely(factory, cancellationToken).ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "RedLock or GetOrAdd logic failed for key {Key}; falling back to factory.", key);
                var result = await ExecuteFactorySafely(factory, cancellationToken).ConfigureAwait(false);
                await SetAsync(key, result, expiry, cancellationToken).ConfigureAwait(false);
                return result;
            }
        }
        else
        {
            var result = await ExecuteFactorySafely(factory, cancellationToken).ConfigureAwait(false);
            await SetAsync(key, result, expiry, cancellationToken).ConfigureAwait(false);
            return result;
        }
    }

    private static async Task<T> ExecuteFactorySafely<T>(Func<CancellationToken, Task<T>> factory,
        CancellationToken cancellationToken)
    {
        return await factory(cancellationToken).ConfigureAwait(false);
    }

    public void Dispose()
    {
        try
        {
            if (_redLockFactory is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }
        catch (Exception ex)
        {
            logger.LogDebug(ex, "Error disposing redlock factory.");
        }

        try
        {
            _redis?.Dispose();
        }
        catch (Exception ex)
        {
            logger.LogDebug(ex, "Error disposing ConnectionMultiplexer.");
        }

        _connectLock.Dispose();
    }
}
