using System;
using Ellemy.CQRS.Event;
using Ellemy.CQRS.Example.Events;

namespace Ellemy.CQRS.Example.Query
{
    public class MessageReadModelWriter : 
        IDomainEventHandler<MessageCreated>,
        IDomainEventHandler<MessageTextChanged>
    {
        private readonly IRepository<MessageReadModel> _repository;

        public MessageReadModelWriter(IRepository<MessageReadModel> repository)
        {
            _repository = repository;
        }

        public void Handle(MessageCreated @event)
        {
            var newReadModel = new MessageReadModel {Id = @event.MessageId, Text = @event.Text};
            _repository.Save(newReadModel);
        }

        public void Handle(MessageTextChanged @event)
        {
            var readModel = _repository.GetById(@event.MessageId);
            readModel.Text = @event.Newtext;
            _repository.Save(readModel);
        }
    }
}