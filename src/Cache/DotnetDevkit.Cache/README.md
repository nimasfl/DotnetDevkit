# DotnetDevkit.Cache

Two packages that provide a small caching abstraction and a resilient Redis-backed implementation with distributed locking.

## Packages at a glance
- **DotnetDevkit.Cache.Abstractions**
  - `ICache` interface for `GetAsync`, `SetAsync`, `RemoveAsync`, and `GetOrAddAsync` with cancellation support.
  - `CacheResult<T>` value-or-miss struct with `HasValue`, `Value`, `Some`, `None`, and implicit conversion from `T`.
- **DotnetDevkit.Cache**
  - `RedisCache` implements `ICache` using `StackExchange.Redis` with JSON serialization via `System.Text.Json`.
  - `GetOrAddAsync` guarded by `RedLockNet` distributed locks to avoid thundering herd while still returning data if Redis is unavailable.
  - Graceful degradation: cache misses are returned instead of exceptions when Redis is down; factories still run.
  - `RedisCacheConnectionService` background worker keeps the connection warm with health checks and exponential backoff retries.
  - `SafeRedisCacheOptions` extends `RedisCacheOptions` with sensible defaults for expiry, locking, and reconnection.

## Installation
Add the packages you need from NuGet:
```
dotnet add package DotnetDevkit.Cache.Abstractions
# plus the Redis implementation
dotnet add package DotnetDevkit.Cache
```

## Configure and wire up RedisCache
Register the cache in DI and configure connection + behavior knobs.

```csharp
using DotnetDevkit.Cache;
using DotnetDevkit.Cache.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using StackExchange.Redis;

var builder = Host.CreateApplicationBuilder(args);

// Option 1: bind from configuration
builder.Services.Configure<SafeRedisCacheOptions>(
    builder.Configuration.GetSection("RedisCache"));

// Option 2: configure in code
builder.Services.Configure<SafeRedisCacheOptions>(o =>
{
    o.Configuration = "localhost:6379"; // from RedisCacheOptions
    o.InstanceName = "devkit:";
    o.DefaultExpiryMs = TimeSpan.FromMinutes(10).TotalMilliseconds;
    o.LockExpiryMs = TimeSpan.FromSeconds(30).TotalMilliseconds;
    o.LockWaitMs = TimeSpan.FromSeconds(10).TotalMilliseconds;
});

// Connection factory used by RedisCache and the background service
builder.Services.AddSingleton<Func<Task<ConnectionMultiplexer?>>>(_ =>
    () => ConnectionMultiplexer.ConnectAsync("localhost:6379"));

builder.Services.AddSingleton<RedisCache>();
builder.Services.AddSingleton<ICache>(sp => sp.GetRequiredService<RedisCache>());
builder.Services.AddHostedService<RedisCacheConnectionService>();

await builder.Build().RunAsync();
```

## Using the cache abstraction
`ICache` gives typed access with explicit cache-miss semantics.

```csharp
public class UserProfileService(ICache cache)
{
    public async Task<UserProfile?> TryGetProfileAsync(string userId, CancellationToken ct)
    {
        var cached = await cache.GetAsync<UserProfile>($"user:{userId}", ct);
        return cached.HasValue ? cached.Value : null;
    }

    public Task CacheProfileAsync(string userId, UserProfile profile, CancellationToken ct) =>
        cache.SetAsync($"user:{userId}", profile, TimeSpan.FromMinutes(30), ct);

    public Task EvictProfileAsync(string userId, CancellationToken ct) =>
        cache.RemoveAsync($"user:{userId}", ct);
}
```

### Get-or-add with distributed lock
`GetOrAddAsync` prevents thundering herd with RedLock and still returns data when Redis is unavailable.

```csharp
public class PricingService(ICache cache)
{
    public Task<PriceCard> GetPriceAsync(string sku, CancellationToken ct) =>
        cache.GetOrAddAsync(
            key: $"price:{sku}",
            factory: async token => await FetchPriceFromUpstreamAsync(sku, token),
            absoluteExpiry: TimeSpan.FromMinutes(5),
            cancellationToken: ct);
}
```

Behavior notes:
- If the value exists, it is returned immediately.
- If Redis is connected, a distributed lock on `${key}:lock` is attempted to serialize factory execution.
- If lock acquisition fails within the configured wait window, or Redis is down, the factory still runs and its result is returned (best-effort set when possible).

## Options
`SafeRedisCacheOptions` derives from `RedisCacheOptions`, so all standard StackExchange.Redis configuration applies (`Configuration`, `ConfigurationOptions`, `InstanceName`, etc.). Additional knobs:

- `DefaultExpiryMs` (default 5 minutes) — TTL used when `absoluteExpiry` is not provided.
- `LockExpiryMs` (default 30 seconds) — lease time for distributed locks.
- `LockWaitMs` (default 10 seconds) — maximum time spent trying to acquire the lock before falling back.
- `HealthCheckIntervalSec` (default 30 seconds) — delay between connection health checks once connected.
- `RetryBaseDelayMs` (default 2 seconds) — initial exponential backoff delay for reconnects.
- `RetryMaxDelayMs` (default 10 seconds) — cap for reconnect backoff.

## Behavior highlights
- JSON serialization uses `System.Text.Json` with default options; primitives and reference types are supported.
- All operations are cancellation-aware; `Get` returns `CacheResult.None` on cache miss or connectivity issues.
- Background connection loop performs health checks and retries with jitter to stabilize Redis connectivity.
- Disposal cleans up the Redis multiplexer, RedLock factory, and semaphores.

## Abstractions reference
- `ICache` — async cache contract with optional expiry per call and `GetOrAddAsync` factory helper.
- `CacheResult<T>` — explicit cache hit/miss wrapper with `HasValue` and implicit conversion from `T` for convenience.

## Testing
See `tests/DotnetDevkit.Cache.Test` for coverage of connection handling, locking behavior, and option defaults.
