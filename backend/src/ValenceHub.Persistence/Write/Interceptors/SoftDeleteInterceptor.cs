using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System;
using System.Collections.Generic;
using System.Text;
using ValenceHub.Domain.Interfaces;

namespace ValenceHub.Persistence.Write.Interceptors;

public class SoftDeleteInterceptor : SaveChangesInterceptor
{
    // private readonly ICurrentUserService _currentUser;
    //TODO : Inject current user service to get the user performing the delete action 
    //public SoftDeleteInterceptor(ICurrentUserService currentUser)
    //{
    //    _currentUser = currentUser;
    //}

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
            if (entry.Entity is ISoftDelete soft && entry.State == EntityState.Deleted)
            {
                soft.IsDeleted = true;
                soft.DeletedAt = DateTime.UtcNow;
               // soft.DeletedBy = userId;
                entry.State = EntityState.Modified;
            }
        }

        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }
}
