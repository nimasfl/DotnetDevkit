namespace DotnetDevkit.Result.Extensions;

public static partial class ResultExtensions
{
    #region This = Result<TIn,TError>

    #region Func With Input

    #region Sync

    /// <summary>
    /// Executes the delegate and return its output.
    /// </summary>
    /// <param name="result"></param>
    /// <param name="func"></param>
    /// <typeparam name="TIn"></typeparam>
    /// <typeparam name="TOut"></typeparam>
    /// <typeparam name="TError"></typeparam>
    /// <returns></returns>
    public static Result<TOut, TError> Then<TIn, TOut, TError>(this Result<TIn, TError> result,
        Func<TIn, Result<TOut, TError>> func) where TError : class
    {
        return result.IsFailure ? result.Error : func(result.Value);
    }

    /// <summary>
    /// Executes the delegate and return its output.
    /// </summary>
    /// <param name="result"></param>
    /// <param name="func"></param>
    /// <typeparam name="TIn"></typeparam>
    /// <typeparam name="TOut"></typeparam>
    /// <typeparam name="TError"></typeparam>
    /// <returns></returns>
    public static Result<TOut, TError> Then<TIn, TOut, TError>(this Result<TIn, TError> result, Func<TIn, TOut> func)
        where TError : class
    {
        return result.IsFailure ? result.Error : func(result.Value);
    }

    /// <summary>
    /// Executes the delegate and return its output.
    /// </summary>
    /// <param name="result"></param>
    /// <param name="func"></param>
    /// <typeparam name="TIn"></typeparam>
    /// <typeparam name="TError"></typeparam>
    /// <returns></returns>
    public static Result<TIn, TError> Then<TIn, TError>(this Result<TIn, TError> result, Func<TIn, Result<TError>> func)
        where TError : class
    {
        if (result.IsFailure)
        {
            return result.Error;
        }

        var funcResult = func(result.Value);
        return funcResult.IsFailure ? funcResult.Error : result.Value;
    }

    #endregion

    #region Func Async

    /// <summary>
    /// Executes the delegate and return its output.
    /// </summary>
    /// <param name="result"></param>
    /// <param name="func"></param>
    /// <typeparam name="TIn"></typeparam>
    /// <typeparam name="TOut"></typeparam>
    /// <typeparam name="TError"></typeparam>
    /// <returns></returns>
    public static async Task<Result<TOut, TError>> Then<TIn, TOut, TError>(this Result<TIn, TError> result,
        Func<TIn, Task<Result<TOut, TError>>> func) where TError : class
    {
        if (result.IsFailure)
        {
            return result.Error;
        }

        return await func(result.Value);
    }

    /// <summary>
    /// Executes the delegate and return its output.
    /// </summary>
    /// <param name="result"></param>
    /// <param name="func"></param>
    /// <typeparam name="TIn"></typeparam>
    /// <typeparam name="TOut"></typeparam>
    /// <typeparam name="TError"></typeparam>
    /// <returns></returns>
    public static async Task<Result<TOut, TError>> Then<TIn, TOut, TError>(this Result<TIn, TError> result,
        Func<TIn, Task<TOut>> func)
        where TError : class
    {
        if (result.IsFailure)
        {
            return result.Error;
        }

        return await func(result.Value);
    }

    /// <summary>
    /// Executes the delegate and return its output.
    /// </summary>
    /// <param name="result"></param>
    /// <param name="func"></param>
    /// <typeparam name="TIn"></typeparam>
    /// <typeparam name="TError"></typeparam>
    /// <returns></returns>
    public static async Task<Result<TIn, TError>> Then<TIn, TError>(this Result<TIn, TError> result,
        Func<TIn, Task<Result<TError>>> func)
        where TError : class
    {
        if (result.IsFailure)
        {
            return result.Error;
        }

        var funcResult = await func(result.Value);
        return funcResult.IsFailure ? funcResult.Error : result.Value;
    }

    #endregion

    #region Input Result Async

    /// <summary>
    /// Executes the delegate and return its output.
    /// </summary>
    /// <param name="result"></param>
    /// <param name="func"></param>
    /// <typeparam name="TIn"></typeparam>
    /// <typeparam name="TOut"></typeparam>
    /// <typeparam name="TError"></typeparam>
    /// <returns></returns>
    public static async Task<Result<TOut, TError>> Then<TIn, TOut, TError>(this Task<Result<TIn, TError>> result,
        Func<TIn, Result<TOut, TError>> func) where TError : class
    {
        var waitedResult = await result;
        return waitedResult.IsFailure ? waitedResult.Error : func(waitedResult.Value);
    }

