using Microsoft.Extensions.Caching.StackExchangeRedis;

namespace DotnetDevkit.Cache.Models;

public class RedisCacheProviderOptions : RedisCacheOptions
{
    public int LockExpiryMs { get; set; } = 500;
    public int LockWaitMs { get; set; } = 600;
    public int LockRetryIntervalMs { get; set; } = 100;
}
