using System;
using Ellemy.CQRS.Event;
using Ellemy.CQRS.Example.Events;

namespace Ellemy.CQRS.Example.Domain
{
    public class  Message
    {
        /// <summary>
        /// Creates a new instance of Message
        /// </summary>
        /// <param name="id"></param>
        /// <param name="text"></param>
        public Message(Guid id,string text)
        {
            Id = id;
            DomainEvents.Raise(new MessageCreated(Id,text));
        }
        public virtual Guid Id { get; set; }
        public virtual void ChangeText(string newtext)
        {
            DomainEvents.Raise(new MessageTextChanged(Id,newtext));
        }
       
    }
}