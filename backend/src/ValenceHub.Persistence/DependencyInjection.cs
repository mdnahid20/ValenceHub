using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MediatR;
using ValenceHub.Application.Abstractions.Transactions;
using ValenceHub.Persistence.Read.Connection;
using ValenceHub.Persistence.Read.Contexts;
using ValenceHub.Persistence.Read.Dapper.QueryExecutor;
using ValenceHub.Persistence.Read.Idempotency;
using ValenceHub.Persistence.Read.Projectors;
using ValenceHub.Persistence.Read.Repositories.Base;
using ValenceHub.Persistence.Write.Contexts;
using ValenceHub.Persistence.Write.Dispatchers;
using ValenceHub.Persistence.Write.Interceptors;
using ValenceHub.Persistence.Write.Outbox.Processors;
using ValenceHub.Persistence.Write.Repositories.Base;
using ValenceHub.Persistence.Write.UnitOfWork;
using ValenceHub.Application.Abstractions.Repositories;

namespace ValenceHub.Persistence;

public static class DependencyInjection
{
    public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ValenceHubWriteDbContext>(options => options.UseSqlServer(configuration.GetConnectionString("WriteDb")));
        services.AddDbContext<ValenceHubReadDbContext>(options =>  options.UseSqlServer(configuration.GetConnectionString("ReadDb")));

        services.AddSingleton<IDbConnectionFactory, SqlConnectionFactory>();
        services.AddScoped<ISqlQueryExecutor, SqlQueryExecutor>();
        services.AddScoped<IProcessedEventStore, ProcessedEventStore>();

        services.AddScoped(typeof(IReadRepository<>), typeof(ReadRepository<>));
        services.AddScoped(typeof(IWriteRepository<>), typeof(WriteRepository<>));
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.AddScoped<ProjectionDispatcher>();
        services.AddScoped<INotificationHandler<DomainEventNotification>, DomainEventNotificationHandler>();

      //  services.AddScoped<ICurrentUserService, CurrentUserService>();
        services.AddScoped<AuditingInterceptor>();
        services.AddScoped<SoftDeleteInterceptor>();
        services.AddScoped<DomainEventDispatcher>();
        services.AddScoped<OutboxProcessingJob>();

        return services;
    }
}
