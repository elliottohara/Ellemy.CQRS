namespace Ellemy.CQRS.Command
{
    public static class CommandDispatcher
    {
        public static void Dispatch<TCommand>(TCommand command) where TCommand:ICommand
        {
            Configure.CurrentConfig.CommandExecutorFactory.GetExecutorFor<TCommand>().Execute(command);
        }
    }
}