    /// <summary>
    /// Executes the delegate and return its output.
    /// </summary>
    /// <param name="result"></param>
    /// <param name="func"></param>
    /// <typeparam name="TIn"></typeparam>
    /// <typeparam name="TOut"></typeparam>
    /// <typeparam name="TError"></typeparam>
    /// <returns></returns>
    public static async Task<Result<TOut, TError>> Then<TIn, TOut, TError>(this Task<Result<TIn, TError>> result,
        Func<TIn, TOut> func)
        where TError : class
    {
        var waitedResult = await result;
        return waitedResult.IsFailure ? waitedResult.Error : func(waitedResult.Value);
    }

    /// <summary>
    /// Executes the delegate and return its output.
    /// </summary>
    /// <param name="result"></param>
    /// <param name="func"></param>
    /// <typeparam name="TIn"></typeparam>
    /// <typeparam name="TError"></typeparam>
    /// <returns></returns>
    public static async Task<Result<TIn, TError>> Then<TIn, TError>(this Task<Result<TIn, TError>> result,
        Func<TIn, Result<TError>> func)
        where TError : class
    {
        var waitedResult = await result;
        if (waitedResult.IsFailure)
        {
            return waitedResult.Error;
        }

        var funcResult = func(waitedResult.Value);
        return funcResult.IsFailure ? funcResult.Error : waitedResult.Value;
    }

    #endregion

    #region Async

    /// <summary>
    /// Executes the delegate and return its output.
    /// </summary>
    /// <param name="result"></param>
    /// <param name="func"></param>
    /// <typeparam name="TIn"></typeparam>
    /// <typeparam name="TOut"></typeparam>
    /// <typeparam name="TError"></typeparam>
    /// <returns></returns>
    public static async Task<Result<TOut, TError>> Then<TIn, TOut, TError>(this Task<Result<TIn, TError>> result,
        Func<TIn, Task<Result<TOut, TError>>> func) where TError : class
    {
        var waitedResult = await result;
        if (waitedResult.IsFailure)
        {
            return waitedResult.Error;
        }

        return await func(waitedResult.Value);
    }

    /// <summary>
    /// Executes the delegate and return its output.
    /// </summary>
    /// <param name="result"></param>
    /// <param name="func"></param>
    /// <typeparam name="TIn"></typeparam>
    /// <typeparam name="TOut"></typeparam>
    /// <typeparam name="TError"></typeparam>
    /// <returns></returns>
    public static async Task<Result<TOut, TError>> Then<TIn, TOut, TError>(this Task<Result<TIn, TError>> result,
        Func<TIn, Task<TOut>> func) where TError : class
    {
        var waitedResult = await result;
        if (waitedResult.IsFailure)
        {
            return waitedResult.Error;
        }

        return await func(waitedResult.Value);
    }

    /// <summary>
    /// Executes the delegate and return its output.
    /// </summary>
    /// <param name="result"></param>
    /// <param name="func"></param>
    /// <typeparam name="TIn"></typeparam>
    /// <typeparam name="TError"></typeparam>
    /// <returns></returns>
    public static async Task<Result<TIn, TError>> Then<TIn, TError>(this Task<Result<TIn, TError>> result,
        Func<TIn, Task<Result<TError>>> func) where TError : class
    {
        var waitedResult = await result;
        if (waitedResult.IsFailure)
        {
            return waitedResult.Error;
        }

        var funcResult = await func(waitedResult.Value);
        return funcResult.IsFailure ? funcResult.Error : waitedResult.Value;
    }

    #endregion

    #endregion

    #region Func Without Input

    #region Sync

    /// <summary>
    /// Executes the delegate and return its output.
    /// </summary>
    /// <param name="result"></param>
    /// <param name="func"></param>
    /// <typeparam name="TIn"></typeparam>
    /// <typeparam name="TOut"></typeparam>
    /// <typeparam name="TError"></typeparam>
    /// <returns></returns>
    public static Result<TOut, TError> Then<TIn, TOut, TError>(this Result<TIn, TError> result,
        Func<Result<TOut, TError>> func)
        where TError : class
    {
        return result.IsFailure ? result.Error : func();
    }

