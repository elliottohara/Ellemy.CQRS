using System;
using Ellemy.CQRS.Event;

namespace Ellemy.CQRS.Example.Events
{
    public class MessageCreated : IDomainEvent
    {
        public Guid MessageId { get; set; }
        public string Text { get; set; }

        public MessageCreated(Guid id, string text)
        {
            MessageId = id;
            Text = text;
        }
    }
}