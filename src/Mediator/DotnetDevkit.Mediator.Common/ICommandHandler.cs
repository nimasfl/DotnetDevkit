using DotnetDevkit.Result.Abstractions;

namespace DotnetDevkit.Mediator.Common;

public interface ICommandHandler<in TCommand, TError> : IRequestHandler<TCommand, IResult<TError>>
    where TCommand : ICommand<TError>
    where TError : class;

public interface ICommandHandler<in TCommand, TResponse, TError> : IRequestHandler<TCommand, IResult<TResponse, TError>>
    where TCommand : ICommand<TResponse, TError>
    where TError : class;
