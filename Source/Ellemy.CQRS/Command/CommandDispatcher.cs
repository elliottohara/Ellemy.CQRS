using System.Linq;
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
            var commandInterface = typeof(TCommand).GetInterfaces().FirstOrDefault(t => t.GetInterfaces().Any(t1 => t1 ==typeof(ICommand)));
            if (commandInterface == null)
            {
                CommandHandlerFactory.GetHandlerFor<TCommand>().Execute(command);
            }
            else
            {
                var cmd = CommandHandlerFactory.GetHandlerFor(commandInterface);
                cmd.GetType().GetMethod("Execute").Invoke(cmd, new object[] {command});
                
            }
            DomainEvents.Publish();
        }
    }
}