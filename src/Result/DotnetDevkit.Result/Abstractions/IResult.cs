using Microsoft.AspNetCore.Mvc;

namespace DotnetDevkit.Result.Abstractions;

public interface IResult<out TError> : IActionResult where TError : class
{
    public bool IsSuccess { get; }

    public bool IsFailure => !IsSuccess;

    public TError? Error { get; }
}

public interface IResult<out TValue, out TError> : IResult<TError> where TError : class
{
    public TValue Value { get; }
}
