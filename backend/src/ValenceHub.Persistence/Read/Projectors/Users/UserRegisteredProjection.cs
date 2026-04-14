using MediatR;
using Microsoft.EntityFrameworkCore;
using ValenceHub.Domain.Users.Events;
using ValenceHub.Persistence.Read.Contexts;
using ValenceHub.Persistence.Read.Models.Users;

namespace ValenceHub.Persistence.Read.Projectors.Users;

public sealed class UserRegisteredProjection :
    INotificationHandler<DomainEventNotification<UserCreatedDomainEvent>>,
    INotificationHandler<DomainEventNotification<UserContactUpdatedDomainEvent>>,
    INotificationHandler<DomainEventNotification<UserDeletedDomainEvent>>
{
    private readonly ReadDbContext _context;

    public UserRegisteredProjection(ReadDbContext context)
    {
        _context = context;
    }

    public async Task Handle(
        DomainEventNotification<UserCreatedDomainEvent> notification,
        CancellationToken cancellationToken)
    {
        var e = notification.DomainEvent;

        var user = new UserReadModel
        {
            Id = e.Id,
            EventId = e.EventId,
            Email = e.Email,
            PhoneNumber = e.PhoneNumber
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task Handle(
        DomainEventNotification<UserContactUpdatedDomainEvent> notification,
        CancellationToken cancellationToken)
    {
        var e = notification.DomainEvent;

        var user = await _context.Users
            .FirstOrDefaultAsync(x => x.Id == e.Id, cancellationToken);

        if (user is null)
        {
            return;
        }

        user.EventId = e.EventId;
        user.Email = e.Email;
        user.PhoneNumber = e.PhoneNumber;

        _context.Users.Update(user);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task Handle(
        DomainEventNotification<UserDeletedDomainEvent> notification,
        CancellationToken cancellationToken)
    {
        var e = notification.DomainEvent;

        var user = await _context.Users
            .FirstOrDefaultAsync(x => x.Id == e.Id, cancellationToken);

        if (user is null)
        {
            return;
        }

        _context.Users.Remove(user);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
