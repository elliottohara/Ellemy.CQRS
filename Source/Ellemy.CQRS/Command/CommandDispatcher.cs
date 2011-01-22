using Ellemy.CQRS.Config;

namespace Ellemy.CQRS.Command
{
    public static class CommandDispatcher
    {
        public static ICommandExecutorFactory CommandExecutorFactory
        {
            get { return Configure.CurrentConfig.CommandExecutorFactory; }
        }
        public static void Dispatch<TCommand>(TCommand command) where TCommand:ICommand
        {
            CommandExecutorFactory.GetExecutorFor<TCommand>().Execute(command);
        }
    }
}