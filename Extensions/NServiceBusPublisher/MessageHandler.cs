using Ellemy.CQRS.Event;
using NServiceBus;
using NServiceBus.ObjectBuilder.Common;

namespace Ellemy.CQRS.Publishing.NServiceBus
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
            foreach (var handler in _container.BuildAll(handlerInterface))
            {
                var handlerMethod = handler.GetType().GetMethod("Handle");
                handlerMethod.Invoke(handler, new object[] { message.Payload });
            }
        }
    }
}