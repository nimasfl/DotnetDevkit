using DotnetDevkit.Result.Abstractions;

namespace DotnetDevkit.Mediator.Common;

public delegate Task<TResponse> QueryHandlerDelegate<TResponse>();

public delegate Task QueryHandlerDelegate();

public interface IQueryBehavior<in TQuery, in TError>
    where TQuery : IQuery<TError>
    where TError : class
{
    Task Handle(TQuery query, CancellationToken cancellationToken, QueryHandlerDelegate next);
}

public interface IQueryBehavior<in TQuery, TResponse, in TError>
    where TQuery : IQuery<TResponse, TError>
    where TError : class
{
    Task<TResponse> Handle(TQuery query, CancellationToken cancellationToken,
        QueryHandlerDelegate<TResponse> next);
}
