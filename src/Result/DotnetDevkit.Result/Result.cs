using DotnetDevkit.Result.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DotnetDevkit.Result;

public readonly record struct Result<TError> : IResult<TError>, IActionResult where TError : class
{
    public bool IsSuccess { get; init; } = true;

    public bool IsFailure => !IsSuccess;

    public TError Error { get; init; }

    public Result()
    {
        IsSuccess = true;
        Error = null!;
    }

    private Result(TError error)
    {
        IsSuccess = false;
        Error = error;
    }

    /// <summary>
    /// Begins the result chain for better readability. it just returns <see cref="Result{TError}.Success()"/>
    /// </summary>
    /// <typeparam name="TError"></typeparam>
    /// <returns></returns>
    public static Result<TError> Begin() => Success();

    public static Result<TError> Success() => new();
    public static Result<TError> Failure(TError error) => new(error);
    public static implicit operator Result<TError>(TError error) => new(error);

    public async Task ExecuteResultAsync(ActionContext context)
    {
        if (IsFailure && Error is IActionResult error)
        {
            await error.ExecuteResultAsync(context);
        }

        var objectResult = new ObjectResult(this)
        {
            ContentTypes = ["application/json"],
            StatusCode = StatusCodes.Status200OK
        };
        await objectResult.ExecuteResultAsync(context);
    }
}

public readonly record struct Result<TValue, TError> : IResult<TError>, IActionResult where TError : class
{
    public bool IsSuccess { get; init; } = true;

    public bool IsFailure => !IsSuccess;

    public TValue Value
    {
        get => IsSuccess ? field : default!;
        init;
    }

    public TError Error
    {
        get => !IsSuccess ? field : null!;
        init;
    }

    private Result(TError error)
    {
        IsSuccess = false;
        Error = error;
        Value = default!;
    }

    /// <summary>
    /// Begins the result chain for better readability. it just returns <see cref="Result{TError}.Success()"/>
    /// </summary>
    /// <typeparam name="TError"></typeparam>
    /// <returns></returns>
    public static Result<TError> Begin() => Success();

    public static Result<TError> Success() => new();

    private Result(TValue value)
    {
        Value = value;
        Error = null!;
        IsSuccess = true;
    }

    public static Result<TValue, TError> Success(TValue value) => new(value);
    public static Result<TValue, TError> Begin(TValue value) => Success(value);
    public static Result<TValue, TError> Failure(TError error) => new(error);
    public static implicit operator Result<TValue, TError>(TValue value) => Success(value);
    public static implicit operator Result<TValue, TError>(TError error) => Failure(error);

    public async Task ExecuteResultAsync(ActionContext context)
    {
        if (IsSuccess && Value is IActionResult result)
        {
            await result.ExecuteResultAsync(context);
        }
        else if (IsFailure && Error is IActionResult error)
        {
            await error.ExecuteResultAsync(context);
        }
        else
        {
            var objectResult = new ObjectResult(this)
            {
                ContentTypes = ["application/json"],
                StatusCode = StatusCodes.Status200OK
            };
            await objectResult.ExecuteResultAsync(context);
        }
    }
}
