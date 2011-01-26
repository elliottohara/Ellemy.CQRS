namespace Ellemy.CQRS.Command
{
    public interface ICommandHandlerFactory
    {
        ICommandHandler<TCommand> GetExecutorFor<TCommand>() where TCommand : ICommand;
    }
}