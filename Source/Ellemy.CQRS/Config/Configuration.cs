using Ellemy.CQRS.Command;
using Ellemy.CQRS.Container;
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
        public Configuration  WithObjectBuilder(IObjectBuilder builder)
        {
            ObjectBuilder = builder;
            return this;
        }
        public ICommandHandlerFactory CommandHandlerFactory { get; private set; }
        public IHandlerFactory HandlerFactory { get; private set; }
        public IEventPublisher EventPublisher { get; private set; }
        public IObjectBuilder ObjectBuilder { get; private set; }
    }
}