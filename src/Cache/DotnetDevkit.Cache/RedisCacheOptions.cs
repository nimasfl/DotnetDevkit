namespace DotnetDevkit.Cache;

using System;

public class RedisCacheOptions
{
    public TimeSpan DefaultExpiry { get; set; } = TimeSpan.FromMinutes(5);
    public TimeSpan LockExpiry { get; set; } = TimeSpan.FromSeconds(30);
    public TimeSpan LockWait { get; set; } = TimeSpan.FromSeconds(10);
    public string? InstanceName { get; set; }
    public bool UseMemoryFallback { get; set; } = true;
    public int MemoryFallbackSizeLimit { get; set; } = 1024; // optional
}
