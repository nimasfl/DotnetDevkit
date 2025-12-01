namespace DotnetDevkit.Result.Abstractions;

public interface IResult;

public interface IResult<out TError> : IResult where TError : class
{
    public bool IsSuccess { get; }

    public bool IsFailure => !IsSuccess;

    public TError Error { get; }
}
