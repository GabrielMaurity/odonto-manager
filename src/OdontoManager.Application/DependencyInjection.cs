using System.Reflection;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using OdontoManager.Application.Abstractions;
using OdontoManager.Application.Abstractions.Clinica;
using OdontoManager.Application.Abstractions.Decorators;
using OdontoManager.Application.Abstractions.Messaging;

namespace OdontoManager.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services, params Assembly[] additionalAssemblies)
    {
        ArgumentNullException.ThrowIfNull(additionalAssemblies);
        Assembly[] assemblies = [typeof(DependencyInjection).Assembly, .. additionalAssemblies];

        services.Scan(scan => scan
            .FromAssemblies(assemblies)
            .AddClasses(classes => classes.AssignableTo(typeof(ICommandHandler<,>)).Where(IsConcreteHandler), publicOnly: false)
                .AsImplementedInterfaces()
                .WithScopedLifetime()
            .AddClasses(classes => classes.AssignableTo(typeof(IQueryHandler<,>)).Where(IsConcreteHandler), publicOnly: false)
                .AsImplementedInterfaces()
                .WithScopedLifetime()
            .AddClasses(classes => classes.AssignableTo(typeof(IDomainEventHandler<>)).Where(IsConcreteHandler), publicOnly: false)
                .AsImplementedInterfaces()
                .WithScopedLifetime());

        services.AddValidatorsFromAssemblies(assemblies, ServiceLifetime.Scoped, includeInternalTypes: true);

        services.TryDecorate(typeof(ICommandHandler<,>), typeof(ValidationCommandHandler<,>));
        services.TryDecorate(typeof(ICommandHandler<,>), typeof(LoggingCommandHandler<,>));
        services.TryDecorate(typeof(IQueryHandler<,>), typeof(LoggingQueryHandler<,>));

        services.TryAddSingleton(TimeProvider.System);
        services.AddOptions<ClinicaOptions>().BindConfiguration(ClinicaOptions.SectionName).ValidateOnStart();
        services.TryAddEnumerable(ServiceDescriptor.Singleton<IValidateOptions<ClinicaOptions>, ClinicaOptionsValidator>());

        return services;
    }

    private static bool IsConcreteHandler(Type type) => !type.IsGenericTypeDefinition;
}
