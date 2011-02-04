using System;
using Ellemy.CQRS.Event;
using NServiceBus;

namespace Ellemy.CQRS.Publishing.NServiceBus
{
    [Serializable]
    public class EventMessage<T> : IMessage
       where T : IDomainEvent
    {
        public EventMessage(T @event)
        {
            Payload = @event;
        }
        public EventMessage(){}
        /// <summary>
        /// Gets or sets transported event.
        /// </summary>
        public T Payload { get; set; }
    }
    
}