namespace DotnetDevkit.Mediator.Common;

public abstract class DomainEventEntity
{
    private readonly List<IDomainEvent> _domainEvents = [];

    public IDomainEvent[] DomainEvents => _domainEvents.ToArray();

    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }

    public void Raise(IDomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }
}
