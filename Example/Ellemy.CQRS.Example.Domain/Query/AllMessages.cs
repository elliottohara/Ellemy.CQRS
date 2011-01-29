using System;
using Ellemy.CQRS.Query;

namespace Ellemy.CQRS.Example.Query
{
    public class AllMessages : IQuery 
    {
        private readonly IRepository<MessageReadModel> _repository;

        public AllMessages(IRepository<MessageReadModel> repository)
        {
            _repository = repository;
        }

        public object Results()
        {
            return _repository.GetAll();
        }
    }
}