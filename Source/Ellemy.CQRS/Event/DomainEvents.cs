using System;
using System.Collections.Generic;
using Ellemy.CQRS.Config;

namespace Ellemy.CQRS.Event
{
    public static class DomainEvents
    {
        [ThreadStatic] //so that each thread has its own callbacks
        private static List<Delegate> actions;

        [ThreadStatic] 
        private static List<Action> handlerActions;

        public static IHandlerFactory Container { get { return Configure.CurrentConfig.HandlerFactory; } } 
        
        public static void Publish()
        {
            handlerActions.ForEach(a => a()); 
            //TODO We wanna start publishing to remote systems here 
        }
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
            {
                if(handlerActions == null)
                    handlerActions = new List<Action>();
                foreach (var handler in Container.GetHandlersFor<T>())
                {
                    var handler1 = handler;
                    handlerActions.Add(() => handler1.Handle(args));
                }
            }

            if (actions != null)
                foreach (Delegate action in actions)
                    if (action is Action<T>)
                        ((Action<T>) action)(args);
        }
    }
}