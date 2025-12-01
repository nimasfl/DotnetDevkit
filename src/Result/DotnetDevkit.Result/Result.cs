using System.Diagnostics.CodeAnalysis;
using DotnetDevkit.Result.Abstractions;
using DotnetDevkit.Result.Converters;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DotnetDevkit.Result;

[System.Text.Json.Serialization.JsonConverter(typeof(ResultSystemTextJsonConverterFactory))]
[Newtonsoft.Json.JsonConverter(typeof(ResultNewtonsoftConverter))]
public readonly record struct Result<TError> : IResult<TError>, IActionResult where TError : class
{
    public bool IsSuccess { get; init; } = true;

    [System.Text.Json.Serialization.JsonIgnore]
    [Newtonsoft.Json.JsonIgnore]
    public bool IsFailure => !IsSuccess;

    [System.Text.Json.Serialization.JsonIgnore]
    [Newtonsoft.Json.JsonIgnore]
    private bool IsDefaultConstructed { get; } = false;

    public Result()
    {
        IsSuccess = false;
        Error = null;
        IsDefaultConstructed = true;
    }


    [MemberNotNullWhen(true, nameof(IsFailure))]
    [MemberNotNullWhen(false, nameof(IsSuccess))]
    public TError? Error => IsFailure
        ? field
        : throw new InvalidOperationException("the error of a success result cannot be accessed.");

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

[System.Text.Json.Serialization.JsonConverter(typeof(ResultSystemTextJsonConverterFactory))]
[Newtonsoft.Json.JsonConverter(typeof(ResultNewtonsoftConverter))]
public readonly record struct Result<TValue, TError> where TError : class
{
    public bool IsSuccess { get; init; } = true;

    [System.Text.Json.Serialization.JsonIgnore]
    [Newtonsoft.Json.JsonIgnore]
    public bool IsFailure => !IsSuccess;

    [System.Text.Json.Serialization.JsonIgnore]
    [Newtonsoft.Json.JsonIgnore]
    private bool IsDefaultConstructed { get; } = false;

    private Result()
    {
    }

    private Result(TError Error) : base(Error)
    {
    }

    private Result(TValue value)
    {
        Value = value;
    }


    [NotNull]
    public TValue Value => IsSuccess
        ? field!
        : throw new InvalidOperationException("The value of a failure result can't be accessed.");


    public static Result<TValue, TError> Success(TValue value) => new(value);
    public static Result<TValue, TError> Begin(TValue value) => Success(value);
    public new static Result<TValue, TError> Failure(TError error) => new(error);
    public static implicit operator Result<TValue, TError>(TValue value) => Success(value);
    public static implicit operator Result<TValue, TError>(TError error) => Failure(error);

    public override async Task ExecuteResultAsync(ActionContext context)
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
