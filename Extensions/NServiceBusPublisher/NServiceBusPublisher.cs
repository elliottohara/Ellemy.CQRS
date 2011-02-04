using Ellemy.CQRS.Event;
using NServiceBus;

namespace Ellemy.CQRS.Publishing.NServiceBus
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
            var message = new EventMessage<TDomainEvent>(@event);
            _bus.Send(message);
        }

       
    }
}