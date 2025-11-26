using Microsoft.Extensions.Caching.StackExchangeRedis;

namespace DotnetDevkit.Cache;

using System;

public class SafeRedisCacheOptions: RedisCacheOptions
{
    public double DefaultExpiryMs { get; set; } = Convert.ToInt32(TimeSpan.FromMinutes(5).TotalMilliseconds);
    public double LockExpiryMs { get; set; } = Convert.ToInt32(TimeSpan.FromSeconds(30).TotalMilliseconds);
    public double LockWaitMs { get; set; } = Convert.ToInt32(TimeSpan.FromSeconds(10).TotalMilliseconds);
    public int HealthCheckIntervalSec { get; set; } = Convert.ToInt32(TimeSpan.FromSeconds(30).TotalMilliseconds);
    public int RetryBaseDelayMs { get; set; } = Convert.ToInt32(TimeSpan.FromSeconds(2).TotalMilliseconds);
    public int RetryMaxDelayMs { get; set; } = Convert.ToInt32(TimeSpan.FromSeconds(10).TotalMilliseconds);
}
