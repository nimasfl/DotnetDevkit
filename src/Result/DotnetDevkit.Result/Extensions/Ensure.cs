namespace DotnetDevkit.Result.Extensions;

public static partial class ResultExtensions
{
    /// <summary>
    /// Executes the predicate. if predicate result is false returns specified error, otherwise returns the last result.
    /// </summary>
    /// <param name="result"></param>
    /// <param name="predicate"></param>
    /// <param name="error"></param>
    /// <typeparam name="TError"></typeparam>
    /// <returns></returns>
    public static async Task<Result<TError>> Ensure<TError>(this Result<TError> result, Func<Task<bool>> predicate,
        TError error) where TError : class
    {
        if (result.IsFailure)
        {
            return result.Error;
        }

        if (await predicate() is false)
        {
            return error;
        }

        return result;
    }

    /// <summary>
    /// Executes the predicate. if predicate result is false returns specified error, otherwise returns the last result.
    /// </summary>
    /// <param name="result"></param>
    /// <param name="predicate"></param>
    /// <param name="error"></param>
    /// <typeparam name="TIn"></typeparam>
    /// <typeparam name="TError"></typeparam>
    /// <returns></returns>
    public static Result<TIn, TError> Ensure<TIn, TError>(this Result<TIn, TError> result,
        Func<TIn, bool> predicate,
        TError error)
        where TError : class
    {
        if (result.IsFailure)
        {
            return result.Error;
        }

        if (predicate(result.Value) is false)
        {
            return error;
        }

        return result;
    }

    /// <summary>
    /// Executes the predicate. if predicate result is false returns specified error, otherwise returns the last result.
    /// </summary>
    /// <param name="result"></param>
    /// <param name="predicate"></param>
    /// <param name="error"></param>
    /// <typeparam name="TIn"></typeparam>
    /// <typeparam name="TError"></typeparam>
    /// <returns></returns>
    public static async Task<Result<TIn, TError>> Ensure<TIn, TError>(this Result<TIn, TError> result,
        Func<TIn, Task<bool>> predicate, TError error) where TError : class
    {
        if (result.IsFailure)
        {
            return result.Error;
        }

        if (await predicate(result.Value) is false)
        {
            return error;
        }

        return result;
    }

    /// <summary>
    /// Executes the predicate. if predicate result is false returns specified error, otherwise returns the last result.
    /// </summary>
    /// <param name="result"></param>
    /// <param name="predicate"></param>
    /// <param name="error"></param>
    /// <typeparam name="TError"></typeparam>
    /// <returns></returns>
    public static async Task<Result<TError>> Ensure<TError>(this Task<Result<TError>> result,
        Func<bool> predicate,
        TError error) where TError : class
    {
        var asyncResult = await result;
        if (asyncResult.IsFailure)
        {
            return asyncResult.Error;
        }

        if (predicate() is false)
        {
            return error;
        }

        return asyncResult;
    }

    /// <summary>
    /// Executes the predicate. if predicate result is false returns specified error, otherwise returns the last result.
    /// </summary>
    /// <param name="result"></param>
    /// <param name="predicate"></param>
    /// <param name="error"></param>
    /// <typeparam name="TError"></typeparam>
    /// <returns></returns>
    public static async Task<Result<TError>> Ensure<TError>(this Task<Result<TError>> result,
        Func<Task<bool>> predicate,
        TError error) where TError : class
    {
        var asyncResult = await result;
        if (asyncResult.IsFailure)
        {
            return asyncResult.Error;
        }

        if (await predicate() is false)
        {
            return error;
        }

        return asyncResult;
    }

    /// <summary>
    /// Executes the predicate. if predicate result is false returns specified error, otherwise returns the last result.
    /// </summary>
    /// <param name="result"></param>
    /// <param name="predicate"></param>
    /// <param name="error"></param>
    /// <typeparam name="TError"></typeparam>
    /// <returns></returns>
    public static Result<TError> Ensure<TError>(this Result<TError> result, Func<bool> predicate,
        TError error) where TError : class
    {
        if (result.IsFailure)
        {
            return result.Error;
        }

        if (predicate() is false)
        {
            return error;
        }

        return result;
    }

    /// <summary>
    /// Executes the predicate. if predicate result is false returns specified error, otherwise returns the last result.
    /// </summary>
    /// <param name="result"></param>
    /// <param name="predicate"></param>
    /// <param name="error"></param>
    /// <typeparam name="TIn"></typeparam>
    /// <typeparam name="TError"></typeparam>
    /// <returns></returns>
    public static async Task<Result<TIn, TError>> Ensure<TIn, TError>(this Task<Result<TIn, TError>> result,
        Func<TIn, bool> predicate,
        TError error)
        where TError : class
    {
        var asyncResult = await result;
        if (asyncResult.IsFailure)
        {
            return asyncResult.Error;
        }

        if (predicate(asyncResult.Value) is false)
        {
            return error;
        }

        return asyncResult;
    }

    /// <summary>
    /// Executes the predicate. if predicate result is false returns specified error, otherwise returns the last result.
    /// </summary>
    /// <param name="result"></param>
    /// <param name="predicate"></param>
    /// <param name="error"></param>
    /// <typeparam name="TIn"></typeparam>
    /// <typeparam name="TError"></typeparam>
    /// <returns></returns>
    public static async Task<Result<TIn, TError>> Ensure<TIn, TError>(this Task<Result<TIn, TError>> result,
        Func<TIn, Task<bool>> predicate, TError error) where TError : class
    {
        var asyncResult = await result;
        if (asyncResult.IsFailure)
        {
            return asyncResult.Error;
        }

        if (await predicate(asyncResult.Value) is false)
        {
            return error;
        }

        return asyncResult;
    }
}
