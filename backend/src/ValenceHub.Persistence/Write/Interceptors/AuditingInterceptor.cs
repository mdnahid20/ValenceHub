using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using ValenceHub.Domain.Interfaces;

namespace ValenceHub.Persistence.Write.Interceptors;  
public class AuditingInterceptor : SaveChangesInterceptor
{
    // private readonly ICurrentUserService _currentUser;
    //TODO : Inject current user service to get the user performing the action  
    /*public AuditingInterceptor(ICurrentUserService currentUser)
    {
        _currentUser = currentUser;
    }*/

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        var context = eventData.Context;
        if (context == null)
            return base.SavingChangesAsync(eventData, result, cancellationToken);

   //     var userId = _currentUser.UserId ?? "System";

        foreach (var entry in context.ChangeTracker.Entries())
        {
            if (entry.Entity is IAuditable auditable)
            {
                var now = DateTime.UtcNow;

                if (entry.State == EntityState.Added)
                {
                    auditable.CreatedAt = now;
                //    auditable.CreatedBy = userId;
                }

                if (entry.State == EntityState.Modified)
                {
                    auditable.UpdatedAt = now;
               //     auditable.UpdatedBy = userId;
                }
            }
        }

        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }
}

