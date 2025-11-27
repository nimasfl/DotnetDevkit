using Microsoft.Extensions.DependencyInjection;
using Scrutor;

namespace DotnetDevkit.Mediator.Common;

public static class Extensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddMediator(Action<MediatorServiceConfiguration> configurationBuilder)
        {
            var configuration = new MediatorServiceConfiguration();
            configurationBuilder(configuration);
            if (configuration.AssembliesToScan.Length == 0)
            {
                throw new ArgumentException("At least one assembly must be specified for scanning.");
            }

            services.Scan(scan => scan.FromAssemblies(configuration.AssembliesToScan)
                .AddClasses(classes => classes.AssignableTo(typeof(IQueryHandler<,>)), publicOnly: false)
                .AsImplementedInterfaces()
                .WithLifetime(configuration.ServiceLifetime)
                .AddClasses(classes => classes.AssignableTo(typeof(ICommandHandler<>)), publicOnly: false)
                .AsImplementedInterfaces()
                .WithLifetime(configuration.ServiceLifetime)
                .AddClasses(classes => classes.AssignableTo(typeof(ICommandHandler<,>)), publicOnly: false)
                .AsImplementedInterfaces()
                .WithLifetime(configuration.ServiceLifetime)
            );

            // services.Decorate(typeof(ICommandHandler<,>), typeof(ValidationDecorator.CommandHandler<,>));
            // services.Decorate(typeof(ICommandHandler<>), typeof(ValidationDecorator.CommandBaseHandler<>));

            // services.Decorate(typeof(IQueryHandler<,>), typeof(LoggingDecorator.QueryHandler<,>));
            // services.Decorate(typeof(ICommandHandler<,>), typeof(LoggingDecorator.CommandHandler<,>));
            // services.Decorate(typeof(ICommandHandler<>), typeof(LoggingDecorator.CommandBaseHandler<>));

            services.Scan(scan => scan.FromAssemblies(configuration.AssembliesToScan)
                .AddClasses(classes => classes.AssignableTo(typeof(IDomainEventHandler<>)), publicOnly: false)
                .AsImplementedInterfaces()
                .WithLifetime(configuration.ServiceLifetime)
            );

            return services;
        }
    }
}
