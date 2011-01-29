namespace Ellemy.CQRS.Command
{
    public interface ICommandHandlerFactory
    {
        ICommandHandler<TCommand> GetHandlerFor<TCommand>() where TCommand : ICommand;
    }
}