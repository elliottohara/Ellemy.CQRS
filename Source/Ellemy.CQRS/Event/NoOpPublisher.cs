namespace Ellemy.CQRS.Event
{
    public class NoOpPublisher : IEventPublisher
    {
        public void Publish<TDomainEvent>(TDomainEvent @event)
        {
            
        }
    }
}