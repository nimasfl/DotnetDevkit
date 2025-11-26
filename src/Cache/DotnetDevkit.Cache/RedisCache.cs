using Microsoft.Extensions.Options;

namespace DotnetDevkit.Cache;

using Abstractions;
using RedLockNet;
using RedLockNet.SERedis;
using RedLockNet.SERedis.Configuration;
using StackExchange.Redis;
using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

public class RedisCache : ICache, IDisposable
{
    internal volatile ConnectionMultiplexer? _redis;
    internal volatile IDistributedLockFactory? _redLockFactory;
    private readonly RedisCacheOptions _options;
    internal readonly Func<Task<ConnectionMultiplexer?>>? _connectionFactory;
    internal int _connectInProgress;

    public RedisCache(ConnectionMultiplexer? redisConnection, IOptions<RedisCacheOptions> options,
        Func<Task<ConnectionMultiplexer?>>? connectionFactory = null)
    {
        _options = options.Value;
        ArgumentNullException.ThrowIfNull(_options);
        _redis = redisConnection;
        _connectionFactory = connectionFactory;

        TryInitializeRedLock();

        if (_redis == null && _connectionFactory != null)
        {
            Task.Run(TryConnectAndInitAsync);
        }
    }

    public async Task<CacheResult<T>> GetAsync<T>(string key, CancellationToken cancellationToken = default)
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
                    var deserialized = JsonSerializer.Deserialize<T>(rawStr);
                    if (deserialized != null || typeof(T).IsValueType)
                    {
                        return CacheResult<T>.Some(deserialized!);
                    }
                }
            }
        }

        return CacheResult<T>.None;
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? absoluteExpiry = null,
        CancellationToken cancellationToken = default)
    {
        var expiry = absoluteExpiry ?? _options.DefaultExpiry;
        var redis = _redis;
        if (redis is { IsConnected: true })
        {
            var db = redis.GetDatabase();
            var json = JsonSerializer.Serialize(value);
            await db.StringSetAsync(key, json, expiry).ConfigureAwait(false);
        }
    }

    public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        var redis = _redis;
        if (redis is { IsConnected: true })
        {
            var db = redis.GetDatabase();
            await db.KeyDeleteAsync(key).ConfigureAwait(false);
        }
    }

    public async Task<CacheResult<T>> GetOrAddAsync<T>(string key, Func<CancellationToken, Task<T>> factory,
        TimeSpan? absoluteExpiry = null, CancellationToken cancellationToken = default)
    {
        var existing = await GetAsync<T>(key, cancellationToken).ConfigureAwait(false);
        if (existing.HasValue)
        {
            return existing;
        }

        var redis = _redis;
        if (redis is not { IsConnected: true })
        {
            return CacheResult<T>.None;
        }

        var expiry = absoluteExpiry ?? _options.DefaultExpiry;
        var lockKey = $"{key}:lock";

        if (_redLockFactory != null)
        {
            try
            {
                await using var redLock = await _redLockFactory.CreateLockAsync(lockKey, _options.LockExpiry,
                    _options.LockWait, TimeSpan.FromMilliseconds(200)).ConfigureAwait(false);
                if (redLock.IsAcquired)
                {
                    var afterAcquire = await GetAsync<T>(key, cancellationToken).ConfigureAwait(false);
                    if (afterAcquire.HasValue)
                    {
                        return afterAcquire;
                    }

                    var result = await ExecuteFactorySafely(factory, cancellationToken).ConfigureAwait(false);
                    await SetAsync(key, result, expiry, cancellationToken).ConfigureAwait(false);
                    return result;
                }
                else
                {
                    var sw = System.Diagnostics.Stopwatch.StartNew();
                    while (sw.Elapsed < _options.LockWait)
                    {
                        await Task.Delay(100, cancellationToken).ConfigureAwait(false);
                        var peek = await GetAsync<T>(key, cancellationToken).ConfigureAwait(false);
                        if (peek.HasValue)
                        {
                            return peek;
                        }
                    }

                    return await ExecuteFactorySafely(factory, cancellationToken).ConfigureAwait(false);
                }
            }
            catch
            {
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

    public async Task TryConnectAndInitAsync()
    {
        if (Interlocked.Exchange(ref _connectInProgress, 1) == 1) return;

        try
        {
            if (_connectionFactory == null) return;

            try
            {
                var candidate = await _connectionFactory().ConfigureAwait(false);
                if (candidate != null)
                {
                    var previous = _redis;
                    _redis = candidate;

                    try
                    {
                        previous?.Dispose();
                    }
                    catch
                    {
                        // ignore
                    }

                    TryInitializeRedLock();
                }
            }
            catch
            {
                // swallow - silent retry behavior
            }
        }
        finally
        {
            Interlocked.Exchange(ref _connectInProgress, 0);
        }
    }

    public void TryInitializeRedLock()
    {
        try
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
                    catch
                    {
                        // ignore
                    }
                }

                _redLockFactory = RedLockFactory.Create(multiplexers);
            }
        }
        catch
        {
            _redLockFactory = null;
        }
    }

    public void Dispose()
    {
        if (_redLockFactory is IDisposable disposable)
        {
            try
            {
                disposable.Dispose();
            }
            catch
            {
                // ignore
            }
        }

        try
        {
            _redis?.Dispose();
        }
        catch
        {
            // ignore
        }
    }
}
