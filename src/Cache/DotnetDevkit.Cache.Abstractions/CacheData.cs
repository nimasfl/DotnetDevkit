namespace DotnetDevkit.Cache.Abstractions;

public record CacheData(CacheContext Context, object? Value);
public record CacheData<T>(CacheContext Context, T Value);
