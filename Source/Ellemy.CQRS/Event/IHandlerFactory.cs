using System.Collections.Generic;

namespace Ellemy.CQRS.Event
{
    public interface IHandlerFactory
    {
        IEnumerable<IDomainEventHandler<TEvent>> GetHandlersFor<TEvent>() where TEvent : IDomainEvent;
    }
}