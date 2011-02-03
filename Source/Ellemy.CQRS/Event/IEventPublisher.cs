namespace Ellemy.CQRS.Event
{
    public interface IEventPublisher
    {
        void Publish<TDomainEvent>(TDomainEvent @event) where TDomainEvent : IDomainEvent;
    }
}