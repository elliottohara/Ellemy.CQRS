using System;
using Ellemy.CQRS.Event;
using NServiceBus;

namespace NServiceBusPublisher
{
    [Serializable]
    public class DomainEventMessage: IMessage
    {
        public IDomainEvent Event { get; set; }
        public Type MessageType { get; set; }

        public DomainEventMessage(IDomainEvent @event)
        {
            Event = @event;
            MessageType = @event.GetType();
        }
    }
    [Serializable]
    public class DomainEventMessageThatWorks<TDomainEvent>:DomainEventMessage where TDomainEvent:IDomainEvent
    {
        public TDomainEvent EventCastProperly { get; set; }
        public DomainEventMessageThatWorks(IDomainEvent @event) : base(@event)
        {
            Event = (TDomainEvent) @event;
            EventCastProperly = (TDomainEvent)@event;
        }
    }
}