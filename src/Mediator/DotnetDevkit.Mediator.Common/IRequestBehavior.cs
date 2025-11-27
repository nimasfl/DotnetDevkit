namespace DotnetDevkit.Mediator.Common;

public delegate Task<TResponse> MediatorHandlerDelegate<TResponse>();

public delegate Task MediatorHandlerDelegate();

public interface IRequestBehavior<in TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken,
        MediatorHandlerDelegate<TResponse> next);
}

public interface IRequestBehavior<in TRequest>
    where TRequest : IRequest
{
    Task Handle(TRequest command, CancellationToken cancellationToken, MediatorHandlerDelegate next);
}
