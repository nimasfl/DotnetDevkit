using DotnetDevkit.Result.Abstractions;

namespace DotnetDevkit.Result.Extensions;

public static partial class ResultExtensions
{
    /// <summary>
    /// Turns value into result only if it's not already is a result.
    /// </summary>
    /// <param name="value"></param>
    /// <typeparam name="TValue"></typeparam>
    /// <typeparam name="TError"></typeparam>
    /// <returns></returns>
    public static Result<TValue, TError> ToResult<TValue, TError>(this TValue value)
        where TError : class
    {
        if (value is IResult<TError> || value is Result<TValue, TError>)
        {
            return value;
        }

        return Result<TValue, TError>.Success(value);
    }
}
