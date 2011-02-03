using System;
using Ellemy.CQRS.Event;
using Ellemy.CQRS.Example.Events;

namespace Ellemy.CQRS.NServiceBusSubscriber
{
    public class PrintStuffToConsole : IDomainEventHandler<MessageCreated>
    {
        public void Handle(MessageCreated @event)
        {
            Console.WriteLine("Recieved Message Created");
            Console.WriteLine("Text:{0}",@event.Text);
            Console.WriteLine("Id:{0}",@event.MessageId);
        }
    }
}