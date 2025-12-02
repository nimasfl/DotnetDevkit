using DotnetDevkit.Result.Abstractions;

namespace DotnetDevkit.Mediator.Common;

public interface IQueryHandler<in TQuery, TError> : IRequestHandler<TQuery, IResult<TError>>
    where TQuery : IQuery<TError>
    where TError : class;

public interface IQueryHandler<in TQuery, TResponse, TError> : IRequestHandler<TQuery, IResult<TResponse, TError>>
    where TQuery : IQuery<TResponse, TError>
    where TError : class;
