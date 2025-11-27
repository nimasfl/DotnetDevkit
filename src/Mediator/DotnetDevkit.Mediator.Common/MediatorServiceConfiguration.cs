using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace DotnetDevkit.Mediator.Common;

public class MediatorServiceConfiguration
{
    internal Assembly[] AssembliesToScan { get; private set; } = [];
    internal ServiceLifetime ServiceLifetime { get; private set; } = ServiceLifetime.Scoped;
    internal List<Type> RequestBehaviors { get; } = [];
    internal List<Type> CommandBehaviors { get; } = [];

    public void RegisterServicesFromAssemblies(params Assembly[] assemblies)
    {
        AssembliesToScan = assemblies;
    }

    public void RegisterServicesFromAssembly(Assembly assembly)
    {
        AssembliesToScan = [assembly];
    }

    public void SetServiceLifetime(ServiceLifetime serviceLifetime)
    {
        ServiceLifetime = serviceLifetime;
    }

    public void AddBehavior<TBehavior, TRequest, TResponse>()
        where TBehavior : IRequestBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        RequestBehaviors.Add(typeof(TBehavior));
    }

    public void AddCommandBehavior<TBehavior, TRequest>()
        where TBehavior : IRequestBehavior<TRequest>
        where TRequest : IRequest
    {
        CommandBehaviors.Add(typeof(TBehavior));
    }
}
