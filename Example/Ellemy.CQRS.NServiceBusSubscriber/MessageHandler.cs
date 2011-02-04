using System;
using Ellemy.CQRS.Event;
using NServiceBus;
using NServiceBusPublisher;
using NServiceBusPublisher.Stuff;
using StructureMap;

namespace Ellemy.CQRS.NServiceBusSubscriber
{
    public class MessageHandler: IHandleMessages<EventMessage<IDomainEvent>>
    {
        private readonly IContainer _container;

        public MessageHandler(IContainer container)
        {
            _container = container;
        }

       
        public void Handle(EventMessage<IDomainEvent> message)
        {
            var handlerInterface = typeof(IDomainEventHandler<>).MakeGenericType(message.Payload.GetType());
            foreach (var handler in _container.GetAllInstances(handlerInterface))
            {
                var handlerMethod = handler.GetType().GetMethod("Handle");
                handlerMethod.Invoke(handler, new object[] { message.Payload });
            }
        }
    }
}