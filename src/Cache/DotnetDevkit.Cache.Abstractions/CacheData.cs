using System.Diagnostics.CodeAnalysis;

namespace DotnetDevkit.Cache.Abstractions;

public record CacheData<T>(bool HasData, T Value)
{
    public static CacheData<TValue?> Empty<TValue>()
    {
        return new CacheData<TValue?>(false, default);
    }
}
