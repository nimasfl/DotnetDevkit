using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace DotnetDevkit.Mediator.Common;

public class MediatorServiceConfiguration
{
    internal Assembly[] AssembliesToScan { get; private set; } = [];
    internal ServiceLifetime ServiceLifetime { get; private set; } = ServiceLifetime.Scoped;
    internal List<Type> RequestBehaviors { get; } = [];
    internal List<Type> RequestResponseBehaviors { get; } = [];

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

    public void AddBehavior(Type behaviorType)
    {
        GuardBehaviorType(behaviorType);
        if (behaviorType.GetInterfaces().Any(i => i.GetGenericTypeDefinition() == typeof(IRequestBehavior<,>)))
        {
            RequestResponseBehaviors.Add(behaviorType);
        }
        else
        {
            RequestBehaviors.Add(behaviorType);
        }
    }

    private static void GuardBehaviorType(Type behaviorType)
    {
        if (behaviorType is null)
        {
            throw new ArgumentNullException(nameof(behaviorType));
        }

        if (!behaviorType.IsClass || behaviorType.IsAbstract)
        {
            throw new ArgumentException("Behavior type must be a non-abstract class.", nameof(behaviorType));
        }

        var matches = behaviorType.GetInterfaces()
            .Any(i => i.IsGenericType && (
                i.GetGenericTypeDefinition() == typeof(IRequestBehavior<,>) ||
                i.GetGenericTypeDefinition() == typeof(IRequestBehavior<>))
            );

        if (!matches)
        {
            throw new ArgumentException($"Behavior type must implement {typeof(IRequestBehavior<,>).Name}.",
                nameof(behaviorType));
        }
    }
}
