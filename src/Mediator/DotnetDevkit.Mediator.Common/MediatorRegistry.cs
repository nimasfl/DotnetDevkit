using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace DotnetDevkit.Mediator.Common;

public class MediatorServiceConfiguration
{
    internal Assembly[] AssembliesToScan { get; private set; } = [];
    internal ServiceLifetime ServiceLifetime { get; private set; } = ServiceLifetime.Scoped;

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

}
