using System.Reflection;
using Mapster;
using MapsterMapper;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using ValenceHub.Application.Behaviors;
using ValenceHub.Application.Messaging;

namespace ValenceHub.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();

        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(assembly));

        var config = new TypeAdapterConfig();
        config.Scan(assembly);
        services.AddSingleton(config);
        services.AddScoped<IMapper, Mapper>();

        services.AddValidatorsFromAssembly(assembly);

        services.AddScoped<ICommandDispatcher, CommandDispatcher>();
        services.AddScoped<IQueryDispatcher, QueryDispatcher>();
        services.AddScoped(typeof(ICommandBehavior<>), typeof(ExceptionHandlingBehavior<>));
        services.AddScoped(typeof(ICommandBehavior<>), typeof(TransactionBehavior<>));
        services.AddScoped(typeof(ICommandBehavior<>), typeof(DomainEventsBehavior<>));
        services.AddScoped(typeof(ICommandBehavior<>), typeof(ValidationBehavior<>));
        services.AddScoped(typeof(ICommandBehavior<>), typeof(LoggingBehavior<>));
        services.AddScoped(typeof(ICommandBehavior<>), typeof(PerformanceBehavior<>));

        services.AddScoped(typeof(IQueryBehavior<,>), typeof(LoggingQueryBehavior<,>));
        services.AddScoped(typeof(IQueryBehavior<,>), typeof(PerformanceQueryBehavior<,>));

        return services;
    }
}