    /// <summary>
    /// Executes the delegate and return its output.
    /// </summary>
    /// <param name="result"></param>
    /// <param name="func"></param>
    /// <typeparam name="TIn"></typeparam>
    /// <typeparam name="TOut"></typeparam>
    /// <typeparam name="TError"></typeparam>
    /// <returns></returns>
    public static Result<TOut, TError> Then<TIn, TOut, TError>(this Result<TIn, TError> result, Func<TOut> func)
        where TError : class
    {
        return result.IsFailure ? result.Error : func();
    }

    /// <summary>
    /// Executes the delegate and return its output.
    /// </summary>
    /// <param name="result"></param>
    /// <param name="func"></param>
    /// <typeparam name="TIn"></typeparam>
    /// <typeparam name="TError"></typeparam>
    /// <returns></returns>
    public static Result<TIn, TError> Then<TIn, TError>(this Result<TIn, TError> result, Func<Result<TError>> func)
        where TError : class
    {
        if (result.IsFailure)
        {
            return result.Error;
        }

        var funcResult = func();
        return funcResult.IsFailure ? funcResult.Error : result.Value;
    }

    #endregion

    #region Func Async

    /// <summary>
    /// Executes the delegate and return its output.
    /// </summary>
    /// <param name="result"></param>
    /// <param name="func"></param>
    /// <typeparam name="TIn"></typeparam>
    /// <typeparam name="TOut"></typeparam>
    /// <typeparam name="TError"></typeparam>
    /// <returns></returns>
    public static async Task<Result<TOut, TError>> Then<TIn, TOut, TError>(this Result<TIn, TError> result,
        Func<Task<Result<TOut, TError>>> func) where TError : class
    {
        if (result.IsFailure)
        {
            return result.Error;
        }

        return await func();
    }

    /// <summary>
    /// Executes the delegate and return its output.
    /// </summary>
    /// <param name="result"></param>
    /// <param name="func"></param>
    /// <typeparam name="TIn"></typeparam>
    /// <typeparam name="TOut"></typeparam>
    /// <typeparam name="TError"></typeparam>
    /// <returns></returns>
    public static async Task<Result<TOut, TError>> Then<TIn, TOut, TError>(this Result<TIn, TError> result,
        Func<Task<TOut>> func)
        where TError : class
    {
        if (result.IsFailure)
        {
            return result.Error;
        }

        return await func();
    }

    /// <summary>
    /// Executes the delegate and return its output.
    /// </summary>
    /// <param name="result"></param>
    /// <param name="func"></param>
    /// <typeparam name="TIn"></typeparam>
    /// <typeparam name="TError"></typeparam>
    /// <returns></returns>
    public static async Task<Result<TIn, TError>> Then<TIn, TError>(this Result<TIn, TError> result,
        Func<Task<Result<TError>>> func)
        where TError : class
    {
        if (result.IsFailure)
        {
            return result.Error;
        }

        var funcResult = await func();
        return funcResult.IsFailure ? funcResult.Error : result.Value;
    }

    #endregion

    #region Input Result Async

    /// <summary>
    /// Executes the delegate and return its output.
    /// </summary>
    /// <param name="result"></param>
    /// <param name="func"></param>
    /// <typeparam name="TIn"></typeparam>
    /// <typeparam name="TOut"></typeparam>
    /// <typeparam name="TError"></typeparam>
    /// <returns></returns>
    public static async Task<Result<TOut, TError>> Then<TIn, TOut, TError>(this Task<Result<TIn, TError>> result,
        Func<Result<TOut, TError>> func) where TError : class
    {
        var waitedResult = await result;
        return waitedResult.IsFailure ? waitedResult.Error : func();
    }

    /// <summary>
    /// Executes the delegate and return its output.
    /// </summary>
    /// <param name="result"></param>
    /// <param name="func"></param>
    /// <typeparam name="TIn"></typeparam>
    /// <typeparam name="TOut"></typeparam>
    /// <typeparam name="TError"></typeparam>
    /// <returns></returns>
    public static async Task<Result<TOut, TError>> Then<TIn, TOut, TError>(this Task<Result<TIn, TError>> result,
        Func<TOut> func)
        where TError : class
    {
        var waitedResult = await result;
        return waitedResult.IsFailure ? waitedResult.Error : func();
    }

