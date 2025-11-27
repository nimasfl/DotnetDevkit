using Microsoft.Extensions.DependencyInjection;

namespace DotnetDevkit.Mediator.Common;

public class Sender(IServiceProvider serviceProvider) : ISender
{
    public Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
    {
        if (request is null) throw new ArgumentNullException(nameof(request));

        return SendInternal((dynamic)request, cancellationToken);
    }

    public Task Send(IRequest request, CancellationToken cancellationToken = default)
    {
        if (request is null) throw new ArgumentNullException(nameof(request));

        return SendInternal((dynamic)request, cancellationToken);
    }

    private Task<TResponse> SendInternal<TRequest, TResponse>(TRequest request, CancellationToken cancellationToken)
        where TRequest : IRequest<TResponse>
    {
        var handler = serviceProvider.GetRequiredService<IRequestHandler<TRequest, TResponse>>();

        var behaviors = serviceProvider.GetServices<IRequestBehavior<TRequest, TResponse>>().ToArray();
        var next = behaviors.Reverse().Aggregate((MediatorHandlerDelegate<TResponse>)HandlerDelegate, (current, behavior) =>
            () => behavior.Handle(request, cancellationToken, current));

        return next();

        Task<TResponse> HandlerDelegate() => handler.Handle(request, cancellationToken);
    }

    private Task SendInternal<TRequest>(TRequest request, CancellationToken cancellationToken)
        where TRequest : IRequest
    {
        var handler = serviceProvider.GetRequiredService<IRequestHandler<TRequest>>();

        var behaviors = serviceProvider.GetServices<IRequestBehavior<TRequest>>().ToArray();
        var next = behaviors.Reverse().Aggregate((RequestHandlerDelegate)HandlerDelegate, (current, behavior) =>
            () => behavior.Handle(request, cancellationToken, current));

        return next();

        Task HandlerDelegate() => handler.Handle(request, cancellationToken);
    }
}
