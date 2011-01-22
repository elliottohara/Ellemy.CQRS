using System;
using Ellemy.CQRS.Command;
using Ellemy.CQRS.Event;

namespace Ellemy.CQRS
{
    public static class Configure
    {
        private static Configuration _currentConfig;
        internal static Configuration CurrentConfig{get { return _currentConfig ?? (_currentConfig = new Configuration()); }
        }
        public static Configuration With()
        {
            return _currentConfig;
        }
        
    }
    public class Configuration
    {
        public Configuration HandlerFactoryOf(IHandlerFactory handlerFactory)
        {
            HandlerFactory = handlerFactory;
            return this;
        }
        public Configuration CommandExecutorFactoryOf(ICommandExecutorFactory commandExecutorFactory)
        {
            CommandExecutorFactory = commandExecutorFactory;
            return this;
        }

        internal ICommandExecutorFactory CommandExecutorFactory { get; private set; }

        internal IHandlerFactory HandlerFactory { get; private set; }
    }
}