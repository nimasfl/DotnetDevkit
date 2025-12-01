namespace DotnetDevkit.Result;

public static partial class ResultExtensions
{
    /// <summary>
    /// Combines the results, returning the error if any of them is failed, otherwise returns successful result.
    /// </summary>
    /// <param name="results"></param>
    /// <typeparam name="TError"></typeparam>
    /// <returns></returns>
    public static Result<TError> Combine<TError>(params Result<TError>[] results) where TError : class
    {
        foreach (var result in results)
        {
            if (result.IsFailure)
            {
                return result.Error;
            }
        }

        return Result<TError>.Success();
    }

    /// <summary>
    /// Combines the results. returns the list of values if all of them are successful, or returns error if any of them is failed.
    /// </summary>
    /// <param name="results"></param>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TError"></typeparam>
    /// <returns></returns>
    public static Result<List<T>, TError> Combine<T, TError>(params Result<T, TError>[] results)
        where TError : class
    {
        var values = new List<T>();

        foreach (var result in results)
        {
            if (result.IsFailure)
            {
                return result.Error;
            }

            values.Add(result.Value);
        }

        return values;
    }
}
