namespace DotnetDevkit.Cache.Models;

public record CacheData(CacheContext Context, object? Value);
public record CacheData<T>(CacheContext Context, T Value);
