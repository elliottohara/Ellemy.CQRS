using System;
using System.Collections.Generic;
using Ellemy.CQRS.Config;

namespace Ellemy.CQRS.Event
{
    public static class DomainEvents
    {
        [ThreadStatic] //so that each thread has its own callbacks
        private static List<Delegate> actions;

        public static IHandlerFactory Container { get { return Configure.CurrentConfig.HandlerFactory; } } //as before
        

        //Registers a callback for the given domain event
        public static void Register<T>(Action<T> callback) where T : IDomainEvent
        {
            if (actions == null)
                actions = new List<Delegate>();

            actions.Add(callback);
        }

        //Clears callbacks passed to Register on the current thread
        public static void ClearCallbacks()
        {
            actions = null;
        }

        //Raises the given domain event
        public static void Raise<T>(T args) where T : IDomainEvent
        {
            if (Container != null)
                foreach (var handler in Container.GetHandlersFor<T>())
                    handler.Handle(args);

            if (actions != null)
                foreach (Delegate action in actions)
                    if (action is Action<T>)
                        ((Action<T>) action)(args);
        }
    }
}