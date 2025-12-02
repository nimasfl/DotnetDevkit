using DotnetDevkit.Result.Abstractions;

namespace DotnetDevkit.Mediator.Common;

public delegate Task<TResponse> CommandHandlerDelegate<TResponse>();

public delegate Task CommandHandlerDelegate();

public interface ICommandBehavior<in TCommand, in TError>
    where TCommand : ICommand<TError>
    where TError : class
{
    Task Handle(TCommand command, CancellationToken cancellationToken, CommandHandlerDelegate next);
}

public interface ICommandBehavior<in TCommand, TResponse, in TError>
    where TCommand : ICommand<TResponse, TError>
    where TError : class
{
    Task<TResponse> Handle(TCommand command, CancellationToken cancellationToken,
        CommandHandlerDelegate<TResponse> next);
}
