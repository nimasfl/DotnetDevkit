using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace DotnetDevkit.Mediator.Common;

public class MediatorServiceConfiguration
{
    internal Assembly[] AssembliesToScan { get; private set; } = [];
    internal ServiceLifetime ServiceLifetime { get; private set; } = ServiceLifetime.Scoped;
    internal List<(Type Behaviour, ServiceLifetime? Lifetime)> RequestBehaviors { get; } = [];
    internal List<(Type Behaviour, ServiceLifetime? Lifetime)> RequestResponseBehaviors { get; } = [];
    internal List<(Type Behaviour, ServiceLifetime? Lifetime)> QueryBehaviors { get; } = [];
    internal List<(Type Behaviour, ServiceLifetime? Lifetime)> CommandBehaviors { get; } = [];

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

    public void AddBehavior(Type behaviorType, ServiceLifetime? lifetime = null)
    {
        GuardBehaviorType(behaviorType);
        if (IsOfType(behaviorType, typeof(IRequestBehavior<,>)))
        {
            RequestResponseBehaviors.Add((behaviorType, lifetime));
        }
        else if (IsOfType(behaviorType, typeof(IRequestBehavior<>)))
        {
            RequestBehaviors.Add((behaviorType, lifetime));
        }
        else if (IsOfType(behaviorType, typeof(IQueryBehavior<,>)))
        {
            QueryBehaviors.Add((behaviorType, lifetime));
        }
        else if (IsOfType(behaviorType, typeof(ICommandBehavior<,>)))
        {
            CommandBehaviors.Add((behaviorType, lifetime));
        }
    }

    private static bool IsOfType(Type behaviorType, Type targetType)
    {
        return behaviorType.GetInterfaces().Any(i => i.GetGenericTypeDefinition() == targetType);
    }

    private static void GuardBehaviorType(Type behaviorType)
    {
        ArgumentNullException.ThrowIfNull(behaviorType);

        if (!behaviorType.IsClass || behaviorType.IsAbstract)
        {
            throw new ArgumentException("Behavior type must be a non-abstract class.", nameof(behaviorType));
        }

        var matches = behaviorType.GetInterfaces()
            .Any(i => i.IsGenericType && (
                    i.GetGenericTypeDefinition() == typeof(IRequestBehavior<,>) ||
                    i.GetGenericTypeDefinition() == typeof(IRequestBehavior<>) ||
                    i.GetGenericTypeDefinition() == typeof(IQueryBehavior<,>) ||
                    i.GetGenericTypeDefinition() == typeof(ICommandBehavior<,>)
                )
            );

        if (!matches)
        {
            throw new ArgumentException($"Behavior type must implement {typeof(IRequestBehavior<,>).Name}.",
                nameof(behaviorType));
        }
    }
}
