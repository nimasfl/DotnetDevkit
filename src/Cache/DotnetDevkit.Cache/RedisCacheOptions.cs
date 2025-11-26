using Microsoft.Extensions.Caching.StackExchangeRedis;

namespace DotnetDevkit.Cache;

using System;

public class SafeRedisCacheOptions: RedisCacheOptions
{
    public double DefaultExpiryMs { get; set; } = TimeSpan.FromMinutes(5).TotalMilliseconds;
    public double LockExpiryMs { get; set; } = TimeSpan.FromSeconds(30).TotalMilliseconds;
    public double LockWaitMs { get; set; } = TimeSpan.FromSeconds(10).TotalMilliseconds;
}
