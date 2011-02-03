using System;
using Ellemy.CQRS.Command;
using Ellemy.CQRS.Example.Domain;
using Ellemy.CQRS.Example.Query;

namespace Ellemy.CQRS.Example.Commands
{
    public class CreateMessageHandler : ICommandHandler<CreateMessage>
    {
        private readonly IRepository<Message> _repository;

        public CreateMessageHandler(IRepository<Message> repository)
        {
            _repository = repository;
        }

        public void Execute(CreateMessage command)
        {
            var message = new Message(command.MessageId, command.Text);
            _repository.Save(message);
        }
    }
}