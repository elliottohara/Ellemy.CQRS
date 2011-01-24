using System;
using Ellemy.CQRS.Command;
using Ellemy.CQRS.Example.Query;

namespace Ellemy.CQRS.Example.Commands
{
    public class CreateMessageHandler : ICommandExecutor<CreateMessage>
    {
        private readonly IRepository<MessageReadModel> _repository;

        public CreateMessageHandler(IRepository<MessageReadModel> repository)
        {
            _repository = repository;
        }

        public void Execute(CreateMessage command)
        {
            var message = new MessageReadModel {Id = command.MessageId, Text = command.Text};
            _repository.Save(message);
        }
    }
}