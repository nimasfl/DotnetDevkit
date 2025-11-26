using System.Diagnostics.CodeAnalysis;

namespace DotnetDevkit.Cache.Abstractions;

public readonly struct CacheResult<T>(T? value, bool hasValue)
{
    [MemberNotNullWhen(true, nameof(Value))]
    public bool HasValue { get; } = hasValue;
    public T? Value => value;
    public static CacheResult<T> None => new(default, false);
    public static CacheResult<T> Some(T value) => new(value, true);

    public static implicit operator CacheResult<T>(T? value) =>
        value is null ? None : Some(value);
}
