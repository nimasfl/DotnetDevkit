namespace DotnetDevkit.Result.Extensions;

public static partial class ResultExtensions
{
    /// <param name="result"></param>
    /// <typeparam name="TError"></typeparam>
    extension<TError>(Result<TError> result) where TError : class
    {
        /// <summary>
        /// Executes the callback if the result is failure, otherwise return the last result.
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public async Task<Result<TError>> TapError(Func<Task> action)
        {
            if (result.IsFailure)
            {
                await action();
                return result.Error;
            }

            return result;
        }

        /// <summary>
        /// Executes the callback if the result is failure, otherwise return the last result.
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public Result<TError> TapError(Action action)
        {
            if (result.IsFailure)
            {
                action();
                return result.Error;
            }

            return result;
        }
    }

    /// <summary>
    /// Executes the callback if the result is failure, otherwise return the last result.
    /// </summary>
    /// <param name="result"></param>
    /// <param name="action"></param>
    /// <typeparam name="TIn"></typeparam>
    /// <typeparam name="TError"></typeparam>
    /// <returns></returns>
    public static Result<TIn, TError> TapError<TIn, TError>(this Result<TIn, TError> result, Action<TError> action)
        where TError : class
    {
        if (result.IsFailure)
        {
            action(result.Error);
            return result.Error;
        }

        return result.Value;
    }

    /// <summary>
    /// Executes the callback if the result is failure, otherwise return the last result.
    /// </summary>
    /// <param name="result"></param>
    /// <param name="action"></param>
    /// <typeparam name="TIn"></typeparam>
    /// <typeparam name="TError"></typeparam>
    /// <returns></returns>
    public static async Task<Result<TIn, TError>> TapError<TIn, TError>(this Result<TIn, TError> result,
        Func<TError, Task> action) where TError : class
    {
        if (result.IsFailure)
        {
            await action(result.Error);
            return result.Error;
        }

        return result.Value;
    }

    /// <summary>
    /// Executes the callback if the result is failure, otherwise return the last result.
    /// </summary>
    /// <param name="resultAsync"></param>
    /// <param name="action"></param>
    /// <typeparam name="TIn"></typeparam>
    /// <typeparam name="TError"></typeparam>
    /// <returns></returns>
    public static async Task<Result<TIn, TError>> TapError<TIn, TError>(this Task<Result<TIn, TError>> resultAsync,
        Func<TError, Task> action) where TError : class
    {
        var result = await resultAsync;
        if (result.IsFailure)
        {
            await action(result.Error);
            return result.Error;
        }

        return result.Value;
    }

    /// <summary>
    /// Executes the callback if the result is failure, otherwise return the last result.
    /// </summary>
    /// <param name="resultAsync"></param>
    /// <param name="action"></param>
    /// <typeparam name="TIn"></typeparam>
    /// <typeparam name="TError"></typeparam>
    /// <returns></returns>
    public static async Task<Result<TIn, TError>> TapError<TIn, TError>(this Task<Result<TIn, TError>> resultAsync,
        Action<TError> action) where TError : class
    {
        var result = await resultAsync;
        if (result.IsFailure)
        {
            action(result.Error);
            return result.Error;
        }

        return result.Value;
    }
}
