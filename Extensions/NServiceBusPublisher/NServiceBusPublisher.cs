using Ellemy.CQRS.Event;
using NServiceBus;

namespace NServiceBusPublisher
{
    public class NServiceBusPublisher : IEventPublisher
    {
        private readonly IBus _bus;

        public NServiceBusPublisher(IBus bus)
        {
            _bus = bus;
        }

        public void Publish<TDomainEvent>(TDomainEvent @event) where TDomainEvent:IDomainEvent
        {
            var message = new DomainEventMessageThatWorks<TDomainEvent>(@event);
            _bus.Send(message);
        }

       
    }
}