    /// <summary>
    /// Executes the delegate and return its output.
    /// </summary>
    /// <param name="result"></param>
    /// <param name="func"></param>
    /// <typeparam name="TIn"></typeparam>
    /// <typeparam name="TError"></typeparam>
    /// <returns></returns>
    public static async Task<Result<TIn, TError>> Then<TIn, TError>(this Task<Result<TIn, TError>> result,
        Func<Result<TError>> func)
        where TError : class
    {
        var waitedResult = await result;
        if (waitedResult.IsFailure)
        {
            return waitedResult.Error;
        }

        var funcResult = func();
        return funcResult.IsFailure ? funcResult.Error : waitedResult.Value;
    }

    #endregion

    #region Async

    /// <summary>
    /// Executes the delegate and return its output.
    /// </summary>
    /// <param name="result"></param>
    /// <param name="func"></param>
    /// <typeparam name="TIn"></typeparam>
    /// <typeparam name="TOut"></typeparam>
    /// <typeparam name="TError"></typeparam>
    /// <returns></returns>
    public static async Task<Result<TOut, TError>> Then<TIn, TOut, TError>(this Task<Result<TIn, TError>> result,
        Func<Task<Result<TOut, TError>>> func) where TError : class
    {
        var waitedResult = await result;
        return waitedResult.IsFailure ? waitedResult.Error : await func();
    }

    /// <summary>
    /// Executes the delegate and return its output.
    /// </summary>
    /// <param name="result"></param>
    /// <param name="func"></param>
    /// <typeparam name="TIn"></typeparam>
    /// <typeparam name="TOut"></typeparam>
    /// <typeparam name="TError"></typeparam>
    /// <returns></returns>
    public static async Task<Result<TOut, TError>> Then<TIn, TOut, TError>(this Task<Result<TIn, TError>> result,
        Func<Task<TOut>> func)
        where TError : class
    {
        var waitedResult = await result;
        return waitedResult.IsFailure ? waitedResult.Error : await func();
    }

    /// <summary>
    /// Executes the delegate and return its output.
    /// </summary>
    /// <param name="result"></param>
    /// <param name="func"></param>
    /// <typeparam name="TIn"></typeparam>
    /// <typeparam name="TError"></typeparam>
    /// <returns></returns>
    public static async Task<Result<TIn, TError>> Then<TIn, TError>(this Task<Result<TIn, TError>> result,
        Func<Task<Result<TError>>> func) where TError : class
    {
        var waitedResult = await result;
        if (waitedResult.IsFailure)
        {
            return waitedResult.Error;
        }

        var funcResult = await func();
        return funcResult.IsFailure ? funcResult.Error : waitedResult.Value;
    }

    #endregion

    #endregion

    #endregion

    #region This = Result<TOut,TError>

    #region Func Without Input

    #region Sync

    /// <summary>
    /// Executes the delegate and return its output.
    /// </summary>
    /// <param name="result"></param>
    /// <param name="func"></param>
    /// <typeparam name="TOut"></typeparam>
    /// <typeparam name="TError"></typeparam>
    /// <returns></returns>
    public static Result<TOut, TError> Then<TOut, TError>(this Result<TError> result, Func<Result<TOut, TError>> func)
        where TError : class
    {
        return result.IsFailure ? result.Error : func();
    }

    /// <summary>
    /// Executes the delegate and return its output.
    /// </summary>
    /// <param name="result"></param>
    /// <param name="func"></param>
    /// <typeparam name="TOut"></typeparam>
    /// <typeparam name="TError"></typeparam>
    /// <returns></returns>
    public static Result<TOut, TError> Then<TOut, TError>(this Result<TError> result, Func<TOut> func)
        where TError : class
    {
        return result.IsFailure ? result.Error : func();
    }

    /// <summary>
    /// Executes the delegate and return its output.
    /// </summary>
    /// <param name="result"></param>
    /// <param name="func"></param>
    /// <typeparam name="TError"></typeparam>
    /// <returns></returns>
    public static Result<TError> Then<TError>(this Result<TError> result, Func<Result<TError>> func)
        where TError : class
    {
        if (result.IsFailure)
        {
            return result.Error;
        }

        var funcResult = func();
        return funcResult.IsFailure ? funcResult.Error : result;
    }

    #endregion

    #region Func Async

    /// <summary>
    /// Executes the delegate and return its output.
    /// </summary>
    /// <param name="result"></param>
    /// <param name="func"></param>
    /// <typeparam name="TOut"></typeparam>
    /// <typeparam name="TError"></typeparam>
    /// <returns></returns>
    public static async Task<Result<TOut, TError>> Then<TOut, TError>(this Result<TError> result,
        Func<Task<Result<TOut, TError>>> func)
        where TError : class
    {
        return result.IsFailure ? result.Error : await func();
    }

