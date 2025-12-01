namespace DotnetDevkit.Result.Extensions;

public static partial class ResultExtensions
{
    /// <param name="result"></param>
    /// <typeparam name="TIn"></typeparam>
    /// <typeparam name="TError"></typeparam>
    extension<TIn, TError>(Result<TIn, TError> result) where TError : class
    {
        /// <summary>
        /// Executes the delegate and return the previous result output.
        /// </summary>
        /// <param name="func"></param>
        /// <returns></returns>
        public Result<TIn, TError> Tap(Action<TIn> func)
        {
            if (result.IsFailure)
            {
                return result.Error;
            }

            func(result.Value);
            return result.Value;
        }

        /// <summary>
        /// Executes the delegate and return the previous result output.
        /// </summary>
        /// <param name="func"></param>
        /// <returns></returns>
        public async Task<Result<TIn, TError>> Tap(Func<TIn, Task> func)
        {
            if (result.IsFailure)
            {
                return result.Error;
            }

            await func(result.Value);
            return result.Value;
        }
    }

    /// <summary>
    /// Executes the delegate and return the previous result output.
    /// </summary>
    /// <param name="result"></param>
    /// <param name="func"></param>
    /// <typeparam name="TIn"></typeparam>
    /// <typeparam name="TError"></typeparam>
    /// <returns></returns>
    public static async Task<Result<TIn, TError>> Tap<TIn, TError>(this Task<Result<TIn, TError>> result, Action<TIn> func)
        where TError : class
    {
        var waitedResult = await result;
        if (waitedResult.IsFailure)
        {
            return waitedResult.Error;
        }

        func(waitedResult.Value);
        return waitedResult.Value;
    }

    /// <summary>
    /// Executes the delegate and return the previous result output.
    /// </summary>
    /// <param name="result"></param>
    /// <param name="func"></param>
    /// <typeparam name="TIn"></typeparam>
    /// <typeparam name="TError"></typeparam>
    /// <returns></returns>
    public static async Task<Result<TIn, TError>> Tap<TIn, TError>(this Task<Result<TIn, TError>> result,
        Func<TIn, Task> func)
        where TError : class
    {
        var waitedResult = await result;
        if (waitedResult.IsFailure)
        {
            return waitedResult.Error;
        }

        await func(waitedResult.Value);
        return waitedResult.Value;
    }

    /// <summary>
    /// Executes the delegate and return the previous result output.
    /// </summary>
    /// <param name="result"></param>
    /// <param name="func"></param>
    /// <typeparam name="TIn"></typeparam>
    /// <typeparam name="TError"></typeparam>
    /// <returns></returns>
    public static Result<TIn, TError> Tap<TIn, TError>(this Result<TIn, TError> result, Action func) where TError : class
    {
        if (result.IsFailure)
        {
            return result.Error;
        }

        func();
        return result.Value;
    }

    /// <summary>
    /// Executes the delegate and return the previous result output.
    /// </summary>
    /// <param name="result"></param>
    /// <param name="func"></param>
    /// <typeparam name="TIn"></typeparam>
    /// <typeparam name="TError"></typeparam>
    /// <returns></returns>
    public static async Task<Result<TIn, TError>> Tap<TIn, TError>(this Result<TIn, TError> result, Func<Task> func)
        where TError : class
    {
        if (result.IsFailure)
        {
            return result.Error;
        }

        await func();
        return result.Value;
    }

    /// <summary>
    /// Executes the delegate and return the previous result output.
    /// </summary>
    /// <param name="result"></param>
    /// <param name="func"></param>
    /// <typeparam name="TIn"></typeparam>
    /// <typeparam name="TError"></typeparam>
    /// <returns></returns>
    public static async Task<Result<TIn, TError>> Tap<TIn, TError>(this Task<Result<TIn, TError>> result, Action func)
        where TError : class
    {
        var waitedResult = await result;
        if (waitedResult.IsFailure)
        {
            return waitedResult.Error;
        }

        func();
        return waitedResult.Value;
    }

    /// <summary>
    /// Executes the delegate and return the previous result output.
    /// </summary>
    /// <param name="result"></param>
    /// <param name="func"></param>
    /// <typeparam name="TIn"></typeparam>
    /// <typeparam name="TError"></typeparam>
    /// <returns></returns>
    public static async Task<Result<TIn, TError>> Tap<TIn, TError>(this Task<Result<TIn, TError>> result, Func<Task> func)
        where TError : class
    {
        var waitedResult = await result;
        if (waitedResult.IsFailure)
        {
            return waitedResult.Error;
        }

        await func();
        return waitedResult.Value;
    }

    /// <summary>
    /// Executes the delegate and return the previous result output.
    /// </summary>
    /// <param name="result"></param>
    /// <param name="func"></param>
    /// <typeparam name="TError"></typeparam>
    /// <returns></returns>
    public static Result<TError> Tap<TError>(this Result<TError> result, Action func) where TError : class
    {
        if (result.IsFailure)
        {
            return result.Error;
        }

        func();
        return result;
    }

    /// <summary>
    /// Executes the delegate and return the previous result output.
    /// </summary>
    /// <param name="result"></param>
    /// <param name="func"></param>
    /// <typeparam name="TError"></typeparam>
    /// <returns></returns>
    public static async Task<Result<TError>> Tap<TError>(this Result<TError> result, Func<Task> func) where TError : class
    {
        if (result.IsFailure)
        {
            return result.Error;
        }

        await func();
        return result;
    }

    /// <summary>
    /// Executes the delegate and return the previous result output.
    /// </summary>
    /// <param name="result"></param>
    /// <param name="func"></param>
    /// <typeparam name="TError"></typeparam>
    /// <returns></returns>
    public static async Task<Result<TError>> Tap<TError>(this Task<Result<TError>> result, Action func) where TError : class
    {
        var waitedResult = await result;
        if (waitedResult.IsFailure)
        {
            return waitedResult.Error;
        }

        func();
        return waitedResult;
    }

    /// <summary>
    /// Executes the delegate and return the previous result output.
    /// </summary>
    /// <param name="result"></param>
    /// <param name="func"></param>
    /// <typeparam name="TError"></typeparam>
    /// <returns></returns>
    public static async Task<Result<TError>> Tap<TError>(this Task<Result<TError>> result, Func<Task> func)
        where TError : class
    {
        var waitedResult = await result;
        if (waitedResult.IsFailure)
        {
            return waitedResult.Error;
        }

        await func();
        return waitedResult;
    }
}
