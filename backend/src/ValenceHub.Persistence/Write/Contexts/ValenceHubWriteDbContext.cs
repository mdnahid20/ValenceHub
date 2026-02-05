using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using ValenceHub.Domain.Interfaces;
using ValenceHub.Persistence.Write.Interceptors;
using ValenceHub.Persistence.Write.Outbox.Models;

namespace ValenceHub.Persistence.Write.Contexts;

public class ValenceHubWriteDbContext : DbContext
{
    private readonly AuditingInterceptor _auditInterceptor;
    private readonly SoftDeleteInterceptor _softDeleteInterceptor;

    public ValenceHubWriteDbContext(
        DbContextOptions<ValenceHubWriteDbContext> options,
        AuditingInterceptor auditInterceptor,
        SoftDeleteInterceptor softDeleteInterceptor)
        : base(options) {
        _auditInterceptor = auditInterceptor;
        _softDeleteInterceptor = softDeleteInterceptor;
    }
    public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.AddInterceptors(_auditInterceptor, _softDeleteInterceptor);
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ValenceHubWriteDbContext).Assembly);

        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(ISoftDelete).IsAssignableFrom(entityType.ClrType))
            {
                var parameter = Expression.Parameter(entityType.ClrType, "e");
                var propertyMethod = typeof(EF).GetMethod("Property")!
                    .MakeGenericMethod(typeof(bool));
                var isDeletedExpression =
                    Expression.Call(propertyMethod, parameter, Expression.Constant("IsDeleted"));
                var filter = Expression.Lambda(
                    Expression.Equal(isDeletedExpression, Expression.Constant(false)),
                    parameter);

                modelBuilder.Entity(entityType.ClrType).HasQueryFilter(filter);
            }
        }
    }
}
