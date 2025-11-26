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
    private volatile ConnectionMultiplexer? _redis;
    private volatile IDistributedLockFactory? _redLockFactory;
    private readonly RedisCacheOptions _options;
    private readonly Func<Task<ConnectionMultiplexer?>>? _connectionFactory;
    private int _connectInProgress;

    public RedisCache(ConnectionMultiplexer? redisConnection, RedisCacheOptions options,
        Func<Task<ConnectionMultiplexer?>>? connectionFactory = null)
    {
        _options = options ?? new RedisCacheOptions();
        _redis = redisConnection;
        _connectionFactory = connectionFactory;

        TryInitializeRedLock();

        if (_redis == null && _connectionFactory != null)
        {
            Task.Run(TryConnectAndInitAsync);
        }
    }

    public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
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
                    if (rawStr is not null)
                    {
                        return JsonSerializer.Deserialize<T>(rawStr);
                    }
                }
            }
        }
        catch
        {
            // Swallow and trigger background reconnect attempt (non-blocking).
            FireAndForgetReconnect();
        }

        // Per requirement: don't cache in memory and act as if calls are no-ops when Redis is unreachable.
        return default;
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? absoluteExpiry = null,
        CancellationToken cancellationToken = default)
    {
        var expiry = absoluteExpiry ?? _options.DefaultExpiry;
        try
        {
            var redis = _redis;
            if (redis != null && redis.IsConnected)
            {
                var db = redis.GetDatabase();
                var json = JsonSerializer.Serialize(value);
                await db.StringSetAsync(key, json, expiry).ConfigureAwait(false);
                return;
            }
        }
        catch
        {
            // Swallow and trigger background reconnect attempt (non-blocking).
            FireAndForgetReconnect();
        }

        // Do nothing when Redis is unreachable (no memory caching).
    }

    public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        try
        {
            var redis = _redis;
            if (redis != null && redis.IsConnected)
            {
                var db = redis.GetDatabase();
                await db.KeyDeleteAsync(key).ConfigureAwait(false);
            }
        }
        catch
        {
            // Swallow and trigger background reconnect attempt (non-blocking).
            FireAndForgetReconnect();
        }

        // Do nothing else when Redis is unreachable.
    }

    public async Task<T> GetOrCreateAsync<T>(string key, Func<CancellationToken, Task<T>> factory,
        TimeSpan? absoluteExpiry = null, CancellationToken cancellationToken = default)
    {
        var existing = await GetAsync<T>(key, cancellationToken).ConfigureAwait(false);
        if (existing != null && !existing.Equals(default(T)!))
        {
            return existing;
        }

        var expiry = absoluteExpiry ?? _options.DefaultExpiry;
        var lockKey = $"{key}:lock";

        if (_redLockFactory != null)
        {
            try
            {
                using (var redLock = await _redLockFactory.CreateLockAsync(lockKey, _options.LockExpiry,
                           _options.LockWait, TimeSpan.FromMilliseconds(200)).ConfigureAwait(false))
                {
                    if (redLock.IsAcquired)
                    {
                        var afterAcquire = await GetAsync<T>(key, cancellationToken).ConfigureAwait(false);
                        if (afterAcquire != null && !afterAcquire.Equals(default(T)!))
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
                            if (peek != null && !peek.Equals(default(T)!))
                            {
                                return peek;
                            }
                        }

                        return await ExecuteFactorySafely(factory, cancellationToken).ConfigureAwait(false);
                    }
                }
            }
            catch
            {
                // RedLock interaction failed -> compute result and attempt to set (set will swallow failures).
                var result = await ExecuteFactorySafely(factory, cancellationToken).ConfigureAwait(false);
                await SetAsync(key, result, expiry, cancellationToken).ConfigureAwait(false);
                return result;
            }
        }
        else
        {
            // No distributed lock available: compute and attempt to set.
            var result = await ExecuteFactorySafely(factory, cancellationToken).ConfigureAwait(false);
            await SetAsync(key, result, expiry, cancellationToken).ConfigureAwait(false);
            return result;
        }
    }

    private static async Task<T> ExecuteFactorySafely<T>(Func<CancellationToken, Task<T>> factory,
        CancellationToken cancellationToken)
    {
        // Factory exceptions should propagate to the caller so they can handle domain errors.
        return await factory(cancellationToken).ConfigureAwait(false);
    }

    private void TryInitializeRedLock()
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

    private void FireAndForgetReconnect()
    {
        if (_connectionFactory != null)
        {
            // Start a background reconnect attempt without awaiting.
            Task.Run(() => TryConnectAndInitAsync());
        }
    }

    private async Task TryConnectAndInitAsync()
    {
        // Ensure a single background connect runs at a time.
        if (Interlocked.Exchange(ref _connectInProgress, 1) == 1) return;

        try
        {
            if (_connectionFactory == null) return;

            try
            {
                var candidate = await _connectionFactory().ConfigureAwait(false);
                if (candidate != null)
                {
                    // Replace current multiplexer in a thread-safe manner.
                    var previous = _redis;
                    _redis = candidate;

                    try
                    {
                        previous?.Dispose();
                    }
                    catch
                    {
                        //ignore
                    }

                    TryInitializeRedLock();
                }
            }
            catch
            {
                // Swallow connection exceptions; caller requirement is to try silently.
            }
        }
        finally
        {
            Interlocked.Exchange(ref _connectInProgress, 0);
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
            }
        }

        try
        {
            _redis?.Dispose();
        }
        catch
        {
        }
    }
}
