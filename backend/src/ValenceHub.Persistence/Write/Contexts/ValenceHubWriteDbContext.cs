using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using ValenceHub.Domain.Interfaces;
using ValenceHub.Persistence.Write.Interceptors;

namespace ValenceHub.Persistence.Write.Contexts;

public class ValenceHubWriteDbContext : DbContext
{
    private readonly AuditingInterceptor _auditInterceptor;
    public ValenceHubWriteDbContext(DbContextOptions<ValenceHubWriteDbContext> options, AuditingInterceptor auditInterceptor)
        : base(options) {
        _auditInterceptor = auditInterceptor;
    }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.AddInterceptors(_auditInterceptor);
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

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
