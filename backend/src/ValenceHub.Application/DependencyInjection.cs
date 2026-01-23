using System.Reflection;
using Mapster;
using MapsterMapper;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using ValenceHub.Application.Behaviors;

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

        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(PerformanceBehavior<,>));

        return services;
    }
}
