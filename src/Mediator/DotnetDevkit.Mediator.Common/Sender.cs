using System.Collections.Concurrent;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace DotnetDevkit.Mediator.Common;

public class Sender(IServiceProvider serviceProvider) : ISender
{
    // caches: requestType -> delegate that returns boxed result (Task<object>)
    private static readonly
        ConcurrentDictionary<(Type requestType, Type responseType), Func<object, CancellationToken, Task<object?>>>
        _responseCache = new();

    // cache for void/command pipeline: requestType -> Func<object, CancellationToken, Task>
    private static readonly ConcurrentDictionary<Type, Func<object, CancellationToken, Task>> _voidCache = new();

    // -----------------------
    // Send with response
    // -----------------------
    public Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
    {
        if (request is null) throw new ArgumentNullException(nameof(request));

        var reqType = request.GetType();
        var key = (reqType, typeof(TResponse));

        // get or create a cached wrapper that returns Task<object?> (boxed TResponse)
        var wrapper = _responseCache.GetOrAdd(key, k => CreateResponseInvoker(k.requestType, k.responseType));

        // invoke delegate and unbox result to TResponse
        return InvokeResponseWrapper<TResponse>(wrapper, request, cancellationToken);
    }

    private static async Task<TResponse> InvokeResponseWrapper<TResponse>(
        Func<object, CancellationToken, Task<object?>> wrapper, object request, CancellationToken ct)
    {
        var boxed = await wrapper(request, ct).ConfigureAwait(false);
        // boxed can be null if TResponse is reference type and handler returned null
        return (TResponse)boxed!;
    }

    // builds and returns a Func<object, CancellationToken, Task<object?>> for a specific request/response pair
    private Func<object, CancellationToken, Task<object?>> CreateResponseInvoker(Type requestType, Type responseType)
    {
        // We'll call the generic helper: SendInternalGeneric<TRequest,TResponse>(TRequest request, CancellationToken ct)
        var genericMethod =
            typeof(Sender).GetMethod(nameof(SendInternalGeneric), BindingFlags.Instance | BindingFlags.NonPublic)
            ?? throw new InvalidOperationException("Missing SendInternalGeneric method");

        var constructed = genericMethod.MakeGenericMethod(requestType, responseType);

        // Return a wrapper delegate that invokes the constructed generic method once per call.
        // The constructed method returns Task<object?> (boxed TResponse).
        return (requestObj, ct) =>
        {
            // Invoke constructed method -> returns Task<object?>
            var resultTask = (Task<object?>)constructed.Invoke(this, new object[] { requestObj, ct })!;
            return resultTask;
        };
    }

    // This is the strongly-typed generic implementation that does the real work and returns boxed result.
    // It is the only generic method we need to compile; we will construct closed generic versions via reflection once.
    private async Task<object?> SendInternalGeneric<TRequest, TResponse>(TRequest request,
        CancellationToken cancellationToken)
        where TRequest : IRequest<TResponse>
    {
        // Resolve the handler for TRequest,TResponse
        var handler = serviceProvider.GetRequiredService<IRequestHandler<TRequest, TResponse>>();

        // Resolve behaviors
        var behaviors = serviceProvider.GetServices<IRequestBehavior<TRequest, TResponse>>().ToArray();

        // Build pipeline
        RequestHandlerDelegate<TResponse> next = () => handler.Handle(request, cancellationToken);

        for (int i = behaviors.Length - 1; i >= 0; i--)
        {
            var behavior = behaviors[i];
            var current = next;
            next = () => behavior.Handle(request, cancellationToken, current);
        }

        // Invoke pipeline and box the result
        var response = await next().ConfigureAwait(false);
        return (object?)response;
    }

    // -----------------------
    // Send without response (IRequest / commands)
    // -----------------------
    public Task Send(IRequest request, CancellationToken cancellationToken = default)
    {
        if (request is null) throw new ArgumentNullException(nameof(request));

        var reqType = request.GetType();

        var wrapper = _voidCache.GetOrAdd(reqType, CreateVoidInvoker);

        return wrapper(request, cancellationToken);
    }

    private Func<object, CancellationToken, Task> CreateVoidInvoker(Type requestType)
    {
        // Find the helper method and construct it closed over TRequest
        var genericMethod = typeof(Sender).GetMethod(nameof(SendInternalGenericNoResponse),
                                BindingFlags.Instance | BindingFlags.NonPublic)
                            ?? throw new InvalidOperationException("Missing SendInternalGenericNoResponse method");

        var constructed = genericMethod.MakeGenericMethod(requestType);

        return (requestObj, ct) =>
        {
            var task = (Task)constructed.Invoke(this, new object[] { requestObj, ct })!;
            return task;
        };
    }

    private async Task SendInternalGenericNoResponse<TRequest>(TRequest request, CancellationToken cancellationToken)
        where TRequest : IRequest
    {
        var handler = serviceProvider.GetRequiredService<IRequestHandler<TRequest>>();

        var behaviors = serviceProvider.GetServices<IRequestBehavior<TRequest>>().ToArray();

        RequestHandlerDelegate next = () => handler.Handle(request, cancellationToken);

        for (int i = behaviors.Length - 1; i >= 0; i--)
        {
            var behavior = behaviors[i];
            var current = next;
            next = () => behavior.Handle(request, cancellationToken, current);
        }

        await next().ConfigureAwait(false);
    }
}
