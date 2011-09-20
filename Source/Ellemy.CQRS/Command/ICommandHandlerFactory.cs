using System;

namespace Ellemy.CQRS.Command
{
    public interface ICommandHandlerFactory
    {
        ICommandHandler<TCommand> GetHandlerFor<TCommand>() where TCommand : ICommand;
        object GetHandlerFor(Type command);
    }
}