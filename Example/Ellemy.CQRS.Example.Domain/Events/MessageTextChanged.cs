using System;
using Ellemy.CQRS.Event;

namespace Ellemy.CQRS.Example.Events
{
    public class MessageTextChanged : IDomainEvent
    {
        public Guid MessageId { get; set; }
        public string Newtext { get; set; }

        public MessageTextChanged(Guid id, string newtext)
        {
            MessageId = id;
            Newtext = newtext;
            throw new NotImplementedException();
        }
    }
}