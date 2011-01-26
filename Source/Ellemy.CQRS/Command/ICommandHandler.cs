namespace Ellemy.CQRS.Command
{
    public interface ICommandHandler<TCommand> where TCommand:ICommand
    {
        void Execute(TCommand command);
    }
}