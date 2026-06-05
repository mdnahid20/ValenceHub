using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ValenceHub.Application.Abstractions.Services;
using ValenceHub.Application.Abstractions.Repositories;
using ValenceHub.Application.Features.Auth.Abstractions;
using ValenceHub.Application.Abstractions.Transactions;
using ValenceHub.Application.Messaging;
using ValenceHub.Domain.Users.Events;
using ValenceHub.Infrastructure.Options;
using ValenceHub.Infrastructure.Services;
using ValenceHub.Persistence.Read.Connection;
using ValenceHub.Infrastructure.Extensions;
using ValenceHub.Persistence.Read.Contexts;
using ValenceHub.Persistence.Read.Dapper.QueryExecutor;
using ValenceHub.Persistence.Read.Helpers.Audit;
using ValenceHub.Persistence.Read.Idempotency;
using ValenceHub.Persistence.Read.Projectors;
using ValenceHub.Persistence.Read.Projectors.Users;
using ValenceHub.Persistence.Read.Repositories.Base;
using ValenceHub.Persistence.Read.Repositories.Users;
using ValenceHub.Persistence.Write.Contexts;
using ValenceHub.Persistence.Write.Dispatchers;
using ValenceHub.Persistence.Write.Outbox.Processors;
using ValenceHub.Persistence.Write.Repositories.Base;
using ValenceHub.Persistence.Write.Services;
using ValenceHub.Persistence.Write.UnitOfWork;

namespace ValenceHub.Persistence;

public static class DependencyInjection
{
    public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.SectionName));
        services.AddMemoryCache();

        services.AddScoped<AuditInterceptor>();

        services.AddDbContext<ValenceHubWriteDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("WriteDb")));

        services.AddDbContext<ReadDbContext>((serviceProvider, options) =>
            options.UseSqlServer(configuration.GetConnectionString("ReadDb"))
                .AddInterceptors(serviceProvider.GetRequiredService<AuditInterceptor>()));

        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly));

        // Auto-register services from both Persistence and Infrastructure assemblies
        services.AddAutoRegisteredServices(
            typeof(DependencyInjection).Assembly,
            typeof(SystemDateTimeOffsetProvider).Assembly
        );

        services.AddScoped(typeof(IReadRepository<>), typeof(ReadRepository<>));
        services.AddScoped(typeof(IWriteRepository<>), typeof(WriteRepository<>));
        services.AddScoped<IUserReadRepository, UserReadRepository>();
        services.AddScoped<IRefreshTokenService, RefreshTokenService>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.AddScoped<DomainEventDispatcher>();
        services.AddScoped<IDomainEventDispatcher>(sp => sp.GetRequiredService<DomainEventDispatcher>());
        services.AddScoped<OutboxProcessingJob>();

        return services;
    }
}
