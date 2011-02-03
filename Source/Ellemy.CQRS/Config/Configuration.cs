using Ellemy.CQRS.Command;
using Ellemy.CQRS.Event;

namespace Ellemy.CQRS.Config
{
    public class Configuration
    {
        public Configuration()
        {
            EventPublisher = new NoOpPublisher();
        }
        public Configuration HandlerFactoryOf(IHandlerFactory handlerFactory)
        {
            HandlerFactory = handlerFactory;
            return this;
        }
        public Configuration CommandExecutorFactoryOf(ICommandHandlerFactory commandHandlerFactory)
        {
            CommandHandlerFactory = commandHandlerFactory;
            return this;
        }
        public Configuration PublishEventsWith(IEventPublisher publisher)
        {
            EventPublisher = publisher;
            return this;
        }
        internal ICommandHandlerFactory CommandHandlerFactory { get; private set; }
        internal IHandlerFactory HandlerFactory { get; private set; }
        internal IEventPublisher EventPublisher { get; private set; }
    }
}