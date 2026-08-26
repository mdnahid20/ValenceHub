using System;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using ValenceHub.Infrastructure.Attributes;

namespace ValenceHub.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAutoRegisteredServices(this IServiceCollection services, Assembly? assembly = null)
    {
        assembly ??= Assembly.GetCallingAssembly();

        var types = assembly.GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract && t.GetCustomAttribute<AutoRegisterAttribute>() != null);

        foreach (var type in types)
        {
            var attr = type.GetCustomAttribute<AutoRegisterAttribute>()!;
            
            if (attr.ServiceType != null)
            {
                // Use specified service type
                RegisterType(services, attr.ServiceType, type, attr.Lifetime);
            }
            else
            {
                // Get all interfaces and register the type for each interface
                var interfaces = type.GetInterfaces();
                if (interfaces.Length > 0)
                {
                    foreach (var interfaceType in interfaces)
                    {
                        RegisterType(services, interfaceType, type, attr.Lifetime);
                    }
                }
                else
                {
                    // No interfaces, register the type itself
                    RegisterType(services, type, type, attr.Lifetime);
                }
            }
        }

        return services;
    }

    public static IServiceCollection AddAutoRegisteredServices(this IServiceCollection services, params Assembly[] assemblies)
    {
        foreach (var assembly in assemblies)
        {
            services.AddAutoRegisteredServices(assembly);
        }

        return services;
    }

    private static void RegisterType(IServiceCollection services, Type serviceType, Type implementationType, ServiceLifetime lifetime)
    {
        switch (lifetime)
        {
            case ServiceLifetime.Singleton:
                services.AddSingleton(serviceType, implementationType);
                break;
            case ServiceLifetime.Scoped:
                services.AddScoped(serviceType, implementationType);
                break;
            case ServiceLifetime.Transient:
                services.AddTransient(serviceType, implementationType);
                break;
        }
    }
}

