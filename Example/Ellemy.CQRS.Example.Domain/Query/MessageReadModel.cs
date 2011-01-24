using System;

namespace Ellemy.CQRS.Example.Query
{
    public class MessageReadModel
    {
        public virtual Guid Id { get; set; }
        public virtual String Text { get; set; }
           
    }
}