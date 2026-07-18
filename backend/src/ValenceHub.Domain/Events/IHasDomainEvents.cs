namespace ValenceHub.Domain.Events;

public interface IHasDomainEvents
{
    IReadOnlyCollection<DomainEvent > GetDomainEvents();
    void ClearDomainEvents();
}
