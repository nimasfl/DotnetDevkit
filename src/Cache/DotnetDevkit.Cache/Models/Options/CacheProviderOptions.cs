using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices.Marshalling;
using DotnetDevkit.Cache.Abstractions;

namespace DotnetDevkit.Cache.Models;

public record CacheProviderOptions
{
    public string Prefix { get; init; } = string.Empty;
    public RedisCacheProviderOptions? RedisConfiguration { get; init; }
    public CacheStrategy Strategy { get; init; } = CacheStrategy.None;

    public static bool Validate(CacheProviderOptions options)
    {
        if (options.Strategy == CacheStrategy.None)
        {
            return false;
        }

        if (options is { Strategy: CacheStrategy.Redis, RedisConfiguration: null })
        {
            return false;
        }

        return true;
    }
}
