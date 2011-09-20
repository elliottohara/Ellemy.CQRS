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
            var commandInterface = typeof (TCommand).GetInterfaces().FirstOrDefault(t => t.GetInterface("ICommand") != null);
            if(commandInterface == null)
                return  _container.GetInstance<ICommandHandler<TCommand>>();
            var openGeneric = typeof (ICommandHandler<>);
            return (ICommandHandler<TCommand>)_container.GetInstance(openGeneric.MakeGenericType(commandInterface));
        }

        public object GetHandlerFor(Type command)
        {
            //if (typeof(ICommand).IsAssignableFrom(command))
            //    throw new InvalidOperationException(String.Format("type {0} does not implement ICommand.", command.Name));

            var t = typeof(ICommandHandler<>);
            return _container.GetInstance(t.MakeGenericType(command));
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