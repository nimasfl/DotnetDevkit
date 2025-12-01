namespace DotnetDevkit.Result.Extensions;

public static partial class ResultExtensions
{
    /// <summary>
    /// Transforms result to specified type based on the result status.
    /// </summary>
    /// <param name="result"></param>
    /// <param name="onSuccess"></param>
    /// <param name="onFailure"></param>
    /// <typeparam name="TIn"></typeparam>
    /// <typeparam name="TOut"></typeparam>
    /// <typeparam name="TError"></typeparam>
    /// <returns></returns>
    public static TOut Match<TIn, TOut, TError>(
        this Result<TIn, TError> result,
        Func<TIn, TOut> onSuccess,
        Func<TError, TOut> onFailure) where TError : class
    {
        return result.IsSuccess ? onSuccess(result.Value) : onFailure(result.Error);
    }

    /// <summary>
    /// Transforms result to specified type based on the result status.
    /// </summary>
    /// <param name="result"></param>
    /// <param name="onSuccess"></param>
    /// <param name="onFailure"></param>
    /// <typeparam name="TOut"></typeparam>
    /// <typeparam name="TError"></typeparam>
    /// <returns></returns>
    public static TOut Match<TOut, TError>(
        this Result<TError> result,
        Func<TOut> onSuccess,
        Func<TError, TOut> onFailure) where TError : class
    {
        return result.IsSuccess ? onSuccess() : onFailure(result.Error);
    }

    /// <summary>
    /// Transforms result to specified type based on the result status.
    /// </summary>
    /// <param name="resultAsync"></param>
    /// <param name="onSuccess"></param>
    /// <param name="onFailure"></param>
    /// <typeparam name="TOut"></typeparam>
    /// <typeparam name="TError"></typeparam>
    /// <returns></returns>
    public static async Task<TOut> Match<TOut, TError>(
        this Task<Result<TError>> resultAsync,
        Func<TOut> onSuccess,
        Func<TError, TOut> onFailure) where TError : class
    {
        var result = await resultAsync;
        return result.IsSuccess ? onSuccess() : onFailure(result.Error);
    }

    /// <summary>
    /// Transforms result to specified type based on the result status.
    /// </summary>
    /// <param name="resultAsync"></param>
    /// <param name="onSuccess"></param>
    /// <param name="onFailure"></param>
    /// <typeparam name="TIn"></typeparam>
    /// <typeparam name="TOut"></typeparam>
    /// <typeparam name="TError"></typeparam>
    /// <returns></returns>
    public static async Task<TOut> Match<TIn, TOut, TError>(
        this Task<Result<TIn, TError>> resultAsync,
        Func<TIn, TOut> onSuccess,
        Func<TError, TOut> onFailure) where TError : class
    {
        var result = await resultAsync;
        return result.IsSuccess ? onSuccess(result.Value) : onFailure(result.Error);
    }
}