    /// <summary>
    /// Executes the delegate and return its output.
    /// </summary>
    /// <param name="result"></param>
    /// <param name="func"></param>
    /// <typeparam name="TOut"></typeparam>
    /// <typeparam name="TError"></typeparam>
    /// <returns></returns>
    public static async Task<Result<TOut, TError>> Then<TOut, TError>(this Result<TError> result, Func<Task<TOut>> func)
        where TError : class
    {
        return result.IsFailure ? result.Error : await func();
    }

    /// <summary>
    /// Executes the delegate and return its output.
    /// </summary>
    /// <param name="result"></param>
    /// <param name="func"></param>
    /// <typeparam name="TError"></typeparam>
    /// <returns></returns>
    public static async Task<Result<TError>> Then<TError>(this Result<TError> result, Func<Task<Result<TError>>> func)
        where TError : class
    {
        return result.IsFailure ? result.Error : await func();
    }

    #endregion

    #region Input Result Async

    /// <summary>
    /// Executes the delegate and return its output.
    /// </summary>
    /// <param name="result"></param>
    /// <param name="func"></param>
    /// <typeparam name="TOut"></typeparam>
    /// <typeparam name="TError"></typeparam>
    /// <returns></returns>
    public static async Task<Result<TOut, TError>> Then<TOut, TError>(this Task<Result<TError>> result,
        Func<Result<TOut, TError>> func)
        where TError : class
    {
        var waitedResult = await result;
        return waitedResult.IsFailure ? waitedResult.Error : func();
    }

    /// <summary>
    /// Executes the delegate and return its output.
    /// </summary>
    /// <param name="result"></param>
    /// <param name="func"></param>
    /// <typeparam name="TOut"></typeparam>
    /// <typeparam name="TError"></typeparam>
    /// <returns></returns>
    public static async Task<Result<TOut, TError>> Then<TOut, TError>(this Task<Result<TError>> result, Func<TOut> func)
        where TError : class
    {
        var waitedResult = await result;
        return waitedResult.IsFailure ? waitedResult.Error : func();
    }

    /// <summary>
    /// Executes the delegate and return its output.
    /// </summary>
    /// <param name="result"></param>
    /// <param name="func"></param>
    /// <typeparam name="TError"></typeparam>
    /// <returns></returns>
    public static async Task<Result<TError>> Then<TError>(this Task<Result<TError>> result, Func<Result<TError>> func)
        where TError : class
    {
        var waitedResult = await result;
        return waitedResult.IsFailure ? waitedResult.Error : func();
    }

    #endregion

    #region Async

    /// <summary>
    /// Executes the delegate and return its output.
    /// </summary>
    /// <param name="result"></param>
    /// <param name="func"></param>
    /// <typeparam name="TOut"></typeparam>
    /// <typeparam name="TError"></typeparam>
    /// <returns></returns>
    public static async Task<Result<TOut, TError>> Then<TOut, TError>(this Task<Result<TError>> result,
        Func<Task<Result<TOut, TError>>> func) where TError : class
    {
        var waitedResult = await result;
        return waitedResult.IsFailure ? waitedResult.Error : await func();
    }

    /// <summary>
    /// Executes the delegate and return its output.
    /// </summary>
    /// <param name="result"></param>
    /// <param name="func"></param>
    /// <typeparam name="TOut"></typeparam>
    /// <typeparam name="TError"></typeparam>
    /// <returns></returns>
    public static async Task<Result<TOut, TError>> Then<TOut, TError>(this Task<Result<TError>> result,
        Func<Task<TOut>> func)
        where TError : class
    {
        var waitedResult = await result;
        return waitedResult.IsFailure ? waitedResult.Error : await func();
    }

    /// <summary>
    /// Executes the delegate and return its output.
    /// </summary>
    /// <param name="result"></param>
    /// <param name="func"></param>
    /// <typeparam name="TError"></typeparam>
    /// <returns></returns>
    public static async Task<Result<TError>> Then<TError>(this Task<Result<TError>> result,
        Func<Task<Result<TError>>> func)
        where TError : class
    {
        var waitedResult = await result;
        return waitedResult.IsFailure ? waitedResult.Error : await func();
    }

    #endregion

    #endregion

    #endregion
}
