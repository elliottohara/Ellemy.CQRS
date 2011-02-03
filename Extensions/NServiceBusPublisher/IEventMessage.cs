using NServiceBus;

namespace NServiceBusPublisher
{
    public class DomainEventMessage<TEvent>: IMessage
    {
        public TEvent Event { get; set; }

        public DomainEventMessage(TEvent @event)
        {
            Event = @event;
        }
    }
}