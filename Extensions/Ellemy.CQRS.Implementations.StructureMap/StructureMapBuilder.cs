using System;
using System.Collections.Generic;
using System.Linq;
using Ellemy.CQRS.Command;
using Ellemy.CQRS.Container;
using Ellemy.CQRS.Event;
using StructureMap;

namespace Ellemy.CQRS.Implementations.StructureMap
{
    public class StructureMapBuilder : IHandlerFactory, ICommandHandlerFactory,IObjectBuilder
    {
        private readonly IContainer _container;

        public StructureMapBuilder(IContainer container)
        {
            _container = container;
        }

        public IEnumerable<IDomainEventHandler<TEvent>> GetHandlersFor<TEvent>() where TEvent : IDomainEvent
        {
            return _container.GetAllInstances<IDomainEventHandler<TEvent>>();
        }

        public ICommandHandler<TCommand> GetHandlerFor<TCommand>() where TCommand : ICommand
        {
            return _container.GetInstance<ICommandHandler<TCommand>>();
        }

        public IEnumerable<object> BuildAll(Type type)
        {
            return _container.GetAllInstances(type).Cast<object>();
        }

        public IEnumerable<T> BuildAll<T>()
        {
            return _container.GetAllInstances<T>();
        }
    }
}