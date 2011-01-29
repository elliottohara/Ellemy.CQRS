using Ellemy.CQRS.Config;
using Ellemy.CQRS.Event;

namespace Ellemy.CQRS.Command
{
    public static class CommandDispatcher
    {
        public static ICommandHandlerFactory CommandHandlerFactory
        {
            get { return Configure.CurrentConfig.CommandHandlerFactory; }
        }
        public static void Dispatch<TCommand>(TCommand command) where TCommand:ICommand
        {
            CommandHandlerFactory.GetHandlerFor<TCommand>().Execute(command);
            DomainEvents.Publish();
        }
    }
}