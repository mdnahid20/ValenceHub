using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ValenceHub.Application.Abstractions.Services;
using ValenceHub.Application.Abstractions.Repositories;
using ValenceHub.Application.Abstractions.Transactions;
using ValenceHub.Application.Common.Clock;
using ValenceHub.Application.Messaging;
using ValenceHub.Domain.Users.Events;
using ValenceHub.Infrastructure.Services;
using ValenceHub.Persistence.Read.Connection;
using ValenceHub.Persistence.Read.Contexts;
using ValenceHub.Persistence.Read.Dapper.QueryExecutor;
using ValenceHub.Persistence.Read.Idempotency;
using ValenceHub.Persistence.Read.Projectors;
using ValenceHub.Persistence.Read.Projectors.Users;
using ValenceHub.Persistence.Read.Repositories.Base;
using ValenceHub.Persistence.Write.Contexts;
using ValenceHub.Persistence.Write.Dispatchers;
using ValenceHub.Persistence.Write.Outbox.Processors;
using ValenceHub.Persistence.Write.Repositories.Base;
using ValenceHub.Persistence.Write.UnitOfWork;

namespace ValenceHub.Persistence;

public static class DependencyInjection
{
    public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ValenceHubWriteDbContext>(options => options.UseSqlServer(configuration.GetConnectionString("WriteDb")));
        services.AddDbContext<ReadDbContext>(options => options.UseSqlServer(configuration.GetConnectionString("ReadDb")));

        services.AddSingleton<IDbConnectionFactory, SqlConnectionFactory>();
        services.AddSingleton<SystemDateTimeOffsetProvider>();
        services.AddSingleton<IClock>(sp => sp.GetRequiredService<SystemDateTimeOffsetProvider>());
        services.AddSingleton<IDateTimeOffsetProvider>(sp => sp.GetRequiredService<SystemDateTimeOffsetProvider>());
        services.AddScoped<ISqlQueryExecutor, SqlQueryExecutor>();
        services.AddScoped<IProcessedEventStore, ProcessedEventStore>();

        services.AddScoped(typeof(IReadRepository<>), typeof(ReadRepository<>));
        services.AddScoped(typeof(IWriteRepository<>), typeof(WriteRepository<>));
        // Register concrete user repository for write operations
        services.AddScoped<IRepository<ValenceHub.Domain.Users.User>, ValenceHub.Persistence.Write.Repositories.UserRepository>();
        services.AddScoped<IUserRepository, ValenceHub.Persistence.Write.Repositories.UserRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.AddScoped<INotificationHandler<DomainEventNotification<UserCreatedDomainEvent>>, UserRegisteredProjection>();
        services.AddScoped<INotificationHandler<DomainEventNotification<UserContactUpdatedDomainEvent>>, UserRegisteredProjection>();
        services.AddScoped<INotificationHandler<DomainEventNotification<UserDeletedDomainEvent>>, UserRegisteredProjection>();
        services.AddScoped<DomainEventDispatcher>();
        services.AddScoped<IDomainEventDispatcher>(sp => sp.GetRequiredService<DomainEventDispatcher>());
        services.AddScoped<OutboxProcessingJob>();

        return services;
    }
}
