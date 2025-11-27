using System;
using Microsoft.Extensions.DependencyInjection;
using Scrutor;

namespace DotnetDevkit.Mediator.Common;

public static class Extensions
{
    public static IServiceCollection AddMediator(this IServiceCollection services,
        Action<MediatorServiceConfiguration> configurationBuilder)
    {
        var configuration = new MediatorServiceConfiguration();
        configurationBuilder(configuration);

        if (configuration.AssembliesToScan.Length == 0)
        {
            throw new ArgumentException("At least one assembly must be specified for scanning.");
        }

        services.Scan(scan => scan.FromAssemblies(configuration.AssembliesToScan)
            .AddClasses(classes => classes.AssignableTo(typeof(IRequestHandler<>)), publicOnly: false)
            .AsImplementedInterfaces()
            .WithLifetime(configuration.ServiceLifetime)
            .AddClasses(classes => classes.AssignableTo(typeof(IRequestHandler<,>)), publicOnly: false)
            .AsImplementedInterfaces()
            .WithLifetime(configuration.ServiceLifetime)
            .AddClasses(classes => classes.AssignableTo(typeof(IDomainEventHandler<>)), publicOnly: false)
            .AsImplementedInterfaces()
            .WithLifetime(configuration.ServiceLifetime)
        );

        foreach (var behavior in configuration.RequestResponseBehaviors)
        {
            services.Add(new ServiceDescriptor(typeof(IRequestBehavior<,>), behavior, configuration.ServiceLifetime));
        }

        foreach (var behavior in configuration.RequestBehaviors)
        {
            services.Add(new ServiceDescriptor(typeof(IRequestBehavior<>), behavior, configuration.ServiceLifetime));
        }

        services.Add(new ServiceDescriptor(typeof(ISender), typeof(Sender), configuration.ServiceLifetime));

        return services;
    }
}
