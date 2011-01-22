using Ellemy.CQRS.Command;
using Ellemy.CQRS.Event;

namespace Ellemy.CQRS.Config
{
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