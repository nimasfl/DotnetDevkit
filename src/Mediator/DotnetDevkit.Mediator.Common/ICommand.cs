using DotnetDevkit.Result.Abstractions;

namespace DotnetDevkit.Mediator.Common;

public interface ICommand<out TError> : IRequest<IResult<TError>> where TError : class;

public interface ICommand<out TResponse, out TError> : IRequest<IResult<TResponse, TError>> where TError : class;
