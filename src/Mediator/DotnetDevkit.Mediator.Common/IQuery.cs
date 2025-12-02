using DotnetDevkit.Result.Abstractions;

namespace DotnetDevkit.Mediator.Common;

public interface IQuery<out TError> : IRequest<IResult<TError>> where TError : class;

public interface IQuery<out TResponse, out TError> : IRequest<IResult<TResponse, TError>> where TError : class;
