namespace Ellemy.CQRS.Event
{
    public interface IDomainEventHandler<TEvent> where TEvent:IDomainEvent
    {
        void Handle(TEvent @event);
    }
}