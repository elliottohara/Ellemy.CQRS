namespace Ellemy.CQRS.Command
{
    public interface ICommandExecutorFactory
    {
        ICommandExecutor<TCommand> GetExecutorFor<TCommand>() where TCommand : ICommand;
    }
}