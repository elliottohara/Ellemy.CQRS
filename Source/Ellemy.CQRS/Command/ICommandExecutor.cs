namespace Ellemy.CQRS.Command
{
    public interface ICommandExecutor<TCommand> where TCommand:ICommand
    {
        void Execute(TCommand command);
    }
}