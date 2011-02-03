using System;
using Ellemy.CQRS.Event;
using NServiceBus;
using NServiceBusPublisher;
using StructureMap;

namespace Ellemy.CQRS.NServiceBusSubscriber
{
    public class MessageHandler : IHandleMessages<DomainEventMessage>
    {
        private readonly IContainer _container;

        public MessageHandler(IContainer container)
        {
            _container = container;
        }

        public void Handle(DomainEventMessage message)
        {
            var eventType = message.Event.GetType();
            var handlerInterface = typeof (IDomainEventHandler<>).MakeGenericType(eventType);
            foreach (var handler in _container.GetAllInstances(handlerInterface))
            {
                var handlerMethod = handler.GetType().GetMethod("Handle");
                handlerMethod.Invoke(handler, new[] {message.Event});
            }

        }
    }
}